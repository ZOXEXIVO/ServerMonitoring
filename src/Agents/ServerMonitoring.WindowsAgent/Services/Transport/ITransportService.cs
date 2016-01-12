using System.Threading.Tasks;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services.Transport
{
    /// <summary>
    /// Service, that push data outer
    /// </summary>
    public interface ITransportService
    {
        /// <summary>
        /// Push monitoring data to ...
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>time in milliseconds</returns>
        Task<long> PushAsync(ServerPushData data);

        /// <summary>
        /// Info for output startup
        /// </summary>
        /// <returns></returns>
        string GetInfo();
    }
}
