using ServerMonitoring.WindowsAgent.Application;

namespace ServerMonitoring.WindowsAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            new MonitoringApp().Start();
        }
    }
}
