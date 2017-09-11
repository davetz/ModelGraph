﻿using System.Text;

namespace ModelGraphLibrary
{
    internal class BYTE : Step
    {
        private byte _value;
        internal BYTE(double val) { _value = (byte)val; }

        byte GetVal() => IsNegated ? (byte)~_value : _value;

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Byte;

        internal override void GetValue(out bool value) { value = Value.ToBool(GetVal()); }
        internal override void GetValue(out byte value) { value = Value.ToByte(GetVal()); }
        internal override void GetValue(out int value) { value = Value.ToInt32(GetVal()); }
        internal override void GetValue(out long value) { value = Value.ToInt64(GetVal()); }
        internal override void GetValue(out short value) { value = Value.ToInt16(GetVal()); }
        internal override void GetValue(out double value) { value = Value.ToDouble(GetVal()); }
        internal override void GetValue(out string value) { value = Value.ToString(GetVal()); }

        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            sb.Append(_value.ToString());
            GetSufix(sb);
        }
        #endregion
    }
}
