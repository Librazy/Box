using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HuangXiLib
{
    public static class SocketUtils
    {
        public static IEnumerable<IPAddress> GetLocalIPs()
        {
            var hostname = Dns.GetHostName();
            //return Dns.GetHostAddresses(hostname);//该函数会返回IPv6和IPv4的地址
            return Dns.GetHostAddresses(hostname).Where(e => e.AddressFamily == AddressFamily.InterNetwork).ToList(); //只返回IPv4地址
        }

        public static IPAddress GetLocalIP() => GetLocalIPs().FirstOrDefault();

        public static bool EndsWith<T>(this IList<T> t, IList<T> s) where T : IEquatable<T>
        {
            if (s.Count == 0) return false;
            if (t.Count < s.Count) return false;
            for (int i = 0; i != s.Count; ++i) {
                if (!(s[s.Count - i - 1].Equals(t[t.Count - i - 1]))) {
                    return false;
                }
            }
            return true;
        }
        public static TcpState GetState(this TcpClient tcpClient)
        {
            var foo = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint)&&x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint));
            return foo?.State ?? TcpState.Unknown;
        }
    }
}
