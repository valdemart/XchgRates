using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XchgRates.Server.Application.Interfaces;
using XchgRates.Server.Domain.Interfaces;
using XchgRates.Server.Domain.Models;

namespace XchgRates.Server.Infrastructure.Services;

internal class ExchangeRateApiResolver(
    IServiceProvider serviceProvider,
    IOptionsMonitor<Settings> settingsMonitor)
    : IExchangeRateApiResolver
{
    public (IExchangeRateProvider Provider, ApiConfiguration Configuration)[] GetSupportedApis()
    {
        var apis = settingsMonitor.CurrentValue.ApiConfigurations;

        var result = new List<(IExchangeRateProvider Provider, ApiConfiguration Configuration)>();
        foreach (var api in apis)
        {
            if (!api.Enabled)
                continue;

            var provider = Resolve(api.Id, out _);
            if (provider == null)
                continue;

            result.Add((Provider: provider, Configuration: api));
        }

        return [.. result];
    }

    public IExchangeRateProvider? Resolve(string apiId, out ApiConfiguration? apiConfiguration)
    {
        var apis = settingsMonitor.CurrentValue.ApiConfigurations;

        apiConfiguration = apis.FirstOrDefault(api => api.Enabled && api.Id == apiId);
        return apiConfiguration == null
            ? null
            : serviceProvider.GetKeyedService<IExchangeRateProvider>("ExchangeRateProvider:" + apiConfiguration.Provider);
    }
}

internal static class ExchangeRateApiProviderDIExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddExchangeRateProvider<TExchangeRateProvider>(string providerId)
            where TExchangeRateProvider : class, IExchangeRateProvider
            => services.AddKeyedSingleton<IExchangeRateProvider, TExchangeRateProvider>("ExchangeRateProvider:" + providerId);
    }
}
