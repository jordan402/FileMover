using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Configuration;
using System;

namespace FileMover
{
    class Program
    {
        private readonly IEnumerable<string> FileTypes = new List<string>
        {
            ".jpg",
            ".png",
            ".mp4"
        };
        private static readonly string location = ConfigurationManager.AppSettings["FileLocation"];

        static void Main(string[] args)
        {
            if(string.IsNullOrWhiteSpace(location))
            {
                Console.WriteLine("File location not set in config.");
                Console.ReadLine();
                return;
            }

            var program = new Program();
            program.Process();
        }

        private void Process()
        {
            foreach(var file in Directory.EnumerateFiles(location).Select(f => new FileInfo(f)).Where(i => FileTypes.Contains(i.Extension)))
            {
                var folderName = CreateAndGetFolder(file);
                MoveFile(folderName, file);
            }
        }

        private void MoveFile(string folderName, FileInfo file)
        {
            var newPath = Path.Combine(folderName, file.Name);
            File.Move(file.FullName, newPath);
        }

        private string CreateAndGetFolder(FileInfo info)
        {
            var monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(info.LastWriteTime.Month);
            var folderName = Path.Combine(location, $"{monthName} - {info.LastWriteTime.Year}");
            Directory.CreateDirectory(folderName);
            return folderName;
        }
    }
}
