using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static HuangXiLib.SocketUtils;

namespace HuangXiLib
{
    public class TcpService : IService
    {
        private readonly List<TcpClient> _client = new List<TcpClient>();
        private TcpListener _listener;

        public IPAddress ListenIP
        {
            get; set;
        }

        public int Port
        {
            get; set;
        }

        public ServiceStatus Status
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

        private IPEndPoint ServerIPEndPoint => IsListener ? null : new IPEndPoint(ServerIP, ServerPort);

        protected Thread WorkerThread
        {
            get; set;
        }

        private readonly List<Thread> _clientThread = new List<Thread>();

        public bool IsListener { get; }

        public bool IsListening { get; set; }

        public List<byte> Delimiter { get; set; } = Encoding.UTF8.GetBytes("\r\n\r\n").ToList();

        public int ActiveConnect { get; set; }

        public event RecieveCallback TcpConnectedCallback;
        public event RecieveCallback TcpDisconnectedCallback;
        public event RecieveCallback RecieveCallback;

        public TcpService(bool isListener)
        {
            IsListener = IsListening = isListener;
            var r = new Random();
            var local = GetLocalIP(); 
            var port = r.Next(1025, 49151); 
            Initialize(local, port);
        }

        public TcpService(IPAddress ip, bool isListener)
        {
            IsListener = IsListening = isListener;
            var r = new Random();
            var local = ip;
            var port = r.Next(1025, 49151);
            Initialize(local, port);
        }

        public TcpService(IPAddress ip, int port,bool isListener)
        {
            IsListener = IsListening = isListener;
            Initialize(ip, port);
        }

