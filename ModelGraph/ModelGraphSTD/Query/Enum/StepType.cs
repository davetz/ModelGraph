namespace ModelGraphSTD
{
    /// <summary>
    /// An expression's step type, as determined by the Parser
    /// </summary>
    public enum StepType : byte
    {
        Parse,   // the parse step type is currentlly unknown

        List,     // a list of parse elements, may be of different types
        Index,    // "[nn]" a list index
        Vector,   // a vector of the same type values
        String,   // string constant
        Double,   // double precision floating point constant
        Integer,  // (sbyte, short, int, or Int64) constant
        Boolean,  // boolean constant
        Property, // a property value
        BitField, // (byte, ushort, uint, or ulong) constant

        Or1,  // "|"   could be (string-concat, bitwise-OR, or boolean-OR) depends on context 
        And1, // "&"   could be (bitwise-AND or boolean-AND) depends on context

        Or2, // "||"   boolean Or
        And2, // "&&"  boolean And
        Not, // "!"    boolean Not

        Min,    // "Min" minimum value in an array
        Max,    // "Max" maximum value in an array
        Sum,    // "Sum" summation of value in an array
        Ave,    // "Ave" average value in an array
        Count,  // "Count" length of an array or string
        Length, // "Length" length of an array or string

        Plus, // "+"   could be (string-concat or numeric-Add) depends on context
        Minus, // "-"  could be (string-remove or numeric-Minus) depends on context
        Negate, // "~" bitwise negate (ones complement)
        Divide, // "/"
        Multiply, // "*"

        Equal,   // "=" or "==" is only test for eqaulty, never used to set a value
        NotEqual,   // "!=" is only test for ineqaulty

        LessThan, // "<"
        LessEqual, // "<="

        GreaterThan, // ">"
        GreaterEqual, // ">="

        EndsWith,   //  "~|" string test e.g. "This-fine-string"  ~| "ing"
        Contains,   // "|~|" string test e.g. "This-fine-string" |~| "fine"
        StartsWith, // "|~"  string test e.g. "This-fine-string" |~  "Thi"
    }
}
