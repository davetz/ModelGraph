using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class UInt64Value : ValueOfType<ulong>
    {
        internal UInt64Value(IValueStore<ulong> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt64;

        internal ValueDictionary<ulong> ValueDictionary => _valueStore as ValueDictionary<ulong>;
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

            return (qx.Select.GetValue(k, out Int64 v)) ? SetValue(key, v) : false;
        }
        #endregion

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out ulong v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out ulong v) && !(v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out Int64 value)
        {
            var b = GetVal(key, out ulong v);
            value = (Int64)v;
            return b;
        }
    internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out ulong v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out ulong v);
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
        internal override bool SetValue(Item key, bool value) => SetVal(key, (ulong)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, Int64 value) => SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, double value) => (value < ulong.MinValue || value > ulong.MaxValue) ? false : SetVal(key, (ulong)value);

        internal override bool SetValue(Item key, string value)
        {
            var (ok, val) = UInt64Parse(value);
            return (ok) ? SetVal(key, val) : false;
        }
        #endregion
    }
}
