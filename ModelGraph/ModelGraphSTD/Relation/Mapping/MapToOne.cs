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

        internal Type ValueType => typeof(T);


        internal int GetKeys(out Item[] keys)
        {
            keys = new Item[KeyCount];

            var i = 0;
            foreach (var val in this)
            {
                keys[i] = val.Key;
                i += 1;
            }
            return i;
        }

        internal int GetValues(out Item[] values)
        {
            values = new Item[ValueCount];

            var i = 0;
            foreach (var val in this)
            {
                values[i] = val.Value;
                i += 1;
            }
            return i;
        }

        internal int GetLinksCount()
        {
            var count = 0;
            foreach (var val in this)
            {
                if (val.Value.IsExternal || val.Key.IsExternal) count += 1;
            }
            return count;
        }

        internal int GetLinks(out Item[] parents, out Item[] children)
        {
            var count = GetLinksCount();
            children = new Item[count];
            parents = new Item[count];

            var i = 0;
            foreach (var val in this)
            {
                if (val.Value.IsExternal || val.Key.IsExternal)
                {
                    children[i] = val.Value;
                    parents[i] = val.Key;
                    i += 1;
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

        internal int GetCount(Item key)
        {
            return ContainsKey(key) ? 1 : 0;
        }
        internal int GetIndex(Item key, T val)
        {
            int index = -1;
            if (TryGetValue(key, out T value) && value == val) index = 0;
            return index;
        }

        internal void RemoveLink(Item key, T val)
        {
            if (TryGetValue(key, out T value) && val == value)
                Remove(key);
        }

        internal bool TryGetVal(Item key, out T val)
        {
            return TryGetValue(key, out val);
        }

        internal bool TryGetVals(Item key, out Item[] vals)
        {
            if (TryGetValue(key, out T value))
            {
                vals = new Item[] { value };
                return true;
            }
            vals = null;
            return false;
        }

        internal bool TryGetVals(Item key, out List<T> vals)
        {
            if (TryGetValue(key, out T value))
            {
                vals = new List<T>(1);
                vals.Add(value);
                return true;
            }
            vals = null;
            return false;
        }

        internal bool TryGetAllKeys(out Item[] keys)
        {
            keys = Keys.ToArray();
            return (Count > 0);
        }

        internal bool ContainsLink(Item key, T val)
        {
            return (TryGetValue(key, out T value) && value == val);
        }

        internal bool CanMapToOne { get { return true; } }
    }

}
