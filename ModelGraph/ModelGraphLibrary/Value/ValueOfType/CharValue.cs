
using System;

namespace ModelGraphSTD
{
    internal class CharValue : ValueOfType<char>
    {
        internal CharValue(IValueStore<char> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Char;

        internal ValueDictionary<char> ValueDictionary => _valueStore as ValueDictionary<char>;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = (GetVal(key, out char v));
            value = v != 0;
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = (GetVal(key, out char v));
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = (GetVal(key, out char v));
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = (GetVal(key, out char v));
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = (GetVal(key, out char v));
            value = v.ToString();
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (char)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => SetVal(key, (char)value);

        internal override bool SetValue(Item key, long value) => SetVal(key, (char)value);

        internal override bool SetValue(Item key, double value) => SetVal(key, (char)value);

        internal override bool SetValue(Item key, string value)
        {
            if (value != null)
            {
                foreach (var c in value)
                {
                    if (!char.IsWhiteSpace(c))
                        return SetVal(key, c);
                }
            }
            return SetVal(key, ' ');
        }
        #endregion
    }
}
