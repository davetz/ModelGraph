namespace ModelGraphLibrary
{/*
 */
    internal interface IStepValue
    {
        NativeType NativeType { get; }

        void SetValue(bool val);
        void SetValue(byte val);
        void SetValue(int val);
        void SetValue(short val);
        void SetValue(long val);
        void SetValue(double val);
        void SetValue(string val);

        void GetValue(out bool val);
        void GetValue(out byte val);
        void GetValue(out int val);
        void GetValue(out short val);
        void GetValue(out long val);
        void GetValue(out double val);
        void GetValue(out string val);
    }
}
