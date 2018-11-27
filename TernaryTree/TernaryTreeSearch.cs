using System;
using System.Collections.Generic;

namespace TernaryTree
{
    public class TernaryTreeSearch<V>
    {
        // TODO: Detect / Throw syntax errors.

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
                case '.':
                    pos = _handleDot(pos, pattern, successState);
                    break;
                //TODO: Write private methods for each of these special characters.
                //TODO: Case insensitive mode?
                //case '\\':
                //    pos = _handleEscape(pos, pattern);
                //    continue;
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
                //TODO: Handle grouping.
                //pos = _handleGroup(pos, pattern);
                //continue;
                //case ']':
                //case ')':
                //case '}':
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

        // TODO: Fix star repeating
        private int _handleStar(int pos, string pattern)
        {
            // TODO: Throw some kind of syntax error (ArgumentException?) if star is in the first position.  (There's nothing to repeat.)
            // TODO: Throw some kind of syntax error (ArgumentException?) if star follows star.  (There's nothing to repeat)
            // ^^Careful - don't throw if it's matching a literal '*'.  Check for escape character preceding that.
            
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
            // TODO: If pos is last character throw error
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
            // TODO: if not ']' && is last pos throw error
            if (isNegativeQuery && isRangeQuery)
            {
                // TODO: if matchingChars.Count > 2 throw error
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
                // TODO: if matchingChars.Count > 2 throw error
                char a, b;
                a = matchingChars.First.Value;
                b = matchingChars.Last.Value;
                _transitions[_state].Add(new Transition(_matchRange(a, b, successState)));
            }
            else
            {
                _transitions[_state].Add(new Transition(_matchAnyOf(matchingChars, successState)));
            }
            // TODO: how are we going to handle _lastSymbol?
            return ++pos;
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
            if (node.IsFinalNode)
            {
                _matches.Add(key + node.Value);
            }
            return t.Invoke(node, key);
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
