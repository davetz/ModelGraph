namespace ModelGraphLibrary
{
    public class ValuesOfByte : ValuesOf<byte>
    {
        internal override ValueType ValueType { get { return ValueType.Byte; } }
        protected override bool TryParse(string input, out byte value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(byte value) => Value.ToBool(value);
        protected override byte ToByte(byte value) => Value.ToByte(value);
        protected override sbyte ToSByte(byte value) => Value.ToSByte(value);
        protected override short ToInt16(byte value) => Value.ToInt16(value);
        protected override ushort ToUInt16(byte value) => Value.ToUInt16(value);
        protected override int ToInt32(byte value) => Value.ToInt32(value);
        protected override uint ToUInt32(byte value) => Value.ToUInt32(value);
        protected override long ToInt64(byte value) => Value.ToInt64(value);
        protected override ulong ToUInt64(byte value) => Value.ToUInt64(value);
        protected override double ToDouble(byte value) => Value.ToDouble(value);
        protected override string ToString(byte value) => Value.ToString(value);
    }
}
