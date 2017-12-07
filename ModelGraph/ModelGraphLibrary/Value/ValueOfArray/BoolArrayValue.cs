using System;

namespace ModelGraphSTD
{
    internal class BoolArrayValue : ValueOfArray<bool>
    {
        internal BoolArrayValue(IValueStore<bool[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.BoolArray;

        internal ValueDictionary<bool[]> ValueDictionary => _valueStore as ValueDictionary<bool[]>;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out bool[] v));
            value = ArrayFormat(v, (i) => ValueFormat(v[i], Format));
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            (var ok, bool[] v) = ArrayParse(value, (s) => BoolParse(s));
            return ok ? SetVal(key, v) : false;
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index) => GetValAt(key, out value, index);

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = GetValAt(key, out bool v, index);
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out bool v, index);
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out bool v, index);
            value = v ? 1 : 0;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out bool v, index);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value) => GetVal(key, out value);

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out bool[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] ? 1 : 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out bool[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] ? 1 : 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out bool[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] ? 1 : 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out bool[] v);
            var c = ValueArray(v, out value, (i) => ValueFormat(v[i], Format));
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value) => SetVal(key, value);

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out bool[] v, (i) => (true, value[i] != 0));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out bool[] v, (i) => (true, value[i] != 0));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out bool[] v, (i) => (true, value[i] != 0));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value)
        {
            var c = ValueArray(value, out bool[] v, (i) => BoolParse(value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
