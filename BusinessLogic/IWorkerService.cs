
using System.Threading.Tasks;

namespace WorkerScheduler
{
    public interface IWorkerService
    {
        Task CollectDailyStatistics();
        Task NotifySubscribers(); 
    }

}