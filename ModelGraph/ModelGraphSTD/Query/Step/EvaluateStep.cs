using System;

namespace ModelGraphSTD
{
    internal abstract class EvaluateStep
    {
        protected ComputeStep _step;

        abstract internal string Text { get; }
        abstract internal ValType ValType { get; }

        internal virtual bool AsBool() => throw new NotImplementedException(); // failed type check
        internal virtual Int32 AsInt32() => throw new NotImplementedException(); // failed type check
        internal virtual Int64 AsInt64() => throw new NotImplementedException(); // failed type check
        internal virtual double AsDouble() => throw new NotImplementedException(); // failed type check
        internal virtual string AsString() => throw new NotImplementedException(); // failed type check
        internal virtual DateTime AsDateTime() => throw new NotImplementedException(); // failed type check

        internal virtual Int32 AsLength() => 1; // default length

        internal virtual bool[] AsBoolArray() => throw new NotImplementedException(); // failed type check
        internal virtual Int32[] AsInt32Array() => throw new NotImplementedException(); // failed type check
        internal virtual Int64[] AsInt64Array() => throw new NotImplementedException(); // failed type check
        internal virtual double[] AsDoubleArray() => throw new NotImplementedException(); // failed type check
        internal virtual string[] AsStringArray() => throw new NotImplementedException(); // failed type check
        internal virtual DateTime[] AsDateTimeArray() => throw new NotImplementedException(); // failed type check

        internal ValGroup ValGroup => Value.GetValGroup(ValType);
    }
}
