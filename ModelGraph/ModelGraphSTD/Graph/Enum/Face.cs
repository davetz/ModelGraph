
namespace ModelGraphSTD
{/*
    An edge may connect (if permited) to any one of the 8 faces of a node's symbol.
                N
            NW     NE             
           W    *    E               
            SW     SE
                S
  */
    public enum Face : byte
    {
        East = 0,
        SouthEast = 1,
        South = 2,
        SouthWest = 3,
        West = 4,
        NorthWest = 5,
        North = 6,
        NorthEast = 7,
    }
}
