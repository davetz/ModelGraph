using System;

namespace ModelGraphSTD
{
    internal enum FormatType : Byte
    {
        None = 0,       // default T.ToString() (int,bool,float)

        IsInt = 0x10,   // is integer format
        Int1 = 0x12,    // 12_456_567 (invariant number group seperators)
        Int2 = 0x13,    // 12,456,567 (culture specific number group seperator)

        IsBin = 0x20,   // is binary format
        Bin1 = 0x21,    // 0b prefix e.g. 0b1101_1110

        IsHex = 0x30,   // is hexadecimal format
        Hex1 = 0x31,    // 0x prefix e.g 0xFF_03_E0_00
        Hex2 = 0x32,    // #  prefix e.g. #FFFE00 (is default)
        Hex3 = 0x33,    // no prefix (ambiguous, use for output only)

        IsBool = 0x40,  // is boolean format
        Bool1 = 0x41,   // True, False
        Bool2 = 0x42,   // T, F
        Bool3 = 0x43,   // 1, 0

        IsFloat = 0x50, // is floating point format
        Float1 = 0x51,  // rounded to nearest whole number eg 1,204
        Float2 = 0x52,  // rounded to nearest 100th e.g. 1,204.05
        Float3 = 0x53,  // rounded to nearest 10,000th e.g. 1,204.0501 (is default)
        Float5 = 0x55,  // E scientific e.g 1.345e3

        IsDate = 0x60,  // is date time format

        IsCurrency = 0x70, // is culture specific curency format 
        Currency1 = 0x71,  // rounded up to whole unit .e.g $1,204
        Currency2 = 0x72,  // rounded up to cent e.g. $1,203.74 
        Currency3 = 0x73,  // rounded up to 100,000th e.g. $1,203.73533

        IsPercent = 0x80,  // is percent
        Percent1 = 0x81,   // .e.g. 25 %
        Percent2 = 0x82,   // .e.g. 24.5 %

        IsSpecific = 0xE0, // specific purpose read-only computed values
        NumericSet = 0xE1, // count, min, max, ave, std (float array)

        FormatMask = 0xF0, // format type mask
        FormatError = 0xFE, // formating error
        EmptyString = 0xFF, // can't determint format, string.IsNullOrWhiteSpace
    }
}
