using Microsoft.Extensions.DependencyInjection;
using ServerMonitoring.Data.Storages;
using ServerMonitoring.Data.Storages.InMemory;

namespace ServerMonitoring.Data
{
    public static class Registration
    {
        public static void RegisterDataServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof (IMonitoringStorage), typeof (InMemoryMonitoringStorage));
        }
    }
}
