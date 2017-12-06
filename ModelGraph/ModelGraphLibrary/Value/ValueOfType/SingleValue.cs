using System;

namespace ModelGraphLibrary
{
    internal class SingleValue : ValueOfType<float>
    {
        internal SingleValue(IValueStore<float> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Single;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out float v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out float v) && !(v < int.MinValue || v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = (GetVal(key, out float v) && !(v < long.MinValue || v > long.MaxValue));
            value = (long)v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out float v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out float v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, value);

        internal override bool SetValue(Item key, long value) => SetVal(key, value);

        internal override bool SetValue(Item key, double value) => SetVal(key, (float)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = SingleParse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}

