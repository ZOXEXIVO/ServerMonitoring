using System.Collections.Generic;
using System.Linq;
using System.Management;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.Models.Enums;

namespace ServerMonitoring.WindowsAgent.MonitoringServices.Memory
{
    public class MemoryMonitoringService : CoreMonitoringService
    {
        public override IEnumerable<ServerStatisticsDataItem> GetData()
        {
            int i = 0;
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = data.Key,
                Order = 1100 + i++,
                Type = ServerStatisticsType.MEMORY,
                CurrentValue = data.Value,
                CurrentValueDisplay = "%"
            });
        }

        protected override void MonitoringThread()
        {
            base.MonitoringThread();

            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            while (true)
            {
                try
                {
                    var memoryInfo = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        FreePhysicalMemory = double.Parse(mo["FreePhysicalMemory"].ToString()),
                        TotalVisibleMemorySize = double.Parse(mo["TotalVisibleMemorySize"].ToString())
                    }).FirstOrDefault();

                    if (memoryInfo != null)
                    {
                        var percent = (int)(((memoryInfo.TotalVisibleMemorySize - memoryInfo.FreePhysicalMemory) / memoryInfo.TotalVisibleMemorySize) * 100);
                        CurrentValues.AddOrUpdate("MEMORY", memoryItem => percent, (memoryItem, val) => percent);
                    }
                    
                    Wait();
                }
                catch { }
            }
        }
    }
}



