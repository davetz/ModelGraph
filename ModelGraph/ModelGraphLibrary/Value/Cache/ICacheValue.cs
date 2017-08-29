namespace ModelGraphLibrary
{
    public interface ICacheValue
    {
        void Clear();
        bool IsInvalid { get; }
        bool IsCircular { get; }
        ValueType ValueType { get; }
        NativeType NativeType { get; }

        void SetValue(Item key, bool val);
        void SetValue(Item key, byte val);
        void SetValue(Item key, int val);
        void SetValue(Item key, short val);
        void SetValue(Item key, long val);
        void SetValue(Item key, double val);
        void SetValue(Item key, string val);

        bool GetValue(Item key, out bool val);
        bool GetValue(Item key, out byte val);
        bool GetValue(Item key, out int val);
        bool GetValue(Item key, out short val);
        bool GetValue(Item key, out long val);
        bool GetValue(Item key, out double val);
        bool GetValue(Item key, out string val);
    }
}
