using System;

namespace ModelGraphSTD
{
    internal class UInt64Value : ValueOfType<ulong>
    {
        internal UInt64Value(IValueStore<ulong> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt64;

        internal ValueDictionary<ulong> ValueDictionary => _valueStore as ValueDictionary<ulong>;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out ulong v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out ulong v) && !(v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out ulong v);
            value = (long)v;
            return b;
        }
    internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out ulong v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out ulong v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (ulong)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, long value) => SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, double value) => (value < ulong.MinValue || value > ulong.MaxValue) ? false : SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = UInt64Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
