namespace ModelGraphLibrary
{
    public class ValuesOfDecimalArray : ValuesOf<decimal[]>
    {
        internal override ValueType ValueType { get { return ValueType.DecimalArray; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out decimal[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(decimal[] value) => Value.ToBool(value);
        protected override byte ToByte(decimal[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(decimal[] value) => Value.ToSByte(value);
        protected override short ToInt16(decimal[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(decimal[] value) => Value.ToUInt16(value);
        protected override int ToInt32(decimal[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(decimal[] value) => Value.ToUInt32(value);
        protected override long ToInt64(decimal[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(decimal[] value) => Value.ToUInt64(value);
        protected override double ToDouble(decimal[] value) => Value.ToDouble(value);
        protected override string ToString(decimal[] value) => Value.ToString(value);
    }
}
