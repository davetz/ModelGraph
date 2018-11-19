using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum FlipRotate : byte
    {
        None = 0,
        Rotate45 = 1,
        Rotate90 = 2,
        Rotate135 = 3,
        Rotate180 = 4,
        Rotate225 = 5,
        Rotate270 = 6,
        Rotate315 = 7,
        FlipVertical = 8,
        FlipRotate45 = 9,
        FlipRotate90 = 10,
        FlipRotate135 = 11,
        FlipRotate180 = 12,
        FlipRotate225 = 13,
        FlipRotate270 = 14,
        FlipRotate315 = 15,
        // obsolete - soon to be deleted
        FlipHorizontal = 20,
        FlipBothWays = 21,
        RotateClockWise = 22,
        RotateFlipVertical = 23,
        RotateFlipHorizontal = 24,
        RotateFlipBothWays = 25,
    }
}
