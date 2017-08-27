using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public enum ParseType : byte
    {
        None = 0,
        Index = 1,
        String = 2,
        Double = 3,
        Integer = 4,
        Property = 5,
        OrOperator = 10,
        AndOperator = 11,
        NotOperator = 12,
        PlusOperator = 13,
        MinusOperator = 14,
        EqualsOperator = 15,
        NegateOperator = 16,
        DivideOperator = 17,
        MultiplyOpartor = 18,
        LessThanOperator = 19,
        GreaterThanOperator = 20,
        NotLessThanOperator = 21,
        NotGreaterThanOperator = 22,
    }
}
