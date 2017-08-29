using System;

namespace ModelGraphLibrary
{
    public class ValuesOfGuid : ValuesOf<Guid>
    {
        internal override ValueType ValueType { get { return ValueType.Guid; } }
        internal override NativeType PreferredType { get { return NativeType.Int64; } }
        protected override bool TryParse(string input, out Guid value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(Guid value) { return Value.ToBool(value); }
        protected override byte ToByte(Guid value) { return Value.ToByte(value); }
        protected override short ToInt16(Guid value) { return Value.ToInt16(value); }
        protected override int ToInt32(Guid value) { return Value.ToInt32(value); }
        protected override long ToInt64(Guid value) { return Value.ToInt64(value); }
        protected override double ToDouble(Guid value) { return Value.ToDouble(value); }
        protected override string ToString(Guid value) { return Value.ToString(value); }
    }
}
