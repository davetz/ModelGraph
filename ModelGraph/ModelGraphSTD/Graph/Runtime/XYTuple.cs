using System;
using System.Numerics;

namespace ModelGraphSTD
{
    public static class XYTuple
    {
        #region Rotation  =====================================================
        static readonly float Radians45Degree = (float)(Math.PI / 4);
        static readonly float Radians90Degree = (float)(Math.PI / 2);
        static internal Matrix3x2 RotateLeft45Matrix((float x, float y) f)
        {
            var cp = new Vector2(f.x, f.y);
            return Matrix3x2.CreateRotation(-Radians45Degree, cp);
        }
        static internal Matrix3x2 RotateLeft90Matrix((float x, float y) f)
        {
            var cp = new Vector2(f.x, f.y);
            return Matrix3x2.CreateRotation(-Radians90Degree, cp);
        }
        static internal Matrix3x2 RotateRight45Matrix((float x, float y) f)
        {
            var cp = new Vector2(f.x, f.y);
            return Matrix3x2.CreateRotation(Radians45Degree, cp);
        }
        static internal Matrix3x2 RotateRight90Matrix((float x, float y) f)
        {
            var cp = new Vector2(f.x, f.y);
            return Matrix3x2.CreateRotation(Radians90Degree, cp);
        }
        static internal (float X, float Y) Transform(Matrix3x2 mx, (float x, float y) p)
        {
            var pt = new Vector2(p.x, p.y);
            pt = Vector2.Transform(pt, mx);
            return (pt.X, pt.Y);
        }
        #endregion

        #region SlopeSlice  ===================================================
        public static (float dx, float dy, float slope, int slice) SlopeSlice((float x1, float y1) startPoint, (float x2, float y2) endPoint)
        {/*
            . 11|12 .     Draw a circle arround the startPoint. Divided the circle into 16 slices numbered 0 to 15        
           8    |    15   The endPoint is contained within reported slice, (slice lines are ourward pointing rays)
           -----o------   The sector index tells you the direction from the startPoint to the endPoint 
           7    |     0   ============================================================================
            .  4|3   .    dx = (x2 - x1),    dy = (y2 - y1),    slope = (dy / dx)           
         */
            const float a = 0.4142135623730950f; //tan(22.5)
            const float b = 1.0f;                //tan(45.0)
            const float c = 2.4142135623730950f; //tan(67.5)

            var (x1, y1) = startPoint;
            var (x2, y2) = endPoint;

            var dx = x2 - x1;
            var dy = y2 - y1;

            bool isVert = dx == 0;
            bool isHorz = dy == 0;

            (float, int) slope_slice = (0, 0);

            if (isVert)
            {
                if (isHorz)
                {
                    slope_slice = (0, 0);
                }
                else if (dy > 0)
                    slope_slice = (1023, 3);
                else
                    slope_slice = (-1023, 12);
            }
            else if (isHorz)
            {
                if (dx > 0)
                    slope_slice = (0, 0);
                else
                    slope_slice = (0, 7);
            }
            else
            {
                var m = dy / dx;
                if (dx < 0)
                {
                    if (dy < 0)
                        slope_slice = (m, (m < a) ? 8 : (m < b) ? 9 : (m < c) ? 10 : 11);
                    else
                        slope_slice = (m, (m < -c) ? 4 : (m < -b) ? 5 : (m < -a) ? 6 : 7);
                }
                else
                {
                    if (dy < 0)
                        slope_slice = (m, (m < -c) ? 12 : (m < -b) ? 13 : (m < -a) ? 14 : 15);
                    else
                        slope_slice = (m, (m < a) ? 0 : (m < b) ? 1 : (m < c) ? 2 : 3);
                }
            }
            var (slope, slice) = slope_slice;
            return (dx, dy, slope, slice);
        }
        #endregion

        #region ScaledNormal  =================================================
        public static ((float dx, float dy) p1, (float dx2, float dy2) p2) GetScaledNormal(Target targ, float x, float y, float d)
        {
            float s;
            switch (targ)
            {
                case Target.N:
                case Target.S:
                    return ((x - d, y), (x + d, y));

                case Target.E:
                case Target.W:
                    return ((x, y - d), (x, y + d));

                case Target.NE:
                case Target.SE:
                case Target.NW:
                case Target.SW:
                    d *= .5f;
                    return ((x - d, y), (x + d, y));

                case Target.EN:
                case Target.WN:
                case Target.ES:
                case Target.WS:
                    d *= .5f;
                    return ((x, y - d), (x, y + d));

                case Target.NEC:
                    s = (x - 1) * d;
                    return ((x - s, y - s), (x + s, y + s));

                case Target.SEC:
                    s = (x - 1) * d;
                    return ((x - s, y + s), (x + s, y - s));

                case Target.NWC:
                    s = (1 + x) * d;
                    return ((x - s, y + s), (x + s, y - s));

                case Target.SWC:
                    s = (1 + x) * d;
                    return ((x - s, y - s), (x + s, y + s));

                default:
                    return ((0, 0), (0, 0));
            }
        }
        public static (Vector2 p1, Vector2 p2) GetScaledNormal(Target targ, Vector2 p, float d, Vector2 center, float scale)
        {
            var (p1, p2) = GetScaledNormal(targ, p.X, p.Y, d);
            var v1 = ToVector(p1);
            var v2 = ToVector(p2);
            return (center + scale * v1, center + scale * v2);

            Vector2 ToVector((float x, float y) q) => new Vector2(q.x, q.y);
        }
        #endregion  

        public static float Diagonal((float dx, float dy) p) => ((p.dx * p.dx) + (p.dy * p.dy));
    }
}