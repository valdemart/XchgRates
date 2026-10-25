using XchgRates.Server.Application.Interfaces;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record SupportedApi(string ApiId, string DisplayName, DateOnly NearestToday);

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
        var supportedApis = exchangeRateApiResolver.GetSupportedApis();

        return [.. supportedApis
            .OrderBy(supportedApi => supportedApi.Configuration.DisplayName)
            .Select(supportedApi => new SupportedApi(supportedApi.Configuration.Id, supportedApi.Configuration.DisplayName, supportedApi.Provider.GetNearestToday()))];
    }
}
