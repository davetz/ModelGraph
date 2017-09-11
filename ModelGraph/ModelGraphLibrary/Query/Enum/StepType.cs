namespace ModelGraphLibrary
{
    /// <summary>
    /// Expression string parser step type
    /// </summary>
    public enum StepType : byte
    {
        None,   // the parse step type is currentlly unknown
        List,   // a list of parse elements, may be of different types
        Index,  // "[nn]" a list index
        Vector, // a vector of the same type values
        String, // string constant
        Double, // double precision floating point constant
        Integer, // (sbyte, short, int, or long) constant
        Boolean, // boolean constant
        NewLine, // marks line breaks in the expression string
        Property, // a property value
        BitField, // (byte, ushort, uint, or ulong) constant

        Or1, // "|" could be (string-concat, bitwise-OR, or boolean-OR) depends on context 
        And1, // "&" could be (bitwise-AND or boolean-AND) depends on context

        Or2, // "||"
        And2, // "&&"
        Not, // "!"
        Plus, // "+" could be (string-concat or numeric-ADD) depends on context
        Minus, // "-"
        Equals, // "=" or "==" can only test for eqaulty, never used to set a value
        Negate, // "~" 
        Divide, // "/"
        Multiply, // "*"
        LessThan, // "<"
        GreaterThan, // ">"
        NotLessThan, // ">="
        NotGreaterThan, // "<="

        Has,
        Ends,
        Starts,
    }
}
