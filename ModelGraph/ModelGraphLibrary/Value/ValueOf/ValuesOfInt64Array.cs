namespace ModelGraphLibrary
{
    public class ValuesOfInt64Array : ValuesOf<long[]>
    {
        internal override ValueType ValueType { get { return ValueType.Int64Array; } }
        protected override bool TryParse(string input, out long[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(long[] value) => Value.ToBool(value);
        protected override byte ToByte(long[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(long[] value) => Value.ToSByte(value);
        protected override short ToInt16(long[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(long[] value) => Value.ToUInt16(value);
        protected override int ToInt32(long[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(long[] value) => Value.ToUInt32(value);
        protected override long ToInt64(long[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(long[] value) => Value.ToUInt64(value);
        protected override double ToDouble(long[] value) => Value.ToDouble(value);
        protected override string ToString(long[] value) => Value.ToString(value);
    }
}
