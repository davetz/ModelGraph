namespace ModelGraphLibrary
{
    public class ValuesOfInt16 : ValuesOf<short>
    {
        internal override ValueType ValueType { get { return ValueType.Int16; } }

        protected override bool TryParse(string input, out short value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(short value) => Value.ToBool(value);
        protected override byte ToByte(short value) => Value.ToByte(value);
        protected override sbyte ToSByte(short value) => Value.ToSByte(value);
        protected override short ToInt16(short value) => Value.ToInt16(value);
        protected override ushort ToUInt16(short value) => Value.ToUInt16(value);
        protected override int ToInt32(short value) => Value.ToInt32(value);
        protected override uint ToUInt32(short value) => Value.ToUInt32(value);
        protected override long ToInt64(short value) => Value.ToInt64(value);
        protected override ulong ToUInt64(short value) => Value.ToUInt64(value);
        protected override double ToDouble(short value) => Value.ToDouble(value);
        protected override string ToString(short value) => Value.ToString(value);
    }
}
