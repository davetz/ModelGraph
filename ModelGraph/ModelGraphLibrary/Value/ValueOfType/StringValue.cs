using System;

namespace ModelGraphSTD
{
    internal class StringValue : ValueOfType<string>
    {
        internal StringValue(IValueStore<string> store) { _valueStore = store; }
        internal override ValType ValType => ValType.String;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value) => (GetVal(key, out string v) && bool.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out int value) => (GetVal(key, out string v) && int.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out long value) => (GetVal(key, out string v) && long.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out double value) => (GetVal(key, out string v) && double.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out DateTime value) => (GetVal(key, out string v) && DateTime.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out string value) => GetVal(key, out value);
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, int value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, long value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, double value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, DateTime value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, string value) => SetVal(key, value);
        #endregion
    }
}
