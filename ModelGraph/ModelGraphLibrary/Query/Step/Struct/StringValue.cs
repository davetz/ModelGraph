namespace ModelGraphLibrary
{/*
 */
    internal struct StringValue : IStepValue
    {
        private string _value;

        internal StringValue(string value) { _value = value; }

        public NativeType NativeType { get { return NativeType.String; } }

        public void GetValue(out bool val) { val = Value.ToBool(_value); }
        public void GetValue(out byte val) { val = Value.ToByte(_value); }
        public void GetValue(out int val) { val = Value.ToInt16(_value); }
        public void GetValue(out short val) { val = Value.ToInt16(_value); }
        public void GetValue(out long val) { val = Value.ToInt32(_value); }
        public void GetValue(out double val) { val = Value.ToDouble(_value); }
        public void GetValue(out string val) { val = _value; }

        public void SetValue(bool val) { _value = Value.ToString(val); }
        public void SetValue(byte val) { _value = Value.ToString(val); }
        public void SetValue(int val) { _value = Value.ToString(val); }
        public void SetValue(short val) { _value = Value.ToString(val); }
        public void SetValue(long val) { _value = Value.ToString(val); }
        public void SetValue(double val) { _value = Value.ToString(val); }
        public void SetValue(string val) { _value = val; }
    }
}
