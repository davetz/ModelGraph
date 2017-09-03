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
        InvalidNumber = 0x20,
        InvalidParens = 0x40,
        UnknownProperty = 0x80,
    }
}
