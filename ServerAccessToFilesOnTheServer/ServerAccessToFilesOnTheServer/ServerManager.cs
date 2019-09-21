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
        public ServerManager(Socket listener)
        {
            this.listener = listener;
            manager = new Manager();
        }
        private Socket listener;
        private Manager manager;
        private byte[] buffer;
        private int size;
        private StringBuilder data;
        public void Server()
        {
            try
            {
                var allDisks = manager.AllDisk();
                listener.Send(Encoding.ASCII.GetBytes($"{allDisks}\r\nSelect a disc\r\nWrite name your disk"));
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
                        listener.Send(Encoding.ASCII.GetBytes("Don`t understand"));
                    }
                }
            }
            catch (SocketException)
            {
                return;
            }
        }
        private void InFolderOrFile(Socket listener)
        {
            listener.Send(Encoding.ASCII.GetBytes("Enter name folder of file"));
            while (true)
            {
                AnswerClient(listener);
                var (allDirectoriesAndFiles, directoryOrFileFount) = manager.InFolderOrFile(data.ToString());
                if (directoryOrFileFount)
                {
                    listener.Send(Encoding.ASCII.GetBytes($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P"));
                    return;
                }
                else if (allDirectoriesAndFiles == "Redact" && directoryOrFileFount == false)
                {
                    try
                    {
                        RedactFile(listener);
                        var adressName = manager.adressName;
                        BackFolder(false, listener);
                        if (data.ToString() == "???")
                        {
                            return;
                        }
                        File.WriteAllText(adressName, data.ToString(), Encoding.Default);
                        return;
                    }
                    catch (SocketException)
                    {
                        return;
                    }
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes(allDirectoriesAndFiles));
                }
            }
        }
        private void RedactFile(Socket listener)
        {
            var fileLines = File.ReadAllText(manager.adressName);
            listener.Send(Encoding.ASCII.GetBytes($"???{fileLines}"));
            AnswerClient(listener);
        }
        private void BackFolder(bool withRemove, Socket listener)
        {
            var (allDisks, findDisk) = manager.BackFolder(withRemove);
            listener.Send(Encoding.ASCII.GetBytes(allDisks));
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
                    listener.Send(Encoding.ASCII.GetBytes($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P"));
                    return;
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes("Have`t this disk, write name your disk"));
                }
            }
        }
        private void AnswerClient(Socket listener)
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
