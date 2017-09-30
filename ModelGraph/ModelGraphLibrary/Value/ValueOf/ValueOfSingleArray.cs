namespace ModelGraphLibrary
{
    public class ValueOfSingleArray : ValueOfType<float[]>
    {
        internal override ValueType ValueType { get { return ValueType.SingleArray; } }
        protected override bool TryParse(string input, out float[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(float[] value) => Value.ToBool(value);
        protected override byte ToByte(float[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(float[] value) => Value.ToSByte(value);
        protected override short ToInt16(float[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(float[] value) => Value.ToUInt16(value);
        protected override int ToInt32(float[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(float[] value) => Value.ToUInt32(value);
        protected override long ToInt64(float[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(float[] value) => Value.ToUInt64(value);
        protected override double ToDouble(float[] value) => Value.ToDouble(value);
        protected override string ToString(float[] value) => Value.ToString(value);
    }
}
