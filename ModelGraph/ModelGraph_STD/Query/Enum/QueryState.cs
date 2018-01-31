using System;

namespace ModelGraphSTD
{
    /*  
        Keep track of the validation state in a possibly multipass
        validation of ComputeX, QueryX, and the where/select clauses
    */
    /// <summary>
    /// QueryX validation state
    /// </summary>
    [Flags]
    public enum QueryState : byte //validation state
    {
        None = 0,

        IsInvalid = 0x1,  // can't even determine what the queryState is
        ParseError = 0x2,  // didn't get past the expression string parse phase
        InvalidRef = 0x4,   // encountered a computeX with ValueType.IsInvalid
        CircularRef = 0x8,   // encountered a computeX with ValueType.IsCircular
        UnresolvedRef = 0x10,  // encountered a computeX with ValueType.IsUnresolved

        ErrorsMask = 0x7F,
        IsValidQuery = 0x80, 
    }
}
