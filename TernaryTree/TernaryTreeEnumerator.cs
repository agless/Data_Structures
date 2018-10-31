using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    class TernaryTreeEnumerator<V> : IEnumerator<KeyValuePair<string, V>>, IEnumerator
    {
        private Node<V> _head;
        private Node<V> _currentNode;
        private StringBuilder _currentKey;
        private bool _isInitialized = false;
        private bool _isOutOfRange = false;

        public TernaryTreeEnumerator(Node<V> head)
        {
            _head = head ?? throw new ArgumentNullException(nameof(head));
        }
        
        /// <summary>
        /// Returns the value associated with the current key.
        /// </summary>
        object IEnumerator.Current => _currentNode.Data;

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey, TValue}"/> with the current key and value.
        /// </summary>
        KeyValuePair<string, V> IEnumerator<KeyValuePair<string, V>>.Current => 
            new KeyValuePair<string, V>(_currentKey.ToString(), _currentNode.Data);

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <remarks>
        /// Microsoft documentation states that this method should be left empty
        /// if there's nothing to clean up (i.e. no database connections to close).
        /// </remarks>
        public void Dispose() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            // If we're already past the end, don't even try
            if (_isOutOfRange)
            {
                return false;
            }

            // If we haven't yet initialized
            if (!_isInitialized)
            {
                _currentKey = new StringBuilder();
                _findNextKey(_head);
                if (!_isInitialized)
                {
                    // The tree must be empty
                    return false;
                }
                else
                {
                    // A key was found
                    return true;
                }
            }

            if (_currentNode.Equal != null)
            {
                // Find keys that have a prefix including this character
                _currentNode = _findNextKey(_currentNode.Equal);
            }
            else
            {
                // Find keys that have the same prefix except a bigger character in this spot
                _currentKey.Remove(_currentKey.Length - 1, 1);
                if (_currentNode.Bigger != null)
                {
                    _currentNode = _findNextKey(_currentNode.Bigger);
                }
            }

            if (_isOutOfRange)
            {
                // If this search has taken us out of range
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _currentNode = null;
            _currentKey = new StringBuilder();
            _isInitialized = false;
            _isOutOfRange = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyBuilder"></param>
        /// <returns></returns>
        private Node<V> _findNextKey(Node<V> node)
        {
            if (node.Smaller != null)
            {
                Node<V> first = _findNextKey(node.Smaller);
                if (first != null)
                {
                    return first;
                }
            }
            _currentKey.Append(node.Value);
            if (node.IsFinalNode)
            {
                _isInitialized = true;
                return node;
            }
            if (node.Equal != null)
            {
                Node<V> first = _findNextKey(node.Equal);
                if (first != null)
                {
                    return first;
                }
            }
            // TODO: _findNextKey()
            // Need to continue up the tree, stripping away characters from
            // current key as appropriate (when _currentKey[_currentKey.Length - 1] == node.Value ?)
            // and searching down the branch for every node.Bigger that isn't null.
            // Only if _currentKey goes empty do we declare _isOutOfRange true and return null.
            // This belongs in a while loop.
            _currentKey.Remove(_currentKey.Length - 1, 1);
            do
            {
                _currentKey.Remove(_currentKey.Length - 1, 1);
                if (node.Bigger != null)
                {
                    return _findNextKey(node.Bigger);
                }
            } while (node.Parent != null);
            _isOutOfRange = true;
            return null;
        }
    }
}
