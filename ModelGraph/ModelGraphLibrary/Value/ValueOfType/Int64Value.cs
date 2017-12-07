using System;

namespace ModelGraphSTD
{
    internal class Int64Value : ValueOfType<long>
    {
        internal Int64Value(IValueStore<long> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int64;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out long v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out long v) && !(v < int.MinValue || v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out long value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out long v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out long v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, value);

        internal override bool SetValue(Item key, long value) => SetVal(key, value);

        internal override bool SetValue(Item key, double value) => (value < long.MinValue || value > long.MaxValue) ? false : SetVal(key, (long)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = Int64Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
