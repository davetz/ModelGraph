using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfInt64Array : ValueOfStep<Int64[]>
    {
        internal override ValType ValType => ValType.Int64Array;

       internal override Int64[] AsInt64Array() => GetVal();
        internal override int AsLength() => AsInt64Array().Length;
        internal override string AsString()
        {
            var v = GetVal();
            return Value.ArrayFormat(v, (i) => Value.ValueFormat(v[i], FormatType.IsInt));
        }
    }
}
