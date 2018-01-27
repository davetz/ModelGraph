
namespace ModelGraphSTD
{/*
    The current types and associated numeric values must never be modified 
    because the numeric value is (saved-to/loaded-from) model repositories.

    However, if needed, a new type pair could be added as indicated below.
 */
    public enum ValType : byte
    {
        Bool = 0, //0,
        IsArray = 1, // flag bit to indicate an array
        BoolArray = 1, //15,

        Char = 2, //1,
        CharArray = 3, //16,

        Byte = 4, //2,
        ByteArray = 5, //17,

        SByte = 6, //3,
        SByteArray = 7, //18,

        Int16 = 8, //4,
        Int16Array = 9, //19,

        UInt16 = 10, //5,
        UInt16Array = 11, //20,

        Int32 = 12, //6,
        Int32Array = 13, //21,

        UInt32 = 14, //7,
        UInt32Array = 15, //22,

        Int64 = 16, //8,
        Int64Array = 17, //23,

        UInt64 = 18, //9,
        UInt64Array = 19, //24,

        Single = 20, //10,
        SingleArray = 21, //25,

        Double = 22, //11,
        DoubleArray = 23, //26,

        Decimal = 24, //12,
        DecimalArray = 25, //27,

        DateTime = 26, //13,
        DateTimeArray = 27, //28,

        String = 28, //14,
        StringArray = 29, //29,

        //<- - - - - - - - - - a new type pair could be added here

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
