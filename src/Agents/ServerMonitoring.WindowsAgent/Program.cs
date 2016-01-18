using System;
using ServerMonitoring.WindowsAgent.Application;

namespace ServerMonitoring.WindowsAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = args[0]; 
            string machineName = args[1];
        
            if (!host.StartsWith("http://"))
                host = "http://" + host;

            new MonitoringApp(host, machineName).Start();
        }
    }
}
