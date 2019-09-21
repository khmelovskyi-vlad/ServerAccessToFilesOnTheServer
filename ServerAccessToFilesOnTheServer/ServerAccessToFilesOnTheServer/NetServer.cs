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
        }
        private const string ip2 = "192.168.0.105";
        private const int port = 2048;
        private Socket tcpSocket;

        public void Start()
        {
            Thread thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }
        private void Run()
        {
            tcpSocket.Listen(1);
            while (true)
            {
                using (Socket listener = tcpSocket.Accept())
                {
                    ServerManager methods = new ServerManager(listener);
                    methods.Server();
                }
            }
        }

    }
}
