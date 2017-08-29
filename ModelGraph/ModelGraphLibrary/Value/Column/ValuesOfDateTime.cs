using System;

namespace ModelGraphLibrary
{
    public class ValuesOfDateTime : ValuesOf<DateTime>
    {
        internal override ValueType ValueType { get { return ValueType.DateTime; } }
        internal override NativeType PreferredType { get { return NativeType.Int64; } }
        protected override bool TryParse(string input, out DateTime value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(DateTime value) { return Value.ToBool(value); }
        protected override byte ToByte(DateTime value) { return Value.ToByte(value); }
        protected override short ToInt16(DateTime value) { return Value.ToInt16(value); }
        protected override int ToInt32(DateTime value) { return Value.ToInt32(value); }
        protected override long ToInt64(DateTime value) { return Value.ToInt64(value); }
        protected override double ToDouble(DateTime value) { return Value.ToDouble(value); }
        protected override string ToString(DateTime value) { return Value.ToString(value); }
    }
}
