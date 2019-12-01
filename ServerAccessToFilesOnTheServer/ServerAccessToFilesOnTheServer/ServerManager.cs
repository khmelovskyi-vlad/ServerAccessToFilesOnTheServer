using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    class ServerManager
    {
        IUserInteractor manager;
        public ServerManager(Socket listener, IUserInteractor userInteractor)
        {
            this.manager = userInteractor;
            this.listener = listener;
        }
        private Socket listener;
        private byte[] buffer;
        const int size = 256;
        private StringBuilder data;
        public void FindFiles()
        {
            try
            {
                var allDisks = manager.AllDisk();
                SendMessage($"{allDisks}\r\nSelect a disc\r\nWrite name your disk");
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
                    else
                    {
                        SendMessage("Don`t understand");
                    }
                }
            }
            catch (SocketException ex)
            {
                listener.Close();
                Console.WriteLine(ex);
                return;
            }
        }
        private void InFolderOrFile()
        {
            SendMessage("Enter name folder of file");
            while (true)
            {
                AnswerClient();
                var (allDirectoriesAndFiles, directoryOrFileFount) = manager.InFolderOrFile(data.ToString());
                if (directoryOrFileFount)
                {
                    SendMessage($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P");
                    return;
                }
                else if (allDirectoriesAndFiles == "Redact" && directoryOrFileFount == false)
                {
                    ReadAndSendFile();
                    manager.SaveFile(data.ToString());
                    BackFolder(false);
                    return;
                }
                else if (allDirectoriesAndFiles == "PathTooLongException" && directoryOrFileFount == false)
                {
                    SendMessage($"Name is too long name, write less");
                }
                else if (allDirectoriesAndFiles == "ArgumentException" && directoryOrFileFount == false)
                {
                    SendMessage($"Bed input {data.ToString()}, try again");
                }
                else
                {
                    SendMessage(allDirectoriesAndFiles);
                }
            }
        }
        private void ReadAndSendFile()
        {
            var fileLines = manager.ReadFile();
            SendMessage(fileLines);
            AnswerClient();
        }
        private void BackFolder(bool withRemove)
        {
            var (allDisks, findDisk) = manager.BackFolder(withRemove);
            SendMessage(allDisks);
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
                    SendMessage($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P");
                    return;
                }
                else
                {
                    SendMessage("Have`t this disk, write name your disk");
                }
            }
        }
        AutoResetEvent resetSend = new AutoResetEvent(false);
        AutoResetEvent resetReceive = new AutoResetEvent(false);
        private void AnswerClient()
        {
            buffer = new byte[size];
            data = new StringBuilder();
            do
            {
                var k = listener.BeginReceive(buffer, 0, size, SocketFlags.None, ReceiveCallback, listener);
                resetReceive.WaitOne();
            } while (listener.Available > 0);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                resetReceive.Set();
                return;
            }
            data.Append(Encoding.ASCII.GetString(buffer, 0, received));
            resetReceive.Set();
        }
        private void SendMessage(string message)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(message);
            listener.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, listener);
            resetSend.WaitOne();
        }
        private void SendCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            try
            {
                current.EndSend(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Can`t send message");
            }
            resetSend.Set();
        }
    }
}
