using XchgRates.Server.Application.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record GetCurrenciesResponse(CurrencyData[] SourceCurrencies, CurrencyData[] TargetCurrencies);

public interface IGetCurrenciesRequestHandler
{
    GetCurrenciesResponse? Handle(string apiId, CancellationToken cancellationToken);
}

internal class GetCurrenciesRequestHandler(
    IExchangeRateApiResolver exchangeRateApiResolver)
    : IGetCurrenciesRequestHandler
{
    public GetCurrenciesResponse? Handle(string apiId, CancellationToken cancellationToken)
    {
        var ratesProvider = exchangeRateApiResolver.Resolve(apiId, out var apiConfiguration);
        if (ratesProvider == null || apiConfiguration?.Enabled is not true)
            return null; //todo: log and return to user: unsupported or disabled api

        return new(
            [.. ratesProvider.GetCurrencies(apiConfiguration, false)],
            [.. ratesProvider.GetCurrencies(apiConfiguration, true)]);
    }
}
