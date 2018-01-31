using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class BoolValue : ValueOfType<bool>
    {
        internal BoolValue(IValueStore<bool> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Bool;

        internal ValueDictionary<bool> ValueDictionary => _valueStore as ValueDictionary<bool>;

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

            return (qx.Select.GetValue(k, out bool v)) ? SetValue(key, v) : false;
        }
        #endregion

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
