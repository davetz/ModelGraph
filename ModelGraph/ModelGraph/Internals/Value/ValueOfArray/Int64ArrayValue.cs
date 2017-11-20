﻿using System;

namespace ModelGraph.Internals
{
    internal class Int64ArrayValue : ValueOfArray<long>
    {
        internal Int64ArrayValue(IValueStore<long[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Int64Array;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out long[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, long[] v) = ArrayParse(value, (s) => Int64Parse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out long v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = (GetValAt(key, out long v, index) && !(v < int.MinValue || v > int.MaxValue));
            value = (int)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index) => GetValAt(key, out value, index);

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out long v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out long v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out long[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out long[] v);
            var c = ValueArray(v, out value, (i) => (!(v[i] < int.MinValue || v[i] > int.MaxValue), (int)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out long[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out long[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out long[] v, (i) => (true, (value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out long[] v, (i) => (true, value[i]));
            var b = SetVal(key, v);
            return b && c;
        }


        internal override bool SetValue(Item key, long[] value) => SetVal(key, value);

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out long[] v, (i) => (!(value[i] < long.MinValue || value[i] > long.MaxValue), (long)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out long[] v, (i) => Int64Parse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
