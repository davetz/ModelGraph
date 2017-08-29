namespace ModelGraphLibrary
{
    public class ValuesOfDouble : ValuesOf<double>
    {
        internal override ValueType ValueType { get { return ValueType.Double; } }
        internal override NativeType PreferredType { get { return NativeType.Double; } }
        protected override bool TryParse(string input, out double value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(double value) { return Value.ToBool(value); }
        protected override byte ToByte(double value) { return Value.ToByte(value); }
        protected override short ToInt16(double value) { return Value.ToInt16(value); }
        protected override int ToInt32(double value) { return Value.ToInt32(value); }
        protected override long ToInt64(double value) { return Value.ToInt64(value); }
        protected override double ToDouble(double value) { return Value.ToDouble(value); }
        protected override string ToString(double value) { return Value.DecimalString(value); }
    }
}
