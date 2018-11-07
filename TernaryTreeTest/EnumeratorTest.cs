using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    class EnumeratorTest
    {
        private readonly ICollection<KeyValuePair<string, int>> _keyValueCollection = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("zero", 0),
            new KeyValuePair<string, int>( "one", 1),
            new KeyValuePair<string, int>( "two", 2),
            new KeyValuePair<string, int>( "three", 3),
            new KeyValuePair<string, int>( "four", 4)
        };

        private readonly ICollection<KeyValuePair<string, int>> _sortedKeyValueCollection = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("four", 4),
            new KeyValuePair<string, int>("one", 1),
            new KeyValuePair<string, int>("three", 3),
            new KeyValuePair<string, int>("two", 2),
            new KeyValuePair<string, int>("zero", 0)
        };

        [Test]
        public void Enumerator_Returns_All_Elements_In_Order()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            List<KeyValuePair<string, int>> actualResult = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> kvPair in subject)
            {
                actualResult.Add(kvPair);
            }
            Assert.That(actualResult, Is.EqualTo(_sortedKeyValueCollection));
        }
    }
}
