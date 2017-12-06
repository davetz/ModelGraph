
using System;

namespace ModelGraphLibrary
{
    internal class ByteValue : ValueOfType<byte>
    {
        internal ByteValue(IValueStore<byte> store) { _valueStore = store; }
        internal override ValType ValType => ValType.Byte;

        #region GetValue  =====================================================
        internal override bool GetValue(Item key, out bool value)
        {
            var b = GetVal(key, out byte v);
            value = (v != 0);
            return b;
        }

        internal override bool GetValue(Item key, out int value)
        {
            var b = GetVal(key, out byte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out long value)
        {
            var b = GetVal(key, out byte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out double value)
        {
            var b = GetVal(key, out byte v);
            value = v;
            return b;
        }

        internal override bool GetValue(Item key, out string value)
        {
            var b = GetVal(key, out byte v);
            value = ValueFormat(v, Format);
            return b;
        }
        #endregion

        #region SetValue ======================================================
        internal override bool SetValue(Item key, bool value) => SetVal(key, (byte)(value ? 1 : 0));

        internal override bool SetValue(Item key, int value) => (value < byte.MinValue || value > byte.MaxValue) ? false : SetVal(key, (byte)value);
        
        internal override bool SetValue(Item key, long value) => (value < byte.MinValue || value > byte.MaxValue) ? false : SetVal(key, (byte)value);

        internal override bool SetValue(Item key, double value) => (value < byte.MinValue || value > byte.MaxValue) ? false : SetVal(key, (byte)value);

        internal override bool SetValue(Item key, string value)
        {
            var v = ByteParse(value);
            return (v.ok) ? SetVal(key, v.val) : false;
        }
        #endregion
    }
}
