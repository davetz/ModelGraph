using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGraphLibrary
{
    internal class ValueOfProperty : Value
    {
        internal readonly Property Property;

        internal ValueOfProperty(Property property)
        {
            Property = property;
        }

        #region Properties  ===================================================
        internal bool IsEmpty => true;
        internal override int Count => 0;

        protected override string IdString => string.Empty;
        #endregion

        #region Overrides  ====================================================
        internal override ValueType ValueType => Property.ValueType;
        internal override string GetValue(Item item) => Property.GetValue(item);
        internal override void Clear() { }
        internal override string GetDefault() => string.Empty;
        internal override bool IsValid(string value) => false;
        internal override bool TryGetKeys(out Item[] keys) { keys = null; return false; }
        internal override void SetCapacity(int capacity) { }
        internal override void Initialize(string defaultValue, int capacity) { }
        internal override bool TrySetValue(Item item, string value) => Property.TrySetValue(item, value);
        internal override bool TrySetDefault(Item[] items, string value) => false;
        internal override void SetDefault(Item[] items) { }
        internal override bool IsSpecific(Item item) => false;
        internal override bool TryGetSerializedValue(Item item, out string value) { value = null; return false; }
        internal override bool HasSpecificDefault() => false;
        internal override void RemoveValue(Item item) { }
        #endregion

        #region GetValue  =====================================================
        internal override void GetValue(Item item, out bool value, int index = 0) => value = ToBool(Property.GetValue(item));
        internal override void GetValue(Item item, out byte value, int index = 0) => value = ToByte(Property.GetValue(item));
        internal override void GetValue(Item item, out sbyte value, int index = 0) => value = ToSByte(Property.GetValue(item));
        internal override void GetValue(Item item, out int value, int index = 0) => value = ToInt32(Property.GetValue(item));
        internal override void GetValue(Item item, out uint value, int index = 0) => value = ToUInt32(Property.GetValue(item));
        internal override void GetValue(Item item, out short value, int index = 0) => value = ToInt16(Property.GetValue(item));
        internal override void GetValue(Item item, out ushort value, int index = 0) => value = ToUInt16(Property.GetValue(item));
        internal override void GetValue(Item item, out long value, int index = 0) => value = ToInt64(Property.GetValue(item));
        internal override void GetValue(Item item, out ulong value, int index = 0) => value = ToUInt64(Property.GetValue(item));
        internal override void GetValue(Item item, out double value, int index = 0) => value = ToDouble(Property.GetValue(item));
        internal override void GetValue(Item item, out string value, int index = 0) => value = Property.GetValue(item);

        internal override void GetValue(ComputeX cx, Item item, out bool value, int index = 0) => value = false;
        internal override void GetValue(ComputeX cx, Item item, out byte value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out sbyte value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out int value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out uint value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out short value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out ushort value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out long value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out ulong value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out double value, int index = 0) => value = 0;
        internal override void GetValue(ComputeX cx, Item item, out string value, int index = 0) => value = string.Empty;
        #endregion
    }
}
