using System.Text;

namespace ModelGraphLibrary
{
    internal class STRING : Step
    {
        private string _value;
        internal STRING(Parser p, string val) :base(p) { _value = val; }

        #region Methods  ======================================================
        internal override ValueType ValueType => ValueType.String;
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
        internal override void GetValue(out string value) { value = _value; }
        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            sb.Append(_value);
            GetSufix(sb);
        }
        #endregion
    }
}