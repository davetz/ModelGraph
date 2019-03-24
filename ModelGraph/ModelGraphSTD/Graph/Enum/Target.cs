using System;

namespace ModelGraphSTD
{    
    [Flags]
    public enum Target : ushort // Allowed directional connections to a symbol 
    {
        N = 0x1,
        S = 0x2,
        E = 0x4,
        W = 0x8,

        NE = 0x10,
        NW = 0x20,
        SE = 0x40,
        SW = 0x80,

        EN = 0x100,
        ES = 0x200,
        WN = 0x400,
        WS = 0x800,

        NEC = 0x1000,
        NWC = 0x2000,
        SEC = 0x4000,
        SWC = 0x8000,

        Any = 0xFFFF,
        None = 0,
    }
    public enum TargetIndex : byte // symbol's target contact
    {
        EN = 0,   // upper east        - horizontal 
        E = 1,    // center east       - horizontal 
        ES = 2,   // lower east        - horizontal 
        SEC = 3,  // south east corner - diagonal
        SE = 4,   // right sidesouth   - vertical  
        S = 5,    // centersouth       - vertical  
        SW = 6,   // left side south   - vertical 
        SWC = 7,  // south west corner - diagonal
        WS = 8,   // lower west        - horizontal
        W = 9,    // center west       - horizontal
        WN = 10,  // upper west        - horizontal
        NWC = 11, // north west corner - diagonal
        NW = 12,  // left side north   - vertical
        N = 13,   // center north      - vertical
        NE = 14,  // right side north  - vertical
        NEC = 15  // north east corner - diagonal 
    }
    public enum Direction : byte // terminal's outward direction
    {
        Any = 0,
        E = 1,   // straight east   - horizontal
        S = 2,   // straight south  - vertical
        W = 3,   // straight west   - horizontal
        N = 4,   // straight north  - vertical
        SEC = 5, // south east corner - diagonal
        SWC = 6, // south west corner - diagonal
        NWC = 7, // north west corner - diagonal
        NEC = 8, // north east corner - diagonal
    }
}
