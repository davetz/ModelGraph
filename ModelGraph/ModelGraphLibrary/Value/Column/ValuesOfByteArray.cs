namespace ModelGraphLibrary
{
    public class ValuesOfByteArray : ValuesOf<byte[]>
    {
        internal override ValueType ValueType { get { return ValueType.ByteArray; } }
        internal override NativeType PreferredType { get { return NativeType.String; } }
        protected override bool TryParse(string input, out byte[] value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(byte[] value) { return Value.ToBool(value); }
        protected override byte ToByte(byte[] value) { return Value.ToByte(value); }
        protected override short ToInt16(byte[] value) { return Value.ToInt16(value); }
        protected override int ToInt32(byte[] value) { return Value.ToInt32(value); }
        protected override long ToInt64(byte[] value) { return Value.ToInt64(value); }
        protected override double ToDouble(byte[] value) { return Value.ToDouble(value); }
        protected override string ToString(byte[] value) { return Value.ToString(value); }
    }
}
