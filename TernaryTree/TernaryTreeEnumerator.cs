using System;
using System.Collections;
using System.Collections.Generic;

namespace TernaryTree
{
    class TernaryTreeEnumerator<V> : IEnumerator<KeyValuePair<string, V>>, IEnumerator
    {
        private delegate string Step();
        private Stack<Step> _nextStep = new Stack<Step>();
        private TernaryTree<V> _tree;
        private Node<V> _head;
        private bool _isInitialized = false;
        private string _currentKey;

        public TernaryTreeEnumerator(TernaryTree<V> tree, Node<V> head)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _head = head ?? throw new ArgumentNullException(nameof(head));
        }

        /// <summary>
        /// Returns the value associated with the current key.
        /// </summary>
        object IEnumerator.Current => _currentKey;

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey, TValue}"/> with the current key and value.
        /// </summary>
        KeyValuePair<string, V> IEnumerator<KeyValuePair<string, V>>.Current => _currentKeyValuePair();

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <remarks>
        /// Microsoft documentation states that this method should be left empty
        /// if there's nothing to clean up (e.g. no database connections to close).
        /// </remarks>
        public void Dispose() { }

        /// <summary>
        /// Moves to the next <see cref="KeyValuePair"/> in the enumeration.
        /// </summary>
        /// <returns>Returns <code>true</code> if the next <see cref="KeyValuePair"/> was found.
        /// Otherwise returns <code>false</code>.</returns>
        public bool MoveNext()
        {
            if (!_isInitialized)
            {
                Step firstStep = new Step(_createStep(_head, default(string)));
                _nextStep.Push(firstStep);

            }
            string s = default(string);
            while(_nextStep.Count > 0 && string.IsNullOrEmpty(s))
            {
                s = _nextStep.Pop().Invoke();
            }
            if (!string.IsNullOrEmpty(s))
            {
                _currentKey = s;
                if (!_isInitialized)
                {
                    _isInitialized = true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the enumerator to before the first element in the enumeration.
        /// </summary>
        public void Reset()
        {
            _nextStep.Clear();
            _isInitialized = false;
        }

        private KeyValuePair<string, V> _currentKeyValuePair()
        {
            _tree.TryGetValue(_currentKey, out V value);
            return new KeyValuePair<string, V>(_currentKey, value);
        }

        private Func<string> _createStep(Node<V> node, string key) => () => 
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
                Step returnKey = new Step(_returnKey(key + node.Value));
                _nextStep.Push(returnKey);
            }
            if (node.Smaller != null)
            {
                Step s = new Step(_createStep(node.Smaller, key));
                _nextStep.Push(s);
            }
            return default(string);
        };

        private Func<string> _returnKey(string key) => () => key;
    }
}
