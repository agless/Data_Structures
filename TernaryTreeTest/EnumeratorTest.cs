using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    class EnumeratorTest
    {
        private readonly IDictionary<string, int> _keyValueDictionary = new Dictionary<string, int>
        {
            { "zero", 0 }, { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }
        };

        [Test]
        public void Enumerator_Returns_Keys_In_Order_On_Repeated_Calls_To_Move_Next_And_Current()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueDictionary);
            Assert.Multiple(() => 
            {
                foreach (KeyValuePair<string, int> kvPair in subject)
                {
                    Assert.That(_keyValueDictionary.Contains(kvPair));
                }
            });
        }
    }
}
