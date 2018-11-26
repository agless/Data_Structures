using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    // TODO: add 'nonMatchingKeys' parameter to all tests and assert that actual results do not contain
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

        [TestCase("zero")]
        [TestCase("one")]
        [TestCase("two")]
        [TestCase("three")]
        [TestCase("four")]
        public void Exact_Match(string matchKey)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject.Match(matchKey);
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo(matchKey));
            });
        }

        [TestCase(".ero", "zero")]
        [TestCase("o.e", "one")]
        [TestCase(".w.", "two")]
        [TestCase("t...e", "three")]
        [TestCase("f...", "four")]
        public void Wildcard_Match(string pattern, string expectedResult)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject.Match(pattern);
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo(expectedResult));
            });
        }

        [TestCase("ab*a", "abbbbbba")]
        [TestCase("a*b", "aaaaaaaab")]
        [TestCase("ab*", "abbbbbbbb")]
        [TestCase("ab*c", "ac")]
        [TestCase("ab*", "a")]
        public void Repeating_Literal(string pattern, string expectedResult)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            subject.Add(expectedResult);
            ICollection<string> actualResult = subject.Match(pattern);
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo(expectedResult));
            });
        }

        [TestCase("a.*a", "abbbcbcccbcbcbbbcccbba")]
        [TestCase(".*a", "bcbcccbcbcbbbcbcbcccbcbbbbcbcbcba")]
        [TestCase("a.*", "abcbcbcbcbcbcbcbcbcbbcccbcbcbccbbbcbc")]
        [TestCase("a.*", "a")]
        [TestCase("a.*c", "ac")]
        public void Repeating_Dot(string pattern, string expectedResult)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            subject.Add(expectedResult);
            ICollection<string> actualResult = subject.Match(pattern);
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public void Prefix_Exact()
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            ICollection<string> actualResult = subject.Match("t.*");
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() => 
            {
                Assert.That(resultArray.Length, Is.EqualTo(2));
                Assert.That(resultArray[0], Is.EqualTo("three"));
                Assert.That(resultArray[1], Is.EqualTo("two"));
            });
        }

        [TestCase(".*a.*", "bbbbbbbbbbbbbbbacccccccccccccc")]
        [TestCase(".*test_case.*", "this can be anything __ test_case ___ ")]
        [TestCase(".*first.*second.*third.*", "xxx_first_xxx_second_xxx_third_xxx")]
        public void Contains_Exact(string pattern, string matchingKey)
        {
            TernaryTree<int> subject = TernaryTree<int>.Create(_keyValueCollection);
            subject.Add(matchingKey);
            ICollection<string> actualResult = subject.Match(pattern);
            string[] resultArray = new string[actualResult.Count];
            actualResult.CopyTo(resultArray, 0);
            Assert.Multiple(() =>
            {
                Assert.That(resultArray.Length, Is.EqualTo(1));
                Assert.That(resultArray[0], Is.EqualTo(matchingKey));
            });
        }

        [TestCase("[a-z]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "A", "B", "C", "D", "E" })]
        [TestCase("[0-9]", new string[] { "0", "1", "2", "3", "4" }, new string[] { "A", "B", "C", "D", "E" })]
        public void Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {

        }

        [TestCase("[^0-9]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "0", "1", "2", "3", "4" })]
        [TestCase("[^a-z]", new string[] { "0", "1", "2", "3", "4" }, new string[] { "a", "b", "c", "d", "e" })]
        public void Any_But_Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {

        }

        [TestCase("[AaBbCc]", new string[] { "A", "a", "B", "b", "C", "c" }, new string[] { "D", "d", "E", "e", "F", "f" })]
        [TestCase("[ABCDE][12345]", new string[] { "A1", "B2", "C3", "D4", "E5" }, new string[] { "F6", "G7", "H8", "I9", "J0" })]
        public void Character_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {

        }

        [TestCase("[^Aa]", new string[] { "B", "b" }, new string[] { "A", "a" })]
        public void Any_But_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {

        }
    }
}
