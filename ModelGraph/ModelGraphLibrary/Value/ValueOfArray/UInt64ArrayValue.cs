using System;

namespace ModelGraphSTD
{
    internal class UInt64ArrayValue : ValueOfArray<ulong>
    {
        internal UInt64ArrayValue(IValueStore<ulong[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt64Array;

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
        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out ulong v, index);
            value = (long)v;
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

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out ulong[] v);
            var c = ValueArray(v, out value, (i) => (true, (long)v[i]));
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


        internal override bool SetValue(Item key, long[] value)
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
