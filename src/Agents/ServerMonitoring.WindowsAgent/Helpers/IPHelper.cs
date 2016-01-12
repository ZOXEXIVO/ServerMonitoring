using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ServerMonitoring.WindowsAgent.Helpers
{
    public static class IPHelper
    {
        public static List<string> GetAllIPs()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Select(ip => ip.ToString()).ToList();
        }
    }
}
