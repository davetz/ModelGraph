using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class UInt64ArrayValue : ValueOfArray<ulong>
    {
        internal UInt64ArrayValue(IValueStore<ulong[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt64Array;

        internal ValueDictionary<ulong[]> ValueDictionary => _valueStore as ValueDictionary<ulong[]>;

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
            var v = new Int64[N];
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
            var b = (GetVal(key, out ulong[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, ulong[] v) = ArrayParse(value, (s) => UInt64Parse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out ulong v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = (GetValAt(key, out ulong v, index) && !(v > int.MaxValue));
            value = (int)v;
            return b;
        }
        internal override bool GetValueAt(Item key, out Int64 value, int index)
        {
            var b = GetValAt(key, out ulong v, index);
            value = (Int64)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out ulong v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out ulong v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetLength  ====================================================
        internal override bool GetLength(Item key, out int value)
        {
            if (GetVal(key, out ulong[] v))
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
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] > int.MaxValue), (int)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out Int64[] value)
        {
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => (true, (Int64)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }
        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out ulong[] v, (i) => (true, (ulong)(value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out ulong[] v, (i) => (true, (ulong)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }


        internal override bool SetValue(Item key, Int64[] value)
        {
            var c = ValueArray(value, out ulong[] v, (i) => (true, (ulong)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out ulong[] v, (i) => (!(value[i] < ulong.MinValue || value[i] > ulong.MaxValue), (ulong)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out ulong[] v, (i) => UInt64Parse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
