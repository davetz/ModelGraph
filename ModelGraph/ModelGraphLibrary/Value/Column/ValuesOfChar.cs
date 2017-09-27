namespace ModelGraphLibrary
{
    public class ValuesOfChar : ValuesOf<char>
    {
        internal override ValueType ValueType { get { return ValueType.Char; } }
        internal override NativeType PreferredType { get { return NativeType.Int32; } }
        protected override bool TryParse(string input, out char value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(char value) => Value.ToBool(value);
        protected override byte ToByte(char value) => Value.ToByte(value);
        protected override sbyte ToSByte(char value) => Value.ToSByte(value);
        protected override short ToInt16(char value) => Value.ToInt16(value);
        protected override ushort ToUInt16(char value) => Value.ToUInt16(value);
        protected override int ToInt32(char value) => Value.ToInt32(value);
        protected override uint ToUInt32(char value) => Value.ToUInt32(value);
        protected override long ToInt64(char value) => Value.ToInt64(value);
        protected override ulong ToUInt64(char value) => Value.ToUInt64(value);
        protected override double ToDouble(char value) => Value.ToDouble(value);
        protected override string ToString(char value) => Value.ToString(value);
    }
}
