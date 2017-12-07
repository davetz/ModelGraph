using System;

namespace ModelGraphSTD
{
    internal class BoolValue : ValueOfType<bool>
    {
        internal BoolValue(IValueStore<bool> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Bool;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out bool v));
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = (GetVal(key, out bool v));
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = (GetVal(key, out bool v));
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out bool v));
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, value);

        internal override bool SetValue(Item key, int value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, long value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, double value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, string value)
        {
            (var ok, var v) = BoolParse(value);
            return ok ? SetVal(key, v) : false;
        }
        #endregion
    }
}
