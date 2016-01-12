using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using ServerMonitoring.WindowsAgent.Enums;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services.CPU
{
    public class CpuMonitoringService : CoreMonitoringService
    {
        public override IEnumerable<ServerStatisticsDataItem> GetData()
        {
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = "CPU_" + data.Key,
                Type = ServerStatisticsType.CPU,
                CurrentValue = data.Value,
                CurrentValueDisplay = "%"
            });
        }

        protected override void MonitoringThread()
        {
            base.MonitoringThread();

            var wmiObject = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            
            while (true)
            {
                try
                {
                    var cpuCoresInfo = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        CoreName = Convert.ToString(mo["Name"]),
                        LoadPercent = Convert.ToInt32(mo["PercentProcessorTime"])
                    }).ToList();

                    foreach (var coreInfo in cpuCoresInfo)
                        CurrentValues.AddOrUpdate(coreInfo.CoreName, corename => coreInfo.LoadPercent, (corename, val) => coreInfo.LoadPercent);

                    Wait();
                }
                catch { }
            }
        }
    }
}
