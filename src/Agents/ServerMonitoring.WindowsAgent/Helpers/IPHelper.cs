using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
