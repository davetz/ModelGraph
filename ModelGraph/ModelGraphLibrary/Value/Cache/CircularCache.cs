namespace ModelGraphLibrary
{
    internal class CircularCache : ICacheValue
    {
        public bool IsInvalid => true;
        public bool IsCircular => true;
        public ValueType ValueType { get { return ValueType.String; } }
        public NativeType NativeType { get { return NativeType.String; } }

        public bool GetValue(Item key, out bool val) { val = false; return true; }
        public bool GetValue(Item key, out byte val) { val = 0; return true; }
        public bool GetValue(Item key, out int val) { val = 0; return true; }
        public bool GetValue(Item key, out short val) { val = 0; return true; }
        public bool GetValue(Item key, out long val) { val = 0; return true; }
        public bool GetValue(Item key, out double val) { val = 0; return true; }
        public bool GetValue(Item key, out string val) { val = Chef.InvalidItem; return true; }

        public void Clear() { }
        public void SetValue(Item key, bool val) { }
        public void SetValue(Item key, byte val) { }
        public void SetValue(Item key, int val) { }
        public void SetValue(Item key, short val) { }
        public void SetValue(Item key, long val) { }
        public void SetValue(Item key, double val) { }
        public void SetValue(Item key, string val) { }
    }
}
