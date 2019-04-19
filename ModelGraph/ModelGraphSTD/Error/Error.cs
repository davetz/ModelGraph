namespace ModelGraphSTD
{
    public abstract class Error : Item
    {
        internal Item Item;
        internal string Name;

        internal abstract void Add(string text);
        internal abstract void Clear();
        internal abstract int Count { get; }
        internal abstract string GetError(int index = 0);
    }
}
