using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum Side : byte
    {
        Any = 0,
        East = 1,   // right
        West = 2,   // left 
        North = 4,  // top
        South = 8,  // bottom 
    };
}
