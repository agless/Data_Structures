using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    // TODO: Every test has the same body.  Should there only be a single test method with 100+ test cases?
    class TernaryTreeSearchTest
    {
        [TestCase("zero", new string[] { "zero" }, new string[] { "one", "two", "three", "four" })]
        [TestCase("one", new string[] { "one" }, new string[] { "zero", "two", "three", "four" })]
        [TestCase("two", new string[] { "two" }, new string[] { "zero", "one", "three", "four" })]
        [TestCase("three", new string[] { "three" }, new string[] { "zero", "one", "two", "four" })]
        [TestCase("four", new string[] { "four" }, new string[] { "zero", "one", "two", "three" })]
        public void Exact_Match(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase(".ero", new string[] { "zero", "hero", "nero" }, new string[] { "none", "of", "these" })]
        [TestCase("o.e", new string[] { "one", "ole", "owe" }, new string[] { "own", "nose", "oreo" })]
        [TestCase(".w.", new string[] { "two", "ewe", "awe" }, new string[] { "tweed", "goo", "swoop" })]
        [TestCase("t...e", new string[] { "three", "twice", "trove" }, new string[] { "I", "don't", "know" })]
        [TestCase("f...", new string[] { "four", "foot", "flip" }, new string[] { "flashy", "freaking", "fools" })]
        public void Wildcard_Match(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("ab*a", new string[] { "abbbbbba", "aba", "aa" }, new string[] { "abca", "ab", "ba" })]
        [TestCase("a*b", new string[] { "aaaaaaaab", "ab", "b" }, new string[] { "acb", "cab", "ba"})]
        [TestCase("ab*", new string[] { "abbbbbbbb", "ab", "a" }, new string[] { "b", "acb", "ba", "abc" })]
        [TestCase("ab*c", new string[] { "abbbbbbbbc", "abc", "ac" }, new string[] { "acb", "cab", "bca" })]
        public void Repeating_Literal(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("a.*a", new string[] { "aa", "aba", "abca", "abbbcbcbccccbba" }, new string[] { "a", "ab", "ba", "abc" })]
        [TestCase(".*a", new string[] { "a", "ba", "bcbcccbcbcbbcbcbbbbcbcbcba" }, new string[] { "b", "ab", "cab" })]
        [TestCase("a.*", new string[] { "a", "ab", "abcbccbcbccbbbcbc" }, new string[] { "b", "ba", "baaaa" })]
        public void Repeating_Dot(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("th.*", new string[] { "this", "these", "those" }, new string[] { "nothing", "in", "here" })]
        public void Prefix_Exact(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase(".*a.*", new string[] { "a", "ba", "ac", "bbbbbbbbbbbbbbbacccccccccccccc" }, new string[] { "none", "of", "these" })]
        [TestCase(".*test.*", new string[] { "test_this", "this is the test", "this test" }, new string[] { "tes", "est", "best" })]
        [TestCase(".*first.*second.*third.*", new string[] { "xxx_first_xxx_second_xxx_third_xxx", "firstsecondthird", "first; second; third" }, new string[] { "third", "this ain't it", "first" })]
        public void Contains_Exact(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("[a-z]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "A", "B", "C", "D", "E" })]
        [TestCase("[0-9]", new string[] { "0", "1", "2", "3", "4" }, new string[] { "A", "B", "C", "D", "E" })]
        public void Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() => 
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("[^0-9]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "0", "1", "2", "3", "4" })]
        [TestCase("[^a-z]", new string[] { "0", "1", "2", "3", "4" }, new string[] { "a", "b", "c", "d", "e" })]
        public void Any_But_Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("[AaBbCc]", new string[] { "A", "a", "B", "b", "C", "c" }, new string[] { "D", "d", "E", "e", "F", "f" })]
        [TestCase("[ABCDE][12345]", new string[] { "A1", "B2", "C3", "D4", "E5" }, new string[] { "F6", "G7", "H8", "I9", "J0" })]
        public void Character_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }

        [TestCase("[^Aa]", new string[] { "B", "b" }, new string[] { "A", "a" })]
        public void Any_But_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(matchingKeys);
            foreach (string nonMatchingKey in nonMatchingKeys)
            {
                subject.Add(nonMatchingKey);
            }
            HashSet<string> actualResult = new HashSet<string>(subject.Match(pattern));
            Assert.Multiple(() =>
            {
                Assert.That(subject.Count, Is.EqualTo(matchingKeys.Length + nonMatchingKeys.Length));
                Assert.That(actualResult.Count, Is.EqualTo(matchingKeys.Length));
                foreach (string key in matchingKeys)
                {
                    Assert.That(actualResult.Contains(key));
                }
            });
        }
    }
}
