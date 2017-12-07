
using System;

namespace ModelGraphSTD
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
