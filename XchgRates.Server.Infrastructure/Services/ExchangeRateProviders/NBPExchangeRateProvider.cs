using System.Net;
using System.Text.Json;
using XchgRates.Server.Application.Exceptions;
using XchgRates.Server.Domain.Models;
using XchgRates.Server.Infrastructure.Extensions;

namespace XchgRates.Server.Infrastructure.Services.ExchangeRateProviders;

internal class NBPExchangeRateProvider(
    IHttpClientFactory httpClientFactory)
    : ExchangeRateProviderBase
{
    private const string ExchangeRateEndpointPattern = "{0}/api/exchangerates/rates/a/{1}/{2:yyyy-MM-dd}/{3:yyyy-MM-dd}/?format=json";

    private const int MaxDays = 366;

    private readonly CurrencyData[] _currencies = [
        new("PLN", "Złoty"),
        new("EUR", "Euro"),
        new("GBP", "Funt brytyjski"),
        new("USD", "Dolar amerykański"),
        new("CHF", "Frank szwajcarski"),
        new("JPY", "Jeny")
    ];

    public override async Task<ExchangeRate[]> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        if (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue) > TimeSpan.FromDays(MaxDays))
            throw new ProviderLimitsViolationException($"Odpytywany zakres może obejmować maksymalnie {MaxDays} dni");

        var client = httpClientFactory.CreateClient();

        var url = string.Format(ExchangeRateEndpointPattern,
            configuration.BaseUrl,
            currency.Code.ToLower(),
            startDate,
            endDate);

        var response = await client.RetryOnError(
            c => c.GetAsync(url, cancellationToken),
            (statusCode, attempt) => attempt < 3 && statusCode switch
            {
                HttpStatusCode.RequestTimeout => true, //timeout? try again after a while
                HttpStatusCode.NotFound => false, //no data? give up
                HttpStatusCode.BadRequest => false, //bad request? give up
                _ => false,
            },
            (_, attempt) => (int)(Math.Pow(2, attempt - 1) * 1000));

        var strContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new NoDataException(currency);

                case HttpStatusCode.BadRequest when strContent.Contains("Przekroczony limit"):
                    throw new ProviderLimitsViolationException($"Odpytywany zakres może obejmować maksymalnie {MaxDays} dni");
                case HttpStatusCode.BadRequest:
                    throw new Exception("Błąd aplikacji - skontaktuj się z administratorem");

                default:
                    break;
            }
        }

        var content = JsonSerializer.Deserialize<ExchangeRateEndpointResponse>(strContent, JsonSerializerOptions.Web)
            ?? throw new NoDataException(currency);

        return [.. content.Rates
            .Select(r => new ExchangeRate(currency, r.EffectiveDate, r.Mid))
            .OrderBy(r => r.Date)];
    }

    //dev-note: requires business decision
    //justification:
    //NBP doesn't provide an endpoint with list of supported currencies.
    //It's declared as to support entire ISO 4217 but experiments shown that may not be true.
    //In theory, we could use table A call to get all supported currencies, but there's no guarantee that any call to table A always returns all supported currencies.
    public override CurrencyData[] GetCurrencies(ApiConfiguration configuration)
        => _currencies;

    internal record ExchangeRateEndpointResponse(string Table, string Currency, string Code, ExchangeRateEndpointRateData[] Rates);
    internal record ExchangeRateEndpointRateData(string No, DateOnly EffectiveDate, decimal Mid);
}
