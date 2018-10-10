using System;
using System.IO;
using System.Text;

namespace Copy_Binary_File
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryStream ms = new MemoryStream();

            using (FileStream fileStream = new FileStream("../../../../../Resources/copyMe.png", FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(ms);
            }
            using (FileStream file = new FileStream("../../../../../Resources/copiedPngFile.png", FileMode.Create, System.IO.FileAccess.Write))
            {
                ms.WriteTo(file);
            }
        }
    }
}
