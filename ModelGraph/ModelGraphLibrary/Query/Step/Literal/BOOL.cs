using System.Text;

namespace ModelGraphLibrary
{
    internal class BOOL : Step
    {
        private bool _value;
        internal BOOL(bool val) { _value = val; }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Bool;
        internal override IStepValue GetValue() => new BoolValue(_value);
        internal override void GetValue(out bool value) { value = _value; }
        internal override void GetValue(out byte value) { value = Value.ToByte(_value); }
        internal override void GetValue(out int value) { value = Value.ToInt32(_value); }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = Value.ToInt16(_value); }
        internal override void GetValue(out double value) { value = Value.ToDouble(_value); }
        internal override void GetValue(out string value) { value = Value.ToString(_value); }
        internal override void GetText(StringBuilder sb)
        {
            GetPrefix(sb);
            sb.Append(_value.ToString());
            GetSufix(sb);
        }
        #endregion
    }
}
