namespace ModelGraphLibrary
{/*
    Enumeration of data value types

    Never change or remove the assigned values because the
    numeric value is (saved to/loaded from) model data files; 
 */
    /// <summary>
    /// Enumerated data value types
    /// </summary>
    public enum ValueType : byte
    {
        Bool = 0,
        Char = 1,
        Byte = 2,
        SByte = 3,
        Int16 = 4,
        Int32 = 5,
        Int64 = 6,
        UInt16 = 7,
        UInt32 = 8,
        UInt64 = 9,
        Single = 10,
        Double = 11,
        Decimal = 12,

        Guid = 13,
        DateTime = 14,
        TimeSpan = 15,

        String = 16,

        CharArray = 17,
        HexArray = 18,
        ByteArray = 19,
        SByteArray = 20,
        Int16Array = 21,
        Int32Array = 22,
        Int64Array = 23,
        UInt16Array = 24,
        UInt32Array = 25,
        UInt64Array = 26,
        SingleArray = 27,
        DoubleArray = 28,
        DecimalArray = 29,

        MaximumType = 30, // loader integrity check, may increase if need more types
        InternalEnum = 99, // don't change this
    }
}
