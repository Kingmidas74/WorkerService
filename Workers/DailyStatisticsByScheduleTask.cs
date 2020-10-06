using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkerScheduler
{
    public class DailyStatisticsByScheduleTask : BackgroundService
    {
        private readonly ILogger<DailyStatisticsByScheduleTask> _logger;
        private readonly DailyStatisticsConfiguration _dailyStatisticsConfiguration;
        private readonly IWorkerService _workerService;

        public DailyStatisticsByScheduleTask(IOptions<DailyStatisticsConfiguration> dailyStatisticsConfiguration, IWorkerService workerService, ILogger<DailyStatisticsByScheduleTask> logger)
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
                    await _workerService.CollectDailyStatistics();                    
                }
                catch(TaskCanceledException) {                    
                }
                catch(Exception e)
                {
                    _logger.LogError(e,e.Message);
                }
                finally {
                    await Task.Delay(new TimeSpan(24,0,0), stoppingToken);          
                }
            }
        }
    }
}