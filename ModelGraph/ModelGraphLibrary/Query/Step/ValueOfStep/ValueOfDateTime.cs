using System;

namespace ModelGraph.Internals
{
    internal abstract class ValueOfDateTime : ValueOfStep<DateTime>
    {
        internal override ValType ValueType => ValType.DateTime;

        internal override bool AsBool() => false;
        internal override long AsLong() => 0;
        internal override double AsDouble() => 0;
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => GetVal();
    }
}
