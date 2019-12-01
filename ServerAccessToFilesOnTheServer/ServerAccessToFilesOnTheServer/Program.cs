using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectSockets netServer = new ConnectSockets();
            netServer.Start(6);
            Console.ReadKey();

        }
    }
}
