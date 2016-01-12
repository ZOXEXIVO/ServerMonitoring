using System;
using System.Collections.Generic;
using System.Linq;
using ServerMonitoring.WindowsAgent.Helpers;
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
        private readonly List<IMonitoringService> _monitoringServices;

        private readonly ServerPushData _pushData;

        public MonitoringApp()
        {
            _monitoringServices = new List<IMonitoringService>
            {
                new CpuMonitoringService(),
                new MemoryMonitoringService(),
                new DiskMonitoringService(),
                new NetworkMonitoringService()
            };

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
                MachineName = Environment.MachineName,
                IPs = IPHelper.GetAllIPs()
            };
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
