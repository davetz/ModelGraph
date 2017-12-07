using System;

namespace ModelGraphSTD
{
    internal abstract class EvaluateStep
    {
        protected ComputeStep _step;

        abstract internal string Text { get; }
        abstract internal ValType ValueType { get; }

        internal abstract bool AsBool();
        internal abstract long AsLong();
        internal abstract double AsDouble();
        internal abstract string AsString();
        internal abstract DateTime AsDateTime();

        internal ValGroup ValueGroup => Value.GetValGroup(ValueType);
    }
}
