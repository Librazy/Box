using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuangXiLib
{
    public delegate void RecieveCallback(object sender, ConnectionsEventArgs e);

    public class ConnectionsEventArgs : EventArgs
    {
        public ConnectionsEventArgs(IPEndPoint remoteEP, byte[] data)
        {
            RemoteEP = remoteEP;
            Data = data;
        }

        /// <summary>
        /// 该连接所对应的Socket，在UDP中无效
        /// </summary>
        public Socket Socket
        {
            get; set;
        }

        public IPEndPoint RemoteEP
        {
            get; set;
        }

        /// <summary>
        /// 收到的二进制数据
        /// </summary>
        public byte[] Data
        {
            get; set;
        }
        /// <summary>
        /// 转换为UTF8格式的文本数据
        /// </summary>
        public string Message
        {
            get { return Encoding.UTF8.GetString(Data); }
            set { Data = Encoding.UTF8.GetBytes(value); }
        }
    }
}
