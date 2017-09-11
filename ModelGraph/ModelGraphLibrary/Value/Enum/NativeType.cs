namespace ModelGraphLibrary
{/*
    NativeType is a modified data ValueType used in computation

    A computeX's value may be coerced into other types, so to keep it simple,
    only the following enumerated types are allowed
 */
    /// <summary>
    /// NativeType is a modified data ValueType used in computation
    /// </summary>
    public enum NativeType : byte
    {
        None = 0,           // a queryX does not have a select clause
        Bool = 1,
        Char = 2,
        Byte = 3,
        SByte = 4,
        Int16 = 5,
        Int32 = 6,
        Int64 = 7,
        UInt16 = 8,
        UInt32 = 9,
        UInt64 = 10,
        Double = 11,
        String = 12,
        DateTime = 13,
        TimeSpan = 14,
        CharArray = 15,
        ByteArray = 16,
        SByteArray = 17,
        Int16Array = 18,
        Int32Array = 19,
        Int64Array = 20,
        UInt16Array = 21,
        UInt32Array = 22,
        UInt64Array = 23,
        DoubleArray = 24,
        Invalid = 25,       // a computeX's select clause is invalid
        Circular = 26,      // a computeX is attempting to create a circular reference cycle 
        Unresolved = 27,    // a computeX's value type is currently unkown
    }
}
