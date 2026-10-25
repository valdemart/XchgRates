using XchgRates.Server.Application.Exceptions;
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
        //validate arguments
        var ratesProvider = exchangeRateApiResolver.Resolve(apiId, out var apiConfiguration)
            ?? throw new UnsupportedApiException();
        if (apiConfiguration?.Enabled is not true)
            throw new DisabledApiException();

        var sourceCurrency = ratesProvider.GetCurrency(apiConfiguration, sourceCurrencyCode)
            ?? throw new UnsupportedSourceCurrencyException();

        var targetCurrency = ratesProvider.GetCurrency(apiConfiguration, targetCurrencyCode)
            ?? throw new UnsupportedTargetCurrencyException();

        //limit arguments and fix acceptable argument issues
        startDate = CorrectDate(startDate);
        endDate = CorrectDate(endDate);

        if (endDate < startDate)
            (endDate, startDate) = (startDate, endDate);

        //get rates
        var sourceRates = await ratesProvider.GetExchangeRatesAsync(apiConfiguration, sourceCurrency, startDate.Value, endDate.Value, cancellationToken);
        if (sourceRates.Length == 0)
            throw new NoDataException(sourceCurrency);
        var targetRates = await ratesProvider.GetExchangeRatesAsync(apiConfiguration, targetCurrency, startDate.Value, endDate.Value, cancellationToken);
        if (targetRates.Length == 0)
            throw new NoDataException(targetCurrency);

        var targetPerSourceRates = sourceRates.OrderBy(r => r.Date)
            .GroupJoin(
                targetRates.OrderBy(r => r.Date),
                sourceRate => sourceRate.Date,
                targetRate => targetRate.Date,
                (sourceRate, targetRates) => targetRates.Select(targetRate => new GetExchangeRatesResponseRate(targetRate.Date, sourceRate.Rate / targetRate.Rate)).FirstOrDefault()
            )
            .Where(rate => rate != null)
            .Select(rate => rate!)
            .ToArray();

        if (targetPerSourceRates.Length == 0)
            throw new NoDataException();

        //todo: CSS
        //todo: README.md with local setup & startup

        //format response
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

    private static DateOnly CorrectDate(DateOnly? date)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return date == null || date.Value > today
            ? today
            : date.Value;
    }
}
