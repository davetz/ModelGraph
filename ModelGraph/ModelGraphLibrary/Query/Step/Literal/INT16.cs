using System.Text;

namespace ModelGraphLibrary
{
    internal class INT16 : Step
    {
        private short _value;
        internal INT16(double val) { _value = (short)val; }

        #region Methods  ======================================================
        internal override NativeType NativeType => NativeType.Int16;
        internal override IStepValue GetValue() => new Int16Value(_value);
        internal override void GetValue(out bool value) { value = Value.ToBool(_value); }
        internal override void GetValue(out byte value) { value = Value.ToByte(_value); }
        internal override void GetValue(out int value) { value = Value.ToInt32(_value); }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = _value; }
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
