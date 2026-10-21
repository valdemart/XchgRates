using XchgRates.Server.Application.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record GetExchangeRatesResponse(CurrencyData Source, CurrencyData Target, decimal MinRate, decimal MaxRate, decimal AvgRate, GetExchangeRatesResponseRate[] Rates);
public sealed record GetExchangeRatesResponseRate(DateOnly Date, decimal Rate);

public interface IGetExchangeRatesRequestHandler
{
    Task<GetExchangeRatesResponse?> Handle(string apiId, string sourceCurrencyCode, string targetCurrencyCode, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken);
}

internal class GetExchangeRatesRequestHandler(
    IExchangeRateApiResolver exchangeRateApiResolver)
    : IGetExchangeRatesRequestHandler
{
    public async Task<GetExchangeRatesResponse?> Handle(string apiId, string sourceCurrencyCode, string targetCurrencyCode, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken)
    {
        //todo: frontend niech pokazuje placeholder "Dzisiaj" w datach
        //todo: zwracanie błędów walidacji (frontend też niech sprawdza samodzielnie żeby odciążyć api)

        var ratesProvider = exchangeRateApiResolver.Resolve(apiId, out var apiConfiguration);
        if (ratesProvider == null || apiConfiguration?.Enabled is not true)
            return null; //todo: log and return to user: unsupported or disabled api

        var sourceCurrency = ratesProvider.GetCurrency(apiConfiguration, sourceCurrencyCode);
        if (sourceCurrency == null)
            return null; //todo: log and return to user: unsupported source currency

        var targetCurrency = ratesProvider.GetCurrency(apiConfiguration, targetCurrencyCode);
        if (targetCurrency == null)
            return null; //todo: log and return to user: unsupported target currency

        startDate ??= DateOnly.FromDateTime(DateTime.Now);
        endDate ??= DateOnly.FromDateTime(DateTime.Now);

        if (endDate < startDate)
            (endDate, startDate) = (startDate, endDate);

        var sourceRates = await ratesProvider.GetExchangeRatesAsync(apiConfiguration, sourceCurrency, startDate.Value, endDate.Value, cancellationToken);
        var targetRates = await ratesProvider.GetExchangeRatesAsync(apiConfiguration, targetCurrency, startDate.Value, endDate.Value, cancellationToken);

        var targetPerSourceRates = sourceRates.OrderBy(r => r.Date)
            .Zip(targetRates.OrderBy(r => r.Date), (sourceRate, targetRate) => new GetExchangeRatesResponseRate(targetRate.Date, sourceRate.Rate / targetRate.Rate))
            .ToArray();
        //todo: add protection against inaccurate date matches (source and target currencies may have incomplete data)

        //todo: add protection against empty data
        var minRate = targetPerSourceRates.Select(x => x.Rate).Min();
        var maxRate = targetPerSourceRates.Select(x => x.Rate).Max();
        var avgRate = targetPerSourceRates.Select(x => x.Rate).Average();

        return new(
            sourceCurrency,
            targetCurrency,
            minRate,
            maxRate,
            avgRate,
            targetPerSourceRates);
    }
}
