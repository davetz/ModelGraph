using System;

namespace ModelGraphSTD
{
    public enum Side : byte
    {/*
         sect         quad       side
        =======       ====       ======
        5\6|7/8        3|4         N
        ~~~+~~~        ~+~       W + E
        4/3|2\1        2|1         S
     */
        East = 0,
        South = 1,
        West =  2,
        North = 3,
    };
}
