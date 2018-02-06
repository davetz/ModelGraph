using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class DoubleArrayValue : ValueOfArray<double>
    {
        internal DoubleArrayValue(IValueStore<double[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.DoubleArray;

        internal ValueDictionary<double[]> ValueDictionary => _valueStore as ValueDictionary<double[]>;

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
            var b = (GetVal(key, out double[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, double[] v) = ArrayParse(value, (s) => DoubleParse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out double v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = (GetValAt(key, out double v, index) && !(v < int.MinValue || v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out Int64 value, int index)
        {
            var b = (GetValAt(key, out double v, index) && !(v < Int64.MinValue || v > Int64.MaxValue));
            value = (Int64)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out double v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out double v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetLength  ====================================================
        internal override bool GetLength(Item key, out int value)
        {
            if (GetVal(key, out double[] v))
            {
                value = v.Length;
                return true;
            }
            value = 0;
            return false;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out double[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out double[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] < int.MinValue || v[i] > int.MaxValue), (int)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out Int64[] value)
        {
            var b = GetVal(key, out double[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] < Int64.MinValue || v[i] > Int64.MaxValue), (Int64)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out double[] v);
            var c = ValueArray(v, out value, (i) => (true, (double)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out double[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out double[] v, (i) => (true, value[i] ? 1 : 0));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out double[] v, (i) => (true, value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, Int64[] value)
        {
            var c = ValueArray(value, out double[] v, (i) => (true, value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value) => SetVal(key, value);

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out double[] v, (i) => DoubleParse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
