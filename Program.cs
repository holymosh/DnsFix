using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace DnsFix
{
    class Program
    {
        static void Main(string[] args)
        {
            var wirelessInterface = NetworkInterface.GetAllNetworkInterfaces()
                .SingleOrDefault(netInterface => netInterface.Description.Contains("Intel"));
            var gateWay = wirelessInterface?.GetIPProperties().GatewayAddresses.First().Address;
            var networkConfigs = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var configs = networkConfigs.GetInstances();
            var intelWireless = configs.Cast<ManagementObject>().Where(o => (bool) o["IpEnabled"])
                .SingleOrDefault(o => o.Properties["Caption"].Value.ToString().Contains("Intel"));
            var value = (string[]) intelWireless.Properties["DNSServerSearchOrder"].Value;
            Console.WriteLine(value[0]);
            var newDNS = intelWireless.GetMethodParameters("SetDNSServerSearchOrder");
            newDNS["DNSServerSearchOrder"] = gateWay.ToString().Split(',');
            var setDNS = intelWireless.InvokeMethod("SetDNSServerSearchOrder", newDNS,null);
        }
    }
}
