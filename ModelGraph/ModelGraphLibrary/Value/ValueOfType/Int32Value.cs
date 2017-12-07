using System;

namespace ModelGraphSTD
{
    internal class Int32Value : ValueOfType<int>
    {
        internal Int32Value(IValueStore<int> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int32;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out int v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out int v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out int v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out int v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, value);

        internal override bool SetValue(Item key, long value) => (value < int.MinValue || value > int.MaxValue) ? false : SetVal(key, (int)value);

        internal override bool SetValue(Item key, double value) => (value < int.MinValue || value > int.MaxValue) ? false : SetVal(key, (int)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = Int32Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
