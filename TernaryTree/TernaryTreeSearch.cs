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
        private List<Transition> _transitions;
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
            // _transitions[0] += _matchRange(pattern[0], pattern[3], 1, 0) as Transition;
            // Alternatively, _transitions could be a List<List<Transition>> and we could do:
            // _transitions[0][0] = new Transition(_matchRange(pattern[0], pattern[3], 1, 0));
            // _transitions[0][1] = new Transition(_matchExact(pattern[1], 1, 0);
            // And then an iteration of Match() would go:
            // foreach (Transition t in _transitions[_state]) t.Invoke(Node.Value);
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
            throw new NotImplementedException();
        }

        private Func<char, int> _matchRange(char a, char b, int successState, int failureState)
        {
            Func<char, int> f = c =>
            {
                if (c >= _getMinChar(a, b) && c <= _getMaxChar(a, b))
                {
                    // Maybe we should handle cloning the state machine
                    // and sending it down the branch here?
                    return successState;
                }
                else
                {
                    return failureState;
                }
            };
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
