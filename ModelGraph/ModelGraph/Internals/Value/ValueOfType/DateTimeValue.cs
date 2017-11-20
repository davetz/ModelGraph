using System;

namespace ModelGraph.Internals
{
    internal class DateTimeValue : ValueOfType<DateTime>
    {
        internal DateTimeValue(IValueStore<DateTime> store) { _valueStore = store; }
        internal override ValType ValType => ValType.DateTime;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out DateTime value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out DateTime v);
            value = v.ToString();
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, DateTime value) => SetVal(key, value);

        internal override bool SetValue(Item key, string value) => DateTime.TryParse(value, out DateTime v) ? SetVal(key, v) : false;
        #endregion
    }
}

