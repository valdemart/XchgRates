using XchgRates.Server.Middlewares;

namespace XchgRates.Server;

internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWebAppServices()
        {
            //middlewares
            services.AddScoped<GlobalExceptionHandlingMiddleware>();

            return services;
        }
    }
}
