namespace ModelGraphLibrary
{/*
 */
    internal struct Int64Value : IStepValue
    {
        private long _value;
        internal Int64Value(long value) { _value = value; }
        public NativeType NativeType { get { return NativeType.Int64; } }

        public void GetValue(out bool val) { val = Value.ToBool(_value); }
        public void GetValue(out byte val) { val = Value.ToByte(_value); }
        public void GetValue(out int val) { val = Value.ToInt16(_value); }
        public void GetValue(out short val) { val = Value.ToInt16(_value); }
        public void GetValue(out long val) { val = _value; }
        public void GetValue(out double val) { val = Value.ToDouble(_value); }
        public void GetValue(out string val) { val = Value.ToString(_value); }

        public void SetValue(bool val) { _value = Value.ToInt64(val); }
        public void SetValue(byte val) { _value = Value.ToInt64(val); }
        public void SetValue(int val) { _value = Value.ToInt64(val); }
        public void SetValue(short val) { _value = Value.ToInt64(val); }
        public void SetValue(long val) { _value = val; }
        public void SetValue(double val) { _value = Value.ToInt64(val); }
        public void SetValue(string val) { _value = Value.ToInt64(val); }
    }
}
