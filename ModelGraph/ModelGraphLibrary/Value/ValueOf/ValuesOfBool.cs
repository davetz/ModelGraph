namespace ModelGraphLibrary
{
    public class ValuesOfBool : ValuesOf<bool>
    {
        internal override ValueType ValueType { get { return ValueType.Bool; } }
        protected override bool TryParse(string input, out bool value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(bool value) => Value.ToBool(value);
        protected override byte ToByte(bool value) => Value.ToByte(value);
        protected override sbyte ToSByte(bool value) => Value.ToSByte(value);
        protected override short ToInt16(bool value) => Value.ToInt16(value);
        protected override ushort ToUInt16(bool value) => Value.ToUInt16(value);
        protected override int ToInt32(bool value) => Value.ToInt32(value);
        protected override uint ToUInt32(bool value) => Value.ToUInt32(value);
        protected override long ToInt64(bool value) => Value.ToInt64(value);
        protected override ulong ToUInt64(bool value) => Value.ToUInt64(value);
        protected override double ToDouble(bool value) => Value.ToDouble(value);
        protected override string ToString(bool value) => Value.ToString(value);
    }
}
