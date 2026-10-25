using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Domain.Interfaces;

public interface IExchangeRateProvider
{
    CurrencyData[] GetCurrencies(ApiConfiguration configuration);
    IEnumerable<CurrencyData> GetCurrencies(ApiConfiguration configuration, bool? ordering = null);
    CurrencyData? GetCurrency(ApiConfiguration configuration, string code);
    public Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
