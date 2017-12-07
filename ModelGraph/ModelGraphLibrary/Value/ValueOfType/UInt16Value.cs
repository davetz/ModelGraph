using System;

namespace ModelGraphSTD
{
    internal class UInt16Value : ValueOfType<ushort>
    {
        internal UInt16Value(IValueStore<ushort> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt16;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out ushort v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out ushort v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out ushort v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out ushort v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out ushort v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (ushort)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => (value < ushort.MinValue || value > ushort.MaxValue) ? false : SetVal(key, (ushort)value);

        internal override bool SetValue(Item key, long value) => (value < ushort.MinValue || value > ushort.MaxValue) ? false : SetVal(key, (ushort)value);

        internal override bool SetValue(Item key, double value) => (value < ushort.MinValue || value > ushort.MaxValue) ? false : SetVal(key, (ushort)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = UInt16Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
