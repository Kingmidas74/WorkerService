
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WorkerScheduler
{
    public class WorkerService : IWorkerService
    {
        private readonly ILogger<WorkerService> _logger;
        public WorkerService(ILogger<WorkerService> logger)
        {
            _logger = logger;
        }

        public Task CollectDailyStatistics()
        {
            return Task.Run(()=>_logger.LogInformation($"{nameof(CollectDailyStatistics)} execution!"));
        }

        public Task NotifySubscribers()
        {
            return Task.Run(()=>_logger.LogInformation($"{nameof(NotifySubscribers)} execution!"));
        }
    }

}