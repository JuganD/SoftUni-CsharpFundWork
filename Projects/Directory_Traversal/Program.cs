using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Directory_Traversal
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = "../../../../../Resources";

            List<string> filePaths = Directory.GetFiles(directory).ToList();
            List<string> fileExtensions = filePaths.Select(file => Path.GetExtension(file)).ToList();
            Dictionary<string, Dictionary<string, long>> files = new Dictionary<string, Dictionary<string, long>>();

            foreach (string extension in fileExtensions)
            {
                if (!files.ContainsKey(extension))
                {
                    files.Add(extension, new Dictionary<string, long>());
                }
            }
            foreach (string file in filePaths)
            {
                string fileName = Path.GetFileName(file);
                string fileExtension = Path.GetExtension(file);
                long fileSize = new FileInfo(file).Length;

                files[fileExtension].Add(fileName, fileSize);
            }
            Dictionary<string, Dictionary<string, long>> orderedFiles = files.OrderByDescending(x => x.Value.Count).ToDictionary(pair => pair.Key,pair => pair.Value);
            foreach (var fileExtCollection in files)
            {
                var fileCollection = fileExtCollection.Value.OrderBy(x => x.Value).ToDictionary(pair=>pair.Key,pair=>pair.Value);

                orderedFiles[fileExtCollection.Key] = fileCollection;
            }

            List<string> information = new List<string>();
            foreach (var kvp in orderedFiles)
            {
                information.Add(kvp.Key);
                foreach (var fileInfo in kvp.Value)
                {
                    string fileName = fileInfo.Key;
                    long fileSize = fileInfo.Value;
                    string fileSizeResult = "";
                    if (fileSize > 1000)
                    {
                        fileSizeResult = SizeSuffix(fileSize, 3);
                    } else
                    {
                        fileSizeResult = $"{fileSize/1024f:f3} KB";
                    }
                    information.Add($"--{fileName} - {fileSizeResult}");
                }
            }

            // Write to file
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string reportFileName = "report.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(desktopPath, reportFileName)))
            {
                foreach (string line in information)
                {
                    outputFile.WriteLine(line);
                }
            }


        }
        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
