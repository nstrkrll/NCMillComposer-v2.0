using NCMillComposer.Models;
using System.Collections.Generic;
using System.IO;

namespace NCMillComposer.Services
{
    public static class FilesHandler
    {
        public static List<FileInfo> GetFilesList()
        {
            var filesList = new List<FileInfo>();
            if (Directory.Exists(Settings.FirstDirectoryForSearch))
            {
                var directoryInfo = new DirectoryInfo(Settings.FirstDirectoryForSearch);
                filesList.AddRange(directoryInfo.GetFiles("*.plt"));
            }

            if (Directory.Exists(Settings.FirstDirectoryForSearch))
            {
                var directoryInfo = new DirectoryInfo(Settings.SecondDirectoryForSearch);
                filesList.AddRange(directoryInfo.GetFiles("*.plt"));
            }

            return filesList;
        }
    }
}