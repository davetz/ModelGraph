using System;

namespace ModelGraphLibrary
{/*
 */
    public enum StepError : byte
    {
        None,

        EmptyString,
        UnbalancedQuotes,
        UnbalancedParens,
        InvalidExpression,

        MissingArgLHS,
        MissingArgRHS,
        InvalidArgsLHS,
        InvalidArgsRHS,

        InvalidText,
        InvalidNumber,
        InvalidOperator,

        UnknownProperty,
    }
}
