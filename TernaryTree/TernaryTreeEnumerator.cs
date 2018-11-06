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

        private TernaryTree<V> _tree;
        private int _pos = -1;
        // TODO: Add a stack, but don't initialize

        public TernaryTreeEnumerator(TernaryTree<V> tree)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        }

        /// <summary>
        /// Returns the value associated with the current key.
        /// </summary>
        object IEnumerator.Current => _currentValue();

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
            // TODO: IF stack is null, put _head on the stack
            // Otherwise, pop and search until you find the next valid key
            _pos++;
            // If we're already past the end, don't even try
            if (_pos > _tree.Count - 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            // TODO: Reset the stack to null
            // reset to BEFORE the first index (per Microsoft docs)
            _pos = -1;
        }

        private KeyValuePair<string, V> _currentValue()
        {
            string key;
            if (_pos >= 0 && _pos <= _tree.Count - 1)
            {
                key = _tree[_pos];
            }
            else
            {
                key = string.Empty;
            }
            V value;
            if (!string.IsNullOrEmpty(key))
            {
                _tree.TryGetValue(key, out value);
            }
            else
            {
                value = default(V);
            }
            return new KeyValuePair<string, V>(key, value);
        }
    }
}
