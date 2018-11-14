using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    class TernaryTreeSearchTest
    {
        private readonly ICollection<KeyValuePair<string, int>> _keyValueCollection = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("zero", 0),
            new KeyValuePair<string, int>("one", 1),
            new KeyValuePair<string, int>("two", 2),
            new KeyValuePair<string, int>("three", 3),
            new KeyValuePair<string, int>("four", 4)
        };

        [Test]
        public void Exact_Match()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject[new TernaryTreeSearch<int>("one")];
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo("one"));
            });
        }

        [Test]
        public void Wildcard_Match()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject[new TernaryTreeSearch<int>("o.e")];
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() => 
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo("one"));
            });
        }

        [Test]
        public void Repeating_Literal()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject[new TernaryTreeSearch<int>("thre*")];
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() => 
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo("three"));
            });
        }

        [Test]
        public void Prefix_Exact()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject[new TernaryTreeSearch<int>("t.*")];
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() => 
            {
                Assert.That(resultArray.Length, Is.EqualTo(2));
                Assert.That(resultArray[0], Is.EqualTo("three"));
                Assert.That(resultArray[1], Is.EqualTo("two"));
            });
        }

        [Test]
        public void Contains_Exact()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject[new TernaryTreeSearch<int>(".*o.*")];
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() => 
            {
                Assert.That(resultArray.Length, Is.EqualTo(4));
                Assert.That(resultArray[0], Is.EqualTo("four"));
                Assert.That(resultArray[1], Is.EqualTo("one"));
                Assert.That(resultArray[2], Is.EqualTo("two"));
                Assert.That(resultArray[3], Is.EqualTo("zero"));
            });
        }
    }
}
