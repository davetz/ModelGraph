using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public class MapToMany<T> : Dictionary<Item, List<T>> where T : Item
    {
        internal MapToMany(int capacity = 0) : base(capacity) { }

        internal int KeyCount { get { return Count; } }
        internal int ValueCount { get { var n = 0; foreach (var ent in this) { n += ent.Value.Count; } return n; } }

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
                foreach (var value in val.Value)
                {
                    values[i] = value;
                    i += 1;
                }
            }
            return i;
        }

        internal int GetLinksCount()
        {
            var count = 0;
            foreach (var val in this)
            {
                if (val.Key.IsExternal)
                    count += val.Value.Count;
                else
                    foreach (var value in val.Value) { if (value.IsExternal) count += 1; }
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
                foreach (var value in val.Value)
                {
                    if (val.Key.IsExternal || value.IsExternal)
                    {
                        children[i] = value;
                        parents[i] = val.Key;
                        i += 1;
                    }
                }
            }
            return count;
        }

        internal void SetLink(Item key, T val, int capacity = 0)
        {
            if (TryGetValue(key, out List<T> values))
            {
                values.Add(val);
                return;
            }

            values = (capacity > 0) ? new List<T>(capacity) : new List<T>(1);
            values.Add(val);
            Add(key, values);
        }

        internal int GetValCount(Item key)
        {
            return TryGetValue(key, out List<T> values) ? values.Count : 0;
        }

        internal void AppendLink(Item key, T val)
        {
            if (TryGetValue(key, out List<T> values)) { values.Remove(val); values.Add(val); return; }

            values = new List<T>(1);
            values.Add(val);
            Add(key, values);
        }

        internal void InsertLink(Item key, T val, int index)
        {
            if (TryGetValue(key, out List<T> values))
            {
                values.Remove(val);
                if (index < 0) values.Insert(0, val);
                else if (values.Count > index) values.Insert(index, val);
                else values.Add(val);
                return;
            }

            values = new List<T>(1);
            values.Add(val);
            Add(key, values);
        }

        internal int GetCount(Item key)
        {
            return TryGetValue(key, out List<T> values) ? values.Count : 0;
        }

        internal int GetIndex(Item key, T val)
        {
            int index = -1;

            if (TryGetValue(key, out List<T> values)) index = values.IndexOf(val);

            return index;
        }

        internal void Move(Item key, T val, int index)
        {
            if (TryGetValue(key, out List<T> values))
            {
                if (values.Remove(val))
                {
                    if (index < 0)
                        values.Insert(0, val);
                    else if (index < values.Count)
                        values.Insert(index, val);
                    else
                        values.Add(val);
                }
            }
        }

        internal void RemoveLink(Item key, T val)
        {
            if (TryGetValue(key, out List<T> values))
            {
                values.Remove(val);
                if (values.Count == 0) Remove(key);
            }
        }

        internal bool TryGetVal(Item key, out T val)
        {
            if (TryGetValue(key, out List<T> values))
            {
                val = values[0];
                return true;
            }
            val = null;
            return false;
        }

        internal bool TryGetVals(Item key, out Item[] vals)
        {
            if (TryGetValue(key, out List<T> values))
            {
                vals = values.ToArray();
                return true;
            }
            vals = null;
            return false;
        }

        internal bool TryGetVals(Item key, out List<T> vals)
        {
            return TryGetValue(key, out vals);
        }
        internal bool TryGetAllKeys(out Item[] keys)
        {
            keys = Keys.ToArray();
            return (Count > 0);
        }

        internal bool ContainsLink(Item key, T val)
        {
            return (TryGetValue(key, out List<T> values) && values.Contains(val));
        }

        /// <summary>
        /// Can this mapToMany dictionary be replaced by a mapToOne dictionary
        /// </summary>
        internal bool CanMapToOne => (MaxListCount() < 2);
        private int MaxListCount()
        {
            if (this == null) return 0;
            var max = 0;
            foreach (var ent in this)
            {
                var count = ent.Value.Count;
                if (count > max) max = count;
            }
            return max;
        }
    }
}
