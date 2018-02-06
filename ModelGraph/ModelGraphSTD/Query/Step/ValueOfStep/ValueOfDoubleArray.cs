using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfDoubleArray : ValueOfStep<double[]>
    {
        internal override ValType ValType => ValType.DoubleArray;

        internal override double[] AsDoubleArray() => GetVal();
        internal override int AsLength() => AsDateTimeArray().Length;
    }
}
