﻿using System.Text;

namespace ModelGraphLibrary
{
    internal class INT64 : Step
    {
        private long _value;
        internal INT64(double val) { _value = (long)val; }

        long GetVal() => IsNegated ? -_value : _value;


        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Int64;

        internal override void GetValue(out bool value) => value = Value.ToBool(GetVal());
        internal override void GetValue(out byte value) => value = Value.ToByte(GetVal());
        internal override void GetValue(out int value) => value = Value.ToInt32(GetVal());
        internal override void GetValue(out long value) => value = Value.ToInt64(GetVal());
        internal override void GetValue(out short value) => value = Value.ToInt16(GetVal());
        internal override void GetValue(out sbyte value) => value = Value.ToSByte(GetVal());
        internal override void GetValue(out uint value) => value = Value.ToUInt32(GetVal());
        internal override void GetValue(out ulong value) => value = Value.ToUInt64(GetVal());
        internal override void GetValue(out ushort value) => value = Value.ToUInt16(GetVal());
        internal override void GetValue(out double value) => value = Value.ToDouble(GetVal());
        internal override void GetValue(out string value) => value = Value.ToString(GetVal());

        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            sb.Append(_value.ToString());
            GetSufix(sb);
        }
        #endregion
    }
}