using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal abstract class ValueOfBoolArray : ValueOfStep<bool[]>
    {
        internal override ValType ValType => ValType.BoolArray;

        internal override bool[] AsBoolArray() => GetVal();

        internal override int AsLength() => AsBoolArray().Length;
    }
}
