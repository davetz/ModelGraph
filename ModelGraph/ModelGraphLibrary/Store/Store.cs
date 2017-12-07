
namespace ModelGraphSTD
{
    public abstract class Store : Item
    {
        internal abstract bool IsValidChild(Item child);
        internal abstract void Insert(Item item, int index);
        internal abstract void Append(Item item);
        internal abstract int IndexOf(Item item);
        public abstract void Remove(Item item);
        internal abstract void Move(Item item, int index);
        internal abstract void Add(Item item);
        internal abstract Item[] GetItems();
        internal abstract int Count { get; }
        internal abstract void RemoveAll();

        internal bool TryLookUpProperty(string name, out Property property, out int index)
        {
            property = null;
            index = 0;
            return false;
            //return GetChef().TryLookUpProperty(this, name, out property, out index);
        }
    }
}
