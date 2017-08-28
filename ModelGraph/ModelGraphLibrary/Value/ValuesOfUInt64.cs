namespace ModelGraphLibrary
{
    public class ValuesOfUInt64 : ValuesOf<ulong>
    {
        internal override ValueType ValueType { get { return ValueType.UInt64; } }
        internal override NativeType PreferredType { get { return NativeType.Int64; } }
        protected override bool TryParse(string input, out ulong value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(ulong value) { return Value.ToBool(value); }
        protected override byte ToByte(ulong value) { return Value.ToByte(value); }
        protected override short ToInt16(ulong value) { return Value.ToInt16(value); }
        protected override int ToInt32(ulong value) { return Value.ToInt32(value); }
        protected override long ToInt64(ulong value) { return Value.ToInt64(value); }
        protected override double ToDouble(ulong value) { return Value.ToDouble(value); }
        protected override string ToString(ulong value) { return Value.ToString(value); }
    }
}
