using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System.Net.Http;
using SlackConnector;
using Oppsyn.SlackClients;
using System.Net.Http.Headers;
using System.Linq;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Oppsyn.Infrastructure.Installation;
using Oppsyn.Models;

namespace Oppsyn
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string configFile = @"data/config/config.json";
            if (args.Any())
            {
                configFile = args[0];
            }
            if (!File.Exists(configFile))
            {
                throw new InvalidOperationException(
                    $"No such config file {configFile}. Current working directory is {Directory.GetCurrentDirectory()}");
            }

            var logger = ConfigureAndCreateSerilogLogger();

            try
            {
                var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, configApp) =>
                {
                    configApp
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile(configFile)
                        .AddCommandLine(args)
                        .Build();
                })
                .ConfigureLogging((context, builder) =>
                    {
                        builder.Services.AddSingleton(logger);
                        builder.Services.AddSingleton((Func<IServiceProvider, ILoggerFactory>)(services => new SerilogLoggerFactory(logger, false)));
                    })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration.GetSection("BotConfig").Get<BotConfig>();
                    services.AddSingleton(config);

                    services.InstallSlackClients(config.Slack);
                    services.AddVisionClient(config.Azure);
                    services.AddMessageHandlers();

                    services.AddHostedService<SlackConnectionHost>();
                })
                .Build();

                using (host)
                {
                    await host.StartAsync();
                    host.WaitForShutdown();
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Error starting app!");
                throw;
            }


        }


        public static Serilog.ILogger ConfigureAndCreateSerilogLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console();

            loggerConfiguration
                .WriteTo.File("data/logs/oppsyn.log", rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day);

            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Filter
                .ByExcluding(e => Serilog.Filters.Matching.FromSource("Microsoft.AspNetCore")(e) && e.Level < Serilog.Events.LogEventLevel.Warning);

            loggerConfiguration.MinimumLevel.Verbose();
            var logger = loggerConfiguration.CreateLogger();
            logger.Debug("Initilized logger.");
            return logger;
        }

    }
}
