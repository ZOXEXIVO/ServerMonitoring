using System;
using System.Management;

namespace ServerMonitoring.WindowsAgent.Services.ComputerID.MacIDService
{
    public class MacAddressIdService : IComputerIdService
    {
        public string GetCurrentComputerId()
        {
            var wmiObjects = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

            var wmiNetwork = wmiObjects.Get();

            string macAddress = string.Empty;
            foreach (var networkInterface in wmiNetwork)
            {
                string macAddr = Convert.ToString(networkInterface["MacAddress"]);
                
                if (!string.IsNullOrWhiteSpace(macAddr))
                    macAddress = macAddr;

                networkInterface.Dispose();
            }

            macAddress = macAddress.Replace(":", "");

            return macAddress;
        }
    }
}
