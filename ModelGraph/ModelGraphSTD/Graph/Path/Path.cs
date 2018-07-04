
namespace ModelGraphSTD
{
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
    }
}
