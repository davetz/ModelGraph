using System;

namespace ModelGraph.Internals
{
    internal class Int16Value : ValueOfType<short>
    {
        internal Int16Value(IValueStore<short> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int16;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out short v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out short v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (short)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, long value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, double value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = Int16Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
