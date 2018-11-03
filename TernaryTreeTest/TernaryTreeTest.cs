using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    [TestFixture]
    public class TernaryTreeTest
    { 
        private readonly string[] _keys = new string[] { "zero", "one", "two", "three", "four" };
        private readonly string[] _sortedKeys = new string[] { "four", "one", "three", "two", "zero" };
        private readonly int[] _sortedValues = new int[] { 4, 1, 3, 2, 0 };
        private readonly KeyValuePair<string, int>[] _sortedKVPairs = new KeyValuePair<string, int>[]
        {
            new KeyValuePair<string, int>("four", 4),
            new KeyValuePair<string, int>("one", 1),
            new KeyValuePair<string, int>("three", 3),
            new KeyValuePair<string, int>("two", 2),
            new KeyValuePair<string, int>("zero", 0)
        };
        private readonly IDictionary<string, int> _keyValueDictionary = new Dictionary<string, int>
        {
            { "zero", 0 }, { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }
        };
        private readonly ICollection<KeyValuePair<string, int>> _keyValueCollection = new List<KeyValuePair<string, int>>
        {
            { new KeyValuePair<string, int>("zero", 0) },
            { new KeyValuePair<string, int>("one", 1) },
            { new KeyValuePair<string, int>("two", 2) },
            { new KeyValuePair<string, int>("three", 3) },
            { new KeyValuePair<string, int>("four", 4) }
        };

        #region Static 'Constructors'

        [Test]
        public void Create_From_ICollection_String()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keys);
            Assert.That(subject.Count, Is.EqualTo(_keys.Length));
        }
        
        [Test]
        public void Create_From_ICollection_KVPair()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            Assert.That(subject.Count, Is.EqualTo(_keyValueCollection.Count));
        }

        [Test]
        public void Create_From_IDictionary_String_Int()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueDictionary);
            Assert.That(subject.Count, Is.EqualTo(_keyValueDictionary.Count));
        }

        #endregion

        #region Add, Remove, and Count

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Add_Key_Calls()
        {
            TernaryTree<string> subject = new TernaryTree<string>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
            Assert.That(subject.Count, Is.EqualTo(_keys.Length));
        }

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Add_Key_Value_Calls()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            foreach (KeyValuePair<string, int> kvPair in _keyValueCollection)
            {
                subject.Add(kvPair.Key, kvPair.Value);
            }
            Assert.That(subject.Count, Is.EqualTo(_keyValueCollection.Count));
        }

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Add_KeyValuePair_Calls()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            foreach (KeyValuePair<string, int> kvPair in _keyValueCollection)
            {
                subject.Add(kvPair);
            }
            Assert.That(subject.Count, Is.EqualTo(_keyValueCollection.Count));
        }

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Calls_To_Remove()
        {
            TernaryTree<string> subject = TernaryTree<string>.Create(_keys);
            foreach (string key in _keys)
            {
                subject.Remove(key);
            }
            Assert.That(subject.Count, Is.EqualTo(0));
        }

        #endregion

        #region Correctness (data in == data out)

        [Test]
        public void Tree_Contains_All_Added_Keys()
        {
            TernaryTree<string> subject = TernaryTree<string>.Create(_keys);
            Assert.Multiple(() => 
            {
                foreach (string key in _keys)
                {
                    Assert.That(subject.Contains(key));
                }
            });
        }

        [Test]
        public void Tree_ContainsKey_All_Added_Keys()
        {
            TernaryTree<string> subject = TernaryTree<string>.Create(_keys);
            Assert.Multiple(() =>
            {
                foreach (string key in _keys)
                {
                    Assert.That(subject.ContainsKey(key));
                }
            });
        }

        [Test]
        public void Contains_Returns_False_For_Invalid_Key()
        {
            TernaryTree<string> subject = TernaryTree<string>.Create(_keys);
            Assert.IsFalse(subject.Contains("invalid key"));
        }

        [Test]
        public void Keys_Returns_A_Sorted_Collection_Of_All_Keys()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueDictionary);
            ICollection<string> actualResult = subject.Keys();
            Assert.That(actualResult, Is.EqualTo(_sortedKeys));
        }

        [Test]
        public void Copy_To_Returns_Correct_Result()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            KeyValuePair<string, int>[] actualResult = new KeyValuePair<string, int>[subject.Count];
            subject.CopyTo(actualResult, 0);
            Assert.That(actualResult, Is.EqualTo(_sortedKVPairs));
        }

        #endregion

        #region Indexers

        [Test]
        public void This_Int_Returns_Correct_Key()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueDictionary);
            Assert.Multiple(() => {
                for (int i = 0; i < _sortedKeys.Length; i++)
                {
                    Assert.That(subject[i], Is.EqualTo(_sortedKeys[i]));
                }
            });
        }

        #endregion

        #region Errors

        [TestCase(-1)]
        [TestCase(5)]
        public void This_Int_Throws_IndexOutOfRangeException(int index)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            Assert.Throws<IndexOutOfRangeException>(() => 
            {
                string actualResult = subject[index];
            });
        }

        [Test]
        public void Add_Key_Throws_ArgumentException_When_Adding_Duplicate_Keys()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            Assert.Throws<ArgumentException>(() =>
            {
                subject.Add(_keys[0]);
                subject.Add(_keys[0]);
            });
        }

        [Test]
        public void Add_Key_Value_Throws_ArgumentException_When_Adding_Duplicate_Keys()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            Assert.Throws<ArgumentException>(() => 
            {
                subject.Add(_keys[0], 0);
                subject.Add(_keys[0], 0);
            });
        }

        [Test]
        public void Add_KeyValuePair_Throws_ArgumentException_When_Adding_Duplicate_Keys()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            KeyValuePair<string, int> kvPair = new KeyValuePair<string, int>(_keys[0], 0);
            Assert.Throws<ArgumentException>(() => 
            {
                subject.Add(kvPair);
                subject.Add(kvPair);
            });
        }

        #endregion
    }
}
