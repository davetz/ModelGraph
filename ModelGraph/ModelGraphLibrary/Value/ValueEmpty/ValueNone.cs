
using System;

namespace ModelGraphLibrary
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
