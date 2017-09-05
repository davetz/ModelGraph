namespace ModelGraphLibrary
{
    public class INT32 : Step
    {
        private int _value;
        internal INT32(double val)
        {
            _value = (int)val;
            InType = NativeType.None;
            OutType = NativeType.Int32;
        }

        #region Methods  ======================================================
        internal override void GetValue(out bool value) { value = Value.ToBool(_value); }
        internal override void GetValue(out byte value) { value = Value.ToByte(_value); }
        internal override void GetValue(out int value) { value = _value; }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = Value.ToInt16(_value); }
        internal override void GetValue(out double value) { value = Value.ToDouble(_value); }
        internal override void GetValue(out string value) { value = Value.ToString(_value); }
        #endregion

    }
}
