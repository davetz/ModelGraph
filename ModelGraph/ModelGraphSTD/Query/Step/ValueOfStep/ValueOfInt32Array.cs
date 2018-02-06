using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfInt32Array : ValueOfStep<Int32[]>
    {
        internal override ValType ValType => ValType.Int32Array;

        internal override Int32[] AsInt32Array() => GetVal();
        internal override int AsLength() => AsInt32Array().Length;
    }
}
