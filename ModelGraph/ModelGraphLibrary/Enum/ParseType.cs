using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public enum ParseType : byte
    {
        Unknown = 0,
        Token = 1,
        String = 2,
        Double = 3,
        Integer = 4,
        Operator = 5,
    }
}
