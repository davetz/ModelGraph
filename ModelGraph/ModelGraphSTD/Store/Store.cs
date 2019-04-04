
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public abstract class Store : Item
    {
        internal abstract void Add(Item item);
        internal abstract void Move(Item item, int index);
        internal abstract void Insert(Item item, int index);
        public abstract void Remove(Item item);
        internal abstract void RemoveAll();
        internal abstract int IndexOf(Item item);
        internal abstract List<Item> GetItems();
        internal abstract int Count { get; }

        internal bool TryLookUpProperty(string name, out Property property)
        {
            property = null;
            return GetChef().TryLookUpProperty(this, name, out property);
        }
    }
}
