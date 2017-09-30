using System;

namespace ModelGraphLibrary
{
    public class ValueOfDateTime : ValueOfType<DateTime>
    {
        internal override ValueType ValueType { get { return ValueType.DateTime; } }
        protected override bool TryParse(string input, out DateTime value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(DateTime value) => Value.ToBool(value);
        protected override byte ToByte(DateTime value) => Value.ToByte(value);
        protected override sbyte ToSByte(DateTime value) => Value.ToSByte(value);
        protected override short ToInt16(DateTime value) => Value.ToInt16(value);
        protected override ushort ToUInt16(DateTime value) => Value.ToUInt16(value);
        protected override int ToInt32(DateTime value) => Value.ToInt32(value);
        protected override uint ToUInt32(DateTime value) => Value.ToUInt32(value);
        protected override long ToInt64(DateTime value) => Value.ToInt64(value);
        protected override ulong ToUInt64(DateTime value) => Value.ToUInt64(value);
        protected override double ToDouble(DateTime value) => Value.ToDouble(value);
        protected override string ToString(DateTime value) => Value.ToString(value);
    }
}
