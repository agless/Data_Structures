using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    class TernaryTreeEnumerator<V> : IEnumerator<KeyValuePair<string, V>>, IEnumerator
    {
        /*
         TODO:  Enumerator should be able to go through one at a time instead of calling the expensive indexer.
         Perhaps this can be accomplished by keeping a stack of state.
         Each pop would expose a node and partial key (or a string builder).
         Keep popping / searching until you find the next key.
         Then push the remaining state back on to the stack.
         (Also have to push every time you leave a Node with children to check.)
             */

        private delegate string Step();
        private Stack<Step> _nextStep;
        private TernaryTree<V> _tree;
        private bool _isInitialized = false;
        // TODO: Save current key as a field.

        public TernaryTreeEnumerator(TernaryTree<V> tree)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        }

        /// <summary>
        /// Returns the value associated with the current key.
        /// </summary>
        object IEnumerator.Current => _currentValue();  // TODO: Need to save current key as a field and build a key value pair for return by calling _tree.

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey, TValue}"/> with the current key and value.
        /// </summary>
        KeyValuePair<string, V> IEnumerator<KeyValuePair<string, V>>.Current => _currentValue();

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <remarks>
        /// Microsoft documentation states that this method should be left empty
        /// if there's nothing to clean up (e.g. no database connections to close).
        /// </remarks>
        public void Dispose() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (!_isInitialized)
            {
                // Need to change back to asking for _head on construction
            }
            string s = default(string);
            while(_nextStep.Count > 0 && string.IsNullOrEmpty(s))
            {
                s = _nextStep.Pop().Invoke();
                // save string to prepare return for Current
                // return true;
            }
            // if stack's empty and s is stil null, return false
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _nextStep.Clear();
            _isInitialized = false;
        }

        private Func<string> _createStep(Node<V> node, string key)
        {
            string f()
            {
                if (node.Bigger != null)
                {
                    Step s = new Step(_createStep(node.Bigger, key));
                    _nextStep.Push(s);
                }
                if (node.Equal != null)
                {
                    Step s = new Step(_createStep(node.Equal, key + node.Value));
                    _nextStep.Push(s);
                }
                if (node.IsFinalNode)
                {
                    Step s = new Step(_createStepCheckFinalNode(node, key + node.Value));
                    _nextStep.Push(s);
                }
                if (node.Smaller != null)
                {
                    Step s = new Step(_createStep(node.Smaller, key));
                    _nextStep.Push(s);
                }
                return default(string);
            }
            return f;
        }

        private Func<string> _createStepCheckFinalNode(Node<V> node, string key)
        {
            string f()
            {
                if (node.IsFinalNode)
                {
                    return key;
                }
                return default(string);
            }
            return f;
        }
    }
}
