
using System;

namespace ModelGraphSTD
{
    internal class CharArrayValue : ValueOfArray<char>
    {
        internal CharArrayValue(IValueStore<char[]> store) { _valueStore = store; }
        internal override ValType ValType => ValType.CharArray;

        internal ValueDictionary<char[]> ValueDictionary => _valueStore as ValueDictionary<char[]>;

        #region Required  =====================================================
        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out char[] v));
            value = (v is null) ? string.Empty : v.ToString();
            return b;
        }
        internal override bool SetValue(Item key, string value)
        {
            return SetVal(key, value.ToCharArray());
        }
        #endregion

        #region GetValueAt  ===================================================
        internal override bool GetValueAt(Item key, out bool value, int index)
        {
            var b = GetValAt(key, out char v, index);
            value = (v != 0);
            return b;
        }

        internal override bool GetValueAt(Item key, out int value, int index)
        {
            var b = GetValAt(key, out char v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out long value, int index)
        {
            var b = GetValAt(key, out char v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out double value, int index)
        {
            var b = GetValAt(key, out char v, index);
            value = v;
            return b;
        }

        internal override bool GetValueAt(Item key, out string value, int index)
        {
            var b = GetValAt(key, out char v, index);
            value = v.ToString();
            return b;
        }
        #endregion

        #region GetValue (array)  =============================================
        internal override bool GetValue(Item key, out bool[] value)
        {
            var b = GetVal(key, out char[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i] != 0));
            return b && c;
        }

        internal override bool GetValue(Item key, out int[] value)
        {
            var b = GetVal(key, out char[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out long[] value)
        {
            var b = GetVal(key, out char[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out double[] value)
        {
            var b = GetVal(key, out char[] v);
            var c = ValueArray(v, out value, (i) => (true, v[i]));
            return b && c;
        }

        internal override bool GetValue(Item key, out string[] value)
        {
            var b = GetVal(key, out char[] v);
            var c = ValueArray(v, out value, (i) => v[i].ToString());
            return b && c;
        }

        #endregion

        #region SetValue (array) ==============================================
        internal override bool SetValue(Item key, bool[] value)
        {
            var c = ValueArray(value, out char[] v, (i) => (true, (char)(value[i] ? 1 : 0)));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, int[] value)
        {
            var c = ValueArray(value, out char[] v, (i) => (!(value[i] < char.MinValue || value[i] > char.MaxValue), (char)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, long[] value)
        {
            var c = ValueArray(value, out char[] v, (i) => (!(value[i] < char.MinValue || value[i] > char.MaxValue), (char)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }

        internal override bool SetValue(Item key, double[] value)
        {
            var c = ValueArray(value, out char[] v, (i) => (!(value[i] < char.MinValue || value[i] > char.MaxValue), (char)value[i]));
            var b = SetVal(key, v);
            return b && c;
        }
        #endregion
    }
}
