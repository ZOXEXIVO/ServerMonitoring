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
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = data.Key,
                Type = ServerStatisticsType.DISK,
                CurrentValue = data.Value,
                CurrentValueDisplay = "%"
            });
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
