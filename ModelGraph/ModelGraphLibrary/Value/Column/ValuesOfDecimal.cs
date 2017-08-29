namespace ModelGraphLibrary
{
    public class ValuesOfDecimal : ValuesOf<decimal>
    {
        internal override ValueType ValueType { get { return ValueType.Decimal; } }
        internal override NativeType PreferredType { get { return NativeType.Double; } }
        protected override bool TryParse(string input, out decimal value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(decimal value) { return Value.ToBool(value); }
        protected override byte ToByte(decimal value) { return Value.ToByte(value); }
        protected override short ToInt16(decimal value) { return Value.ToInt16(value); }
        protected override int ToInt32(decimal value) { return Value.ToInt32(value); }
        protected override long ToInt64(decimal value) { return Value.ToInt64(value); }
        protected override double ToDouble(decimal value) { return Value.ToDouble(value); }
        protected override string ToString(decimal value) { return Value.ToString(value); }
    }
}
