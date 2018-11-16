using System;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    // TODO: Need to make an object to be a regex-like state machine.
    // Constructor takes a regex pattern and uses it to build logic for each state.  (AKA, the hard part.)

    public class TernaryTreeSearch<V>
    {
        private delegate int Transition(char c);
        private List<List<Transition>> _transitions;
        private int _state;

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
            _state = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public ICollection<string> Match(Node<V> head)
        {
            ICollection<string> matches = new LinkedList<string>();
            _getBranchMatches(head, default(string), matches);
            return matches;
        }

        #region Private Methods

        private TernaryTreeSearch(ref List<List<Transition>> transitions, int state)
        {
            _transitions = transitions;
            _state = state;
        }

        private void _buildState(int pos, string pattern)
        {
            while (pos < pattern.Length)
            {
                char c = pattern[pos];
                if (_transitions.Count == _state)
                {
                    _transitions.Add(new List<Transition>());
                }
                switch (c)
                {
                    case '.':
                        pos = _handleDot(pos, pattern);
                        continue;
                    //TODO: Write private methods for each of these special characters.
                    //TODO: Case insensitive mode?
                    //case '\\':
                    //    pos = _handleEscape(pos, pattern);
                    //    continue;
                    //case '^':
                    //case '$':
                    //case '|':
                    //case '?':
                    //case '*': // This could be handled by doing lookahead on every other symbol.  Seems repetitive.
                    //case '+':
                    //case '(':
                    //case '[':
                    //case '{':
                        //TODO: Handle grouping.
                        //pos = _handleGroup(pos, pattern);
                        //continue;
                    //case ']':
                    //case ')':
                    //case '}':
                    default:
                        pos = _handleLiteral(pos, pattern);
                        break;
                }
            }
        }

        private bool _checkRepeating(int pos, string pattern)
        {
            // TODO: Does repeating mean _zero_ or more instances?
            // If so, I'm doing it wrong.
            if (pos < pattern.Length - 1 && pattern[pos + 1] == '*')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int _handleLiteral(int pos, string pattern)
        {
            int finalPos = pos;
            if (_checkRepeating(pos, pattern))
            {
                Transition repeat = new Transition(_matchExact(pattern[pos], _state));
                _transitions[_state].Add(repeat);
                finalPos++;
            }
            char c = pattern[pos];
            Transition t = new Transition(_matchExact(c, _transitions.Count));
            _transitions[_state++].Add(t);
            return ++finalPos;
        }

        private int _handleDot(int pos, string pattern)
        {
            int finalPos = pos;
            if (_checkRepeating(pos, pattern)) 
            {
                Transition repeat = new Transition(_matchEverything(_state));
                _transitions[_state].Add(repeat);
                finalPos++;
            }
            Transition t = new Transition(_matchEverything(_transitions.Count));
            _transitions[_state++].Add(t);
            return ++finalPos;
        }

        //private int _handleEscape(int pos, string pattern)
        //{
        //    if (pos + 1 >= pattern.Length)
        //    {
        //        // An escape character was passed as the last character in a string.
        //        // Could just ignore?
        //        // Otherwise:
        //        // _handleSyntaxError()
        //    }
        //    else
        //    {
        //        // TODO: Call the right method for the escaped character.
        //        // Switch statement?
        //    }
        //    return pos;
        //}

        private void _getBranchMatches(Node<V> node, string key, ICollection<string> matches)
        {
            if (_state >= _transitions.Count)
            {
                return;
            }
            if (node.Smaller != null)
            {
                TernaryTreeSearch<V> tts = new TernaryTreeSearch<V>(ref _transitions, _state);
                tts._getBranchMatches(node.Smaller, key, matches);
            }
            int oldState = _state;
            string newKey = key + node.Value;
            foreach (Transition transition in _transitions[_state])
            {
                int nextState = transition.Invoke(node.Value);
                if (nextState > -1)
                {
                    if (nextState == _transitions.Count - 1 &&
                        nextState == oldState && 
                        node.Equal != null)
                    {
                        _getPrefixMatches(node.Equal, newKey, matches);
                        nextState++;
                    }
                    if (nextState == _transitions.Count && node.IsFinalNode)
                    {
                        matches.Add(newKey);
                    }
                    if (nextState < _transitions.Count && node.Equal != null)
                    {
                        _state = nextState;
                        _getBranchMatches(node.Equal, newKey, matches);
                        _state = oldState;
                    }
                }
            }
            if (node.Bigger != null)
            {
                _getBranchMatches(node.Bigger, key, matches);
            }
        }

        private void _getPrefixMatches(Node<V> node, string prefix, ICollection<string> matches)
        {
            if (node.Smaller != null)
            {
                _getPrefixMatches(node, prefix, matches);
            }
            if (node.IsFinalNode)
            {
                matches.Add(prefix + node.Value);
            }
            if (node.Equal != null)
            {
                _getPrefixMatches(node.Equal, prefix + node.Value, matches);
            }
            if (node.Bigger != null)
            {
                _getPrefixMatches(node.Bigger, prefix, matches);
            }
        }

        private char _getMinChar(char a, char b)
        {
            if (a <= b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        private char _getMaxChar(char a, char b)
        {
            if (a <= b)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

        #endregion

        #region Delegatee Factories

        private Func<char, int> _matchEverything(int successState) => (c) => successState;

        private Func<char, int> _matchNothing() => (c) => -1;

        private Func<char, int> _matchExact(char a, int successState) => (c) =>
        {
            if (c == a)
            {
                return successState;
            }
            else
            {
                return -1;
            }
        };

        private Func<char, int> _matchRange(char a, char b, int successState) => (c) =>
        {
            if (c >= _getMinChar(a, b) && c <= _getMaxChar(a, b))
            {
                return successState;
            }
            else
            {
                return -1;
            }
        };

        private Func<char, int> _matchAnyOf(ICollection<char> matches, int successState) => (c) =>
        {
            foreach (char match in matches)
            {
                if (c == match)
                {
                    return successState;
                }
            }
            return -1;
        };

        #endregion
    }
}
