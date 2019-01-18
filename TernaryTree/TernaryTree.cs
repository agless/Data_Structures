using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    // TODO: Error Handling and Documentation.  Follow Microsoft lead with interfaces.
    // TODO: More unit tests.  Check line coverage.
    // TODO: Consider storing a KeyValuePair at node.Data.  This would do away with all the string building.

    /// <summary>
    /// Provides a structure for storing key value pairs.
    /// Key must be a <code>string</code>.
    /// Provides fast insert and lookup operations.
    /// Provides regex matching for keys.
    /// </summary>
    public class TernaryTree<V> :
        IDictionary<string, V>,
        ICollection<KeyValuePair<string, V>>,
        IEnumerable<KeyValuePair<string, V>>,
        IEnumerable
        // TODO: Non-Generic Interfaces?
    {
        #region Fields and Properties

        /// <summary>
        /// An entry point to the data structure.
        /// </summary>
        internal Node<V> Head;

        /// <summary>
        /// Gets the number of keys in the <see cref="TernaryTree"/>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TernaryTree"/> is
        /// read-only.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TernaryTree"/> has
        /// a fixed size.
        /// </summary>
        public bool IsFixedSize { get { return false; } }

        // TODO: Concurrency
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="TernaryTree"/>
        /// is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="TernaryTree"/>
        /// </summary>
        public object SyncRoot { get { return this; } }

        #endregion

        #region Static 'Constructors'

        public static TernaryTree<V> Create(ICollection<string> keySet)
        {
            TernaryTree<V> tree = new TernaryTree<V>();
            foreach (string key in keySet)
            {
                tree.Add(key);
            }
            return tree;
        }

        public static TernaryTree<V> Create(ICollection<KeyValuePair<string, V>> keyValueSet)
        {
            TernaryTree<V> tree = new TernaryTree<V>();
            foreach (KeyValuePair<string, V> keyValuePair in keyValueSet)
            {
                tree.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return tree;
        }

        public static TernaryTree<V> Create(IDictionary<string, V> keyValueSet)
        {
            TernaryTree<V> tree = new TernaryTree<V>();
            foreach (KeyValuePair<string, V> keyValuePair in keyValueSet)
            {
                tree.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return tree;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V this[string key]
        {
            get
            {
                TryGetValue(key, out V value);
                return value;
            }
            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is a relatively expensive operation.
        /// Time and space complexity increase linearly with index.
        /// </remarks>
        public string this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                _getKeyAtIndex(Head, new StringBuilder(), ref index, out string s);
                return s;
            }
        }

        #endregion

        #region Collections and Enumerators

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<string> Keys()
        {
            List<string> _keyList = new List<string>();
            _getBranchKeys(Head, new StringBuilder(), _keyList);
            return _keyList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<V> Values()
        {
            List<V> _valueList = new List<V>();
            _getBranchValues(Head, _valueList);
            return _valueList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">A regular expression to match.</param>
        /// <returns>A collection of keys matching the regular expression.</returns>
        public ICollection<string> Match(string pattern)
        {
            TernaryTreeSearch<V> search = new TernaryTreeSearch<V>(pattern);
            return search.Match(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">A <see cref="TernaryTreeSearch{V}"/> 
        /// which has been configured to match a regular expression.</param>
        /// <returns>A collection of keys matchig the regular expression.</returns>
        public ICollection<string> Match(TernaryTreeSearch<V> pattern) => pattern.Match(this);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix">The prefix to match.</param>
        /// <returns>A collection of keys matching the prefix.</returns>
        public ICollection<string> MatchPrefix(string prefix)
        {
            List<string> keys = new List<string>();
            Node<V> node = _getFinalNode(prefix, 0, Head);
            _getBranchKeys(node, new StringBuilder(), keys);
            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICollection<string> IDictionary<string, V>.Keys => Keys();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICollection<V> IDictionary<string, V>.Values => Values();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TernaryTreeEnumerator<V>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, V>> IEnumerable<KeyValuePair<string, V>>.GetEnumerator()
        {
            return new TernaryTreeEnumerator<V>(this);
        }

        #endregion

        #region Remaining Interface Implimentation

        /// <summary>
        /// Adds a key-value pair to the <see cref="TernaryTree"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, V value = default(V))
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }
            if (Head == null)
            {
                Head = new Node<V> { Value = key[0] };
            }
            Node<V> nd = _insertKey(key, 0, Head) ?? throw new ArgumentException(nameof(key));
            nd.Data = value;
            Count++;
        }

        /// <summary>
        /// Adds a key-value pair to the <see cref="TernaryTree"/>.
        /// </summary>
        /// <param name="item">A <see cref="KeyValuePair"/> containing a 
        /// <code>string</code> key and a <code>V</code> value.</param>
        public void Add(KeyValuePair<string, V> item) => Add(item.Key, item.Value);

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Head = null;
            Count = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }
            if (Count == 0)
            {
                return false;
            }
            Node<V> node = _getFinalNode(key, 0, Head);
            if (node == null)
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
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, V> item)
        {
            // TODO: This has to also chek that values are equal.  How do we compare V for equality?
            return Contains(item.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return Contains(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
        {
            _ = array ?? throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
            {
                throw new IndexOutOfRangeException();
            }
            // TODO: ICollection API docs want me to do a check for multidimensional array.
            // How do I check for multidimensional array?
            // Is it even possible to call this with a multidimensional array?
            // Seems like it wouldn't compile, given the above signature. (Test this.)
            if (Count > array.Length - arrayIndex) // TODO: Test for off by one.
            {
                throw new ArgumentException($"{nameof(array)} does not have sufficient space");
            }
            List<string> keys = new List<string>();
            _getBranchKeys(Head, new StringBuilder(), keys);
            for (int i = 0; i < keys.Count; i++)
            {
                TryGetValue(keys[i], out V value);
                KeyValuePair<string, V> kvPair = new KeyValuePair<string, V>(keys[i], value);
                array[arrayIndex++] = kvPair;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }
            Node<V> node = _getFinalNode(key, 0, Head);
            if (node == null)
            {
                return false;
            }
            node.Data = default(V);
            node.IsFinalNode = false;
            _pruneDeadBranch(node);
            Count--;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, V> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out V value)
        {
            Node<V> node = _getFinalNode(key, 0, Head);
            if (node != null)
            {
                value = node.Data;
                return true;
            }
            value = default(V);
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pos"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private Node<V> _insertKey(string key, int pos, Node<V> node)
        {
            char keyChar = key[pos];

            // Character matches this Node
            if (keyChar == node.Value)
            {
                // This is the last Node for this key
                if (pos == key.Length - 1)
                {
                    // Check key collision (identical key already exists)
                    if (node.IsFinalNode)
                    {
                        return null;
                    }
                    else
                    {
                        node.IsFinalNode = true;
                        return node;
                    }
                }

                // There's more key characters to add
                if (node.Equal == null)
                {
                    // TODO: Abstract out Node creation in this method
                    node.Equal = new Node<V>
                    {
                        Value = key[pos + 1],
                        Parent = node
                    };
                }
                return _insertKey(key, ++pos, node.Equal);
            }
            // Character is lower
            else if (keyChar < node.Value)
            {
                if (node.Smaller == null)
                {
                    node.Smaller = new Node<V>
                    {
                        Value = key[pos],
                        Parent = node
                    };
                }
                return _insertKey(key, pos, node.Smaller);
            }
            // Character is higher
            else
            {
                if (node.Bigger == null)
                {
                    node.Bigger = new Node<V>
                    {
                        Value = key[pos],
                        Parent = node
                    };
                }
                return _insertKey(key, pos, node.Bigger);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pos"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private Node<V> _getFinalNode(string key, int pos, Node<V> node)
        {
            char keyChar = key[pos];

            if (keyChar == node.Value)
            {
                if (pos == key.Length - 1)
                {
                    return node;
                }
                return (node.Equal != null) ? _getFinalNode(key, ++pos, node.Equal) : null;
            }
            else if (keyChar < node.Value)
            {
                return (node.Smaller != null) ? _getFinalNode(key, pos, node.Smaller) : null;
            }
            else return (node.Bigger != null) ? _getFinalNode(key, pos, node.Bigger) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="keyBuild"></param>
        /// <param name="keySet"></param>
        private void _getBranchKeys(Node<V> head, StringBuilder keyBuild, IList<string> keySet)
        {
            if (head.Smaller != null)
            {
                _getBranchKeys(head.Smaller, new StringBuilder(keyBuild.ToString()), keySet);
            }
            StringBuilder oldString = new StringBuilder(keyBuild.ToString());
            keyBuild.Append(head.Value);
            // TODO: Maybe this should be a LinkedList instead of a List?
            // Seems like performance would be better.
            if (head.IsFinalNode)
            {
                keySet.Add(keyBuild.ToString()); 
            }
            if (head.Equal != null)
            {
                _getBranchKeys(head.Equal, keyBuild, keySet);
            }
            if (head.Bigger != null)
            {
                _getBranchKeys(head.Bigger, oldString, keySet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="valueSet"></param>
        private void _getBranchValues(Node<V> node, IList<V> valueSet)
        {
            if (node.Smaller != null)
            {
                _getBranchValues(node.Smaller, valueSet);
            }
            if (node.IsFinalNode)
            {
                valueSet.Add(node.Data);
            }
            if (node.Equal != null)
            {
                _getBranchValues(node.Equal, valueSet);
            }
            if (node.Bigger != null)
            {
                _getBranchValues(node.Bigger, valueSet);
            }
        }

        // TODO: Replace stringbuilder with plain string.
        // We end up making a new string builder nearly every step anyway.
        // Also, recursive calls allocate new copies of ints and strings (right?), so
        // might as well make use of it by doing concatenations there.
        private void _getKeyAtIndex(Node<V> node, StringBuilder keyBuild, ref int index, out string s)
        {
            if (node.Smaller != null)
            {
                _getKeyAtIndex(node.Smaller, new StringBuilder(keyBuild.ToString()), ref index, out s);
                if (!string.IsNullOrEmpty(s))
                {
                    return;
                }
            }
            StringBuilder oldString = new StringBuilder(keyBuild.ToString());
            keyBuild.Append(node.Value);
            if (node.IsFinalNode)
            {
                index--;
                if (index < 0)
                {
                    s = keyBuild.ToString();
                    return;
                }
            }
            if (node.Equal != null)
            {
                _getKeyAtIndex(node.Equal, keyBuild, ref index, out s);
                if (!string.IsNullOrEmpty(s))
                {
                    return;
                }
            }
            if (node.Bigger != null)
            {
                _getKeyAtIndex(node.Bigger, oldString, ref index, out s);
                if (!string.IsNullOrEmpty(s))
                {
                    return;
                }
            }
            s = default(string);
        }

        /// <summary>
        /// Removes a branch of unused <see cref="Node"/>s, starting
        /// from a leaf and moving up the branch.  Only
        /// removes <see cref="Node"/>s that have no children.  Stops when
        /// encountering the first non-removeable <see cref="Node"/>.
        /// </summary>
        /// <param name="node">The leaf <see cref="Node"/> to check for removal.</param>
        private void _pruneDeadBranch(Node<V> node)
        {
            if (node.Parent == null)
            {
                return;
            }
            if (node.Smaller == null && node.Equal == null && node.Bigger == null)
            {
                Node<V> next = node.Parent;
                if (next.Smaller != null && next.Smaller == node)
                {
                    next.Smaller = null;
                }
                else if (next.Equal != null && next.Equal == node)
                {
                    next.Equal = null;
                }
                else if (next.Bigger != null && next.Bigger == node)
                {
                    next.Bigger = null;
                }
                _pruneDeadBranch(next);
            }
        }

        #endregion
    }
}
