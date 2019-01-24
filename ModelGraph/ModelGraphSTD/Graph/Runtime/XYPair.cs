using System;
using System.Numerics;

namespace ModelGraphSTD
{
    public static class XYPair
    {
        static readonly int _ds = GraphDefault.HitMargin;

        public static (float X, float Y) Move((float x, float y) p, (float dx, float dy) b) => ((p.x + b.dx), (p.y + b.dy));
        public static (float X, float Y) Rotate((float x, float y) p, (float x, float y) b) => ((b.x - (p.y - b.y)), (b.y + (p.x - b.x)));
        public static (float X, float Y) VerticalFlip((float x, float y) p, float y) => ((y + (y - p.y)), (p.x));
        public static (float X, float Y) HorizontalFlip((float x, float y) p, float x) => ((x + (x - p.x)), p.y);
        public static (float X, float Y) RotateFlip((float x, float y) point, (float x, float y) focus, FlipRotate flip)
        {
            switch (flip)
            {
                case ModelGraphSTD.FlipRotate.None:
                    return focus;

                case ModelGraphSTD.FlipRotate.FlipVertical:
                    return VerticalFlip(point, focus.y);

                case ModelGraphSTD.FlipRotate.FlipHorizontal:
                    return HorizontalFlip(point, focus.x);

                case ModelGraphSTD.FlipRotate.FlipBothWays:
                    return VerticalFlip(HorizontalFlip(point, focus.x), focus.y);

                case ModelGraphSTD.FlipRotate.RotateClockWise:
                    return Rotate(point, focus);

                case ModelGraphSTD.FlipRotate.RotateFlipVertical:
                    return VerticalFlip(Rotate(point, focus), focus.y);

                case ModelGraphSTD.FlipRotate.RotateFlipHorizontal:
                    return HorizontalFlip(Rotate(point, focus), focus.x);

                case ModelGraphSTD.FlipRotate.RotateFlipBothWays:
                    return VerticalFlip(HorizontalFlip(Rotate(point, focus), focus.x), focus.y);
            }
            return focus;
        }
        public static float Diagonal((float dx, float dy) p) => ((p.dx * p.dx) + (p.dy * p.dy));
        public static float Diagonal((float x, float y) p1, (float x, float y) p2) => Diagonal((p2.x - p1.x, p2.y - p1.y));
        public static (double ux, double uy) OrthoginalUnitVector(float dx, float dy)
        {
            var M = System.Math.Sqrt(dx * dx + dy * dy);
            return (dy / M, -dx / M);
        }
        public static (float x, float y) OrthoginalDisplacedPoint(float dx, float dy, float x0, float y0, float ds)
        {
            var (ux, uy) = OrthoginalUnitVector(dx, dy);
            return ((float)(ds * ux + x0), (float)(ds* uy + y0));
        }
        public static (Quad quad, Sect sect) QuadSect((float x, float y) p)
        {
            if (p.x < 0)
            {
                if (p.y < 0)
                    return (-p.x < -p.y) ? (Quad.Q3, Sect.S6) : (Quad.Q3, Sect.S5);
                else
                    return (-p.x < p.y) ? (Quad.Q2, Sect.S3) : (Quad.Q2, Sect.S4);
            }
            else
            {
                if (p.y < 0)
                    return (p.x < -p.y) ? (Quad.Q4, Sect.S7) : (Quad.Q4, Sect.S8);
                else
                    return (p.x < p.y) ? (Quad.Q1, Sect.S2) : (Quad.Q1, Sect.S1);
            }
        }
        public static (Quad quad, Sect sect, double slope) QuadSectSlope((float x, float y) p1, (float x, float y) p2)
        {
            var x1 = (double)p1.x;
            var y1 = (double)p1.y;
            var x2 = (double)p2.x;
            var y2 = (double)p2.y;

            var dx = x2 - x1;
            var dy = y2 - y1;
            bool isVert = dx == 0;
            bool isHorz = dy == 0;

            if (isVert)
            {
                if (isHorz)
                    return (Quad.Any, Sect.Any, 0.0);
                else
                    return (dy > 0) ? (Quad.Q1, Sect.S2, 1023.0) : (Quad.Q4, Sect.S7, -1024.0);
            }
            else
            {
                if (isHorz)
                    return (dx > 0) ? (Quad.Q1, Sect.S1, 0.0) : (Quad.Q2, Sect.S4, 0.0);
                else
                {
                    var (quad, sect) = QuadSect(((int)dx, (int)dy));
                    return (quad, sect, dy / dx);
                }
            }
        }
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
    }
}