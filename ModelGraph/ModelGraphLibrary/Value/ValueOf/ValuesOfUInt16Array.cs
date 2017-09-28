namespace ModelGraphLibrary
{
    public class ValuesOfUInt16Array : ValuesOf<ushort[]>
    {
        internal override ValueType ValueType { get { return ValueType.UInt16Array; } }
        protected override bool TryParse(string input, out ushort[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(ushort[] value) => Value.ToBool(value);
        protected override byte ToByte(ushort[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(ushort[] value) => Value.ToSByte(value);
        protected override short ToInt16(ushort[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(ushort[] value) => Value.ToUInt16(value);
        protected override int ToInt32(ushort[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(ushort[] value) => Value.ToUInt32(value);
        protected override long ToInt64(ushort[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(ushort[] value) => Value.ToUInt64(value);
        protected override double ToDouble(ushort[] value) => Value.ToDouble(value);
        protected override string ToString(ushort[] value) => Value.ToString(value);
    }
}
