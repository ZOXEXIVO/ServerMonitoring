using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.Services;
using ServerMonitoring.WindowsAgent.Services.CPU;
using ServerMonitoring.WindowsAgent.Services.Disk;
using ServerMonitoring.WindowsAgent.Services.Memory;
using ServerMonitoring.WindowsAgent.Services.Network;

namespace ServerMonitoring.WindowsAgent.Application
{
    public class MonitoringApp
    {
        private readonly List<IMonitoringService> _monitoringServices = new List<IMonitoringService>();

        private readonly ServerPushData _pushData;

        public MonitoringApp()
        {
            _monitoringServices.Add(new CpuMonitoringService());
            _monitoringServices.Add(new MemoryMonitoringService());
            _monitoringServices.Add(new DiskMonitoringService());
            _monitoringServices.Add(new NetworkMonitoringService());

            _pushData = new ServerPushData();

            InitServerInfo();
        }

        public void Start()
        {
            foreach (var service in _monitoringServices)
                service.Start();
        }

        public void Stop()
        {
            foreach (var service in _monitoringServices)
                service.Stop();
        }

        private void InitServerInfo()
        {
            _pushData.Server = new ServerInfo
            {
                MachineName = Environment.MachineName
            };

            var currentIps = Dns.GetHostAddresses(Dns.GetHostName());

            _pushData.Server.IP = currentIps.Any() ? currentIps.LastOrDefault().ToString() : "127.0.0.1";
        }

        public ServerInfo GetServerInfo()
        {
            return _pushData.Server;
        }

        public ServerPushData GetDataToPush()
        {
            _pushData.Items = _monitoringServices.SelectMany(x => x.GetData()).ToList();

            return _pushData;
        }
    }
}
