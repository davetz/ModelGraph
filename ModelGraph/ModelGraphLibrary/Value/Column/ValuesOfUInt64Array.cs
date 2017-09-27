namespace ModelGraphLibrary
{
    public class ValuesOfUInt64Array : ValuesOf<ulong[]>
    {
        internal override ValueType ValueType { get { return ValueType.UInt64Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out ulong[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(ulong[] value) => Value.ToBool(value);
        protected override byte ToByte(ulong[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(ulong[] value) => Value.ToSByte(value);
        protected override short ToInt16(ulong[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(ulong[] value) => Value.ToUInt16(value);
        protected override int ToInt32(ulong[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(ulong[] value) => Value.ToUInt32(value);
        protected override long ToInt64(ulong[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(ulong[] value) => Value.ToUInt64(value);
        protected override double ToDouble(ulong[] value) => Value.ToDouble(value);
        protected override string ToString(ulong[] value) => Value.ToString(value);
    }
}
