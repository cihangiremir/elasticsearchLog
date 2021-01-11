using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using WebApp.Serilog;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ConfigureSerilog.ConfigureLogging();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Startup Error !");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(config => config.ClearProviders())
                .UseSerilog(); //clear providers ile ILogger ın sadece serilog tarafından kullanılması sağlanır.
    }
}
