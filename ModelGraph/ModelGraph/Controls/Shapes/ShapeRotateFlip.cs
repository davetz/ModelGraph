using System.Numerics;

namespace ModelGraph.Controls
{
    public enum RFCode : byte
    {
        R0 =  0x0,  // rotate  0 * (PI / 8) radians
        R2 =  0x2,  // rotate  2 * (PI / 8) radians
        R4 =  0x4,  // rotate  4 * (PI / 8) radians
        R6 =  0x6,  // rotate  6 * (PI / 8) radians
        R8 =  0x8,  // rotate  8 * (PI / 8) radians
        R10 = 0xA,  // rotate 10 * (PI / 8) radians
        R12 = 0xC,  // rotate 12 * (PI / 8) radians
        R14 = 0xE,  // rotate 14 * (PI / 8) radians

        Mask = 0x0F, // mask to extract the rotation component
        Flip = 0x10, // flip vertically

        FR0 = Flip | R0,
        FR2 = Flip | R2,
        FR4 = Flip | R4,
        FR6 = Flip | R6,
        FR8 = Flip | R8,
        FR10 = Flip | R10,
        FR12 = Flip | R12,
        FR14 = Flip | R14,
    }
    internal partial class Shape
    {
        private static (byte V, byte H)[] RF_State_Transition =
        {
            (16,  24),
            (31,  23),
            (30,  22),
            (29,  21),
            (28,  20),
            (27,  19),
            (26,  18),
            (25,  17),
            (24,  16),
            (23,  31),
            (22,  30),
            (21,  29),
            (20,  28),
            (19,  27),
            (18,  26),
            (17,  25),
            ( 0,   8),
            (15,   7),
            (14,   6),
            (13,   5),
            (12,   4),
            (11,   3),
            (10,   2),
            ( 9,   1),
            ( 8,   0),
            ( 7,  15),
            ( 6,  14),
            ( 5,  13),
            ( 4,  12),
            ( 3,  11),
            ( 2,  10),
            ( 1,   9),
        };
        internal void PrepairPoints(RFCode srf, float scale, Vector2 center)
        {
            var r1 = RF & 0xF;
            var r2 = (int)srf & 0xF;

            var f1 = (RF & 0x10) != 0;
            var f2 = ((int)srf & 0x10) != 0;

            foreach  (var (dx, dy) in DXY)
            {
                var v = new Vector2(dx, dy) * scale + center;
            }
        }
    }
}
