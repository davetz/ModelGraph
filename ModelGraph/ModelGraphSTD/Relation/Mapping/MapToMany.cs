using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class MapToMany<T> : Dictionary<Item, List<T>> where T : Item
    {
        internal MapToMany(int capacity = 0) : base(capacity) { }

        internal int KeyCount => Count;
        internal int ValueCount { get { var n = 0; foreach (var e in this) { n += e.Value.Count; } return n; } }


        internal int GetLinksCount()
        {
            var n = 0;
            foreach (var e in this)
            {
                if (e.Key.IsExternal) n += e.Value.Count;
                else foreach (var v in e.Value) { if (v.IsExternal) n++; }
            }
            return n;
        }

        internal int GetLinks(out List<Item> parents, out List<Item> children)
        {
            var n = GetLinksCount();
            children = new List<Item>(n);
            parents = new List<Item>(n);

            foreach (var e in this)
            {
                foreach (var v in e.Value)
                {
                    if (e.Key.IsExternal || v.IsExternal)
                    {
                        children.Add(v);
                        parents.Add(e.Key);
                    }
                }
            }
            return n;
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

        internal int GetValCount(Item key) => TryGetValue(key, out List<T> vals) ? vals.Count : 0;

        internal void InsertLink(Item key, T val, int index)
        {
            if (TryGetValue(key, out List<T> values))
            {
                values.Remove(val);
                if (index < 0) values.Insert(0, val);
                else if (values.Count > index) values.Insert(index, val);
                else values.Add(val);
            }
            else
            {
                values = new List<T>(1) { val };
                Add(key, values);
            }
        }

        internal int GetIndex(Item key, T val) => TryGetValue(key, out List<T> vals) ? vals.IndexOf(val) : -1;

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

        internal bool TryGetVals(Item key, out IList<T> vals)
        {
            if (TryGetValue(key, out List<T> values))
            {
                vals = values.AsReadOnly(); // protected from acidental corruption
                return true;
            }
            vals = null;
            return false;
        }

        internal bool ContainsLink(Item key, T val) => (TryGetValue(key, out List<T> values) && values.Contains(val));

        /// <summary>
        /// Can this mapToMany dictionary be replaced by a mapToOne dictionary
        /// </summary>
        internal bool CanMapToOne
        {
            get
            {
                foreach (var e in this) { if (e.Value.Count > 1) return false; }
                return true;
            }
        }
    }
}
