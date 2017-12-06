using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphLibrary
{
    internal abstract class ValueOfBool : ValueOfStep<bool>
    {
        internal override ValType ValueType => ValType.Bool;

        internal override bool AsBool() => GetVal();
        internal override long AsLong() => GetVal() ? 1 : 0;
        internal override double AsDouble() => GetVal() ? 1 : 0;
        internal override string AsString() => GetVal() ? "True" : "False";
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check
    }
}
