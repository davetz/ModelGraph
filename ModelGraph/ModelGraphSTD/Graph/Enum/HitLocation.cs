using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum HitLocation
    {
        Void = 0x0000, // we hit a void (pointer is in an empty space)

        Open = 0x1000,
        Node = 0x2000,
        Edge = 0x4000,
        Region = 0x8000,
        Multiple = 0x10000,

        Top = 0x0010,
        Left = 0x0001,
        Right = 0x0002,
        Bottom = 0x0020,
        Center = 0x0040,
        SideOf = Top | Left | Right | Bottom,

        End1 = 0x0100,
        End2 = 0x0200,
        Bend = 0x0400,
        Term = 0x0800,

        NodeTop = Node | Top,
        NodeLeft = Node | Left,
        NodeRight = Node | Right,
        NodeBottom = Node | Bottom,
        NodeCenter = Node | Center,
        NodeTopLeft = Node | Top | Left,
        NodeTopRight = Node | Top | Right,
        NodeBottomLeft = Node | Bottom | Left,
        NodeBotemRight = Node | Bottom | Right,

        EdgeEnd1 = Edge | End1,
        EdgeEnd2 = Edge | End2,
        EdgeTerm = Edge | Term,
        EdgeEnd1Bend = Edge | End1 | Bend,
        EdgeEnd2Bend = Edge | End2 | Bend,
        EdgeEnd1Term = Edge | End1 | Term,
        EdgeEnd2Term = Edge | End2 | Term,
    }
}
