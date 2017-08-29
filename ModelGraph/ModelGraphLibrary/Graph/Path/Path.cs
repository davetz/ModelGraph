using System;

namespace ModelGraphLibrary
{/*
 */
    public abstract class Path : Item
    {
        internal abstract Item Head { get; }
        internal abstract Item Tail { get; }
        internal abstract Query Query { get; }

        internal abstract double Width { get; }
        internal abstract double Height { get; }

        internal abstract int Count { get; }
        internal abstract Item[] Items { get; }

        internal void Reverse() { IsReversed = !IsReversed; }
    }
}
