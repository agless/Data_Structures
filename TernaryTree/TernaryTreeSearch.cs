using System;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    // TODO: Need to make an object to be a regex-like state machine.
    // Constructor takes a regex pattern and uses it to build logic for each state.  (AKA, the hard part.)

    public class TernaryTreeSearch<V>
    {
        private int _state;
        private Func<int>[] _stateImplementations;
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
            // Just need to think of a delegate that can handle any type of transition / edge.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ICollection<string> Match(Node<V> node)
        {
            // This method should actually return a bool
            // and accept an int for state, and a char for the character to evaluate.
            // update state and return yes or no for whether a match was made.
            // Ternary tree needs a _matchPattern method to drive it.
            throw new NotImplementedException();
        }
    }
}
