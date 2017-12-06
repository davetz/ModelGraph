
namespace ModelGraphLibrary
{/*
    The current types and associated numeric values must never be modified 
    because the numeric value is (saved-to/loaded-from) model repositories.

    However, if needed, a new type could be added as indicated below.
 */
    public enum ValType : byte
    {
        Bool = 0,
        Char = 1,

        Byte = 2,
        SByte = 3,

        Int16 = 4,
        UInt16 = 5,

        Int32 = 6,
        UInt32 = 7,

        Int64 = 8,
        UInt64 = 9,

        Single = 10,
        Double = 11,

        Decimal = 12,
        DateTime = 13,

        String = 14,

        BoolArray = 15,
        CharArray = 16,

        ByteArray = 17,
        SByteArray = 18,

        Int16Array = 19,
        UInt16Array = 20,

        Int32Array = 21,
        UInt32Array = 22,

        Int64Array = 23,
        UInt64Array = 24,

        SingleArray = 25,
        DoubleArray = 26,

        DecimalArray = 27,
        DateTimeArray = 28,

        StringArray = 29,

        //<- - - - - - - - - - a new type could be added here

        MaximumType = 30,   // used as integrity check durring load 
                            // MaximumType = last_valid_type + 1

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // the following are for application internal use
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        IsUnknown = 64,     // unassigned value type
        IsInvalid = 65,     // computed values that failed validation
        IsCircular = 66,    // indicates a circular references
        IsUnresolved = 67,  // indicates an unresolved dependancy
    }
}
