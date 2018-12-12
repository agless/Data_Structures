using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TernaryTree;

namespace TernaryTreeAcceptanceTest
{
    class Program
    {
        private static string[] tenants = new string[]
        {
            "ABC Co.",
            "XYZ Co.",
            "The Great Parcheisi",
            "The Group, LLC",
            "Dingleberg, Inc."
        };

        private static string[] docTypes = new string[]
        {
            "photo_id",
            "passport",
            "invoice",
            "receipt",
            "customer_history",
            "correspondence"
        };

        static void Main(string[] args)
        {
            long startTime = default(long);
            long endTime = default(long);
            List<string> keys = new List<string>();
            foreach (string tenant in tenants)
            {
                Console.Write($"Generating 10,000 keys for tenant {tenant}.  ");
                Random random = new Random();
                startTime = DateTime.Now.Ticks;
                for (int i = 0; i < 10000; i++)
                {
                    int docType = random.Next(0, docTypes.Length);
                    string key = $"{tenant}.{docTypes[docType]}.{Guid.NewGuid()}";
                    keys.Add(key);
                }
                endTime = DateTime.Now.Ticks;
                Console.Write($"Took {(endTime - startTime) / 10000} ms." + Environment.NewLine);
            }

            Console.Write(Environment.NewLine + "Adding all keys to TernaryTree.  ");
            startTime = DateTime.Now.Ticks;
            TernaryTree<int> subject = TernaryTree<int>.Create(keys);
            endTime = DateTime.Now.Ticks;
            Console.WriteLine($"Took {(endTime - startTime) / 10000} ms." + Environment.NewLine + Environment.NewLine);

            string pattern = @"ABC Co\..*";
            Console.Write($"Finding all records for ABC Co.: {pattern}" + Environment.NewLine + Environment.NewLine);
            Console.WriteLine("Running System regex across list.");
            startTime = DateTime.Now.Ticks;
            int count = 0;
            foreach (string key in keys)
            {
                if (Regex.IsMatch(key, pattern))
                {
                    count++;
                }
            }
            endTime = DateTime.Now.Ticks;
            Console.WriteLine($"Found {count} keys in {(endTime - startTime) / 10000} ms." + Environment.NewLine);
            Console.WriteLine("Running TernaryTreeSearch.");
            startTime = DateTime.Now.Ticks;
            ICollection<string> results = subject.Match(pattern);
            endTime = DateTime.Now.Ticks;
            Console.WriteLine($"Found {results.Count} keys in {(endTime - startTime) / 10000} ms." + Environment.NewLine + Environment.NewLine);

            // TODO:  Write an input loop to accept regex and run the above comparison on the pattern.
            Console.ReadLine();
        }
    }
}
