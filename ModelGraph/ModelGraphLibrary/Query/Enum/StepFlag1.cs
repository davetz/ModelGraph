using System;

namespace ModelGraphLibrary
{
    /*  
        Compute step flags.
    */
    [Flags]
    public enum StepFlag1 : byte
    {
        None = 0,
        IsNegated = 0x1, // the step value should be negated
        IsBatched = 0x2, // the expression has a succession of repeats e.g. ADD(A, B, C) becomes A + B + C
        HasParens = 0x4, // enclose the subexpression in parens (...)
        HasNewLine = 0x8, // start subexpression with a newLine

        ParseAborted = 0x10, // the parser abort because of invalid syntax 
    }
}
