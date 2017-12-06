using System;

namespace ModelGraphLibrary
{/*

 */
    [Flags]
    public enum FlipRotate : byte
    {
        None = 0,
        FlipVertical = 1,
        FlipHorizontal = 2,
        FlipBothWays = 3,
        RotateClockWise = 4,
        RotateFlipVertical = 5,
        RotateFlipHorizontal = 6,
        RotateFlipBothWays = 7,
    }
}
