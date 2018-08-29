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
        public static int Quad((int dx, int dy) p) => (p.dx >= 0) ? ((p.dy >= 0) ? 1 : 4) : (p.dy >= 0) ? 2 : 3;
        public static int Diagonal((int dx, int dy) p) => ((p.dx * p.dx) + (p.dy * p.dy));
        public static float Slope((int dx, int dy) p) => p.dy / ((p.dx == 0) ? 0.4f : p.dx);
        public static (int quad, int sect, Side side, float slope) QuadSectSideSlope((int dx, int dy) p)
        {
            var quad = Quad(p);
            var slope = Slope(p);

            if (quad == 1)
            {
                if (slope > 1)
                    return (quad, 2, Side.South, slope);
                else
                    return (quad, 1, Side.East, slope);
            }
            else if (quad == 2)
            {
                if (slope < -1)
                    return (quad, 3, Side.South, slope);
                else
                    return (quad, 4, Side.West, slope);
            }
            else if (quad == 3)
            {
                if (slope > 1)
                    return (quad, 6, Side.North, slope);
                else
                    return (quad, 5, Side.West, slope);
            }
            else
            {
                if (slope < -1)
                    return (quad, 7, Side.North, slope);
                else
                    return (quad, 8, Side.East, slope);
            }
        }
        public static bool HitTest((int x, int y) p, (int x, int y) d)
        {
            var dx = p.x - d.x;
            if (dx * dx < _ds2) return true;

            var dy = p.y - d.y;
            if (dy * dy < _ds2) return true;

            return false;
        }
    }
}
