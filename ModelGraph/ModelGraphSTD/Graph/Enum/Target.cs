﻿using System;

namespace ModelGraphSTD
{
    /// <summary>
    /// Allowed directional connections to a symbol 
    /// </summary>
    public enum Target : ushort
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
}