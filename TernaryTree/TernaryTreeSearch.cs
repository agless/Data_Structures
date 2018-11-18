using System;
using System.Collections.Generic;

namespace TernaryTree
{
    public class TernaryTreeSearch<V>
    {
        private delegate int Transition(char c);
        private List<List<Transition>> _transitions;
        private int _state;
        private string _lastSymbol;

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
                //case '[':
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
            Transition t = new Transition(_matchExact(c, successState));
            _transitions[_state].Add(t);
            return ++pos;
        }

        private int _handleDot(int pos, string pattern, int successState)
        {
            _lastSymbol = ".";
            Transition t = new Transition(_matchEverything(successState));
            _transitions[_state].Add(t);
            return ++pos;
        }

        // TODO: Fix star repeating
        private int _handleStar(int pos, string pattern)
        {
            // The current last spot for transitions (which is also currently empty) shouldn't count for purposes of final state 
            _transitions.RemoveAt(_state); 
            if (pos + 1 < pattern.Length)
            {
                // If there's more to match, then the previous match shouldn't transition to final state
                _transitions[_transitions.Count - 1].RemoveAt(_transitions[_transitions.Count - 1].Count - 1);
            }
            _state--;
            // the previous transition should loop back to same state
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
                    // TODO: Get efficient prefix match working.
                    // TODO: Also check for repeating state that's not final state (aka, a prefix match).
                    // Put the _transitions.Count - 1 check inside the check for the other two.
                    // If it's a prefix match, call prefix.
                    // Else call a method that can handle a repeating match in a straight line without
                    // worrying about advancing state until it finds the right character for the next state.
                    // (But can we even get that character here?)
                    //if (nextState == _transitions.Count - 1 &&
                    //    nextState == oldState && 
                    //    node.Equal != null)
                    //{
                    //    _getPrefixMatches(node.Equal, newKey, matches);
                    //    continue;
                    //}
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

        #region Delegate Factories

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
