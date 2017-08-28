namespace ModelGraphLibrary
{
    public class ValuesOfInt16Array : ValuesOf<short[]>
    {
        internal override ValueType ValueType { get { return ValueType.Int16Array; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out short[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(short[] value) { return Value.ToBool(value); }
        protected override byte ToByte(short[] value) { return Value.ToByte(value); }
        protected override short ToInt16(short[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(short[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(short[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(short[] value) { return Value.ToDouble(value); }
        protected override string ToString(short[] value) { return Value.ToString(value); }
    }
}
