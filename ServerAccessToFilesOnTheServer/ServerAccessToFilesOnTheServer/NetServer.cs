using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    class NetServer
    {
        public NetServer()
        {
            manager = new Manager();
        }
        Manager manager;
        const string ip2 = "192.168.0.105";
        const int port = 2048;

        byte[] buffer;
        int size;
        StringBuilder data;
        Socket listener;

        public void Server()
        {
            var tcpEndPoint = new IPEndPoint(IPAddress.Any, port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(6);
            listener = tcpSocket.Accept();

            var allDisks = manager.AllDisk();
            listener.Send(Encoding.ASCII.GetBytes($"{allDisks}\r\nSelect a disc\r\nWrite name your disk"));
            SelectDisk();
            while (true)
            {
                AnswerClient();

                if (data.ToString() == "C")
                {
                    InFolderOrFile();
                }
                else if (data.ToString() == "P")
                {
                    BackFolder(true);
                }
                else if (data.ToString() == "Q")
                {
                    listener.Send(Encoding.ASCII.GetBytes("Don`t understand"));
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes("Don`t understand"));
                }
            }
        }
        private void InFolderOrFile()
        {
            listener.Send(Encoding.ASCII.GetBytes("Enter name folder of file"));
            while (true)
            {
                AnswerClient();
                var (allDirectoriesAndFiles, directoryOrFileFount) = manager.InFolderOrFile(data.ToString());
                if (directoryOrFileFount)
                {
                    listener.Send(Encoding.ASCII.GetBytes($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P"));
                    return;
                }
                else if (allDirectoriesAndFiles == "Redact" && directoryOrFileFount == false)
                {
                    RedactFile();
                    if (data.ToString() == "rrrrrrrrrrrrrr")
                    {
                        BackFolder(false);
                        return;
                    }
                    File.WriteAllText(manager.adressName, data.ToString(), Encoding.Default);
                    BackFolder(false);
                    return;
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes(allDirectoriesAndFiles));
                }
            }
        }
        private void RedactFile()
        {
            var fileLines = File.ReadAllText(manager.adressName);
            listener.Send(Encoding.ASCII.GetBytes($"rrrrrrrrrrrrrr{fileLines}"));
            AnswerClient();
        }
        private void BackFolder(bool withRemove)
        {
            var (allDisks, findDisk) = manager.BackFolder(withRemove);
            listener.Send(Encoding.ASCII.GetBytes(allDisks));
            if (findDisk)
            {
                SelectDisk();
            }
        }
        private void SelectDisk()
        {
            while (true)
            {
                AnswerClient();
                var (allDirectoriesAndFiles, diskFound) = manager.SelectDisk(data.ToString());
                if (diskFound)
                {
                    listener.Send(Encoding.ASCII.GetBytes($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P"));
                    return;
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes("Have`t this disk, write name your disk"));
                }
            }
        }
        private void AnswerClient()
        {
            buffer = new byte[256];
            size = 0;
            data = new StringBuilder();

            do
            {
                try
                {
                    size = listener.Receive(buffer);
                }
                catch (SocketException)
                {

                }
                data.Append(Encoding.ASCII.GetString(buffer, 0, size));
            } while (listener.Available > 0);
        }

    }
}
