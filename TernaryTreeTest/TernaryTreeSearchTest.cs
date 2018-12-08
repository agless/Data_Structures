using System;
using System.Collections.Generic;
using NUnit.Framework;
using TernaryTree;

namespace TernaryTreeTest
{
    class TernaryTreeSearchTest
    {
        public void Regex_Match_Test(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
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

        [TestCase("zero", new string[] { "zero" }, new string[] { "one", "two", "three", "four" })]
        [TestCase("one", new string[] { "one" }, new string[] { "zero", "two", "three", "four" })]
        [TestCase("two", new string[] { "two" }, new string[] { "zero", "one", "three", "four" })]
        [TestCase("three", new string[] { "three" }, new string[] { "zero", "one", "two", "four" })]
        [TestCase("four", new string[] { "four" }, new string[] { "zero", "one", "two", "three" })]
        public void Exact_Match(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(".ero", new string[] { "zero", "hero", "nero" }, new string[] { "none", "of", "these" })]
        [TestCase("o.e", new string[] { "one", "ole", "owe" }, new string[] { "own", "nose", "oreo" })]
        [TestCase(".w.", new string[] { "two", "ewe", "awe" }, new string[] { "tweed", "goo", "swoop" })]
        [TestCase("t...e", new string[] { "three", "twice", "trove" }, new string[] { "I", "don't", "know" })]
        [TestCase("f...", new string[] { "four", "foot", "flip" }, new string[] { "flashy", "freaking", "fools" })]
        public void Wildcard_Match(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("ab*a", new string[] { "abbbbbba", "aba", "aa" }, new string[] { "abca", "ab", "ba" })]
        [TestCase("a*b", new string[] { "aaaaaaaab", "ab", "b" }, new string[] { "acb", "cab", "ba"})]
        [TestCase("ab*", new string[] { "abbbbbbbb", "ab", "a" }, new string[] { "b", "acb", "ba", "abc" })]
        [TestCase("ab*c", new string[] { "abbbbbbbbc", "abc", "ac" }, new string[] { "acb", "cab", "bca" })]
        public void Repeating_Literal(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("a.*a", new string[] { "aa", "aba", "abca", "abbbcbcbccccbba" }, new string[] { "a", "ab", "ba", "abc" })]
        [TestCase(".*a", new string[] { "a", "ba", "bcbcccbcbcbbcbcbbbbcbcbcba" }, new string[] { "b", "ab", "cab" })]
        [TestCase("a.*", new string[] { "a", "ab", "abcbccbcbccbbbcbc" }, new string[] { "b", "ba", "baaaa" })]
        public void Repeating_Dot(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("th.*", new string[] { "this", "these", "those" }, new string[] { "nothing", "in", "here" })]
        public void Prefix_Exact(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(".*a.*", new string[] { "a", "ba", "ac", "bbbbbbbbbbbbbbbacccccccccccccc" }, new string[] { "none", "of", "these" })]
        [TestCase(".*test.*", new string[] { "test_this", "this is the test", "this test right here" }, new string[] { "tes", "est", "best" })]
        [TestCase(".*first.*second.*third.*", new string[] { "xxx_first_xxx_second_xxx_third_xxx", "firstsecondthird", "first; second; third" }, new string[] { "third", "this ain't it", "first" })]
        public void Contains_Exact(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("[a-z]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "A", "B", "C", "D", "E" })]
        [TestCase("[A-Z][0-9]", new string[] { "A0", "B1", "C2", "D3", "E4" }, new string[] { "9A", "BB", "AC", "6D", "a1" })]
        public void Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("[^0-9]", new string[] { "a", "b", "c", "d", "e" }, new string[] { "0", "1", "2", "3", "4" })]
        [TestCase("[^a-z]", new string[] { "0", "1", "2", "3", "4" }, new string[] { "a", "b", "c", "d", "e" })]
        public void Any_But_Range(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("[AaBbCc]", new string[] { "A", "a", "B", "b", "C", "c" }, new string[] { "D", "d", "E", "e", "F", "f" })]
        [TestCase("[ABCDE][12345]", new string[] { "A1", "B2", "C3", "D4", "E5" }, new string[] { "F6", "G7", "H8", "I9", "J0" })]
        [TestCase("[Ll1][Ee3][Ee3][Tt7]", new string[] { "1337", "leet", "LEET", "13eT" }, new string[] { "Elite", "lite", "leat", "light" })]
        public void Character_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase("[^Aa]", new string[] { "B", "b" }, new string[] { "A", "a" })]
        public void Any_But_Group(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(@"\d", new string[] { "1", "2", "3" }, new string[] { "a", "b", "c" })]
        [TestCase(@"\d\d\d", new string[] { "123", "911", "000" }, new string[] { "abc", "14A", "AAA" })]
        public void Escape_d(string pattern, string[] matchingKeys, string [] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(@"\D", new string[] { "a", "b", "c" }, new string[] { "1", "2", "3" })]
        [TestCase(@"\D\D\D", new string[] { "___", "   ", "\r\nA" }, new string[] { "123", "456", "789" })]
        public void Escape_D(string pattern, string [] matchingKeys, string [] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(@"\x61", new string[] { "a" }, new string[] { "b" })]
        [TestCase(@"\x77\x6F\x72\x6B\x69\x6E\x67", new string[] { "working" }, new string[] { "not working" })]
        public void Escape_x(string pattern, string [] matchingKeys, string [] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        }

        [TestCase(@"\040", new string[] { " " }, new string[] { "a", "b", "c" })]
        [TestCase(@"\10", new string[] { "\b" }, new string[] { "a", "b", "c" })]
        public void Escape_nnn_octal(string pattern, string[] matchingKeys, string[] nonMatchingKeys)
        {
            Regex_Match_Test(pattern, matchingKeys, nonMatchingKeys);
        } 

        [TestCase("*........")]
        [TestCase("....**....")]
        [TestCase("abcd[")]
        [TestCase("[abcdef")]
        [TestCase(@"\xzz")]
        public void Throws_ArgumentException_For_Bad_Pattern(string pattern)
        {
            TernaryTree<int> subject = new TernaryTree<int>();
            Assert.Throws<ArgumentException>(() => { subject.Match(pattern); });
        }
    }
}
