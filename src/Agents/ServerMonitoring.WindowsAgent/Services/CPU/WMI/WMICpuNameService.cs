using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ServerMonitoring.WindowsAgent.Services.CPU.WMI
{
    public class WMICpuNameService : ICpuNameService
    {
        public IEnumerable<string> GetCPUNames()
        {
            return from ManagementObject queryObj in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor").Get()
                          select queryObj["Name"].ToString();
        }
    }
}
