using System;

namespace ModelGraphLibrary
{
    public class ValuesOfTimeSpan : ValuesOf<TimeSpan>
    {
        internal override ValueType ValueType { get { return ValueType.TimeSpan; } }
        internal override NativeType PreferredType { get { return NativeType.Int64; } }
        protected override bool TryParse(string input, out TimeSpan value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(TimeSpan value) { return Value.ToBool(value); }
        protected override byte ToByte(TimeSpan value) { return Value.ToByte(value); }
        protected override short ToInt16(TimeSpan value) { return Value.ToInt16(value); }
        protected override int ToInt32(TimeSpan value) { return Value.ToInt32(value); }
        protected override long ToInt64(TimeSpan value) { return Value.ToInt64(value); }
        protected override double ToDouble(TimeSpan value) { return Value.ToDouble(value); }
        protected override string ToString(TimeSpan value) { return Value.ToString(value); }
    }
}
