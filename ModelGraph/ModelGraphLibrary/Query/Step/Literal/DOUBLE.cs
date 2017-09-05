namespace ModelGraphLibrary
{
    public class DOUBLE : Step
    {
        private double _value;
        internal DOUBLE(double val)
        {
            _value = val;
            InType = NativeType.None;
            OutType = NativeType.Double;
        }

        #region Methods  ======================================================
        internal override void GetValue(out bool value) { value = Value.ToBool(_value); }
        internal override void GetValue(out byte value) { value = Value.ToByte(_value); }
        internal override void GetValue(out int value) { value = Value.ToInt32(_value); }
        internal override void GetValue(out long value) { value = Value.ToInt64(_value); }
        internal override void GetValue(out short value) { value = Value.ToInt16(_value); }
        internal override void GetValue(out double value) { value = _value; }
        internal override void GetValue(out string value) { value = Value.ToString(_value); }
        #endregion

    }
}
