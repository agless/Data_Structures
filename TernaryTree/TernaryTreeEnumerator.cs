using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    class TernaryTreeEnumerator<V> : IEnumerator<KeyValuePair<string, V>>, IEnumerator
    {
        private TernaryTree<V> _tree;
        private int _pos = -1;

        public TernaryTreeEnumerator(TernaryTree<V> tree)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
        }

        /// <summary>
        /// Returns the value associated with the current key.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return _currentValue();
            }
        }

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey, TValue}"/> with the current key and value.
        /// </summary>
        KeyValuePair<string, V> IEnumerator<KeyValuePair<string, V>>.Current
        {
            get
            {
                return _currentValue();
            }
        }

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
            // reset to BEFORE the first index (per Microsoft docs)
            _pos = -1;
        }

        private KeyValuePair<string, V> _currentValue()
        {
            string key;
            if (_pos < 0 || _pos > _tree.Count - 1)
            {
                key = _tree[_pos];
            }
            else
            {
                key = string.Empty;
            }
            _tree.TryGetValue(key, out V value);
            return new KeyValuePair<string, V>(key, value);
        }
    }
}
