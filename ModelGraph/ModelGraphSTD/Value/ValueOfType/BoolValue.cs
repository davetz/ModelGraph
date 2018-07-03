using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class BoolValue : ValueOfType<bool>
    {
        internal BoolValue(IValueStore<bool> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Bool;

        internal ValueDictionary<bool> ValueDictionary => _valueStore as ValueDictionary<bool>;

        internal override bool IsSpecific(Item key) => _valueStore.IsSpecific(key);

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

        internal override bool GetValue(Item key, out Int64 value)
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

        #region PseudoArrayValue  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetValue(key, out bool v);
            value = new bool[] { v };
            return b;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetValue(key, out int v);
            value = new int[] { v };
            return b;
        }

        internal override bool GetValue(Item key, out Int64[] value)
        {
            var b = GetValue(key, out Int64 v);
            value = new Int64[] { v };
            return b;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetValue(key, out double v);
            value = new double[] { v };
            return b;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetValue(key, out string v);
            value = new string[] { v };
            return b;
        }
        internal override bool GetValue(Item key, out DateTime[] value)
        {
            var b = GetValue(key, out DateTime v);
            value = new DateTime[] { v };
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, value);

        internal override bool SetValue(Item key, int value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, Int64 value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, double value) => SetVal(key, (value != 0));

        internal override bool SetValue(Item key, string value)
        {
            (var ok, var v) = BoolParse(value);
            return ok ? SetVal(key, v) : false;
        }
        #endregion
    }
}
