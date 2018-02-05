
using System;

namespace ModelGraphSTD
{
    internal class ValueUnknown : ValueEmpty
    {
        internal ValueUnknown()
        {
            _idString = "??????";
            _valueType = ValType.IsUnknown;
        }
    }
}
