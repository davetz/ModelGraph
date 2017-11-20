using System;

namespace ModelGraph.Internals
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
        InvalidSyntax,
        InvalidNumber,
        InvalidOperator,

        UnknownProperty,
    }
}
