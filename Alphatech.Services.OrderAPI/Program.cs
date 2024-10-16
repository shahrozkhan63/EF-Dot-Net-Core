using Alphatech.Services.OrderAPI;
using NLog.Extensions.Logging;

namespace Alphatech.Services.OrderAPI;
public class Program
{
    public static void Main(string[] args)
    {
        //var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
        //builder.Services.AddJWTTokenServices(builder.Configuration);

        CreateHostBuilder(args).Build().Run();


    }
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
             .ConfigureLogging((hostingContext, logging) =>
             {
                 logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                 logging.AddConsole();
                 logging.AddDebug();
                 logging.AddEventSourceLogger();
                 logging.AddNLog();

             })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}