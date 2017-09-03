using System;

namespace ModelGraphLibrary
{
    /*  
        Expression tree step flags.
    */
    [Flags]
    public enum StepFlag : byte
    {
        None = 0,
        IsWierd = 0x1, // the actual input data type does not match the expected type, ok, but wierd
        HasParens = 0x2, // enclose generated subexpression in parens (...)
        HasNewLine = 0x4, // start generated subexpression with a newLine
        IsUnresolved = 0x8, // a computed value is unresoved
        InvalidReference = 0x10, // refernces a invalid computeX
        CircularReference = 0x20, // references a cuputeX who's nativeType IsCircular
    }
}
