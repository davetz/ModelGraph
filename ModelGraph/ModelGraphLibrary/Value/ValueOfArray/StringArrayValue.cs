using System;

namespace ModelGraph.Internals
{
    internal class StringArrayValue : ValueOfArray<string>
    {
        internal StringArrayValue(IValueStore<string[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.StringArray;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value) => throw new NotImplementedException();
        internal override bool SetValue(Item key, string value) => throw new NotImplementedException();
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out string v, index);
            (bool c, bool val) = BoolParse(v);
            value = val;
            return (b && c);
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = GetValAt(key, out string v, index);
            (bool c, int val) = Int32Parse(v);
            value = val;
            return (b && c);
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out string v, index);
            (bool c, long val) = Int64Parse(v);
            value = val;
            return (b && c);
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out string v, index);
            (bool c, double val) = DoubleParse(v);
            value = val;
            return (b && c);
        }

        internal override bool GetValueAt(Item key, out DateTime value, int index)
        {
            var b = GetValAt(key, out string v, index);
            (bool c, DateTime val) = DateTimeParse(v);
            value = val;
            return (b && c);
        }

        internal override bool GetValueAt(Item key, out string value, int index) => GetValAt(key, out value, index);
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out string[] v);
            var c = ValueArray(v, out value, (i) => BoolParse(v[i]));
            return (b && c);
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out string[] v);
            var c = ValueArray(v, out value, (i) => Int32Parse(v[i]));
            return (b && c);
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out string[] v);
            var c = ValueArray(v, out value, (i) => Int64Parse(v[i]));
            return (b && c);
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out string[] v);
            var c = ValueArray(v, out value, (i) => DoubleParse(v[i]));
            return (b && c);
        }

        internal override bool GetValue(Item key, out DateTime[] value)
        {
            var b = GetVal(key, out string[] v);
            var c = ValueArray(v, out value, (i) => DateTimeParse(v[i]));
            return (b && c);
        }

        internal override bool GetValue(Item key, out string[] value) => GetVal(key, out value);
        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out string[] v, (i) => value[i].ToString());
            var b = SetVal(key, v);
            return b && c;
        }
        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out string[] v, (i) => value[i].ToString());
            var b = SetVal(key, v);
            return b && c;
        }
        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out string[] v, (i) => value[i].ToString());
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out string[] v, (i) => value[i].ToString());
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, DateTime[] value)
        {
            var c = ValueArray(value, out string[] v, (i) => value[i].ToString());
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, string[] value) => SetVal(key, value);
        #endregion
    }
}
