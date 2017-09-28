namespace ModelGraphLibrary
{
    public class ValuesOfString : ValuesOf<string>
    {
        internal override ValueType ValueType { get { return ValueType.String; } }
        protected override bool TryParse(string input, out string value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(string value) => Value.ToBool(value);
        protected override byte ToByte(string value) => Value.ToByte(value);
        protected override sbyte ToSByte(string value) => Value.ToSByte(value);
        protected override short ToInt16(string value) => Value.ToInt16(value);
        protected override ushort ToUInt16(string value) => Value.ToUInt16(value);
        protected override int ToInt32(string value) => Value.ToInt32(value);
        protected override uint ToUInt32(string value) => Value.ToUInt32(value);
        protected override long ToInt64(string value) => Value.ToInt64(value);
        protected override ulong ToUInt64(string value) => Value.ToUInt64(value);
        protected override double ToDouble(string value) => Value.ToDouble(value);
        protected override string ToString(string value) { return (string.IsNullOrWhiteSpace(value)) ? string.Empty : value; }
    }
}
