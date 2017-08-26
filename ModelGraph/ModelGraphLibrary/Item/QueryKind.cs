using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public enum QueryType : byte
    {
        View = 1,
        Path = 2,
        Group = 3,
        Segue = 4,
        Graph = 5,
        Value = 6,
        Symbol = 7,
    }
}
