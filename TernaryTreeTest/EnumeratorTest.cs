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

        [Test]
        public void Enumerator_Returns_Keys_In_Order()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            Assert.Multiple(() => 
            {
                foreach (KeyValuePair<string, int> kvPair in subject)
                {
                    Assert.That(_keyValueCollection.Contains(kvPair));
                }
            });
        }
    }
}
