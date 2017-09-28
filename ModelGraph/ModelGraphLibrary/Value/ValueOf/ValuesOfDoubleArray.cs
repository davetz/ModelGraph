namespace ModelGraphLibrary
{
    public class ValuesOfDoubleArray : ValuesOf<double[]>
    {
        internal override ValueType ValueType { get { return ValueType.DoubleArray; } }
        protected override bool TryParse(string input, out double[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(double[] value) => Value.ToBool(value);
        protected override byte ToByte(double[] value) => Value.ToByte(value);
        protected override sbyte ToSByte(double[] value) => Value.ToSByte(value);
        protected override short ToInt16(double[] value) => Value.ToInt16(value);
        protected override ushort ToUInt16(double[] value) => Value.ToUInt16(value);
        protected override int ToInt32(double[] value) => Value.ToInt32(value);
        protected override uint ToUInt32(double[] value) => Value.ToUInt32(value);
        protected override long ToInt64(double[] value) => Value.ToInt64(value);
        protected override ulong ToUInt64(double[] value) => Value.ToUInt64(value);
        protected override double ToDouble(double[] value) => Value.ToDouble(value);
        protected override string ToString(double[] value) => Value.ToString(value);
    }
}
