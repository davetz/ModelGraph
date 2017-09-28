namespace ModelGraphLibrary
{/*
    Enumeration of data value types

    Never remove of changed the assigned values of these types.
    The values are (saved to/loaded from) modeling data files
    and must be immutable to maintain backwards compatability.
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
                            // <== add more types here, then modify the method Value.Create()
        MaximumType = 30,   // loader integrity check, increase this if more types are added

        None = 95,          // a computeX before it has been validated 
        Invalid = 96,       // a computeX's select clause is invalid
        Circular = 97,      // a computeX is attempting to create a circular reference cycle 
        Unresolved = 98,    // a computeX's value type is currently unkown
        InternalEnum = 99,  // used for internal enums
    }
}
