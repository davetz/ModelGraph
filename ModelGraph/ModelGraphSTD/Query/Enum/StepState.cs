using System;

namespace ModelGraphSTD
{
    /*  
        Compute step has changed.
    */
    [Flags]
    public enum StepState : byte
    {
        None = 0,
        IsError = 0x1,
        IsChanged = 0x2,
        IsOverflow = 0x4,
        IsUnresolved = 0x8,

        LowerMask = 0xF, // facilitate bubble-up
        UpperMask = 0xF0, // facilitate bubble-up

        AnyError = 0x10, // bubble-up any step IsError
        AnyChange = 0x20, // bubble-up any change 
        AnyOverflow = 0x40, // bubble-up any step IsOverflow
        AnyUnresolved = 0x80, // bubble-up any step IsUnresolved
    }
}
