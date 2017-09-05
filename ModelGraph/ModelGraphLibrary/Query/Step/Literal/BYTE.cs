namespace ModelGraphLibrary
{
    public class BYTE : Step
    {
        private byte _value;
        internal BYTE(double val)
        {
            _value = (byte)val;
            InType = NativeType.None;
            OutType = NativeType.Byte;
        }

        #region Methods  ======================================================
        internal override void GetValue(out bool value) { value = Value.ToBool(_value); }
        internal override void GetValue(out byte value) { value = _value; }
        internal override void GetValue(out int value) { value = Value.ToInt32(_value); }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = Value.ToInt16(_value); }
        internal override void GetValue(out double value) { value = Value.ToDouble(_value); }
        internal override void GetValue(out string value) { value = Value.ToString(_value); }
        #endregion

    }
}
