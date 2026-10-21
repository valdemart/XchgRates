using Microsoft.Extensions.Options;
using XchgRates.Server.Domain.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Infrastructure.Services;

internal abstract class ExchangeRateProviderBase(
    IOptionsMonitor<Settings> settingsMonitor)
    : IExchangeRateProvider
{
    public abstract Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    protected abstract IEnumerable<CurrencyData> GetCurrencies(ApiConfiguration configuration);

    public IEnumerable<CurrencyData> GetCurrencies(ApiConfiguration configuration, bool? ordering = null)
    {
        var currencies = GetCurrencies(configuration);
        return ordering switch
        {
            null => currencies,
            false => ApplyOrdering(currencies, settingsMonitor.CurrentValue.SourceCurrencyOrdering),
            true => ApplyOrdering(currencies, settingsMonitor.CurrentValue.TargetCurrencyOrdering),
        };
    }

    public CurrencyData? GetCurrency(ApiConfiguration configuration, string currencyCode)
        => GetCurrencies(configuration).FirstOrDefault(c => string.Equals(c.Code, currencyCode, StringComparison.OrdinalIgnoreCase));

    protected static IEnumerable<CurrencyData> ApplyOrdering(IEnumerable<CurrencyData> currencies, CurrencyOrderings ordering)
        => ordering switch
        {
            CurrencyOrderings.CodeDescending => currencies.OrderByDescending(c => c.Code),
            CurrencyOrderings.NameAscending => currencies.OrderBy(c => c.Name),
            CurrencyOrderings.NameDescending => currencies.OrderByDescending(c => c.Name),
            _ => currencies.OrderBy(c => c.Code), //default: CurrencyOrderings.CodeAscending
        };
}
