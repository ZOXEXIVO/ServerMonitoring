using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.Models.Enums;

namespace ServerMonitoring.WindowsAgent.MonitoringServices.Disk
{
    public class DiskMonitoringService : CoreMonitoringService
    {
        public override IEnumerable<ServerStatisticsDataItem> GetData()
        {
            int i = 0;
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = data.Key,
                Order = 1200 + i++,
                Type = ServerStatisticsType.DISK,
                CurrentValue = data.Value,
                CurrentValueDisplay = "%"
            });
        }

        protected int GetOrderForName(string name, int count)
        {
            if (name == "_Total")
                return 1000;

            return 1000 + Convert.ToInt32(name);
        }

        protected override void MonitoringThread()
        {
            base.MonitoringThread();

            var wmiObject = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

            while (true)
            {
                try
                {
                    var readWriteValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        Name = Convert.ToInt32(mo["Name"]),
                        DiskReadTime = Convert.ToInt32(mo["PercentDiskReadTime"]),
                        DiskWriteTime = Convert.ToInt32(mo["PercentDiskWriteTime"])
                    });

                    foreach (var item in readWriteValues)
                    {
                        CurrentValues.AddOrUpdate("DISK_READ_" + item.Name, diskItem => item.DiskReadTime, (diskItem, val) => item.DiskReadTime);
                        CurrentValues.AddOrUpdate("DISK_WRITE_" + item.Name, diskItem => item.DiskWriteTime, (diskItem, val) => item.DiskWriteTime);
                    }

                    Wait();
                }
                catch { }
            }
        }
    }
}
