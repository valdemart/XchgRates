using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XchgRates.Server.Application.Interfaces;
using XchgRates.Server.Domain.Models;
using XchgRates.Server.Infrastructure.Services;
using XchgRates.Server.Infrastructure.Services.ExchangeRateProviders;

namespace XchgRates.Server.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            services.Configure<Settings>(configuration);

            services.AddHttpClient();

            services.AddSingleton<IExchangeRateApiResolver, ExchangeRateApiResolver>();
            services.AddExchangeRateProvider<NBPExchangeRateProvider>("NBP");

            return services;
        }
    }
}
