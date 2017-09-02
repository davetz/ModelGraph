namespace ModelGraphLibrary
{
    /// <summary>
    /// Expression string parser step type
    /// </summary>
    public enum StepType : byte
    {
        None, // the parse step type is currentlly unknown
        Index, // "[nn]" an array index
        String, // string constant
        Double, // double precision floating point constant
        Integer, // (byte, short, int, or long) constant
        Boolean, // boolean constant
        NewLine, // marks line breaks in the expression string
        Property, // a property value

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
