using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfDouble : ValueOfStep<double>
    {
        internal override ValType ValType => ValType.Double;

        internal override bool AsBool() => (GetVal() != 0);
        internal override Int64 AsInt64() => throw new NotImplementedException(); // failed type check
        internal override double AsDouble() => GetVal();
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => throw new NotImplementedException(); // failed type check

        internal override bool[] AsBoolArray() => new bool[] { (GetVal() != 0) };
        internal override Int64[] AsInt64Array() => throw new NotImplementedException(); // failed type check
        internal override double[] AsDoubleArray() => new double[] { GetVal() };
        internal override string[] AsStringArray() => new string[] { GetVal().ToString() };
        internal override DateTime[] AsDateTimeArray() => throw new NotImplementedException(); // failed type check
    }
}
