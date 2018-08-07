using System;

namespace ModelGraphSTD
{
    /// <summary>
    /// Allowed directional connections to a symbol 
    /// </summary>
    [Flags]
    public enum Connect : byte
    {
        Any = 0,    // Side.Any
        East = 1,   // Side.East
        West = 2,   // Side.West
        North = 4,  // Side.North
        South = 8,  // Side.South
        East_West = (East | West),
        North_South = (North | South),
        North_East = (North | East),
        North_West = (North | West),
        North_East_West = (North | West | East),
        North_South_East = (North | East | South),
        North_South_West = (North | West | South),
        South_East = (South | East),
        South_West = (South | West),
        South_East_West = (South | East | West),
    }
}
