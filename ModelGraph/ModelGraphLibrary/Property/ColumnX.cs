using System;

namespace ModelGraphLibrary
{/*

 */
    public class ColumnX : Property
    {
        private Value _values;
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Initial;
        internal string Description;

        #region Constructor  ==================================================
        internal ColumnX(StoreOf<ColumnX> owner)
        {
            Owner = owner;
            Trait = Trait.ColumnX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            _values = Value.Create(ValueType.String);

            owner.Add(this);
        }
        internal ColumnX(StoreOf<ColumnX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.ColumnX;
            Guid = guid;

            owner.Add(this);
        }
        internal void Initialize(ValueType type, string defaultVal, int count)
        {
            _values = Value.Create(type);
            _values.Initialize(defaultVal, count);
        }
        #endregion

        #region Properties/Methods ============================================
        internal override bool HasItemName => false;
        internal override string GetItemName(Item itm) { return null; }
        internal override ValueType ValueType => _values.ValueType;
        internal bool HasSpecificDefault => _values.HasSpecificDefault();
        internal bool IsSpecific(Item item) { return _values.IsSpecific(item); }
        internal string Default => (_values == null) ? null : _values.GetDefault();
        internal void SetDefault(Item[] items) { _values.SetDefault(items); }
        internal override string GetValue(Item item, NumericTerm term = NumericTerm.None) { return _values.GetValue(item); }
        internal override bool GetBool(Item item) { bool val; GetValue(item, out val); return val; }
        internal override bool TrySetValue(Item item, string value) { return _values.TrySetValue(item, value); }
        internal override void GetValue(Item item, out bool value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out byte value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out sbyte value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out uint value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out int value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out ushort value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out short value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out ulong value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, out long value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, NumericTerm term, out double value) { _values.GetValue(item, out value); }
        internal override void GetValue(Item item, NumericTerm term, out string value) { _values.GetValue(item, out value); }

        internal bool TrySetDefault(Item[] items, string defaultValue)
        {
            return _values == null ? false : _values.TrySetDefault(items, defaultValue);
        }

        internal void RemoveValue(Item item)
        {
            if (_values != null) _values.RemoveValue(item);
        }
        internal bool TryGetKeys(out Item[] items)
        {
            if (_values == null)
            {
                items = null;
                return false;
            }
            return _values.TryGetKeys(out items);
        }
        internal bool TrySetValueType(Item[] items, ValueType type)
        {
            if (_values.ValueType == type) return true;

            var len = (items == null) ? 0 : items.Length;
            if (len > 0)
            {
                var newValues = Value.Create(type);
                foreach (var item in items)
                {
                    var val = _values.GetValue(item);
                    if (string.IsNullOrEmpty(val)) continue;
                    if (!newValues.TrySetValue(item, val)) return false;
                }
                _values.Clear();
                _values = newValues;
            }
            else
            {
                _values = Value.Create(type);
            }
            return true;
        }
        #endregion
    }
}

