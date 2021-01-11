using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Formatting.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using System;
using System.Reflection;

namespace WebApp.Serilog
{
    public static class ConfigureSerilog
    {
        private static string getName => Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-");
        public static void ConfigureLogging(bool writeDebug = true, bool writeConsole = true, bool writeElastic = true)
        {
            SelfLog.Enable(Console.Error);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", true).Build();

            var logConfiguration = new LoggerConfiguration().Enrich.FromLogContext();
            if (writeDebug == true) logConfiguration.WriteTo.Debug();
            if (writeConsole == true) logConfiguration.WriteTo.Console(new ElasticsearchJsonFormatter()); // Elastic search formatında yazmayı sağlar.
            if (writeElastic == true) logConfiguration.WriteTo.Elasticsearch(ConfigureElasticsearchSink(configuration, environment));

            // enrich property'si ,log'u zenginleştiren ve sabit olarak her log atışınızda yazan değerler olarak düşünebiliriz.
            Log.Logger = logConfiguration.Enrich.WithProperty("Environment", environment).ReadFrom.Configuration(configuration).CreateLogger();

            Log.Information("Logger created.");
        }
        private static ElasticsearchSinkOptions ConfigureElasticsearchSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration.GetSection("ElasticsearchConfig").GetSection("Uri").Value))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                DetectElasticsearchVersion = true,
                IndexFormat = $"{getName}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM-dd}", //Index formatı proje adı-debug ya da release modu-tarih olarak indexlenir.
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback,
                FailureSink = new FileSink("./fail.json", new JsonFormatter(), null, null)
            };
        }
    }
}
