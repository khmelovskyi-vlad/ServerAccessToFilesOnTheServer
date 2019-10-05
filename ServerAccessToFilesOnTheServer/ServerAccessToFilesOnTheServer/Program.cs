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
            NetServer netServer = new NetServer();
            netServer.Start(6);
            Console.ReadKey();

        }
    }
}
