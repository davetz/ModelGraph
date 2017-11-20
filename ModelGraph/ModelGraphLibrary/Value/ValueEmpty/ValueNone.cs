
using System;

namespace ModelGraph.Internals
{
    internal class ValueNone : ValueEmpty
    {
        internal ValueNone()
        {
            _idString = "??????";
            _valueType = ValType.IsUnknown;
        }
    }
}