        public void Dispose()
        {
            try
            {
                _client.ForEach(x=>x.Close());
                _listener?.Stop();
            }
            catch (Exception)
            {
                _listener?.Stop();
                _client.ForEach(x => x.Close());
            }
            finally
            {
                _client.ForEach(x => ((IDisposable)x).Dispose());
            }

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

        public void Send(string data)
        {
            _client.ForEach(x => Send(x, data));
        }
        
        public void Send(object data)
        {
            _client.ForEach(x => Send(x, data));
        }

        public void Send(byte[] data)
        {
            _client.ForEach(x => Send(x, data));
        }

        public void Send(TcpClient pclient, string data)
        {
            var d = Encoding.UTF8.GetBytes(data);
            Send(pclient,d);
        }

        public void Send(TcpClient pclient, object data)
        {
            var strData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(strData);
            Send(pclient,bytes);
        }

        public void Send(TcpClient pclient,byte[] data)
        {
            if (pclient.GetState() == TcpState.Established)
            {
                var s = pclient.GetStream();
                s.Write(data, 0, data.Length);
                s.Write(Delimiter.ToArray(), 0, Delimiter.Count);
            }
        }

        public void Send(IPEndPoint ep, string data)
        {
            (from x in _client
             where (x.Client.RemoteEndPoint as IPEndPoint)?.Equals(ep) ?? false
             select x).ToList().ForEach(x => Send(x, data));
        }

        public void Send(IPEndPoint ep, object data)
        {
            (from x in _client
             where (x.Client.RemoteEndPoint as IPEndPoint)?.Equals(ep)??false
             select x).ToList().ForEach(x=>Send(x,data));
        }

        public void Send(Guid uuid, string data)
        {
            throw new InvalidOperationException();
        }

        public void Send(Guid uuid, object data)
        {
            throw new InvalidOperationException();
        }

        public void Send(IPEndPoint ep, byte[] data)
        {
            (from x in _client
             where (x.Client.RemoteEndPoint as IPEndPoint)?.Equals(ep) ?? false
             select x).ToList().ForEach(x => Send(x, data));
        }

        public void Send(Guid uuid, byte[] data)
        {
            throw new InvalidOperationException();
        }

        public void Send(IPAddress ipAddr, int port, string data)
        {
            throw new InvalidOperationException();
        }

        public void Send(IPAddress ipAddr, int port, object data)
        {
            throw new InvalidOperationException();
        }
        
        public void Send(IPAddress ipAddr, int port, byte[] data)
        {
            throw new InvalidOperationException();
        }

        public void SetServerEndPoint(IPAddress serverIP, int serverPort)
        {
            if (Status == ServiceStatus.Running) {
                throw new InvalidOperationException();
            }
            ServerIP = serverIP;
            ServerPort = serverPort;
        }

        public void StartService()
        {
            if (Status == ServiceStatus.Running)
            {
                //不能启动已经启动的服务
                return;
            }
            WorkerThread = IsListener ? new Thread(StartListen) : new Thread(StartConnect);
            WorkerThread.Start();
        }

        public void StopService()
        {
            if (IsListener) _listener.Stop();
            Status = ServiceStatus.Stop;
            if (Status == ServiceStatus.Stop) {
                return;
            }
            if (WorkerThread.IsAlive)
            {
                WorkerThread.Abort();
            }
            _clientThread.ForEach(t => {
                if (t.IsAlive)
                {
                    t.Abort();
                }
            });
            _client.ForEach(x=> {
                x.GetStream().Close();
                x.Close();
            });
        }

        private void OnConnect(TcpClient pclient)
        {
            Status = ServiceStatus.Running;
            ActiveConnect += 1;
            TcpConnectedCallback?.Invoke(pclient, new ConnectionsEventArgs(pclient.Client.RemoteEndPoint as IPEndPoint, null));
            var s = pclient.GetStream();
            var str = new List<byte>();
            while (true)
            {
                if (s.DataAvailable)
                {
                    var c = (byte)s.ReadByte();
                    str.Add(c);
                    if (str.EndsWith(Delimiter)) {
                        str.RemoveRange(str.Count  - Delimiter.Count, Delimiter.Count);
                        RecieveCallback?.Invoke(pclient, 
                            new ConnectionsEventArgs(
                                pclient.Client.RemoteEndPoint as IPEndPoint, 
                                str.ToArray())
                            );
                        str.Clear();
                    }
                }
                var state = pclient.GetState();
                if (state == TcpState.Closing  ||
                    state == TcpState.Closed   ||
                    state == TcpState.CloseWait||
                    state == TcpState.Unknown
                    ) {
                    ActiveConnect -= 1;
                    if (ActiveConnect == 0) {
                        Status = ServiceStatus.Stop;
                    }
                    TcpDisconnectedCallback?.Invoke(pclient, new ConnectionsEventArgs(pclient.Client.RemoteEndPoint as IPEndPoint, null));
                    break;
                }
            }
            _client.Remove(pclient);
            _clientThread.Remove(Thread.CurrentThread);
            Thread.CurrentThread.Abort();
        }

        private void StartListen()
        {
            _listener = new TcpListener(LocalIPEndPoint);
            _listener.Start();
            Status = ServiceStatus.Running;
            while (Status == ServiceStatus.Running) {
                var c = _listener.AcceptTcpClient();
                if (!IsListening)
                {
                    c.GetStream().Close();
                    c.Close();
                    return;
                }
                var s = new ParameterizedThreadStart(x => OnConnect((TcpClient)x));
                var t = new Thread(s);
                t.Start(c);
                _client.Add(c);
                _clientThread.Add(t);
            }
        }
        private void StartConnect()
        {
            _client.Capacity = 1;
            _client.Clear();
            if (ServerIPEndPoint.Address.Equals(IPAddress.Loopback)) {
                ListenIP = IPAddress.Loopback;
            }
            ListenIP = IPAddress.Parse("172.16.1.233");
            _client.Add(new TcpClient(LocalIPEndPoint));
            _client[0].Connect(ServerIPEndPoint);
            OnConnect(_client[0]);
        }

        public void StopListen()
        {
            if (!IsListener) {
                throw new InvalidOperationException();
            }
            
        }
    }
}