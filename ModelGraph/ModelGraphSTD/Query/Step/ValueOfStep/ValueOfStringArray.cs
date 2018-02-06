using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfStringArray : ValueOfStep<string[]>
    {
        internal override ValType ValType => ValType.StringArray;

        internal override string[] AsStringArray() => GetVal();
        internal override int AsLength() => AsStringArray().Length;
        internal override string AsString()
        {
            var v = GetVal();
            return Value.ArrayFormat(v, (i) => v[i]);
        }
    }
}
