using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGraphLibrary
{
    public abstract class ValuesOf<T> : Value
    {
        private T _default = default(T);
        private Dictionary<Item, T> _values;

        #region Properties  ===================================================
        private T Value(Item item) { T val; return (_values != null && _values.TryGetValue(item, out val)) ? val : _default; }

        internal bool IsEmpty { get { return (Count == 0); } }
        internal override int Count { get { return (_values == null) ? 0 : _values.Count; } }

        internal bool IsDefault(T value) { return IsDefault(value, _default); }
        internal bool IsDefault(T v, T d) { return (d == null) ? ((v == null) ? true : false) : ((v == null) ? false : d.Equals(v)); }
        #endregion

        #region Overrides  ====================================================
        internal override string GetValue(Item item) { return ToString(Value(item)); }
        internal override string GetDefault() { return (_default == null) ? null : _default.ToString(); }

        internal override bool TryGetKeys(out Item[] keys)
        {
            if (IsEmpty)
            {
                keys = null;
                return false;
            }

            keys = _values.Keys.ToArray();
            return true;
        }

        internal override bool IsValid(string value)
        {
            T val;
            return TryParse(value, out val);
        }

        internal override void SetCapacity(int capacity)
        {
            var count = Count;
            if (count < capacity)
            {
                var oldValues = _values;
                _values = new Dictionary<Item, T>(capacity);

                if (oldValues != null)
                {
                    foreach (var ent in oldValues) { _values[ent.Key] = ent.Value; }
                }
            }
        }

        internal override void Initialize(string defaultValue, int capacity)
        {
            T def;
            if (TryParse(defaultValue, out def)) _default = def;
            _values = new Dictionary<Item, T>(capacity);
        }

        internal override bool TrySetValue(Item item, string value)
        {
            T val;
            if (!TryParse(value, out val)) return false;
            if (IsEmpty) _values = new Dictionary<Item, T>();

            if (item != null)
            {
                if (IsDefault(val))
                    _values.Remove(item);
                else
                    _values[item] = val;
            }

            return true;
        }

        internal override bool TrySetDefault(Item[] items, string value)
        {
            T def;
            if (!TryParse(value, out def)) return false;

            if (items == null || items.Length == 0)
            {
                _default = def;
                _values = null;
            }
            else
            {
                var len = items.Length;
                var vals = new T[len];
                int cnt = 0;
                for (int i = 0; i < len; i++) { vals[i] = Value(items[i]); if (!IsDefault(vals[i])) cnt += 1; }

                _default = def;
                _values = new Dictionary<Item, T>(cnt);
                for (int i = 0; i < len; i++)
                {
                    var item = items[i];
                    var val = vals[i];

                    if (IsDefault(val)) continue;
                    _values[item] = val;
                }
            }
            return true;
        }

        internal override void SetDefault(Item[] items)
        {
            if (items == null || items.Length == 0)
            {
                _default = default(T);
                _values = null;
            }
            else
            {
                var len = items.Length;
                var vals = new T[len];
                var counts = new Dictionary<T, int>(len);
                for (int i = 0; i < len; i++)
                {
                    var item = items[i];
                    T val = Value(item);
                    vals[i] = val;
                    int n;
                    if (counts.TryGetValue(val, out n))
                        counts[val] = n + 1;
                    else
                        counts[val] = 0;
                }

                int bestN = 0;
                T bestVal = _default;
                foreach (var ent in counts)
                {
                    if (ent.Value < bestN) continue;
                    bestN = ent.Value;
                    bestVal = ent.Key;
                }
                _default = bestVal;

                if (counts.Count == 1)
                {
                    _values = null;
                }
                else
                {
                    _values = new Dictionary<Item, T>(items.Length - bestN);
                    for (int i = 0; i < len; i++)
                    {
                        var item = items[i];
                        var val = vals[i];

                        if (IsDefault(val)) continue;
                        _values[item] = val;
                    }
                }
            }
        }

        internal override void Clear()
        {
            _values = null;
        }

        internal override bool IsSpecific(Item item) { return _values != null && _values.ContainsKey(item); }

        internal override bool TryGetSerializedValue(Item item, out string value)
        {
            value = null;
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) return false;

            value = (val == null) ? string.Empty : val.ToString();
            return true;
        }
        internal override bool HasSpecificDefault()
        {
            var def = default(T);
            if (def == null)
                return (_default != null);
            else
                return (_default == null) ? true : !def.Equals(_default);
        }
        internal override void RemoveValue(Item item)
        {
            if (_values != null) _values.Remove(item);
        }
        #endregion

        #region GetValue  =====================================================
        internal override void GetValue(Item item, out bool value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToBool(val);
        }
        internal override void GetValue(Item item, out byte value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToByte(val);
        }
        internal override void GetValue(Item item, out int value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToInt32(val);
        }
        internal override void GetValue(Item item, out short value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToInt16(val);
        }
        internal override void GetValue(Item item, out long value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToInt64(val);
        }
        internal override void GetValue(Item item, out double value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToDouble(val);
        }
        internal override void GetValue(Item item, out string value)
        {
            T val;
            if (_values == null || !_values.TryGetValue(item, out val)) val = _default;
            value = ToString(val);
        }
        #endregion

        #region Methods  ======================================================
        protected abstract bool TryParse(string input, out T value);
        protected abstract bool ToBool(T value);
        protected abstract byte ToByte(T value);
        protected abstract short ToInt16(T value);
        protected abstract int ToInt32(T value);
        protected abstract long ToInt64(T value);
        protected abstract double ToDouble(T value);
        protected abstract string ToString(T value);
        #endregion
    }
}
