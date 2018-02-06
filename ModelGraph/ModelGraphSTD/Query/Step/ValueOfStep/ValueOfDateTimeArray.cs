using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfDateTimeArray : ValueOfStep<DateTime[]>
    {
        internal override ValType ValType => ValType.DateTimeArray;

        internal override DateTime[] AsDateTimeArray() => GetVal();
        internal override int AsLength() => AsDateTimeArray().Length;
        internal override string AsString()
        {
            var v = GetVal();
            return Value.ArrayFormat(v, (i) => Value.ValueFormat(v[i], FormatType.IsDate));
        }
    }
}
