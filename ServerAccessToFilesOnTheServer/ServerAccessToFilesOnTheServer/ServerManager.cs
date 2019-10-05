using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
        private int size;
        private StringBuilder data;
        public void Server()
        {
            try
            {
                var allDisks = manager.AllDisk();
                SendMessage($"{allDisks}\r\nSelect a disc\r\nWrite name your disk");
                SelectDisk(listener);
                while (true)
                {
                    AnswerClient(listener);

                    if (data.ToString() == "C")
                    {
                        InFolderOrFile(listener);
                    }
                    else if (data.ToString() == "P")
                    {
                        BackFolder(true, listener);
                    }
                    else
                    {
                        SendMessage("Don`t understand");
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
        private void InFolderOrFile(Socket listener)
        {
            SendMessage("Enter name folder of file");
            while (true)
            {
                AnswerClient(listener);
                var (allDirectoriesAndFiles, directoryOrFileFount) = manager.InFolderOrFile(data.ToString());
                if (directoryOrFileFount)
                {
                    SendMessage($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P");
                    return;
                }
                else if (allDirectoriesAndFiles == "Redact" && directoryOrFileFount == false)
                {
                    ReadAndSendFile(listener);
                    manager.SaveFile(data.ToString());
                    BackFolder(false, listener);
                    return;
                }
                else if (allDirectoriesAndFiles == "PathTooLongException" && directoryOrFileFount == false)
                {
                    SendMessage($"Name is too long name, wtite less");
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
        private void ReadAndSendFile(Socket listener)
        {
            var fileLines = manager.ReadFile();
            SendMessage(fileLines);
            AnswerClient(listener);
        }
        private void BackFolder(bool withRemove, Socket listener)
        {
            var (allDisks, findDisk) = manager.BackFolder(withRemove);
            SendMessage(allDisks);
            if (findDisk)
            {
                SelectDisk(listener);
            }
        }
        private void SelectDisk(Socket listener)
        {
            while (true)
            {
                AnswerClient(listener);
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
        private void AnswerClient(Socket listener)
        {
            buffer = new byte[256];
            size = 0;
            data = new StringBuilder();
            IList<ArraySegment<byte>> buffers = new ArraySegment<ArraySegment<byte>>();
            do
            {
                var s = listener.BeginReceive(buffers, SocketFlags.None, x => Console.WriteLine(), null);
                size = listener.Receive(buffer);
                data.Append(Encoding.ASCII.GetString(buffer, 0, size));
            } while (listener.Available > 0);
        }
        private void SendMessage(string message)
        {
            listener.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}
