using System;
using System.Collections.Generic;
using System.IO;

namespace Line_Numbers
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = new List<string>();
            string line;
            int counter = 1;
            using (StreamReader reader = new StreamReader("../../../../../Resources/text.txt"))
            {
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    } else
                    {
                        lines.Add("Line " + counter + ": " + line);
                    }
                    counter++;
                }

            }

            using (StreamWriter outputFile = new StreamWriter("../../../../../Resources/output.txt"))
            {
                foreach (string newLine in lines)
                {
                    outputFile.WriteLine(newLine);
                }
            }
        }
    }
}
