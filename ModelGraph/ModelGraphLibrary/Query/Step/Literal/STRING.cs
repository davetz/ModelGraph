using System.Text;

namespace ModelGraphLibrary
{
    internal class STRING : Step
    {
        private string _value;
        internal STRING(string val) { _value = val; }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.String;
        internal override IStepValue GetValue() => new StringValue(_value);
        internal override void GetValue(out bool value) { value = Value.ToBool(_value); }
        internal override void GetValue(out byte value) { value = Value.ToByte(_value); }
        internal override void GetValue(out int value) { value = Value.ToInt32(_value); }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = Value.ToInt16(_value); }
        internal override void GetValue(out double value) { value = Value.ToDouble(_value); }
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