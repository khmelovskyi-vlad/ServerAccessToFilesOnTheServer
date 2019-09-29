using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    class NetServer
    {
        public NetServer()
        {
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var tcpEndPoint = new IPEndPoint(IPAddress.Any, port);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(6);
        }
        private const string ip2 = "192.168.0.107";
        private const int port = 2048;
        private Socket tcpSocket;

        public void Start(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Run();
            }
        }
        private void Run()
        {
            tcpSocket.BeginAccept(ar => {
                var listener = (Socket)ar.AsyncState;
                var socket = listener.EndAccept(ar);
                ServerManager methods = new ServerManager(socket);
                methods.Server();
                Run();
            }, tcpSocket);              
        }

    }
}
