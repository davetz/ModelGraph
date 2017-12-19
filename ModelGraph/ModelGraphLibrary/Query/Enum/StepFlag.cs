using System;

namespace ModelGraphSTD
{
    /*  
        Compute step flags.
    */
    [Flags]
    public enum StepFlag : byte
    {
        None = 0,
        IsInverse = 0x1, // the step value should be inverted (ones complemnt)
        IsNegated = 0x2, // the step value should be negated (twos complement)
        IsBatched = 0x4, // the expression has a succession of repeats e.g. ADD(A, B, C) becomes A + B + C
        HasParens = 0x8, // enclose the subexpression in parens (...)

        HasNewLine = 0x10, // start subexpression with a newLine

        ParseAborted = 0x80, // the parser abort because of invalid syntax 
    }
}
