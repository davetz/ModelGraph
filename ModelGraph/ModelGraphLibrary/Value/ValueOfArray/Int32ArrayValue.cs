using System;

namespace ModelGraphLibrary
{
    internal class Int32ArrayValue : ValueOfArray<int>
    {
        internal Int32ArrayValue(IValueStore<int[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int32Array;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out int[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, int[] v) = ArrayParse(value, (s) => Int32Parse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out int v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index) => GetValAt(key, out value, index);

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out int v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out int v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out int v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out int[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out int[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out int[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out int[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out int[] v, (i) => (true, (value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value) => SetVal(key, value);

        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out int[] v, (i) => (!(value[i] < int.MinValue || value[i] > int.MaxValue), (int)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out int[] v, (i) => (!(value[i] < int.MinValue || value[i] > int.MaxValue), (int)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out int[] v, (i) => Int32Parse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}

