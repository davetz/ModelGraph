using System;

namespace ModelGraphLibrary
{
    internal class UInt32Value : ValueOfType<uint>
    {
        internal UInt32Value(IValueStore<uint> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt32;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out uint v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out uint v);
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out uint v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out uint v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out uint v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (uint)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, (uint)value);

        internal override bool SetValue(Item key, long value) => (value < uint.MinValue || value > uint.MaxValue) ? false : SetVal(key, (uint)value);

        internal override bool SetValue(Item key, double value) => (value < uint.MinValue || value > uint.MaxValue) ? false : SetVal(key, (uint)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = UInt32Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
