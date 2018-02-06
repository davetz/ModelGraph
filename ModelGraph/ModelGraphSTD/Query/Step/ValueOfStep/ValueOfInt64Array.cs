using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfInt64Array : ValueOfStep<Int64[]>
    {
        internal override ValType ValType => ValType.Int64Array;

       internal override Int64[] AsInt64Array() => GetVal();
        internal override int AsLength() => AsInt64Array().Length;
    }
}
