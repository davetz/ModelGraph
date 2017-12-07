
using System;

namespace ModelGraphSTD
{
    internal class SByteValue : ValueOfType<sbyte>
    {
        internal SByteValue(IValueStore<sbyte> store) { _valueStore = store; }
        internal override ValType ValType => ValType.SByte;

        internal ValueDictionary<sbyte> ValueDictionary => _valueStore as ValueDictionary<sbyte>;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out sbyte v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out sbyte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out sbyte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out sbyte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out sbyte v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (sbyte)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => (value < sbyte.MinValue || value > sbyte.MaxValue) ? false : SetVal(key, (sbyte)value);

        internal override bool SetValue(Item key, long value) => (value < sbyte.MinValue || value > sbyte.MaxValue) ? false : SetVal(key, (sbyte)value);

        internal override bool SetValue(Item key, double value) => (value < sbyte.MinValue || value > sbyte.MaxValue) ? false : SetVal(key, (sbyte)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = SByteParse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
