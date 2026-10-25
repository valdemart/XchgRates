using Microsoft.Extensions.Options;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Application.RequestHandlers;

public sealed record GetSettingsResponse(string DateFormat);

public interface IGetSettingsRequestHandler
{
    GetSettingsResponse Handle();
}

internal class GetSettingsRequestHandler(
    IOptionsMonitor<Settings> settingsMonitor)
    : IGetSettingsRequestHandler
{
    public GetSettingsResponse Handle()
    {
        var settings = settingsMonitor.CurrentValue;

        return new(
            settings.DateFormat
        );
    }
}
