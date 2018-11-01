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
        private string _currentKey;
        private V _currentValue;
        private HashSet<Node<V>> _visited = new HashSet<Node<V>>();
        private StringBuilder _keyBuilder = new StringBuilder();
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
            new KeyValuePair<string, V>(_currentKey, _currentValue);

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
            // If we're already past the end, don't even try
            if (_isOutOfRange)
            {
                return false;
            }

            // If we haven't yet initialized
            if (!_isInitialized)
            {
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

            return _findNextKey(_currentNode);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _currentNode = null;
            _keyBuilder = new StringBuilder();
            _isInitialized = false;
            _isOutOfRange = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyBuilder"></param>
        /// <returns></returns>
        private bool _findNextKey(Node<V> node)
        {
            if (node.Smaller != null && !_visited.Contains(node.Smaller)) 
            {
                return _findNextKey(node.Smaller);
            }
            if (!_visited.Contains(node))
            {
                _keyBuilder.Append(node.Value);
                _visited.Add(node);
                if (node.IsFinalNode)
                {
                    _isInitialized = true;
                    _currentNode = node;
                    _currentKey = _keyBuilder.ToString();
                    _currentValue = _currentNode.Data;
                    return true;
                }
                else
                {
                    return _findNextKey(node);
                }
            }
            if (node.Equal != null && !_visited.Contains(node.Equal))
            {
                return _findNextKey(node.Equal);
            }
            while (node.Bigger == null )
            {
                if (node.Parent == null)
                {
                    break;
                }
                if (node.Parent.Equal == node) 
                {
                    _keyBuilder.Remove(_keyBuilder.Length - 1, 1);
                }
                node = node.Parent;
            }
            if (node.Bigger != null)
            {
                return _findNextKey(node.Bigger);
            }
            _isOutOfRange = true;
            return false;
        }
    }
}
