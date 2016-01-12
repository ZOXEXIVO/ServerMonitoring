using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services
{
    public abstract class CoreMonitoringService : IMonitoringService
    {
        protected readonly ConcurrentDictionary<string, double> CurrentValues;
        protected readonly ManualResetEvent WaitEvent = new ManualResetEvent(false);

        protected int WaitPeriod = 1000;

        private Task _monitoringTask;

        protected CoreMonitoringService()
        {
            CurrentValues = new ConcurrentDictionary<string, double>();
            ThreadPriority = ThreadPriority.BelowNormal;
        }

        public void Start()
        {
            _monitoringTask = new Task(MonitoringThread);
            _monitoringTask.Start();
        }

        public void Stop()
        {
            try
            {
                if (_monitoringTask != null)
                    _monitoringTask?.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        public void Wait()
        {
            WaitEvent.WaitOne(WaitPeriod);
        }

        public virtual ThreadPriority ThreadPriority { get; }

        public abstract IEnumerable<ServerStatisticsDataItem> GetData();

        protected virtual void MonitoringThread()
        {
            Thread.CurrentThread.Priority = ThreadPriority;
        }
    }
}
