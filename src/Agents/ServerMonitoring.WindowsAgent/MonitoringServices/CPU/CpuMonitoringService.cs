using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.Models.Enums;

namespace ServerMonitoring.WindowsAgent.MonitoringServices.CPU
{
    public class CpuMonitoringService : CoreMonitoringService
    {
        private int _cpuCount;

        public override IEnumerable<ServerStatisticsDataItem> GetData()
        {
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = "CPU_" + data.Key,
                Order = GetOrderForName(data.Key, _cpuCount),
                Type = ServerStatisticsType.CPU,
                CurrentValue = data.Value,
                CurrentValueDisplay = "%"
            });
        }

        protected int GetOrderForName(string name, int count)
        {
            if (name == "_Total")
                return 1000;

            return 1001 + Convert.ToInt32(name);
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

                    _cpuCount = cpuCoresInfo.Count();

                    foreach (var coreInfo in cpuCoresInfo)
                        CurrentValues.AddOrUpdate(coreInfo.CoreName, corename => coreInfo.LoadPercent, (corename, val) => coreInfo.LoadPercent);

                    Wait();
                }
                catch { }
            }
        }
    }
}
