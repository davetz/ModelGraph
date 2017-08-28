namespace ModelGraphLibrary
{
    public class ValuesOfString : ValuesOf<string>
    {
        internal override ValueType ValueType { get { return ValueType.String; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out string value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(string value) { return Value.ToBool(value); }
        protected override byte ToByte(string value) { return Value.ToByte(value); }
        protected override short ToInt16(string value) { return Value.ToInt16(value); }
        protected override int ToInt32(string value) { return Value.ToInt32(value); }
        protected override long ToInt64(string value) { return Value.ToInt64(value); }
        protected override double ToDouble(string value) { return Value.ToDouble(value); }
        protected override string ToString(string value) { return (string.IsNullOrWhiteSpace(value)) ? string.Empty : value; }
    }
}
