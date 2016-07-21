using System;
using System.Net;

namespace HuangXiLib
{
    public interface IService
    {
        IPAddress ListenIP { get; set; }
        int Port { get; set; }
        IPAddress ServerIP { get; set; }
        int ServerPort { get; set; }
        ServiceStatus Status { get; set; }

        event RecieveCallback RecieveCallback;

        void Dispose();
        void Initialize(IPAddress ip, int port);
        void Send(string data);
        void Send(object data);
        void Send(byte[] data);
        void Send(IPEndPoint ep, string data);
        void Send(IPEndPoint ep, object data);
        void Send(Guid uuid, string data);
        void Send(Guid uuid, object data);
        void Send(IPEndPoint ep, byte[] data);
        void Send(Guid uuid, byte[] data);
        void Send(IPAddress ipAddr, int port, string data);
        void Send(IPAddress ipAddr, int port, object data);
        void Send(IPAddress ipAddr, int port, byte[] data);
        void SetServerEndPoint(IPAddress serverIP, int serverPort);
        void StartService();
        void StopService();
    }
}