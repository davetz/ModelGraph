using System;

namespace ModelGraph.Internals
{
    /*  
        Compute step evaluation value type.
    */
    [Flags]
    public enum ValGroup : ushort
    {
        None = 0,

        Bool = 0x1,
        Long = 0x2,
        String = 0x4,
        Double = 0x8,
        DateTime = 0x10,

        BoolArray = 0x100,
        LongArray = 0x200,
        StringArray = 0x400,
        DoubleArray = 0x800,
        DateTimeArray = 0x1000,

        ScalarGroup = Bool | Long | String | Double, //may be coerced 
        ArrayGroup = BoolArray | LongArray | StringArray | DoubleArray, //may be coerced 
    }
}
