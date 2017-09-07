using System;
using System.Text;

namespace ModelGraphLibrary
{/*
 */
    internal class SelectQuery : WhereQuery
    {
        internal ICacheValue[] ValueCache;

        internal SelectQuery(string text) : base(text) { }

        internal void GetValue(Item item, out bool value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[0].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[0].SetValue(item, value);
            }
            value = false;
        }
        internal void GetValue(Item item, out byte value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            if (ValueCache == null) { _root.GetValue(out value); return; }

            if (ValueCache[0].GetValue(item, out value)) return;

            _root.GetValue(out value);
            ValueCache[0].SetValue(item, value);
        }
        internal void GetValue(Item item, out short value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[0].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[0].SetValue(item, value);
            }
            value = 0;
        }
        internal void GetValue(Item item, out int value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[0].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[0].SetValue(item, value);
            }
            value = 0;
        }
        internal void GetValue(Item item, out long value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[0].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[0].SetValue(item, value);
            }
            value = 0;
        }
        internal void GetValue(Item item, NumericTerm term, out double value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[(int)term].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[(int)term].SetValue(item, value);
            }
            value = 0;
        }
        internal void GetValue(Item item, NumericTerm term, out string value)
        {
            if (_root == null || ValueCache != null)
            {
                _item = item;
                if (ValueCache[(int)term].GetValue(item, out value)) return;
                _root.GetValue(out value);
                ValueCache[(int)term].SetValue(item, value);
            }
            value = Chef.InvalidItem;
        }
    }
}
