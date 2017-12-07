using System;

namespace ModelGraphSTD
{
    internal class UInt32ArrayValue : ValueOfArray<uint>
    {
        internal UInt32ArrayValue(IValueStore<uint[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.UInt32Array;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out uint[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, uint[] v) = ArrayParse(value, (s) => UInt32Parse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out uint v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = GetValAt(key, out uint v, index);
            value = (int)v;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out uint v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out uint v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out uint v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out uint[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out uint[] v);
            var c = ValueArray(v, out value, (i) => (true, (int)v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out uint[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out uint[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out uint[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out uint[] v, (i) => (true, (uint)(value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out uint[] v, (i) => (true, (uint)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

    internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out uint[] v, (i) => (!(value[i] < uint.MinValue || value[i] > uint.MaxValue), (uint)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out uint[] v, (i) => (!(value[i] < uint.MinValue || value[i] > uint.MaxValue), (uint)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out uint[] v, (i) => UInt32Parse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}

