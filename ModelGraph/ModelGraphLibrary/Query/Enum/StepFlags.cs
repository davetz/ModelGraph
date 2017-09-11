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
        IsWierd = 0x1, // the input type does not match the expected type, it's ok, but wierd
        IsNegated = 0x2, // the parse step value should be negated
        IsBatched = 0x4, // generated expression has a succession of repeats e.g. ADD(A, B, C) becomes A + B + C
        HasParens = 0x8, // enclose generated expression in parens (...)
        HasNewLine = 0x10, // start generated expression with a newLine
        IsBitField = 0x20, // can particiapte in (~, &, |, ^, >>, and <<) bit field operations
    }
}
