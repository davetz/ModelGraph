using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum FlipState : byte
    {
        None = 0,
        VertFlip = 1,
        HorzFlip = 2,
        VertHorzFlip = 3,
        LeftRotate = 4,
        LeftHorzFlip = 5,
        RightRotate = 6,
        RightHorzFlip = 7,
    }
}
