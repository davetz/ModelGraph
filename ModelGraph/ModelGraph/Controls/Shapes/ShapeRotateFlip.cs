
using System;
using System.Numerics;

namespace ModelGraph.Controls
{
    public enum RFCode : byte
    {
        R0 = 0,     // 0 * (PI / 8) radian
        R2 = 2,     // 2 * (PI / 8) radian
        R4 = 4,     // 4 * (PI / 8) radian
        R6 = 6,     // 6 * (PI / 8) radian
        R8 = 8,     // 8 * (PI / 8) radian
        R10 = 10,   //10 * (PI / 8) radian
        R12 = 12,   //12 * (PI / 8) radian
        R14 = 14,   //14 * (PI / 8) radian
        R16 = 16,   //vert flip and 0 * (PI / 8) radian
        R18 = 18,   //vert flip and 2 * (PI / 8) radian
        R20 = 20,   //vert flip and 4 * (PI / 8) radian
        R22 = 22,   //vert flip and 6 * (PI / 8) radian
        R24 = 24,   //vert flip and 8 * (PI / 8) radian
        R26 = 26,   //vert flip and 10 * (PI / 8) radian
        R28 = 28,   //vert flip and 12 * (PI / 8) radian
        R30 = 30,   //vert flip and 14 * (PI / 8) radian
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

            Points.Clear();
            foreach  (var (dx, dy) in DXY)
            {
                var v = new Vector2(dx, dy) * scale + center;
                Points.Add(v);
            }
        }
    }
}
