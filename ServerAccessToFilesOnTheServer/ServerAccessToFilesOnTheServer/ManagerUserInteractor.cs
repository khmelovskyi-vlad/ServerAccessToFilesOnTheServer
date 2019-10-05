using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    class ManagerUserInteractor : IUserInteractor
    {
        public ManagerUserInteractor()
        {
        }
        public string adressName { get; set; }
        private string[] allDisks;
        private const string enter = "\r\n";

        public (string allDirectoriesAndFilesOrDisks, bool findDisk) BackFolder(bool withRemove)
        {
            var allDirectoriesAndFilesOrDisks = "";
            bool findDisk = false;
            try
            {
                if (withRemove)
                {
                    adressName = adressName.Remove(adressName.Length - 1);
                }
                adressName = Path.GetDirectoryName(adressName);
                if (!withRemove)
                {
                    adressName = $"{adressName}\\";
                }
                allDirectoriesAndFilesOrDisks = $"{OutPutFoldersAndFiles()}If you want to select a different folder or file, click C else if you want return to previous folder click P";
            }
            catch (ArgumentNullException)
            {
                findDisk = true;
                allDirectoriesAndFilesOrDisks = $"{AllDisk()}Select Disk";
            }
            return (allDirectoriesAndFilesOrDisks, findDisk);
        }
        public (string allDirectoriesAndFiles, bool directoryOrFileFount) InFolderOrFile(string fileName)
        {
            var allDirectoriesAndFiles = ($"Select a folder{enter}");
            var saveAdress = adressName;
            try
            {
                var adress = $"{fileName}\\";
                adressName = $"{adressName}{adress}";
                allDirectoriesAndFiles += OutPutFoldersAndFiles();
            }
            catch (DirectoryNotFoundException ex)
            {
                adressName = saveAdress;
                return ($"Bed input {ex}, try again", false);
            }
            catch (PathTooLongException)
            {
                adressName = saveAdress;
                return ("PathTooLongException", false);
            }
            catch (ArgumentException)
            {
                adressName = saveAdress;
                return ("ArgumentException", false);
            }
            catch (IOException)
            {
                adressName = adressName.Substring(0, adressName.Length - 1);
                return ("Redact", false);
            }
            return (allDirectoriesAndFiles, true);
        }
        public string ReadFile()
        {
            return $"???{File.ReadAllText(adressName)}";
        }
        public void SaveFile(string data)
        {
            if (data == "???")
            {
                return;
            }
            File.WriteAllText(adressName, data.ToString(), Encoding.Default);
            return;
        }
        public string AllDisk()
        {
            StringBuilder allDisksReturn = new StringBuilder();
            allDisksReturn.Append($"You have disks:{enter}");
            allDisks = Directory.GetLogicalDrives();
            foreach (var disk in allDisks)
            {
                allDisksReturn.Append($"{disk}{enter}");
            }
            return allDisksReturn.ToString();
        }
        public (string allDirectoriesAndFiles, bool diskFound) SelectDisk(string line)
        {
            adressName = $"{line}:\\";
            var flag = false;
            foreach (var disk in allDisks)
            {
                if (disk == adressName)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return (OutPutFoldersAndFiles(), flag);
            }
            else
            {
                return ("", flag);
            }
        }
        private string OutPutFoldersAndFiles()
        {
            StringBuilder allDirectoriesAndFiles = new StringBuilder();
            var allDirectories = Directory.GetDirectories(adressName);
            var allFiles = Directory.GetFiles(adressName);
            allDirectoriesAndFiles.Append(enter);
            //var allFiles = allDirectories.Concat(allDocument);
            if (allDirectories.Length != 0)
            {
                allDirectoriesAndFiles.Append($"This directory has following directories:{enter}");
                foreach (var directory in allDirectories)
                {
                    allDirectoriesAndFiles.Append($"{Path.GetFileName(directory)}{enter}");
                }
            }
            if (allFiles.Length != 0)
            {
                allDirectoriesAndFiles.Append($"This directory has following files:{enter}");
                foreach (var file in allFiles)
                {
                    allDirectoriesAndFiles.Append($"{Path.GetFileName(file)}{enter}");
                }
            }
            return allDirectoriesAndFiles.ToString();
        }
    }
}
