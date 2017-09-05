using System;

namespace ModelGraphLibrary
{/*
 */
    public enum ParseError : byte
    {
        None,

        MissingArgLHS,
        MissingArgRHS,
        InvalidArgsLHS,
        InvalidArgsRHS,

        InvalidText,
        InvalidString,
        InvalidNumber,
        InvalidParens,

        UnknownProperty,
    }
}
