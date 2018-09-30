using System.Numerics;

namespace ModelGraphSTD
{
    public static class XYPair
    {
        static readonly int _ds = GraphDefault.HitMargin;
        static readonly int _ds2 = GraphDefault.HitMarginSquared;

        public static (int X, int Y) Move((int x, int y) p, (int dx, int dy) b) => ((p.x + b.dx), (p.y + b.dy));
        public static (int X, int Y) Rotate((int x, int y) p, (int x, int y) b) => ((b.x - (p.y - b.y)), (b.y + (p.x - b.x)));
        public static (int X, int Y) VerticalFlip((int x, int y) p, int y) => ((y + (y - p.y)), (p.x));
        public static (int X, int Y) HorizontalFlip((int x, int y) p, int x) => ((x + (x - p.x)), p.y);
        public static (int X, int Y) RotateFlip((int x, int y) point, (int x, int y) focus, FlipRotate flip)
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
        public static int Diagonal((int dx, int dy) p) => ((p.dx * p.dx) + (p.dy * p.dy));
        public static (double ux, double uy) OrthoginalUnitVector(int dx, int dy)
        {
            var M = System.Math.Sqrt(dx * dx + dy * dy);
            return (dy / M, -dx / M);
        }
        public static (int x, int y) OrthoginalDisplacedPoint(int dx, int dy, int x0, int y0, double ds)
        {
            var (ux, uy) = OrthoginalUnitVector(dx, dy);
            return ((int)(ds * ux + x0), (int)(ds* uy + y0));
        }
        public static (Quad quad, Sect sect) QuadSect((int x, int y) p)
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
        public static (Quad quad, Sect sect, double slope) QuadSectSlope((int x, int y) p1, (int x, int y) p2)
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
    }
}