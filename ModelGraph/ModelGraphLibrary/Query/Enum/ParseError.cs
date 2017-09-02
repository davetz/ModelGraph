using System;

namespace ModelGraphLibrary
{/*
 */
    [Flags]
    public enum ParseError : byte
    {
        None = 0,
        TooFewArgs = 0x1,
        TooManyArgs = 0x2,
        InvalidArgs = 0x4,
        InvalidText = 0x8,
        InvalidString = 0x10,
        InvalidParens = 0x20,
        UnknownProperty = 0x40,
    }
}
