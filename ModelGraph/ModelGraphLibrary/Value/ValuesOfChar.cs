namespace ModelGraphLibrary
{
    public class ValuesOfChar : ValuesOf<char>
    {
        internal override ValueType ValueType { get { return ValueType.Char; } }
        internal override NativeType PreferredType { get { return NativeType.Int32; } }
        protected override bool TryParse(string input, out char value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(char value) { return Value.ToBool(value); }
        protected override byte ToByte(char value) { return Value.ToByte(value); }
        protected override short ToInt16(char value) { return Value.ToInt16(value); }
        protected override int ToInt32(char value) { return Value.ToInt32(value); }
        protected override long ToInt64(char value) { return Value.ToInt64(value); }
        protected override double ToDouble(char value) { return Value.ToDouble(value); }
        protected override string ToString(char value) { return Value.ToString(value); }
    }
}
