using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Keep.Common.Utilitiy
{
    public static class DnsHelper
    {
        public static string ResolveHostName()
        {
            try
            {
                var hostName = Dns.GetHostName();
                if (!string.IsNullOrEmpty(hostName))
                {
                    var entry = Dns.GetHostEntry(hostName);
                    if (entry != null)
                    {
                        return entry.HostName;
                    }
                }
                return hostName;
            }
            catch (Exception)
            {
                // Ignore
                return null;
            }
        }

        public static string ResolveIpAddress(string hostName)
        {
            try
            {
                var entry = Dns.GetHostEntry(hostName);
                var ipAddress = entry?.AddressList?
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.ToString())
                    .FirstOrDefault();
                return ipAddress;
            }
            catch (Exception)
            {
                // Ignore
                return null;
            }
        }
    }
}
