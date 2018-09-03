using System.Numerics;

namespace ModelGraphSTD
{
    public static class XYPair
    {
        private const byte B1 = 1, B2 = 2, B3 = 3, B4 = 4, B5 = 5, B6 = 6, B7 = 7, B8 = 8;

        static readonly int _ds = GraphDefault.HitMargin;
        static readonly int _ds2 = GraphDefault.HitMarginSquared;

        public static (int X, int Y) Move((int x, int y) p, (int dx, int dy) b) => ((p.x + b.dx), (p.y + b.dy));
        public static (int X, int Y) Rotate((int x, int y) p, (int x, int y) b) => ((b.x - (p.y - b.y)), (b.y + (p.x - b.x)));
        public static (int X, int Y) VerticalFlip((int x, int y) p, int y) => ((y + (y - p.y)), (p.x));
        public static (int X, int Y) HorizontalFlip((int x, int y) p, int x) => ((x + (x - p.x)), p.y);
        public static byte Quad((int dx, int dy) p) => (p.dy >= 0) ? ((p.dx <= 0) ? B2 : B1) : (p.dx >= 0) ? B4 : B3;
        public static int Diagonal((int dx, int dy) p) => ((p.dx * p.dx) + (p.dy * p.dy));
        public static (double ux, double uy) OrthoginalUnitVector(int dx, int dy)
        {
            var mag = System.Math.Sqrt(dx * dx + dy * dy);
            return (dy / mag, -dx / mag);
        }
        public static (int x, int y) OrthoginalDisplacedPoint(int dx, int dy, int x0, int y0, double ds)
        {
            var (ux, uy) = OrthoginalUnitVector(dx, dy);
            return ((int)(ds * ux + x0), (int)(ds* uy + y0));
        }
        public static double Slope((int dx, int dy) p)
        {
            var dy = (double)p.dy;
            var dx = (double)p.dx;
            if (dx == 0) dx = 1.0;
            var slope = dy / dx;
            return slope;
        }
        public static (byte quad, byte sect, double slope) QuadSectSlope((int dx, int dy) p)
        {
            var quad = Quad(p);
            var slope = XYPair.Slope(p);

            switch (quad)
            {
                case 1: return (slope > 1) ? (quad, B2, slope) : (quad, B1, slope);
                case 2: return (slope < -1) ? (quad, B3, slope) : (quad, B4, slope);
                case 3: return (slope > 1) ? (quad, B6, slope) : (quad, B5, slope);
                case 4: return (slope < -1) ? (quad, B7, slope) : (quad, B8, slope);
                default: return (0, 0, 0);
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
