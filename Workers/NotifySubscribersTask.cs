using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkerScheduler
{

    public class NotifySubscribersTask : BackgroundService
    {
        private readonly ILogger<NotifySubscribersTask> _logger;
        private readonly NotifySubscribersConfiguration _notifySubscribersConfiguration;
        private readonly IWorkerService _workerService;

        public NotifySubscribersTask(IOptions<NotifySubscribersConfiguration> notifySubscribersConfiguration, IWorkerService workerService, ILogger<NotifySubscribersTask> logger)
        {
            _logger = logger;
            _notifySubscribersConfiguration = notifySubscribersConfiguration?.Value ?? throw new ArgumentOutOfRangeException(nameof(notifySubscribersConfiguration));
            _workerService = workerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try {
                    await _workerService.NotifySubscribers();                    
                }
                catch(TaskCanceledException) {                    
                }
                catch(Exception e)
                {
                    _logger.LogError(e,e.Message);
                }
                finally {
                    await Task.Delay(_notifySubscribersConfiguration.Interval, stoppingToken);
                }
            }
        }
    }
}