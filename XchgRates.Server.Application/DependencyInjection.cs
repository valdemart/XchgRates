using Microsoft.Extensions.DependencyInjection;
using XchgRates.Server.Application.RequestHandlers;

namespace XchgRates.Server.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            //settings requests
            services.AddTransient<IGetSettingsRequestHandler, GetSettingsRequestHandler>();

            //exchange rate & apis requests
            services.AddTransient<IGetSupportedApisRequestHandler, GetSupportedApisRequestHandler>();
            services.AddTransient<IGetExchangeRatesRequestHandler, GetExchangeRatesRequestHandler>();

            //currency requests
            services.AddTransient<IGetCurrenciesRequestHandler, GetCurrenciesRequestHandler>();

            return services;
        }
    }
}
