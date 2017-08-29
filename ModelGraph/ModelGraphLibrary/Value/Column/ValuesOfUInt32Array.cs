namespace ModelGraphLibrary
{
    public class ValuesOfUInt32Array : ValuesOf<uint[]>
    {
        internal override ValueType ValueType { get { return ValueType.UInt32Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out uint[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(uint[] value) { return Value.ToBool(value); }
        protected override byte ToByte(uint[] value) { return Value.ToByte(value); }
        protected override short ToInt16(uint[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(uint[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(uint[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(uint[] value) { return Value.ToDouble(value); }
        protected override string ToString(uint[] value) { return Value.ToString(value); }
    }
}
