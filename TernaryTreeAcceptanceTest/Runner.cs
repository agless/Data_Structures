using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TernaryTree;

namespace TernaryTreeAcceptanceTest
{
    class Runner
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

        private long startTime = default(long);
        private long endTime = default(long);
        private List<string> keys = new List<string>();
        private TernaryTree<int> subject;

        public void Setup(int numKeys)
        {
            foreach (string tenant in tenants)
            {
                Console.Write($"Generating {numKeys / 5} keys for tenant {tenant}.  ");
                Random random = new Random();
                startTime = DateTime.Now.Ticks;
                for (int i = 0; i < numKeys / 5; i++)
                {
                    int docType = random.Next(0, docTypes.Length);
                    string key = $"{tenant}-{docTypes[docType]}-{Guid.NewGuid()}";
                    keys.Add(key);
                }
                endTime = DateTime.Now.Ticks;
                Console.Write($"Took {(endTime - startTime) / 10000} ms." + Environment.NewLine);
            }

            Console.Write(Environment.NewLine + $"Adding {numKeys} keys to TernaryTree.  ");
            startTime = DateTime.Now.Ticks;
            subject = TernaryTree<int>.Create(keys);
            endTime = DateTime.Now.Ticks;
            Console.WriteLine($"Took {(endTime - startTime) / 10000} ms." + Environment.NewLine + Environment.NewLine);
        }

        public void RunSystemRegex(string pattern)
        {
            startTime = DateTime.Now.Ticks;
            List<string> results = new List<string>();
            foreach (string key in keys)
            {
                if (Regex.IsMatch(key, pattern))
                {
                    results.Add(key);
                }
            }
            endTime = DateTime.Now.Ticks;
            Console.Write($"System regex found {results.Count} keys in {(endTime - startTime) / 10000} ms.");
        }

        public void RunTernaryTreeSearch(string pattern)
        {
            startTime = DateTime.Now.Ticks;
            ICollection<string> results = subject.Match(pattern);
            endTime = DateTime.Now.Ticks;
            Console.WriteLine($"TernaryTree found {results.Count} keys in {(endTime - startTime) / 10000} ms.");
        }
    }
}
