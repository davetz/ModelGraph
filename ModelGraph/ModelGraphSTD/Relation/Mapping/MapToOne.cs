using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public class MapToOne<T> : Dictionary<Item, T> where T : Item
    {
        internal MapToOne(int capacity = 0) : base(capacity) { }

        internal int KeyCount { get { return Count; } }
        internal int ValueCount { get { return Count; } }

        internal int GetLinksCount()
        {
            var count = 0;
            foreach (var val in this)
            {
                if (val.Value.IsExternal || val.Key.IsExternal) count += 1;
            }
            return count;
        }

        internal int GetLinks(out List<Item> parents, out List<Item> children)
        {
            var count = GetLinksCount();
            children = new List<Item>(count);
            parents = new List<Item>(count);

            foreach (var val in this)
            {
                if (val.Value.IsExternal || val.Key.IsExternal)
                {
                    children.Add(val.Value);
                    parents.Add(val.Key);
                }
            }
            return count;
        }

        internal void SetLink(Item key, T val, int capacity = 0)
        {
            this[key] = val;
        }

        internal int GetValCount(Item key)
        {
            return ContainsKey(key) ? 1 : 0;
        }

        internal void AppendLink(Item key, T val)
        {
            SetLink(key, val);
        }

        internal void InsertLink(Item key, T val, int index)
        {
            SetLink(key, val);
        }
        internal int GetIndex(Item key, T val)
        {
            return (TryGetValue(key, out T value) && value == val) ? 0 : -1;
        }

        internal void RemoveLink(Item key, T val)
        {
            if (TryGetValue(key, out T value) && val == value) Remove(key);
        }

        internal bool TryGetVal(Item key, out T val) => TryGetValue(key, out val);

        internal bool TryGetVals(Item key, out List<Item> vals)
        {
            if (TryGetValue(key, out T value))
            {
                vals = new List<Item>(1) { value };
                return true;
            }
            vals = null;
            return false;
        }

        internal bool TryGetVals(Item key, out List<T> vals)
        {
            if (TryGetValue(key, out T value))
            {
                vals = new List<T>(1) { value };
                return true;
            }
            vals = null;
            return false;
        }

        internal bool ContainsLink(Item key, T val)
        {
            return (TryGetValue(key, out T value) && value == val);
        }

        internal bool CanMapToOne { get { return true; } }
    }

}
