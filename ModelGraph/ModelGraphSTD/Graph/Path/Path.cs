using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public abstract class Path : Item
    {
        internal abstract Item Head { get; }
        internal abstract Item Tail { get; }
        internal abstract Query Query { get; }

        internal abstract double Width { get; }
        internal abstract double Height { get; }
        internal abstract Path[] Paths { get; }
        internal int Count => (Paths == null) ? 0 : Paths.Length;
        internal void Reverse() { IsReversed = !IsReversed; }
        protected int Last => Count - 1;

        //internal Path(Graph owner, Trait trait, List<Path
        //{
        //    Owner = owner;
        //    Trait = trait;
        //    if (buffer != null)
        //    {
        //        _paths = new List<Path>(buffer);
        //        buffer.Clear();
        //    }
        //    owner.Add(this);
        //}
    }
}
