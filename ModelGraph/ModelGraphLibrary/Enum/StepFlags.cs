using System;

namespace ModelGraphLibrary
{
    [Flags]
    public enum StepFlags : byte
    {
        None = 0,
        IsBinary = 0x01,
        IsFlexable = 0x02, //allow coerced inputs
        IsVariable = 0x04, //references a property
        HasParens = 0x08,
        HasNewLine = 0x10,
    }
}
