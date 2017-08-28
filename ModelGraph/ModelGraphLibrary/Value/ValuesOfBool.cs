namespace ModelGraphLibrary
{
    public class ValuesOfBool : ValuesOf<bool>
    {
        internal override ValueType ValueType { get { return ValueType.Bool; } }
        internal override NativeType PreferredType { get { return NativeType.Bool; } }
        protected override bool TryParse(string input, out bool value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(bool value) { return value; }
        protected override byte ToByte(bool value) { return Value.ToByte(value); }
        protected override short ToInt16(bool value) { return Value.ToInt16(value); }
        protected override int ToInt32(bool value) { return Value.ToInt32(value); }
        protected override long ToInt64(bool value) { return Value.ToInt64(value); }
        protected override double ToDouble(bool value) { return Value.ToDouble(value); }
        protected override string ToString(bool value) { return Value.ToString(value); }
    }
}
