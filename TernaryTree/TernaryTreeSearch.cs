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
        // Delegates are MatchRange, MatchExact, MatchAny, etc.
        // They return bool (whether a match was made) - so you know whether to send
        // the machine down the branch.
        // They set _state according to whether a match was made

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public TernaryTreeSearch(string pattern)
        {
            // Build a state machine by filling the _stateImplementations array with delegates.
            // Need to define my delegates below so that they can be plugged in with values.
            // Just need to think of all delegates to handle every type of transition / edge.
            // Example of how to add an edge to the state machine:
            // Transition t = new Transition(_matchRange(pattern[0], pattern[3], 1, 0));
            // _transitions[i].Add(t);
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
            _getBranchMatches(node, new StringBuilder(), matches);
            return matches;
        }

        private void _getBranchMatches(Node<V> node, StringBuilder keyBuilder, ICollection<string> matches)
        {
            if (node.Smaller != null)
            {
                TernaryTreeSearch<V> tts = new TernaryTreeSearch<V>(_transitions, _state);
                tts._getBranchMatches(node.Smaller, new StringBuilder(keyBuilder.ToString()), matches);
            }
            StringBuilder oldString = new StringBuilder(keyBuilder.ToString());
            int oldState = _state;
            keyBuilder.Append(node.Value);
            foreach (Transition transition in _transitions[_state])
            {
                int nextState = transition.Invoke(node.Value);
                if (nextState > -1)
                {
                    if (nextState == _transitions.Count)
                    {
                        matches.Add(keyBuilder.ToString());
                    }
                    if (node.Equal != null)
                    {
                        _state = nextState;
                        _getBranchMatches(node.Equal, keyBuilder, matches);
                        _state = oldState;
                    }
                }
            }
            if (node.Bigger != null)
            {
                _getBranchMatches(node.Bigger, oldString, matches);
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
