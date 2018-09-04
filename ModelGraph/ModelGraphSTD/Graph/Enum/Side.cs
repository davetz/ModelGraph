﻿using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum Side : byte
    {/*
         sect         quad       side
        =======       ====       ======
        5\6|7/8        3|4         N
        ~~~+~~~        ~+~       W + E
        4/3|2\1        2|1         S
     */
        Any = 0,
        East = 1,   // right
        West = 2,   // left 
        North = 4,  // top
        South = 8,  // bottom 
    };
}
