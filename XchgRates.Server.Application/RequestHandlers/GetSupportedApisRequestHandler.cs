using XchgRates.Server.Application.Interfaces;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record SupportedApi(string ApiId, string DisplayName);

public interface IGetSupportedApisRequestHandler
{
    SupportedApi[] Handle();
}

internal class GetSupportedApisRequestHandler(
    IExchangeRateApiResolver exchangeRateApiResolver)
    : IGetSupportedApisRequestHandler
{
    public SupportedApi[] Handle()
    {
        var apis = exchangeRateApiResolver.GetSupportedApis();

        return [.. apis
            .OrderBy(api => api.DisplayName)
            .Select(api => new SupportedApi(api.Id, api.DisplayName))];
    }
}
