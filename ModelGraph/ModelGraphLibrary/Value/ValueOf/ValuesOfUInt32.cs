namespace ModelGraphLibrary
{
    public class ValuesOfUInt32 : ValuesOf<uint>
    {
        internal override ValueType ValueType { get { return ValueType.UInt32; } }
        protected override bool TryParse(string input, out uint value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(uint value) => Value.ToBool(value);
        protected override byte ToByte(uint value) => Value.ToByte(value);
        protected override sbyte ToSByte(uint value) => Value.ToSByte(value);
        protected override short ToInt16(uint value) => Value.ToInt16(value);
        protected override ushort ToUInt16(uint value) => Value.ToUInt16(value);
        protected override int ToInt32(uint value) => Value.ToInt32(value);
        protected override uint ToUInt32(uint value) => Value.ToUInt32(value);
        protected override long ToInt64(uint value) => Value.ToInt64(value);
        protected override ulong ToUInt64(uint value) => Value.ToUInt64(value);
        protected override double ToDouble(uint value) => Value.ToDouble(value);
        protected override string ToString(uint value) => Value.ToString(value);
    }
}
