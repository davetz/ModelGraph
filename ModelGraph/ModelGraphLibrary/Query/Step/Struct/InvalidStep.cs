namespace ModelGraphLibrary
{/*
 */
    internal struct InvalidStep : IStepValue
    {
        public NativeType NativeType { get { return NativeType.Invalid; } }

        public void GetValue(out bool val) { val = false; }
        public void GetValue(out byte val) { val = 0; }
        public void GetValue(out int val) { val = 0; }
        public void GetValue(out short val) { val = 0; }
        public void GetValue(out long val) { val = 0; }
        public void GetValue(out double val) { val = 0; }
        public void GetValue(out string val) { val = Chef.InvalidItem; }

        public void SetValue(bool val) { }
        public void SetValue(byte val) { }
        public void SetValue(int val) { }
        public void SetValue(short val) { }
        public void SetValue(long val) { }
        public void SetValue(double val) { }
        public void SetValue(string val) { }
    }
}
