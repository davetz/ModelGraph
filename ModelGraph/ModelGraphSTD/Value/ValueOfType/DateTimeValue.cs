using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class DateTimeValue : ValueOfType<DateTime>
    {
        internal DateTimeValue(IValueStore<DateTime> store) { _valueStore = store; }
        internal override ValType ValType => ValType.DateTime;

        internal ValueDictionary<DateTime> ValueDictionary => _valueStore as ValueDictionary<DateTime>;

        #region LoadCache  ====================================================
        internal override bool LoadCache(ComputeX cx, Item key, List<Query> qList)
        {
            if (cx == null || qList == null || qList.Count == 0) return false;

            var q = qList[0];
            if (q.Items == null || q.Items.Length == 0) return false;

            var qx = q.QueryX;
            if (!qx.HasSelect) return false;

            var k = q.Items[0];
            if (k == null) return false;

            return (qx.Select.GetValue(k, out DateTime v)) ? SetValue(key, v) : false;
        }
        #endregion

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

