using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class UInt32Value : ValueOfType<uint>
    {
        internal UInt32Value(IValueStore<uint> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt32;

        internal ValueDictionary<uint> ValueDictionary => _valueStore as ValueDictionary<uint>;
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
            var b = GetVal(key, out uint v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out uint v);
            value = (int)v;
            return b;
        }

        internal override bool GetValue(Item key, out Int64 value)
        {
            var b = GetVal(key, out uint v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out uint v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out uint v);
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
        internal override bool SetValue(Item key, bool value) => SetVal(key, (uint)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, (uint)value);

        internal override bool SetValue(Item key, Int64 value) => (value < uint.MinValue || value > uint.MaxValue) ? false : SetVal(key, (uint)value);

        internal override bool SetValue(Item key, double value) => (value < uint.MinValue || value > uint.MaxValue) ? false : SetVal(key, (uint)value);

        internal override bool SetValue(Item key, string value)
        {
            var (ok, val) = UInt32Parse(value);
            return (ok) ? SetVal(key, val) : false;
        }
        #endregion
    }
}
