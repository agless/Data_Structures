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
        object IEnumerator.Current => _currentValue();  // TODO: Need to save current key as a field and build a key value pair for return by calling _tree.

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey, TValue}"/> with the current key and value.
        /// </summary>
        KeyValuePair<string, V> IEnumerator<KeyValuePair<string, V>>.Current => _currentValue();

        private KeyValuePair<string, V> _currentValue()
        {
            return new KeyValuePair<string, V>(_currentKey, _head.Data);
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
                return true;
            }
            else
            {
                return false;
            }
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
                    Step s = new Step(_createStepReturnKey(node, key + node.Value));
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

        private Func<string> _createStepReturnKey(Node<V> node, string key)
        {
            string f()
            {
                return key;
            }
            return f;
        }
    }
}
