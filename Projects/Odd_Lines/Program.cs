using System;
using System.IO;

namespace Odd_Lines
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            int counter = 0;
            using (StreamReader reader = new StreamReader("../../../../../Resources/text.txt"))
            {
                while (true)
                {
                    line = reader.ReadLine();
                    if (counter % 2 == 1)
                    {
                        Console.WriteLine(line);
                    }
                    if (line == null)
                    {
                        break;
                    }
                    counter++;
                }

            }
        }
    }
}
