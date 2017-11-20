using System;

namespace ModelGraph.Internals
{/*  
    Resulting value type produced by a Compute step.

    Note: these flags are agragated over an array of inputs for error checking
  */
    [Flags]
    public enum ValGroup : ushort
    {
        None = 0,

        Int = 0x1,
        Bool = 0x2,
        Long = 0x4,
        String = 0x8,
        Double = 0x10,
        DateTime = 0x20,

        IntArray = 0x100,
        BoolArray = 0x200,
        LongArray = 0x400,
        StringArray = 0x800,
        DoubleArray = 0x1000,
        DateTimeArray = 0x2000,

        ScalarGroup = Int | Bool | Long | String | Double, //may be coerced 
        ArrayGroup = IntArray | BoolArray | LongArray | StringArray | DoubleArray, //may be coerced 
    }
}
