using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class DateTimeArrayValue : ValueOfArray<DateTime>
    {
        internal DateTimeArrayValue(IValueStore<DateTime[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.DateTimeArray;

        internal ValueDictionary<DateTime[]> ValueDictionary => _valueStore as ValueDictionary<DateTime[]>;

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
            var v = new DateTime[N];
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
            var b = (GetVal(key, out DateTime[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, DateTime[] v) = ArrayParse(value, (s) => DateTimeParse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out DateTime value, int index) => GetValAt(key, out value, index);

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out DateTime v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out DateTime[] value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out DateTime[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, DateTime[] value) => SetVal(key, value);

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out DateTime[] v, (i) => DateTimeParse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
