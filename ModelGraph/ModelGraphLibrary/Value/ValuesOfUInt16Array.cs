namespace ModelGraphLibrary
{
    public class ValuesOfUInt16Array : ValuesOf<ushort[]>
    {
        internal override ValueType ValueType { get { return ValueType.UInt16Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out ushort[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(ushort[] value) { return Value.ToBool(value); }
        protected override byte ToByte(ushort[] value) { return Value.ToByte(value); }
        protected override short ToInt16(ushort[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(ushort[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(ushort[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(ushort[] value) { return Value.ToDouble(value); }
        protected override string ToString(ushort[] value) { return Value.ToString(value); }
    }
}
