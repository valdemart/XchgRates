using XchgRates.Server.Application;
using XchgRates.Server.Infrastructure;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
