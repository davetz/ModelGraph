namespace ModelGraphLibrary
{/*
 */
    internal struct DoubleValue : IStepValue
    {
        private double _value;
        internal DoubleValue(double value) { _value = value; }

        public NativeType NativeType { get { return NativeType.Double; } }

        public void GetValue(out bool val) { val = Value.ToBool(_value); }
        public void GetValue(out byte val) { val = Value.ToByte(_value); }
        public void GetValue(out int val) { val = Value.ToInt16(_value); }
        public void GetValue(out short val) { val = Value.ToInt16(_value); }
        public void GetValue(out long val) { val = Value.ToInt32(_value); }
        public void GetValue(out double val) { val = _value; }
        public void GetValue(out string val) { val = Value.ToString(_value); }

        public void SetValue(bool val) { _value = Value.ToDouble(val); }
        public void SetValue(byte val) { _value = Value.ToDouble(val); }
        public void SetValue(int val) { _value = Value.ToDouble(val); }
        public void SetValue(short val) { _value = Value.ToDouble(val); }
        public void SetValue(long val) { _value = Value.ToDouble(val); }
        public void SetValue(double val) { _value = val; }
        public void SetValue(string val) { _value = Value.ToDouble(val); }
    }
}
