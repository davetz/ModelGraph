using System;
using System.Text;

namespace ModelGraphLibrary
{
    internal class DATETIME : Step
    {
        private DateTime _value;
        internal DATETIME(DateTime val) { _value = val; }

        #region Methods  ======================================================
        internal override ValueType ValueType => ValueType.Double;
        internal override void GetValue(out bool value) => value = Value.ToBool(_value);
        internal override void GetValue(out byte value) => value = Value.ToByte(_value);
        internal override void GetValue(out int value) => value = Value.ToInt32(_value);
        internal override void GetValue(out long value) => value = Value.ToInt64(_value);
        internal override void GetValue(out short value) => value = Value.ToInt16(_value);
        internal override void GetValue(out sbyte value) => value = Value.ToSByte(_value);
        internal override void GetValue(out uint value) => value = Value.ToUInt32(_value);
        internal override void GetValue(out ulong value) => value = Value.ToUInt64(_value);
        internal override void GetValue(out ushort value) => value = Value.ToUInt16(_value);
        internal override void GetValue(out double value) => value = Value.ToDouble(_value);
        internal override void GetValue(out string value) => value = Value.ToString(_value);
        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            sb.Append(Value.ToString(_value));
            GetSufix(sb);
        }
        #endregion
    }
}