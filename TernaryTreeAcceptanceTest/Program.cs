using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TernaryTree;

namespace TernaryTreeAcceptanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Runner runner = new Runner();

            while (true)
            {
                int numKeys = default(int);
                try
                {
                    Console.Write("Enter total number of keys:");
                    string numKeysString = Console.ReadLine();
                    if (numKeysString == "exit")
                    {
                        Environment.Exit(0);
                    }
                    numKeys = Convert.ToInt32(numKeysString);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Overflow Exception.  Type 'exit' to quit or try again.");
                    continue;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Format Exception.  Type 'exit' to quit or try again.");
                    continue;
                };

                runner.Setup(numKeys);
                break;
            }

            while (true)
            {
                Console.Write("Enter a regex pattern (or type 'exit' to quit):  ");
                string pattern = Console.ReadLine();
                if (pattern == "exit")
                {
                    Environment.Exit(0);
                }
                // TODO:  Add try/catch for bad pattern
                runner.RunTernaryTreeSearch(pattern);
                runner.RunSystemRegex(pattern);
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

    }
}
