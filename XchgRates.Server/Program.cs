using XchgRates.Server;
using XchgRates.Server.Application;
using XchgRates.Server.Infrastructure;
using XchgRates.Server.Middlewares;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddWebAppServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
