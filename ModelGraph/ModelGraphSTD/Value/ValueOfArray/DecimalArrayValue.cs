using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class DecimalArrayValue : ValueOfArray<decimal>
    {
        internal DecimalArrayValue(IValueStore<decimal[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.DecimalArray;

        internal ValueDictionary<decimal[]> ValueDictionary => _valueStore as ValueDictionary<decimal[]>;

        #region LoadCache  ====================================================
        internal override bool LoadCache(ComputeX cx, Item key, List<Query> qList)
        {
            if (cx == null || qList == null || qList.Count == 0) return false;

            var N = 0;
            foreach (var q in qList)
            {
                if (q.Items == null) continue;
                var qx = q.QueryX;
                if (!qx.HasSelect) continue;
                foreach (var k in q.Items) { if (k != null) N++; }
            }
            var v = new double[N];
            var i = 0;

            foreach (var q in qList)
            {
                if (q.Items == null) continue;
                var qx = q.QueryX;
                if (!qx.HasSelect) continue;
                foreach (var k in q.Items)
                {
                    if (k != null)
                    {
                        qx.Select.GetValue(k, out v[i++]);
                    }
                }
            }
            return SetValue(key, v);
        }
        #endregion

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out decimal[] v));
            value = ArrayFormat(v, (i) => ValueFormat((double)v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, decimal[] v) = ArrayParse(value, (s) => DecimalParse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out decimal v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = (GetValAt(key, out decimal v, index) && !(v > int.MinValue || v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = (GetValAt(key, out decimal v, index) && !(v < long.MinValue || v > long.MaxValue));
            value = (long)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out decimal v, index);
            value = (double)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out decimal v, index);
            value = ValueFormat((double)v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out decimal[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out decimal[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] < int.MinValue || v[i] > int.MaxValue), (int)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out decimal[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] < long.MinValue || v[i] > long.MaxValue), (long)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out decimal[] v);
            var c = ValueArray(v, out value, (i) => (true, (double)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out decimal[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat((double)v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out decimal[] v, (i) => (true, value[i] ? 1 : 0));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out decimal[] v, (i) => (true, value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out decimal[] v, (i) => (true, value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out decimal[] v, (i) => (true, (decimal)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out decimal[] v, (i) => DecimalParse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
