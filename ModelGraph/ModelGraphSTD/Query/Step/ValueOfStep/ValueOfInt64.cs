using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfInt64 : ValueOfStep<Int64>
    {
        internal override ValType ValType => ValType.Int64;

        internal override bool AsBool() => (GetVal() != 0);
        internal override Int64 AsInt64() => GetVal();
        internal override double AsDouble() => GetVal();
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check
    }
}
