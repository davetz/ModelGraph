using System;

namespace ModelGraphLibrary
{
    internal abstract class ValueOfDouble : ValueOfStep<double>
    {
        internal override ValType ValueType => ValType.Double;

        internal override bool AsBool() => (GetVal() != 0);
        internal override long AsLong() => throw new NotImplementedException(); // failed type check
        internal override double AsDouble() => GetVal();
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check
    }
}
