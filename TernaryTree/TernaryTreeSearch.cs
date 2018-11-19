using System;
using System.Collections.Generic;

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
            // If the last symbol is a repeating dot, add a prefix match decorator to all transitions in the previous state
            if (pos == pattern.Length - 1 &&
                pattern[pos - 1] == '.' &&
                pos > 1)
            {
                for (int i = 0; i < _transitions[_state - 2].Count; i++)
                {
                    Transition t = _transitions[_state - 2][i];
                    t = new Transition(_prefixMatchDecorator(t));
                }
            }
            // The current last spot for transitions (which is also currently empty) shouldn't count for purposes of final state 
            _transitions.RemoveAt(_state); 
            if (pos + 1 < pattern.Length)
            {
                // If there's more pattern to match, then the repeating match shouldn't transition forward.
                // (It should only loop - which is added below.)
                // TODO: Instead of doing this finicky delete, is some way to just decorate the other transitions so that they return a different result?
                // See the prefix match decorator used above.
                _transitions[_transitions.Count - 1].RemoveAt(_transitions[_transitions.Count - 1].Count - 1);
            }
            // the previous transition should loop back to it's own state
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

        private void _getBranchMatches(Node<V> node, string key)
        {
            if (_state >= _transitions.Count)
            {
                return;
            }
            if (node.Smaller != null)
            {
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
                _getPrefixMatches(node, prefix);
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

        // TODO: get prefix match decorator working
        private Func<Node<V>, string, int> _prefixMatchDecorator(Transition t) => (node, key) =>
        {
            int newState = t.Invoke(node, key);
            if (newState > -1)
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
    }
}
