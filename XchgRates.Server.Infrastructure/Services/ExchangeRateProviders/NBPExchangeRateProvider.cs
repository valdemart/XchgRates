using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Infrastructure.Services.ExchangeRateProviders;

internal class NBPExchangeRateProvider(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<Settings> settingsMonitor)
    : ExchangeRateProviderBase(settingsMonitor)
{
    private const string ExchangeRateEndpointPattern = "{0}/api/exchangerates/rates/a/{1}/{2:yyyy-MM-dd}/{3:yyyy-MM-dd}/?format=json";

    private readonly IEnumerable<CurrencyData> _currencies = [
    private readonly CurrencyData[] _currencies = [
        new("PLN", "Złoty"),
        new("EUR", "Euro"),
        new("GBP", "Funt brytyjski"),
        new("USD", "Dolar amerykański"),
        new("CHF", "Frank szwajcarski"),
        new("JPY", "Jeny")
    ];

    public override async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        if (endDate < startDate)
            (endDate, startDate) = (startDate, endDate);
        if (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue) > TimeSpan.FromDays(93))
            return []; //todo: log and report too wide date range

        var client = httpClientFactory.CreateClient();

        var url = string.Format(ExchangeRateEndpointPattern,
            configuration.BaseUrl,
            currency.Code.ToLower(),
            startDate,
            endDate);

        var response = await client.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode) //todo: retry policy
            return [];

        var content = await response.Content.ReadFromJsonAsync<ExchangeRateEndpointResponse>(cancellationToken);
        if (content == null)
            return []; //todo: log and report empty response

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
