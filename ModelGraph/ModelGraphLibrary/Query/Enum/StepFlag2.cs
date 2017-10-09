using System;

namespace ModelGraphLibrary
{
    /*  
        Compute step has changed.
    */
    [Flags]
    public enum StepFlag2 : byte
    {
        None = 0,

        AnyChange = 0x1, // bubble-up any Evaluate instance changed or any IsUnresoved changed
        AnyInvalid = 0x2, // bubble-up any step is invalid
        AnyUnresolved = 0x4, // bubble-up any step is unresolved

        AnyUInt = 0x8, // some input is unsigned integer
        AnyULong = 0x10, // some input is unsigned long integer

        AnyInt = 0x20,    // some input is integer
        AnyDouble = 0x40, // some input is double 

        AnyNonNumeric = 80, // some input is not numeric
    }
}
