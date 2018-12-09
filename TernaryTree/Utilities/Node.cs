using System;
using System.Collections.Generic;
using System.Text;

namespace TernaryTree
{
    internal class Node<V>
    {
        /// <summary>
        /// The character that this <see cref="Node"/> represents.
        /// </summary>
        public char Value { get; set; }

        /// <summary>
        /// A flag indicating whether this node represents the final character
        /// in a valid key.
        /// </summary>
        public bool IsFinalNode { get; set; }
        
        /// <summary>
        /// The value to be associated with a key.
        /// Data is stored in the final <see cref="Node"/> for the key.
        /// </summary>
        public V Data { get; set; }

        /// <summary>
        /// A <see cref="Node"/> representing the previous character for this key.
        /// </summary>
        public Node<V> Parent { get; set; }
        
        /// <summary>
        /// A child <see cref="Node"/> with a larger <code>Value</code>.
        /// </summary>
        public Node<V> Bigger { get; set; }

        /// <summary>
        /// A child <see cref="Node"/> with a smaller <code>Value</code>.
        /// </summary>
        public Node<V> Smaller { get; set; }

        /// <summary>
        /// A <see cref="Node"/> representing the next character for this key.
        /// </summary>
        public Node<V> Equal { get; set; }
    }
}
