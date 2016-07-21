using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HuangXiLib
{
    public class UdpService : IDisposable, IService
    {
        private UdpClient listenClient;
        private UdpClient sendingClient;

        public ServiceStatus Status
        {
            get; set;
        }

        public IPAddress ListenIP
        {
            get; set;
        }

        public int Port
        {
            get; set;
        }

        private IPEndPoint LocalIPEndPoint => new IPEndPoint(ListenIP, Port);

        public IPAddress ServerIP
        {
            get; set;
        }

        public int ServerPort
        {
            get; set;
        }

        private IPEndPoint ServerIPEndPoint => new IPEndPoint(ServerIP, ServerPort);

        protected Thread listenThread
        {
            get; set;
        }

        #region 初始化
        public UdpService()
        {
            var r = new Random();
            var local = SocketUtils.GetLocalIP(); //取本机
            var port = r.Next(1025, 49151); //随意从注册端口里找一个出来
            //端口范围：0-1024 知名端口，1025-49151 注册端口，49152-65535 动态端口

            Initialize(local, port);
        }
        
        public UdpService(IPAddress ip, int port)
        {
            Initialize(ip, port);
        }

        public void Initialize(IPAddress ip, int port)
        {
            ListenIP = ip;
            Port = port;
            Status = ServiceStatus.Stop;

            RecieveCallback += (obj, e) =>
            {

            };
        }

        public void SetServerEndPoint(IPAddress serverIP, int serverPort)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;
        }
        #endregion

        public event RecieveCallback RecieveCallback;

        public void StartService()
        {
            if (Status == ServiceStatus.Running)
            {
                //不能启动已经启动的服务
                return;
            }

            listenClient = new UdpClient(LocalIPEndPoint);
            sendingClient = new UdpClient();

            Status = ServiceStatus.Running;
            listenThread = new Thread(OnListen);
            listenThread.Start();
        }

        public void StopService()
        {
            if (Status == ServiceStatus.Stop)
            {
                //不能关闭没有启动的服务
                return;
            }

            listenThread?.Abort();

            listenClient?.Close();

            Status = ServiceStatus.Stop;
        }

        private void OnListen()
        {
            while(true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var data = listenClient.Receive(ref remoteEP);

                var ipBytes = new byte[4];
                Array.Copy(data, 0, ipBytes, 0, 4);
                var portBytes = new byte[4];
                Array.Copy(data, 4, portBytes, 0, 4);
                var finalData = new byte[data.Length - 8];
                Array.Copy(data, 8, finalData, 0, data.Length - 8);

                var ip = new IPAddress(ipBytes);
                var port = BitConverter.ToInt32(portBytes, 0);
                remoteEP = new IPEndPoint(ip, port);

                var e = new ConnectionsEventArgs(remoteEP, finalData);
                RecieveCallback?.Invoke(this, e);
            }//while true
            // ReSharper disable once FunctionNeverReturns
        }

        #region 发送
        public void Send(byte[] data)            
        {
            var ipBytess = ListenIP.GetAddressBytes();
            var portBytes = BitConverter.GetBytes(Port);
            var final = new byte[ipBytess.Length + portBytes.Length + data.Length];

            var offset = 0;
            ipBytess.CopyTo(final, 0);
            offset += ipBytess.Length;
            portBytes.CopyTo(final, offset);
            offset += portBytes.Length;
            data.CopyTo(final, offset);

            sendingClient.Send(final, final.Length, ServerIPEndPoint);
        }

        public void Send(IPEndPoint ep, byte [] data)
        {
            var ipBytess = ListenIP.GetAddressBytes();
            var portBytes = BitConverter.GetBytes(Port);
            var final = new byte[ipBytess.Length + portBytes.Length + data.Length];

            var offset = 0;
            ipBytess.CopyTo(final, 0);
            offset += ipBytess.Length;
            portBytes.CopyTo(final, offset);
            offset += portBytes.Length;
            data.CopyTo(final, offset);
            sendingClient.Send(final, final.Length, ep);
        }

        public void Send(IPAddress ipAddr, int port, byte[] data)
        {
            var ep = new IPEndPoint(ipAddr, port);
            Send(ep, data);
        }

        /// <summary>
        /// UDP是无连接协议，所以该函数在UDP下是无效的
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="data"></param>
        public void Send(Guid uuid, byte[] data)
        {
            throw new InvalidOperationException();
        }

        public void Send(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            Send(bytes);
        }

        public void Send(IPEndPoint ep, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            Send(ep, bytes);
        }

        public void Send(IPAddress ipAddr, int port, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            Send(ipAddr, port, bytes);
        }

        /// <summary>
        /// UDP是无连接协议，所以该函数在UDP下是无效的
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="data"></param>
        public void Send(Guid uuid, string data)
        {
            throw new InvalidOperationException();
        }


        public void Send(object data)
        {
            var strData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(strData);
            Send(bytes);
        }

        public void Send(IPEndPoint ep, object data)
        {
            var strData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(strData);
            Send(ep, bytes);
        }

        public void Send(IPAddress ipAddr, int port, object data)
        {
            var strData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(strData);
            Send(ipAddr, port, bytes);
        }

        /// <summary>
        /// UDP是无连接协议，所以该函数在UDP下是无效的
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="data"></param>
        public void Send(Guid uuid, object data)
        {
            throw new InvalidOperationException();
        }
        #endregion
        public void Dispose()
        {
            try
            {
                listenThread?.Abort();
                listenClient?.Close();
            }
            catch (Exception) {
                listenClient?.Close();
                listenThread?.Abort();
            }
            finally {
                var disposable = (IDisposable)listenClient;
                disposable?.Dispose();
                ((IDisposable)sendingClient).Dispose();
            }
        }
    }
}
