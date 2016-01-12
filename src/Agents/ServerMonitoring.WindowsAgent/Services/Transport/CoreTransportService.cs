using System.Diagnostics;
using System.Threading.Tasks;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services.Transport
{
    public abstract class CoreTransportService : ITransportService
    {
        public abstract Task InternalPushDataAsync(ServerPushData data);

        public async Task<long> PushAsync(ServerPushData data)
        {
            var timer = new Stopwatch();

            timer.Start();
            await InternalPushDataAsync(data);
            timer.Stop();

            return timer.ElapsedMilliseconds;
        }

        public abstract string GetInfo();
    }
}
