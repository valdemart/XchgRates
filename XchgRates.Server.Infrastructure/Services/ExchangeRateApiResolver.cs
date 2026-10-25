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
    public ApiConfiguration[] GetSupportedApis()
    {
        var apis = settingsMonitor.CurrentValue.ApiConfigurations;

        return [.. apis.Where(api => api.Enabled && Resolve(api.Id, out _) != null)];
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
