
namespace ModelGraphLibrary
{
    internal class SByteArrayValue : ValueOfArray<sbyte>
    {
        internal SByteArrayValue(IValueStore<sbyte[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.SByteArray;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out sbyte[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, sbyte[] v) = ArrayParse(value, (s) => SByteParse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out sbyte v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = GetValAt(key, out sbyte v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out sbyte v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out sbyte v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out sbyte v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out sbyte[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out sbyte[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out sbyte[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out sbyte[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out sbyte[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out sbyte[] v, (i) => (true, (sbyte)(value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out sbyte[] v, (i) => (!(value[i] < sbyte.MinValue || value[i] > sbyte.MaxValue), (sbyte)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out sbyte[] v, (i) => (!(value[i] < sbyte.MinValue || value[i] > sbyte.MaxValue), (sbyte)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out sbyte[] v, (i) => (!(value[i] < sbyte.MinValue || value[i] > sbyte.MaxValue), (sbyte)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out sbyte[] v, (i) => SByteParse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
