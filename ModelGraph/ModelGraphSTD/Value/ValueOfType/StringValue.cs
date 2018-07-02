using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class StringValue : ValueOfType<string>
    {
        internal StringValue(IValueStore<string> store) { _valueStore = store; }
        internal override ValType ValType => ValType.String;

        internal ValueDictionary<string> ValueDictionary => _valueStore as ValueDictionary<string>;
        internal override bool IsSpecific(Item key) => _valueStore.IsSpecific(key);

        #region LoadCache  ====================================================
        internal override bool LoadCache(ComputeX cx, Item key, List<Query> qList)
        {
            if (cx == null || qList == null || qList.Count == 0) return false;

            var q = qList[0];
            if (q.Items == null || q.Items.Count == 0) return false;

            var qx = q.QueryX;
            if (!qx.HasSelect) return false;

            var k = q.Items[0];
            if (k == null) return false;

            return (qx.Select.GetValue(k, out string v)) ? SetValue(key, v) : false;
        }
        #endregion

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value) => (GetVal(key, out string v) && bool.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out int value) => (GetVal(key, out string v) && int.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out Int64 value) => (GetVal(key, out string v) && Int64.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out double value) => (GetVal(key, out string v) && double.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out DateTime value) => (GetVal(key, out string v) && DateTime.TryParse(v, out value)) ? true : NoValue(out value);

        internal override bool GetValue(Item key, out string value) => GetVal(key, out value);
        #endregion

        #region GetLength  ====================================================
        internal override bool GetLength(Item key, out int value)
        {
            if (GetVal(key, out string v))
            {
                value = v.Length;
                return true;
            }
            value = 0;
            return false;
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
        internal override bool SetValue(Item key, bool value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, int value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, Int64 value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, double value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, DateTime value) => SetVal(key, value.ToString());

        internal override bool SetValue(Item key, string value) => SetVal(key, value);
        #endregion
    }
}
