using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    public class TernaryTreeTest
    {
        private readonly string[] _keys = new string[] { "zero", "one", "two", "three", "four" };
        private readonly string[] _sortedKeys = new string[] { "four", "one", "three", "two", "zero" };

        #region State 'Constructors'

        [Test]
        public void Create_From_ICollection_String()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keys);
            Assert.That(subject.Count, Is.EqualTo(_keys.Length));
        }
        // TODO: Test the other 'constructors'
        // TODO: Use static constructors in tests (since they're more compact)

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
            for (int i = 0; i < _keys.Length; i++)
            {
                subject.Add(_keys[i], i);
            }
            Assert.That(subject.Count, Is.EqualTo(_keys.Length));
        }

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Add_KeyValuePair_Calls()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            for (int i = 0; i < _keys.Length; i++)
            {
                KeyValuePair<string, int> kvPair = new KeyValuePair<string, int>(_keys[i], i);
                subject.Add(kvPair);
            }
            Assert.That(subject.Count, Is.EqualTo(_keys.Length));
        }

        [Test]
        public void Count_Returns_Correct_Value_After_Consecutive_Calls_To_Remove()
        {
            TernaryTree<string> subject = new TernaryTree<string>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
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
            TernaryTree<string> subject = new TernaryTree<string>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
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
            TernaryTree<string> subject = new TernaryTree<string>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
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
            TernaryTree<string> subject = new TernaryTree<string>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
            Assert.IsFalse(subject.Contains("invalid key"));
        }

        [Test]
        public void Keys_Returns_A_Sorted_Collection_Of_All_Keys()
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            foreach (string key in _keys)
            {
                subject.Add(key);
            }
            ICollection<string> actualResult = subject.Keys();
            Assert.That(actualResult, Is.EqualTo(_sortedKeys));
        }

        #endregion

        #region Errors

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
