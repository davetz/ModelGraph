using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGraphLibrary
{
    public abstract class ValueOfType<T> : Value
    {
        private T _default = default(T);
        private Dictionary<Item, T> _values;

        #region Properties  ===================================================
        private T Value(Item item) => (_values != null && _values.TryGetValue(item, out T val)) ? val : _default;

        internal bool IsEmpty => Count == 0;
        internal override int Count => (_values == null) ? 0 : _values.Count;

        internal bool IsDefault(T value) => IsDefault(value, _default);
        private bool IsDefault(T v, T d) => (d == null) ? ((v == null) ? true : false) : ((v == null) ? false : d.Equals(v));
        protected override string IdString => string.Empty;
        #endregion

        #region Overrides  ====================================================
        internal override string GetValue(Item item) => ToString(Value(item));
        internal override string GetDefault() => (_default == null) ? null : _default.ToString();
        internal override bool IsValid(string value) => TryParse(value, out T val);

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
            if (TryParse(defaultValue, out T def)) _default = def;
            _values = new Dictionary<Item, T>(capacity);
        }

        internal override bool TrySetValue(Item item, string value)
        {
            if (!TryParse(value, out T val)) return false;
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
            if (!TryParse(value, out T def)) return false;

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

        #region ColumnX-GetValue  =============================================
        internal override void GetValue(Item item, out bool value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToBool(val);
        }
        internal override void GetValue(Item item, out byte value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToByte(val);
        }
        internal override void GetValue(Item item, out sbyte value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToSByte(val);
        }
        internal override void GetValue(Item item, out int value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToInt32(val);
        }
        internal override void GetValue(Item item, out uint value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToUInt32(val);
        }
        internal override void GetValue(Item item, out short value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToInt16(val);
        }
        internal override void GetValue(Item item, out ushort value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToUInt16(val);
        }
        internal override void GetValue(Item item, out long value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToInt64(val);
        }
        internal override void GetValue(Item item, out ulong value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToUInt64(val);
        }
        internal override void GetValue(Item item, out double value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToDouble(val);
        }
        internal override void GetValue(Item item, out string value, int index = 0)
        {
            if (_values == null || !_values.TryGetValue(item, out T val)) val = _default;
            value = ToString(val);
        }
        #endregion

        #region ComputeX-GetValue  ============================================

        internal override void GetValue(ComputeX cx, Item item, out bool value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToBool(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out byte value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToByte(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out sbyte value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToSByte(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out int value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToInt32(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out uint value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToUInt32(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out short value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToInt16(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out ushort value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToUInt16(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out long value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToInt64(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out ulong value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToUInt64(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out double value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToDouble(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        internal override void GetValue(ComputeX cx, Item item, out string value, int index = 0)
        {
            if (_values != null && _values.TryGetValue(item, out T val))
                value = ToString(val);
            else
                cx.GetChef().GetValue(cx, item, out value, index);
        }
        #endregion

        #region SetValue  =====================================================
        internal void SetValue(Item item, T val)
        {
            if (IsEmpty) _values = new Dictionary<Item, T>();

            _values[item] = val;
        }
        #endregion

        #region Methods  ======================================================
        protected abstract bool TryParse(string input, out T value);
        protected abstract bool ToBool(T value);
        protected abstract byte ToByte(T value);
        protected abstract sbyte ToSByte(T value);
        protected abstract ushort ToUInt16(T value);
        protected abstract short ToInt16(T value);
        protected abstract uint ToUInt32(T value);
        protected abstract int ToInt32(T value);
        protected abstract ulong ToUInt64(T value);
        protected abstract long ToInt64(T value);
        protected abstract double ToDouble(T value);
        protected abstract string ToString(T value);
        #endregion
    }
}
