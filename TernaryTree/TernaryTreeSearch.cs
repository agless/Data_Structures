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
            // Build a state machine by filling the _stateImplementations array with delegates.
            // Need to define my delegates below so that they can be plugged in with values.
            // Just need to provide all delegates to handle every type of transition / edge.
            // Each state can have more than one transition, each leading to a different state.
            //
            // Example of how to add an edge to the state machine:
            // Transition t = new Transition(_matchRange(pattern[0], pattern[3], 1, 0));
            // _transitions[i].Add(t);
            //
            // Should account for prefix match case.  It seems more efficient to
            // grab all keys from a prefix branch at once than to go through the state check/set
            // rigamarole.

            // Kick off the state building process
            // TODO: Current state building strategy is not right.
            // Will result in a bunch of long paths.
            // No position in _transitions will have more than one edge.
            // Work backwards?
            _buildState(0, pattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ICollection<string> Match(Node<V> node)
        {
            ICollection<string> matches = new LinkedList<string>();
            _getBranchMatches(node, default(string), matches);
            return matches;
        }

        #region Delegatees

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

        #region Private Methods

        private TernaryTreeSearch(List<List<Transition>> transitions, int state)
        {
            _transitions = transitions;
            _state = state;
        }

        private void _buildState(int pos, string pattern)
        {
            while (pos < pattern.Length)
            {
                char c = pattern[pos];
                switch (c)
                {
                    case '\\':
                        pos = _handleEscape(pos, pattern);
                        continue;
                    case '^': // TODO: Write private methods for each of these special characters.
                    case '$':
                    case '.':
                    case '|':
                    case '?':
                    case '*':
                    case '+':
                        pos = _handleLiteral(pos, pattern);
                        continue;
                    case '(':
                    case '[':
                    case '{':
                        // TODO: Handle grouping.
                        // pos = _handleGroup(pos, pattern);
                        continue;
                    case ']':
                    case ')':
                    case '}':
                    default:
                        pos = _handleLiteral(pos, pattern);
                        break;
                }
            }
        }

        private int _handleLiteral(int pos, string pattern)
        {
            char c = pattern[pos++];
            Transition t = new Transition(_matchExact(c, _transitions.Count));
            _transitions[_state].Add(t);
            int oldState = _state;
            _state = _transitions.Count;
            _buildState(pos, pattern);
            _state = oldState;
            return pos;
        }

        private int _handleEscape(int pos, string pattern)
        {
            if (pos + 1 >= pattern.Length)
            {
                // An escape character was passed as the last character in a string.
                // Could just ignore?
                // Otherwise:
                // _handleSyntaxError()
            }
            else
            {
                // TODO: Call the right method for the escaped character.
                // Switch statement?
            }
            return pos;
        }

        private void _getBranchMatches(Node<V> node, string key, ICollection<string> matches)
        {
            if (_state >= _transitions.Count)
            {
                return;
            }
            if (node.Smaller != null)
            {
                TernaryTreeSearch<V> tts = new TernaryTreeSearch<V>(_transitions, _state);
                tts._getBranchMatches(node.Smaller, key, matches);
            }
            StringBuilder oldString = new StringBuilder(key.ToString());
            int oldState = _state;
            string newKey = key + node.Value;
            foreach (Transition transition in _transitions[_state])
            {
                int nextState = transition.Invoke(node.Value);
                if (nextState > -1)
                {
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
