using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal abstract class ValueOfBool : ValueOfStep<bool>
    {
        internal override ValType ValType => ValType.Bool;

        internal override bool AsBool() => GetVal();
        internal override Int32 AsInt32() => GetVal() ? 1 : 0;
        internal override Int64 AsInt64() => GetVal() ? 1 : 0;
        internal override double AsDouble() => GetVal() ? 1 : 0;
        internal override string AsString() => GetVal() ? "True" : "False";
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check
    }
}
