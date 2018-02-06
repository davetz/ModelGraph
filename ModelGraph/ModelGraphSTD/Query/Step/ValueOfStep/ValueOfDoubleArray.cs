using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfDoubleArray : ValueOfStep<double[]>
    {
        internal override ValType ValType => ValType.DoubleArray;

        internal override double[] AsDoubleArray() => GetVal();
        internal override int AsLength() => AsDateTimeArray().Length;
        internal override string AsString()
        {
            var v = GetVal();
            return Value.ArrayFormat(v, (i) => Value.ValueFormat(v[i], FormatType.Float1));
        }
    }
}
