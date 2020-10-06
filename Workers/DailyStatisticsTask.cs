using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkerScheduler
{
    [Obsolete("Do not use this in production! Use external scheduler!")]
    public class DailyStatisticsTask : BackgroundService
    {
        private readonly ILogger<DailyStatisticsTask> _logger;
        private readonly DailyStatisticsConfiguration _dailyStatisticsConfiguration;
        private readonly IWorkerService _workerService;

        public DailyStatisticsTask(IOptions<DailyStatisticsConfiguration> dailyStatisticsConfiguration, IWorkerService workerService, ILogger<DailyStatisticsTask> logger)
        {
            _logger = logger;
            _dailyStatisticsConfiguration = dailyStatisticsConfiguration?.Value ?? throw new ArgumentOutOfRangeException(nameof(dailyStatisticsConfiguration));            
            _workerService = workerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try {
                    var currentHour = DateTime.Now.TimeOfDay.Hours;
                    var currentMinutes = DateTime.Now.TimeOfDay.Minutes;

                    var isRequiredTime = currentHour == _dailyStatisticsConfiguration.TargetTime.Hours  
                        && currentMinutes == _dailyStatisticsConfiguration.TargetTime.Minutes;

                    if(isRequiredTime) {
                        await _workerService.CollectDailyStatistics();
                        await Task.Delay(new TimeSpan(24,0,0), stoppingToken);
                    }
                    else {                    
                        await Task.Delay(1000, stoppingToken);
                    }    
                }
                catch(TaskCanceledException) {
                }
                catch(Exception e)
                {
                    _logger.LogError(e,e.Message);
                }
            }
        }
    }
}