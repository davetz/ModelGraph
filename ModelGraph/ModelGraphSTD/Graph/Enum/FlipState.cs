using System;

namespace ModelGraphSTD
{
    [Flags]
    public enum FlipState : byte //reorientate the symbol to minimize crossed edge connections
    {
        None = 0,
        VertFlip = 1,        //flip vertically
        HorzFlip = 2,        //flip horizontally
        VertHorzFlip = 3,    //flip both vertical and horizontal
        LeftRotate = 4,      //rotate 90 degress counter clockwise
        LeftHorzFlip = 5,    //after rotate flip horizontal
        RightRotate = 6,     //rotate 90 degree clockwise
        RightHorzFlip = 7,   //after rotate flip horizontal
    }
    public enum AutoFlip : byte //automaticlly reorientate the symbol to minimize crossed edge connections
    {
        None = 0,
        VertFlip = 0x01,        //flip vertically
        HorzFlip = 0x02,        //flip horizontally
        VertHorzFlip = 0x04,    //flip both vertical and horizontal
        LeftRotate = 0x08,      //rotate 90 degress counter clockwise
        LeftHorzFlip = 0x10,  //after rotate flip horizontal
        RightRotate = 0x20,     //rotate 90 degree clockwise
        RightHorzFlip = 0x40, //after rotate flip horizontal
    }
}
