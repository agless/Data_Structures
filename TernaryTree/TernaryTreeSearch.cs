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
            // Build a state machine by filling the _stateImplementations array with delegates.
            // Need to define my delegates below so that they can be plugged in with values.
            // Just need to think of all delegates to handle every type of transition / edge.
            // Each state can have more than one transition, each leading to a different state.
            //
            // Example of how to add an edge to the state machine:
            // Transition t = new Transition(_matchRange(pattern[0], pattern[3], 1, 0));
            // _transitions[i].Add(t);
            //
            // Should account for prefix match case.  It seems more efficient to
            // grab all keys from a prefix branch at once than to go through the state check/set
            // rigamarole.
        }

        private TernaryTreeSearch(List<List<Transition>> transitions, int state)
        {
            _transitions = transitions;
            _state = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ICollection<string> Match(Node<V> node)
        {
            // Make a private method to 
            // recursively invoke state, make a copy of this state machine,
            // and send the copy down branches as instructed by the result?
            // Or write the delegates to handle that?
            ICollection<string> matches = new LinkedList<string>();
            _getBranchMatches(node, default(string), matches);
            return matches;
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

        private Func<char, int> _matchEverything(int successState)
        {
            int f(char c)
            {
                return successState;
            }
            return f;
        }

        private Func<char, int> _matchNothing()
        {
            int f(char c)
            {
                return -1;
            }
            return f;
        }

        private Func<char, int> _matchExact(char a, int successState)
        {
            int f(char c)
            {
                if (c == a)
                {
                    return successState;
                }
                else
                {
                    return -1;
                }
            }
            return f;
        }
        
        private Func<char, int> _matchRange(char a, char b, int successState)
        {
            int f(char c)
            {
                if (c >= _getMinChar(a, b) && c <= _getMaxChar(a, b))
                {
                    return successState;
                }
                else
                {
                    return -1;
                }
            }
            return f;
        }

        private Func<char, int> _matchAnyOf(ICollection<char> matches, int successState)
        {
            int f(char c)
            {
                foreach (char match in matches)
                {
                    if (c == match)
                    {
                        return successState;
                    }
                }
                return -1;
            }
            return f;
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
    }
}
