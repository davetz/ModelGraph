namespace ModelGraphLibrary
{
    public class ValuesOfSByteArray : ValuesOf<sbyte[]>
    {
        internal override ValueType ValueType { get { return ValueType.SByteArray; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out sbyte[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(sbyte[] value) => Value.ToBool(value);
        protected override byte ToByte(sbyte[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(sbyte[] value) => Value.ToSByte(value);
        protected override short ToInt16(sbyte[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(sbyte[] value) => Value.ToUInt16(value);
        protected override int ToInt32(sbyte[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(sbyte[] value) => Value.ToUInt32(value);
        protected override long ToInt64(sbyte[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(sbyte[] value) => Value.ToUInt64(value);
        protected override double ToDouble(sbyte[] value) => Value.ToDouble(value);
        protected override string ToString(sbyte[] value) => Value.ToString(value);
    }
}
