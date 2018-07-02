using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public abstract class Path : Item
    {
        private readonly List<Path> _paths;
        internal IList<Path> Paths => _paths;
        internal void Reverse() { IsReversed = !IsReversed; }
        internal int Count => (Paths == null) ? 0 : Paths.Count;

        internal abstract Item Head { get; }
        internal abstract Item Tail { get; }
        internal abstract Query Query { get; }

        internal abstract double Width { get; }
        internal abstract double Height { get; }

        internal Path(Graph owner, Trait trait, List<Path> buffer = null)
        {/*
            Typically the input argument "buffer" is a larger list used
            to collect paths while traversing a graph. Its contents are copied
            to a minimally sized _path list and the buffer list is cleared
            (to be reused as the graph traversal continues)
         */
            Owner = owner;
            Trait = trait;

            _paths = new List<Path>(buffer);
            buffer.Clear();

            owner.Add(this);
        }
    }
}
