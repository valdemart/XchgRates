using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Domain.Interfaces;

public interface IExchangeRateProvider
{
    /// <param name="ordering"><see cref="null"/> - no ordering; <see cref="false"/> - source currencies ordering; <see cref="true"/> - target currencies ordering</param>
    IEnumerable<CurrencyData> GetCurrencies(ApiConfiguration configuration, bool? ordering = null);
    CurrencyData? GetCurrency(ApiConfiguration configuration, string code);
    public Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(ApiConfiguration configuration, CurrencyData currency, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
