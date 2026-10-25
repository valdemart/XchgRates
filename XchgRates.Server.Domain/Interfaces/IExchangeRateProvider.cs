using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Domain.Interfaces;

public interface IExchangeRateProvider
{
    CurrencyData[] GetCurrencies(ApiConfiguration configuration);
    CurrencyData? GetCurrency(ApiConfiguration configuration, string code);
    Task<ExchangeRate[]> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    DateOnly GetNearestToday();
}
