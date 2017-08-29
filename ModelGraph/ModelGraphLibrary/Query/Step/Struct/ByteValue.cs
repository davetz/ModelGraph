namespace ModelGraphLibrary
{/*
 */
    internal struct ByteValue : IStepValue
    {
        private byte _value;
        internal ByteValue(byte value) { _value = value; }
        public NativeType NativeType { get { return NativeType.Byte; } }

        public void GetValue(out bool val) { val = Value.ToBool(_value); }
        public void GetValue(out byte val) { val = _value; }
        public void GetValue(out int val) { val = Value.ToInt32(_value); }
        public void GetValue(out short val) { val = Value.ToInt16(_value); }
        public void GetValue(out long val) { val = Value.ToInt64(_value); }
        public void GetValue(out double val) { val = Value.ToDouble(_value); }
        public void GetValue(out string val) { val = Value.ToString(_value); }

        public void SetValue(bool val) { _value = Value.ToByte(val); }
        public void SetValue(byte val) { _value = val; }
        public void SetValue(int val) { _value = Value.ToByte(val); }
        public void SetValue(short val) { _value = Value.ToByte(val); }
        public void SetValue(long val) { _value = Value.ToByte(val); }
        public void SetValue(double val) { _value = Value.ToByte(val); }
        public void SetValue(string val) { _value = Value.ToByte(val); }
    }
}
