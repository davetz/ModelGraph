using System;

namespace ModelGraphLibrary
{
    /*  
        Expression tree step flags.
    */
    [Flags]
    public enum ParseFlag : ushort
    {
        None = 0,
        IsDone = 0x1,
        IsWierd = 0x2, // the input type does not match the expected type, it's ok, but wierd
        IsNegated = 0x4, // the parse step value should be negated
        IsBatched = 0x8, // generated expression has a succession of repeats e.g. ADD(A, B, C) becomes A + B + C
        HasParens = 0x10, // enclose generated expression in parens (...)
        HasNewLine = 0x20, // start generated expression with a newLine
        IsUnresolved = 0x40,
        IsInvalidRef = 0x80,
        IsCircularRef = 0x100,
    }
}
