using System;

namespace ModelGraphLibrary
{
    internal abstract class ValueOfLong : ValueOfStep<long>
    {
        internal override ValType ValueType => ValType.Int64;

        internal override bool AsBool() => (GetVal() != 0);
        internal override long AsLong() => GetVal();
        internal override double AsDouble() => GetVal();
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check
    }
}
