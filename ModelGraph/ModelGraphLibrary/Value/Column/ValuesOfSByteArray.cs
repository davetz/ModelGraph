namespace ModelGraphLibrary
{
    public class ValuesOfSByteArray : ValuesOf<sbyte[]>
    {
        internal override ValueType ValueType { get { return ValueType.SByteArray; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out sbyte[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(sbyte[] value) { return Value.ToBool(value); }
        protected override byte ToByte(sbyte[] value) { return Value.ToByte(value); }
        protected override short ToInt16(sbyte[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(sbyte[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(sbyte[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(sbyte[] value) { return Value.ToDouble(value); }
        protected override string ToString(sbyte[] value) { return Value.ToString(value); }
    }
}
