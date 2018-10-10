using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Word_Count
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> words = new List<string>();
            string word;
            using (StreamReader reader = new StreamReader("../../../../../Resources/words.txt"))
            {
                while (true)
                {
                    word = reader.ReadLine();
                    if (word == null)
                    {
                        break;
                    }
                    else
                    {
                        words.Add(word);
                    }
                }

            }

            List<string> lines = new List<string>();
            string line;
            using (StreamReader reader = new StreamReader("../../../../../Resources/text.txt"))
            {
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }

            }

            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            string[] textWords = GetWords(string.Join(" ", lines));
            foreach (string textWord in textWords)
            {
                if (words.Contains(textWord))
                {
                    if (!wordCount.ContainsKey(textWord))
                    {
                        wordCount.Add(textWord, 0);
                    }
                    wordCount[textWord]++;
                }
            }
            var orderedWordCount = wordCount.OrderByDescending(x => x.Value);

            using (StreamWriter outputFile = new StreamWriter("../../../../../Resources/result.txt"))
            {
                foreach (var KVP in orderedWordCount)
                {
                    outputFile.WriteLine(KVP.Key+" - "+KVP.Value);  
                }
            }
        }

        static string[] GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input.ToLower(), @"\b[\w']*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select TrimSuffix(m.Value);

            return words.ToArray();
        }
        static string TrimSuffix(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }
    }
}
