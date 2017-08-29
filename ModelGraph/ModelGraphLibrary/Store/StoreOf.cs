using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{
    public class StoreOf<T> : Store where T : Item
    {
        protected List<T> _items; // protected against accidental corruption
        internal Guid Guid; //as of 3/14/2017 all stores will have a Guid

        #region Constructor  ==================================================
        internal StoreOf() { } // dummy parameterless constructor
        internal StoreOf(Chef owner, Trait trait, Guid guid, int capacity)
        {
            Owner = owner;
            Trait = trait;
            Guid = guid;
            SetCapacity(capacity);

            if (owner == null || owner.IsRootChef) return;
            owner.Append(this); // we want this store to be in the dataChef's item tree hierarchy
        }
        #endregion

        #region Count/Items/ItemsReversed  ====================================
        // outsiders can only get a copy of the List<T> _items 
        internal T[] Items => (_items != null) ? _items.ToArray() : new T[0];
        internal T[] ItemsReversed => GetItemsReversed();
        private T[] GetItemsReversed()
        {
            var N = Count;
            var M = N - 1;
            var items = new T[N];
            for (int i = M, j = 0; j < N; j++, i--) { items[j] = _items[i]; }
            return items;
        }
        internal override int Count => (_items == null) ? 0 : _items.Count;
        internal override Item[] GetItems() { return Items; }
        internal override void RemoveAll() { _items.Clear(); }
        #endregion

        #region Methods  ======================================================
        internal void SetCapacity(int exactCount)
        {
            var cap = (int)((exactCount + 1) * 1.1); // allow for modest expansion

            if (_items == null) _items = new List<T>(cap);
            else _items.Capacity = cap;
        }
        internal override bool IsValidChild(Item item) { return (item is T); }


        // Add  =============================================================
        internal override void Add(Item item)
        {
            var child = item as T;
            if (child != null) Add(child);
        }
        internal void Add(T item)
        {
            _items.Add(item);
        }

        // Remove  ==========================================================
        internal override void Remove(Item item)
        {
            var child = item as T;
            Remove(child);
        }
        internal void Remove(T item)
        {
            _items.Remove(item);
        }

        // Insert  ============================================================
        internal override void Insert(Item item, int index)
        {
            var child = item as T;
            if (child != null) Insert(child, index);
        }
        internal void Insert(T item, int index)
        {
            if (_items == null) _items = new List<T>();

            if (index < 0)
                _items.Insert(0, item);
            else if (index < _items.Count)
                _items.Insert(index, item);
            else
                _items.Add(item);
        }

        // Append  ============================================================
        internal override void Append(Item item)
        {
            var child = item as T;
            if (child != null) Append(child);
        }
        internal void Append(T item)
        {
            if (_items == null) _items = new List<T>();

            _items.Add(item);
        }

        // IndexOf  ===========================================================
        internal override int IndexOf(Item item)
        {
            var child = item as T;
            return (child == null) ? -1 : IndexOf(child);
        }
        internal int IndexOf(T item)
        {
            return (Count > 0) ? _items.IndexOf(item) : -1;
        }

        // Move  ==============================================================
        internal override void Move(Item item, int index)
        {
            var child = item as T;
            if (child != null) Move(child, index);
        }
        internal void Move(T item, int index)
        {
            if (_items.Remove(item))
            {
                if (index < 0)
                    _items.Insert(0, item);
                else if (index < _items.Count)
                    _items.Insert(index, item);
                else
                    _items.Add(item);
            }
        }

        #endregion
    }
}
