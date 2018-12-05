﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    public class TernaryTreeSearch<V>
    {
        private delegate int Transition(Node<V> node, string key);
        private List<List<Transition>> _transitions;
        private int _state;
        private string _lastSymbol;
        private List<string> _matches;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public TernaryTreeSearch(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException(nameof(pattern));
            }
            _transitions = new List<List<Transition>>();
            _buildState(0, pattern);
            _matches = new List<string>();
            _state = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public ICollection<string> Match(Node<V> head)
        {
            _ = head ?? throw new ArgumentNullException(nameof(head));
            _getBranchMatches(head, default(string));
            return _matches;
        }

        #region Private Methods

        private TernaryTreeSearch(ref List<List<Transition>> transitions, ref List<string> matches, int state)
        {
            _transitions = transitions;
            _state = state;
            _matches = matches;
        }

        private void _buildState(int pos, string pattern)
        {
            while (pos < pattern.Length)
            {
                char c = pattern[pos];
                while (_transitions.Count <= _state)
                {
                    _transitions.Add(new List<Transition>());
                }
                pos = _switchNextSymbol(pos, pattern, _transitions.Count);
                _state++;
            }
        }

        private int _switchNextSymbol(int pos, string pattern, int successState)
        {
            char c = pattern[pos];
            switch (c)
            {
                //TODO: Write private methods for each of these special characters.
                //TODO: Case insensitive mode?
                case '.':
                    pos = _handleDot(pos, pattern, successState);
                    break;
                case '\\':
                    pos = _handleEscape(pos, pattern, successState);
                    break;
                //case '^':
                //case '$':
                //case '|':
                //case '?':
                case '*':
                    pos = _handleStar(pos, pattern);
                    break;
                //case '+':
                //case '(':
                case '[':
                    pos = _handleBrackets(pos, pattern, successState);
                    break;
                //case '{':
                default:
                    pos = _handleLiteral(pos, pattern, successState);
                    break;
            }
            return pos;
        }

        private int _handleLiteral(int pos, string pattern, int successState)
        {
            char c = pattern[pos];
            _lastSymbol = c.ToString();
            _transitions[_state].Add(new Transition(_matchExact(c, successState)));
            return ++pos;
        }

        private int _handleDot(int pos, string pattern, int successState)
        {
            _lastSymbol = ".";
            _transitions[_state].Add(new Transition(_matchEverything(successState)));
            return ++pos;
        }

        private int _handleStar(int pos, string pattern)
        {
            if ((pos == 0) || (pos > 1 && pattern[pos - 1] == '*' && pattern[pos - 2] != '\\'))
            {
                _throwSyntaxError(pos, pattern);
            }
            
            // If the last symbol is repeating, add an appropriate decorator to all transitions out of the preceding state.
            // TODO: Is there a risk of double-adding keys here?  (i.e. more than one run down the same branch)
            if (pos == pattern.Length - 1 && pos > 1)
            {
                for (int i = 0; i < _transitions[_state - 2].Count; i++)
                {
                    if (pattern[pos - 1] == '.')
                    {
                        // use the more efficient prefix match function to traverse the branch
                        _transitions[_state - 2][i] = new Transition(
                            _prefixMatchDecorator(_transitions[_state - 2][i]));
                    }
                    else
                    {
                        // match _zero_ or more instances of the repeating symbol
                        _transitions[_state - 2][i] = new Transition(
                            _checkValidKeyDecorator(_transitions[_state - 2][i]));
                    }
                }
            }
            // The current last spot for transitions (which is also currently empty) shouldn't count for purposes of final state 
            _transitions.RemoveAt(_state); 
            if (pos + 1 < pattern.Length)
            {
                // If there's more pattern to match, then the repeating match shouldn't also transition forward.
                // (It should only loop - which is added below.)
                // TODO: Instead of doing this finicky delete, is there some way to just decorate the other transitions so that they return a different result?
                // See the prefix match decorator used above.
                _transitions[_transitions.Count - 1].RemoveAt(_transitions[_transitions.Count - 1].Count - 1);
            }
            // TODO: Can we add the loop state with a decorator instead?
            // Could have loop plus advance and loop without advance decorators.

            // The previous transition should loop back to it's own state
            _state--;
            _switchNextSymbol(0, _lastSymbol, _state);
            if (pos + 1 < pattern.Length)
            {
                // If there's more to match, then add a transition for the next symbol here
                // (match zero or more of the repeating symbol)
                pos = _switchNextSymbol(pos + 1, pattern, _transitions.Count);
                return pos;
            }
            return ++pos;
        }

        private int _handleBrackets(int pos, string pattern, int successState)
        {
            if (pos == pattern.Length - 1)
            {
                _throwSyntaxError(pos, pattern);
            }

            int startPos = pos;
            bool isNegativeQuery = false;
            bool isRangeQuery = false;
            LinkedList<char> matchingChars = new LinkedList<char>();

            if (pattern[++pos] == '^')
            {
                isNegativeQuery = true;
                pos++;
            }

            while (pos < pattern.Length && pattern[pos] != ']')
            {
                if (pattern[pos] == '-')
                {
                    isRangeQuery = true;
                    pos++;
                    continue;
                }
                else
                {
                    matchingChars.AddLast(pattern[pos++]);
                }
            }

            if ((pos == pattern.Length && pattern[pos - 1] != ']') ||
                (isRangeQuery && matchingChars.Count > 2))
            {
                _throwSyntaxError(pos, pattern);
            }

            if (isNegativeQuery && isRangeQuery)
            {
                char a, b;
                a = matchingChars.First.Value;
                b = matchingChars.Last.Value;
                _transitions[_state].Add(new Transition(_matchAnythingButRange(a, b, successState)));
            }
            else if (isNegativeQuery)
            {
                _transitions[_state].Add(new Transition(_matchAnythingBut(matchingChars, successState)));
            }
            else if (isRangeQuery)
            {
                char a, b;
                a = matchingChars.First.Value;
                b = matchingChars.Last.Value;
                _transitions[_state].Add(new Transition(_matchRange(a, b, successState)));
            }
            else
            {
                _transitions[_state].Add(new Transition(_matchAnyOf(matchingChars, successState)));
            }

            _lastSymbol = pattern.Substring(startPos, pos - startPos + 1);
            return ++pos;
        }

        private int _handleEscape(int pos, string pattern, int successState)
        {
            if (pos == pattern.Length - 1)
            {
                _throwSyntaxError(pos, pattern);
            }
            if (pattern[++pos] >= '0' && pattern[pos] <= '7')
            {
                // TODO: Should I include this?  Regex reference page talks about octal escape, but it's not available in C#.
                string octal;
                if ((pattern[pos + 1] >= '0' && pattern[pos + 1] <= '7') &&
                    (pattern[pos + 2] >= '0' && pattern[pos + 2] <= '7'))
                {
                    octal = pattern.Substring(pos, 3);
                }
                else if (pattern[pos + 1] >= '0' && pattern[pos + 1] <= '7')
                {
                    octal = pattern.Substring(pos, 2);
                }
                else
                {
                    _throwSyntaxError(pos, pattern);
                }
                // TODO: convert the octal string to the char it represents and add a match exact
            }
            else
            {
                switch (pattern[pos])
                {
                    case 'a':
                        _specialCharExactMatch("\\a", '\u0007', successState);
                        break;
                    case 'b':
                        _specialCharExactMatch("\\b", '\u0008', successState);
                        break;
                    case 't':
                        _specialCharExactMatch("\\t", '\u0009', successState);
                        break;
                    case 'r':
                        _specialCharExactMatch("\\r", '\u000D', successState);
                        break;
                    case 'v':
                        _specialCharExactMatch("\\v", '\u000B', successState);
                        break;
                    case 'f':
                        _specialCharExactMatch("\\f", '\u000C', successState);
                        break;
                    case 'n':
                        _specialCharExactMatch("\\n", '\u000A', successState);
                        break;
                    case 'e':
                        _specialCharExactMatch("\\e", '\u001B', successState);
                        break;
                    //case 'x': 
                    //    break;
                    //case 'c':
                    //    break;
                    //case 'u':
                    //    break;
                    // TODO: Should there be a class that just returns collection of character groups?
                    // Could be a static class with a property for each group (e.g. word, non-word, punctuation, etc.)
                    // This would make it easy to call _matchAnyOf() to build state for these escapes.
                    default:
                        _specialCharExactMatch($"\\{pattern[pos]}", pattern[pos], successState);
                        break;
                }
            }
            return ++pos;
        }

        private void _specialCharExactMatch(string lastSymbol, char special, int successState)
        {
            _lastSymbol = lastSymbol;
            _transitions[_state].Add(new Transition(_matchExact(special, successState)));
        }

        // TODO: Should _state actually just be a parameter of this method instead of a field?
        // Would also have to be a parameter of the state builder method.  Could make things easier, though.
        private void _getBranchMatches(Node<V> node, string key)
        {
            if (_state >= _transitions.Count)
            {
                return;
            }
            if (node.Smaller != null)
            {
                // TODO: Why construct a new object here?  Why not just go down the branch recursively and restore oldState after?
                TernaryTreeSearch<V> tts = new TernaryTreeSearch<V>(ref _transitions, ref _matches, _state);
                tts._getBranchMatches(node.Smaller, key);
            }
            int oldState = _state;
            string newKey = key + node.Value;
            foreach (Transition transition in _transitions[_state])
            {
                int nextState = transition.Invoke(node, key);
                if (nextState > -1)
                {
                    if (nextState == _transitions.Count && node.IsFinalNode)
                    {
                        _matches.Add(newKey);
                    }
                    if (nextState < _transitions.Count && node.Equal != null)
                    {
                        _state = nextState;
                        _getBranchMatches(node.Equal, newKey);
                        _state = oldState;
                    }
                }
            }
            if (node.Bigger != null)
            {
                _getBranchMatches(node.Bigger, key);
            }
        }

        private void _getPrefixMatches(Node<V> node, string prefix)
        {
            if (node.Smaller != null)
            {
                _getPrefixMatches(node.Smaller, prefix);
            }
            if (node.IsFinalNode)
            {
                _matches.Add(prefix + node.Value);
            }
            if (node.Equal != null)
            {
                _getPrefixMatches(node.Equal, prefix + node.Value);
            }
            if (node.Bigger != null)
            {
                _getPrefixMatches(node.Bigger, prefix);
            }
        }

        private void _throwSyntaxError(int pos, string pattern)
        {
            StringBuilder message = new StringBuilder("Invalid pattern near:");
            message.Append(Environment.NewLine);
            message.Append(pattern);
            message.Append(Environment.NewLine);
            if (pos > 0)
            {
                message.Append(' ', pos - 1);
            }
            message.Append('^');
            throw new ArgumentException(message.ToString());
        }

        #endregion

        #region Delegate Factories

        private Func<Node<V>, string, int> _prefixMatchDecorator(Transition t) => (node, key) =>
        {
            int newState = t.Invoke(node, key);
            if (newState > -1 && newState == _transitions.Count - 1)
            {
                if (node.IsFinalNode)
                {
                    _matches.Add(key + node.Value);
                }
                if (node.Equal != null)
                {
                    _getPrefixMatches(node.Equal, key + node.Value);
                }
                // We just did the whole branch in one go.  No need to explore further.
                return -1;
            }
            return newState;
        };

        private Func<Node<V>, string, int> _checkValidKeyDecorator(Transition t) => (node, key) => 
        {
            int newState = t.Invoke(node, key);
            if (newState > -1 && node.IsFinalNode)
            {
                _matches.Add(key + node.Value);
            }
            return newState;
        };

        private Func<Node<V>, string, int> _matchEverything(int successState) => (node, key) => successState;

        private Func<Node<V>, string, int> _matchNothing() => (node, key) => -1;

        private Func<Node<V>, string, int> _matchExact(char a, int successState) => (node, key) =>
        {
            if (node.Value == a)
            {
                return successState;
            }
            else
            {
                return -1;
            }
        };

        private Func<Node<V>, string, int> _matchRange(char a, char b, int successState) => (node, key) =>
        {
            if (node.Value >= _getMinChar(a, b) && node.Value <= _getMaxChar(a, b))
            {
                return successState;
            }
            else
            {
                return -1;
            }
        };

        private Func<Node<V>, string, int> _matchAnyOf(ICollection<char> matchingChars, int successState) => (node, key) =>
        {
            foreach (char match in matchingChars)
            {
                if (node.Value == match)
                {
                    return successState;
                }
            }
            return -1;
        };

        private Func<Node<V>, string, int> _matchAnythingBut(ICollection<char> nonMatchingChars, int successState) => (node, key) =>
        {
            foreach (char nonMatch in nonMatchingChars)
            {
                if (node.Value == nonMatch)
                {
                    return -1;
                }
            }
            return successState;
        };

        private Func<Node<V>, string, int> _matchAnythingButRange(char a, char b, int successState) => (node, key) =>
        {
            if (node.Value >= _getMinChar(a, b) && node.Value <= _getMaxChar(a, b))
            {
                return -1;
            }
            else
            {
                return successState;
            }
        };

        private char _getMinChar(char a, char b) => a <= b ? a : b;

        private char _getMaxChar(char a, char b) => a <= b ? b : a;

        #endregion
    }
}
