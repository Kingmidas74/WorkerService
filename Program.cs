using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WorkerScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggingBuilder =>
                {
                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    loggingBuilder.AddSerilog(logger, dispose: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IWorkerService,WorkerService>();
                    var dailyStatisticsConfiguration = hostContext.Configuration.GetSection(nameof(DailyStatisticsConfiguration));
                    var notifySubscribersConfiguration = hostContext.Configuration.GetSection(nameof(NotifySubscribersConfiguration));
                    services.Configure<DailyStatisticsConfiguration>(dailyStatisticsConfiguration);
                    services.Configure<NotifySubscribersConfiguration>(notifySubscribersConfiguration);
                    services.AddHostedService<NotifySubscribersTask>();
                    #if DEBUG
                    Console.WriteLine("Test");
                    services.AddHostedService<DailyStatisticsTask>();
                    #else
                    services.AddHostedService<DailyStatisticsByScheduleTask>();
                    #endif
                });
    }
}
