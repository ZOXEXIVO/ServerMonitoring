using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.Models.Enums;

namespace ServerMonitoring.WindowsAgent.MonitoringServices.Network
{
    public class NetworkMonitoringService : CoreMonitoringService
    {
        protected readonly ConcurrentDictionary<string, string> DisplayValues;

        public NetworkMonitoringService()
        {
            DisplayValues = new ConcurrentDictionary<string, string>();
        }

        public override IEnumerable<ServerStatisticsDataItem> GetData()
        {
            int i = 0;
            return CurrentValues.Select(data => new ServerStatisticsDataItem
            {
                Name = data.Key,
                Order = 1400 + i++,
                Type = ServerStatisticsType.NETWORK,
                CurrentValue = data.Value,
                CurrentValueDisplay = DisplayValues.GetOrAdd(data.Key, key => "")
            });
        }

        protected override void MonitoringThread()
        {
            base.MonitoringThread();

            var wmiObject = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_Tcpip_NetworkInterface");

            while (true)
            {
                try
                {
                    var networkValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                    {
                        BytesReceived = Convert.ToInt32(mo["BytesReceivedPersec"]),
                        BytesSent = Convert.ToInt32(mo["BytesSentPersec"])
                    }).ToList();

                    if (networkValues.Any())
                    {
                        //receive
                        var recieveRawValue = networkValues.Sum(x => x.BytesReceived);

                        var recieveFormattedValue = SizeHelper.GetSizeDisplay(recieveRawValue);

                        CurrentValues.AddOrUpdate("NETWORK_RECEIVE", item => (int)recieveFormattedValue.Item1, (networkItem, val) => (int)recieveFormattedValue.Item1);
                        DisplayValues.AddOrUpdate("NETWORK_RECEIVE", item => recieveFormattedValue.Item2, (networkItem, val) => recieveFormattedValue.Item2);

                        //send
                        var sendRawValue = networkValues.Sum(x => x.BytesSent);

                        var sendFormattedValue = SizeHelper.GetSizeDisplay(sendRawValue);

                        CurrentValues.AddOrUpdate("NETWORK_SEND", item => (int)sendFormattedValue.Item1, (networkItem, val) => (int)sendFormattedValue.Item1);
                        DisplayValues.AddOrUpdate("NETWORK_SEND", item => sendFormattedValue.Item2, (networkItem, val) => sendFormattedValue.Item2);
                    }

                    Wait();
                }
                catch { }
            }
        }

        private static class SizeHelper
        {
            public static Tuple<long, string> GetSizeDisplay(long bytes)
            {
                if (bytes < 1024)
                {
                    return new Tuple<long, string>(bytes, "B");
                }

                if (bytes < 1024 * 1024)
                {
                    return new Tuple<long, string>(bytes / 1024, "KB");
                }

                if (bytes < 1024 * 1024 * 1024)
                {
                    return new Tuple<long, string>(bytes / (1024 * 1024), "MB");
                }

                return new Tuple<long, string>(bytes / (1024 * 1024 * 1024), "GB");
            }
        }
    }
}
