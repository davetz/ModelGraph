using System;

namespace ModelGraphLibrary
{
    public class ValueOfTimeSpan : ValueOfType<TimeSpan>
    {
        internal override ValueType ValueType { get { return ValueType.TimeSpan; } }
        protected override bool TryParse(string input, out TimeSpan value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(TimeSpan value) => Value.ToBool(value);
        protected override byte ToByte(TimeSpan value) => Value.ToByte(value);
        protected override sbyte ToSByte(TimeSpan value) => Value.ToSByte(value);
        protected override short ToInt16(TimeSpan value) => Value.ToInt16(value);
        protected override ushort ToUInt16(TimeSpan value) => Value.ToUInt16(value);
        protected override int ToInt32(TimeSpan value) => Value.ToInt32(value);
        protected override uint ToUInt32(TimeSpan value) => Value.ToUInt32(value);
        protected override long ToInt64(TimeSpan value) => Value.ToInt64(value);
        protected override ulong ToUInt64(TimeSpan value) => Value.ToUInt64(value);
        protected override double ToDouble(TimeSpan value) => Value.ToDouble(value);
        protected override string ToString(TimeSpan value) => Value.ToString(value);
    }
}
