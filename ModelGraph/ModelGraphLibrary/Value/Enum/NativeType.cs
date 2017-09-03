namespace ModelGraphLibrary
{/*
    NativeType is simplified  data ValueType it's used in computation

    A computeX's value may be coerced into other types, so to keep it simple,
    only the following enumerated types are allowed
 */
    /// <summary>
    ///     NativeType is simplified  data ValueType it's used in computation
    /// </summary>
    public enum NativeType : byte
    {
        None = 0,           // a queryX does not have a select clause
        Bool = 1,
        Byte = 2,
        Int16 = 3,
        Int32 = 4,
        Int64 = 5,
        Double = 6,
        String = 7,
        Invalid = 8,        // a computeX's select clause is invalid
        Circular = 9,       // a computeX is attempting to create a circular reference cycle 
        Unresolved = 10,    // a computeX's value type is currently unkown
    }
}
