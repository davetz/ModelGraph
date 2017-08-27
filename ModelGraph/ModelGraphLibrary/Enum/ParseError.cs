using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public enum ParseError : byte
    {
        None,
        TooFewArgs,
        TooManyArgs,
        InvalidArgs,
        InvalidText,
        InvalidString,
        InvalidParens,
    }
}
