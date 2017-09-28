using System;

namespace ModelGraphLibrary
{
    public class ValuesOfGuid : ValuesOf<Guid>
    {
        internal override ValueType ValueType { get { return ValueType.Guid; } }
        protected override bool TryParse(string input, out Guid value) { return Value.TryParse(input, out value); }

        protected override bool ToBool(Guid value) => Value.ToBool(value);
        protected override byte ToByte(Guid value) => Value.ToByte(value);
        protected override sbyte ToSByte(Guid value) => Value.ToSByte(value);
        protected override short ToInt16(Guid value) => Value.ToInt16(value);
        protected override ushort ToUInt16(Guid value) => Value.ToUInt16(value);
        protected override int ToInt32(Guid value) => Value.ToInt32(value);
        protected override uint ToUInt32(Guid value) => Value.ToUInt32(value);
        protected override long ToInt64(Guid value) => Value.ToInt64(value);
        protected override ulong ToUInt64(Guid value) => Value.ToUInt64(value);
        protected override double ToDouble(Guid value) => Value.ToDouble(value);
        protected override string ToString(Guid value) => Value.ToString(value);
    }
}
