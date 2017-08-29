using System.Collections.Generic;

namespace ModelGraphLibrary
{
    internal class ByteCache : Dictionary<Item, byte>, ICacheValue
    {
        public bool IsInvalid => false;
        public bool IsCircular => false;
        public ValueType ValueType { get { return ValueType.Byte; } }
        public NativeType NativeType { get { return NativeType.Byte; } }

        public bool GetValue(Item key, out bool val) { if (TryGetValue(key, out byte value)){ val = Value.ToBool(value); return true; } val = false; return false; }
        public bool GetValue(Item key, out byte val) { return TryGetValue(key, out val); }
        public bool GetValue(Item key, out short val) { if (TryGetValue(key, out byte value)) { val = Value.ToInt16(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out int val) { if (TryGetValue(key, out byte value)) { val = Value.ToInt32(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out long val) { if (TryGetValue(key, out byte value)) { val = Value.ToInt64(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out double val) { if (TryGetValue(key, out byte value)) { val = Value.ToDouble(value); return true; } val = 0; return false; }
        public bool GetValue(Item key, out string val) { if (TryGetValue(key, out byte value)) { val = Value.ToString(value); return true; } val = null; return false; }

        public void SetValue(Item key, bool val) { this[key] = Value.ToByte(val); }
        public void SetValue(Item key, byte val) { this[key] = val; }
        public void SetValue(Item key, int val) { this[key] = Value.ToByte(val); }
        public void SetValue(Item key, short val) { this[key] = Value.ToByte(val); }
        public void SetValue(Item key, long val) { this[key] = Value.ToByte(val); }
        public void SetValue(Item key, double val) { this[key] = Value.ToByte(val); }
        public void SetValue(Item key, string val) { this[key] = Value.ToByte(val); }
    }
}
