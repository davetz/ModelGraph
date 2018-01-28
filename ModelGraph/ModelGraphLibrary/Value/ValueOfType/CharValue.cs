
using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class CharValue : ValueOfType<char>
    {
        internal CharValue(IValueStore<char> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Char;

        internal ValueDictionary<char> ValueDictionary => _valueStore as ValueDictionary<char>;

        #region LoadCache  ====================================================
        internal override bool LoadCache(ComputeX cx, Item key, List<Query> qList)
        {
            if (cx == null || qList == null || qList.Count == 0) return false;

            var q = qList[0];
            if (q.Items == null || q.Items.Length == 0) return false;

            var qx = q.QueryX;
            if (!qx.HasSelect) return false;

            var k = q.Items[0];
            if (k == null) return false;

            return (qx.Select.GetValue(k, out string v)) ? SetValue(key, v) : false;
        }
        #endregion

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
