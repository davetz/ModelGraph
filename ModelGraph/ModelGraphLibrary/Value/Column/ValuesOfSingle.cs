namespace ModelGraphLibrary
{
    public class ValuesOfSingle : ValuesOf<float>
    {
        internal override ValueType ValueType { get { return ValueType.Single; } }
        internal override NativeType PreferredType { get { return NativeType.Double; } }
        protected override bool TryParse(string input, out float value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(float value) { return Value.ToBool(value); }
        protected override byte ToByte(float value) { return Value.ToByte(value); }
        protected override short ToInt16(float value) { return Value.ToInt16(value); }
        protected override int ToInt32(float value) { return Value.ToInt32(value); }
        protected override long ToInt64(float value) { return Value.ToInt64(value); }
        protected override double ToDouble(float value) { return Value.ToDouble(value); }
        protected override string ToString(float value) { return Value.ToString(value); }
    }
}
