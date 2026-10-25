using Microsoft.Extensions.Options;
﻿using XchgRates.Server.Application.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record GetCurrenciesResponse(CurrencyData[] SourceCurrencies, CurrencyData[] TargetCurrencies);

public interface IGetCurrenciesRequestHandler
{
    GetCurrenciesResponse? Handle(string apiId, CancellationToken cancellationToken);
}

internal class GetCurrenciesRequestHandler(
    IExchangeRateApiResolver exchangeRateApiResolver,
    IOptionsMonitor<Settings> settingsMonitor)
    : IGetCurrenciesRequestHandler
{
    public GetCurrenciesResponse? Handle(string apiId, CancellationToken cancellationToken)
    {
        var ratesProvider = exchangeRateApiResolver.Resolve(apiId, out var apiConfiguration);
        if (ratesProvider == null || apiConfiguration?.Enabled is not true)
            return null; //todo: log and return to user: unsupported or disabled api
        var currencies = ratesProvider.GetCurrencies(apiConfiguration);

        return new(
            ApplyOrdering(currencies, settingsMonitor.CurrentValue.SourceCurrencyOrdering),
            ApplyOrdering(currencies, settingsMonitor.CurrentValue.TargetCurrencyOrdering));
    }

    private static CurrencyData[] ApplyOrdering(CurrencyData[] currencies, CurrencyOrderings ordering)
        => [.. ordering switch
        {
            CurrencyOrderings.CodeDescending => currencies.OrderByDescending(c => c.Code),
            CurrencyOrderings.NameAscending => currencies.OrderBy(c => c.Name),
            CurrencyOrderings.NameDescending => currencies.OrderByDescending(c => c.Name),
            _ => currencies.OrderBy(c => c.Code), //default: CurrencyOrderings.CodeAscending
        }];
}
