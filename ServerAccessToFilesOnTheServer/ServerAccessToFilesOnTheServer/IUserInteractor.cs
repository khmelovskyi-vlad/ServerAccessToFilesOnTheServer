using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAccessToFilesOnTheServer
{
    interface IUserInteractor
    {
        (string allDirectoriesAndFilesOrDisks, bool findDisk) BackFolder(bool withRemove);
        (string allDirectoriesAndFiles, bool directoryOrFileFount) InFolderOrFile(string fileName);
        string AllDisk();
        (string allDirectoriesAndFiles, bool diskFound) SelectDisk(string line);
        string ReadFile();
        void SaveFile(string data);

    }
}
