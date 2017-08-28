﻿using System;

namespace ModelGraphLibrary
{
    /*  
        Expression tree step flags.
    */
    [Flags]
    public enum StepFlag : ushort
    {
        None = 0,
        IsWierd = 0x1, // the actual input data type does not match the expected type, ok, but wierd
        HasParens = 0x2, // enclose generated subexpression in parens (...)
        HasNewLine = 0x4, // start generated subexpression with a newLine
    }
}
