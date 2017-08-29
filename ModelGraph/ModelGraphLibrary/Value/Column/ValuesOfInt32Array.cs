namespace ModelGraphLibrary
{
    public class ValuesOfInt32Array : ValuesOf<int[]>
    {
        internal override ValueType ValueType { get { return ValueType.Int32Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out int[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(int[] value) { return Value.ToBool(value); }
        protected override byte ToByte(int[] value) { return Value.ToByte(value); }
        protected override short ToInt16(int[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(int[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(int[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(int[] value) { return Value.ToDouble(value); }
        protected override string ToString(int[] value) { return Value.ToString(value); }
    }
}
