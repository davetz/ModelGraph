using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum Facet : byte
    {
        None = 0,
        Nubby = 1,
        Diamond = 2,
        InArrow = 3,

        Mask = 0x1F,
        Forced = 0x20,

        Force_None = None | Forced,
        Force_Nubby = Nubby | Forced,
        Force_Diamond = Diamond | Forced,
        Force_InArrow = InArrow | Forced,

    }
}
