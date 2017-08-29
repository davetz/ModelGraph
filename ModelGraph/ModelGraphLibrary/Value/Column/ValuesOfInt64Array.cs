namespace ModelGraphLibrary
{
    public class ValuesOfInt64Array : ValuesOf<long[]>
    {
        internal override ValueType ValueType { get { return ValueType.Int64Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out long[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(long[] value) { return Value.ToBool(value); }
        protected override byte ToByte(long[] value) { return Value.ToByte(value); }
        protected override short ToInt16(long[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(long[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(long[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(long[] value) { return Value.ToDouble(value); }
        protected override string ToString(long[] value) { return Value.ToString(value); }
    }
}
