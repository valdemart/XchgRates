using XchgRates.Server.Domain.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Infrastructure.Services;

internal abstract class ExchangeRateProviderBase
    : IExchangeRateProvider
{
    public abstract Task<ExchangeRate[]> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    public abstract CurrencyData[] GetCurrencies(ApiConfiguration configuration);

    public CurrencyData? GetCurrency(ApiConfiguration configuration, string currencyCode)
        => GetCurrencies(configuration).FirstOrDefault(c => string.Equals(c.Code, currencyCode, StringComparison.OrdinalIgnoreCase));

    public virtual DateOnly GetNearestToday()
        => DateOnly.FromDateTime(DateTime.Now);
}
