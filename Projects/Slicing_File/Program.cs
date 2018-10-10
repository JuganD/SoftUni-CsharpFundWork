using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Slicing_File
{
    class Program
    {
        // To anyone reading this: I really enjoyed this homework, because my true passion is working with files.
        // I had the idea to make it async, so that you can slice and assemble larger files a lot faster, but I figured
        // this is not the point of this homework.
        static void Main(string[] args)
        {
            // Config
            string fileLocation = "../../../../../Resources/sliceMe.mp4";
            string slicedFolderLocation = "../../../../../Resources/Sliced";
            string assembledFolderLocation = "../../../../../Resources/Assembled";

            // Core information across methods (for ease of use, it lowers performance tho)
            string fileName = Path.GetFileNameWithoutExtension(fileLocation);
            string fileExtension = Path.GetExtension(fileLocation);
            List<string> slicedFiles = new List<string>();
            List<string> zippedFiles = new List<string>();
            if (Directory.Exists(slicedFolderLocation))
            {
                slicedFiles = Directory.GetFiles(slicedFolderLocation, "*").Where(s => s.EndsWith(fileExtension) && s.StartsWith(Path.Combine(slicedFolderLocation, fileName))).ToList();
                zippedFiles = Directory.GetFiles(slicedFolderLocation, "*").Where(s => s.EndsWith(".gz") && s.StartsWith(Path.Combine(slicedFolderLocation, fileName))).ToList();
            }


            // You can comment any part below to test the different methods. They will all work separately (except assembling non-existent files).

            // SLICE
            int parts = 4;
            if (File.Exists(fileLocation))
            {
                Slice(fileLocation, slicedFolderLocation, parts);
                slicedFiles = Directory.GetFiles(slicedFolderLocation, "*").Where(s => s.EndsWith(fileExtension) && s.StartsWith(Path.Combine(slicedFolderLocation, fileName))).ToList();
            }

            // ZIP
            ZipFiles(slicedFiles);
            zippedFiles = Directory.GetFiles(slicedFolderLocation, "*").Where(s => s.EndsWith(".gz") && s.StartsWith(Path.Combine(slicedFolderLocation, fileName))).ToList();

            // UNZIP
            UnzipFiles(zippedFiles, fileExtension);


            // ASSEMBLE
            if (slicedFiles.Count > 0)
            {
                Assemble(slicedFiles, Path.Combine(assembledFolderLocation, fileName + fileExtension));
            }
        }




        // Core methods

        static void Slice(string sourceFile, string destinationDirectory, int parts)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);
            string fileExtension = Path.GetExtension(sourceFile);
            byte[] bytes = GetBytes(sourceFile);
            int size = bytes.Length / parts;

            List<List<byte>> byteChunks = bytes
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / size)
                    .Select(grp => grp.Select(x => x.Value).ToList())
                    .ToList();

            if (byteChunks.Count > parts)
            {
                byteChunks[parts - 1].AddRange(byteChunks.Last());
                byteChunks.Remove(byteChunks.Last());
            }

            EnsureDirExists(destinationDirectory);

            for (int i = 0; i < parts; i++)
            {
                WriteAllBytes(byteChunks[i], Path.Combine(destinationDirectory, fileName) + "-" + i + fileExtension);
            }
        }
        static void Assemble(List<string> files, string destinationDirectory)
        {
            List<byte> byteBuffer = new List<byte>();
            foreach (string fileLocation in files)
            {
                byteBuffer.AddRange(GetBytes(fileLocation));
            }
            EnsureDirExists(Path.GetDirectoryName(destinationDirectory));
            WriteAllBytes(byteBuffer, destinationDirectory);
        }

        static void ZipFiles(List<string> files)
        {
            byte[] fileBytes = null;
            foreach (string fileLocation in files)
            {
                fileBytes = GetBytes(fileLocation);
                using (FileStream fs = new FileStream(fileLocation, FileMode.Truncate))
                using (GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress, false))
                {
                    zipStream.Write(fileBytes, 0, fileBytes.Length);
                }
                File.Move(fileLocation, Path.ChangeExtension(fileLocation, ".gz"));
            }
        }

        private static void UnzipFiles(List<string> files, string fileExtension)
        {
            byte[] decompressedBytes = null;
            foreach (string fileLocation in files)
            {
                decompressedBytes = GetDecompressedBytes(GetBytes(fileLocation));
                WriteAllBytes(decompressedBytes.ToList(), fileLocation);
                File.Move(fileLocation, Path.ChangeExtension(fileLocation, fileExtension));
            }
        }


        // Helper methods

        static void WriteAllBytes(List<byte> bytes, string location)
        {
            using (var fs = new FileStream(location, FileMode.Create, FileAccess.Write))
            {
                byte[] arrBytes = bytes.ToArray();
                fs.Write(arrBytes, 0, arrBytes.Length);
            }
        }

        static byte[] GetBytes(string source)
        {
            byte[] bytesBuffer = null;
            using (FileStream fileStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    int length = (int)fileStream.Length;
                    bytesBuffer = new byte[length];
                    int count;
                    int sum = 0;

                    while ((count = fileStream.Read(bytesBuffer, sum, length - sum)) > 0)
                        sum += count;
                }
                finally
                {
                    fileStream.Close();
                }
            }
            return bytesBuffer;
        }
        static void EnsureDirExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        static byte[] GetDecompressedBytes(byte[] compressedBytes)
        {
            using (MemoryStream compressedStream = new MemoryStream(compressedBytes))
            using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (MemoryStream resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
