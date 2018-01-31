using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class Int16Value : ValueOfType<short>
    {
        internal Int16Value(IValueStore<short> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int16;

        internal ValueDictionary<short> ValueDictionary => _valueStore as ValueDictionary<short>;

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

            return (qx.Select.GetValue(k, out long v)) ? SetValue(key, v) : false;
        }
        #endregion

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out short v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out short v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out short v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (short)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, long value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, double value) => (value < short.MinValue || value > short.MaxValue) ? false : SetVal(key, (short)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = Int16Parse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
