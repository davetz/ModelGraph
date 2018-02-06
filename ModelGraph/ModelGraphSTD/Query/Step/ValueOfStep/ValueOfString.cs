using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfString : ValueOfStep<string>
    {
        internal override ValType ValType => ValType.String;

        internal override bool AsBool() => Convert.ToBoolean(GetVal());
        internal override Int64 AsInt64() => Convert.ToInt64(GetVal());
        internal override double AsDouble() => Convert.ToDouble(GetVal());
        internal override string AsString() => GetVal();
        internal override DateTime AsDateTime() => Convert.ToDateTime(GetVal());
    }
}
