namespace ModelGraphLibrary
{
    public class ValueOfInt32 : ValueOfType<int>
    {
        internal override ValueType ValueType { get { return ValueType.Int32; } }
        protected override bool TryParse(string input, out int value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(int value) => Value.ToBool(value);
        protected override byte ToByte(int value) => Value.ToByte(value);
        protected override sbyte ToSByte(int value) => Value.ToSByte(value);
        protected override short ToInt16(int value) => Value.ToInt16(value);
        protected override ushort ToUInt16(int value) => Value.ToUInt16(value);
        protected override int ToInt32(int value) => Value.ToInt32(value);
        protected override uint ToUInt32(int value) => Value.ToUInt32(value);
        protected override long ToInt64(int value) => Value.ToInt64(value);
        protected override ulong ToUInt64(int value) => Value.ToUInt64(value);
        protected override double ToDouble(int value) => Value.ToDouble(value);
        protected override string ToString(int value) => Value.ToString(value);
    }
}
