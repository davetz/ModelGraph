using System.Collections.Generic;

namespace ModelGraphLibrary
{
    internal class Int64Cache : Dictionary<Item, long>, ICacheValue
    {
        public bool IsInvalid => false;
        public bool IsCircular => false;
        public ValueType ValueType { get { return ValueType.Int64; } }
        public NativeType NativeType { get { return NativeType.Int64; } }

        public bool GetValue(Item key, out bool val) { if (TryGetValue(key, out long value)) { val = Value.ToBool(value); return true; } val = false; return false; }
        public bool GetValue(Item key, out byte val) { if (TryGetValue(key, out long value)) { val = Value.ToByte(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out short val) { if (TryGetValue(key, out long value)) { val = Value.ToInt16(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out int val) { if (TryGetValue(key, out long value)) { val = Value.ToInt16(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out long val) { return TryGetValue(key, out val); }
        public bool GetValue(Item key, out double val) { if (TryGetValue(key, out long value)) { val = Value.ToDouble(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out string val) { if (TryGetValue(key, out long value)) { val = Value.ToString(value); return true; } val = null; return false; }

        public void SetValue(Item key, bool val) { this[key] = Value.ToInt64(val); }
        public void SetValue(Item key, byte val) { this[key] = Value.ToInt64(val); }
        public void SetValue(Item key, int val) { this[key] = Value.ToInt64(val); }
        public void SetValue(Item key, short val) { this[key] = Value.ToInt64(val); }
        public void SetValue(Item key, long val) { this[key] = val; }
        public void SetValue(Item key, double val) { this[key] = Value.ToInt64(val); }
        public void SetValue(Item key, string val) { this[key] = Value.ToInt64(val); }
    }
}
