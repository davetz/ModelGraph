using System.Numerics;

namespace ModelGraphLibrary
{
    public struct XYPoint
    {
        internal int X;
        internal int Y;

        static int _ds = GraphParm.HitMargin;
        static int _ds2 = GraphParm.HitMarginSquared;

        internal XYPoint(int x, int y) { X = x; Y = y; }

        internal XYPoint(Vector2 val) { X = (int)val.X; Y = (int)val.X; }

        internal XYPoint(double x, double y) { X = (int)x; Y = (int)y; }

        internal Vector2 Vector2 { get { return new Vector2(X, Y); } }

        internal Vector2 GetVector2(int x, int y, float z)
        {
            return new Vector2(z * (X - x), z * (Y - y));
        }

        internal XYPoint Move(XYPoint delta)
        {
            X += delta.X;
            Y += delta.Y;
            return new XYPoint(X, Y);
        }

        internal XYPoint Rotate(int x, int y)
        {
            var px = X;
            var py = Y;
            X = x - (py - y);
            Y = (y + (px - x));
            return new XYPoint(X, Y);
        }

        internal XYPoint VerticalFlip(int y)
        {
            Y = y + (y - Y);
            return new XYPoint(X, Y);
        }

        internal XYPoint HorizontalFlip(int x)
        {
            X = x + (x - X);
            return new XYPoint(X, Y);
        }


        internal bool HitTest(XYPoint p)
        {
            var dx = X - p.X;
            if (dx * dx < _ds2) return true;

            var dy = Y - p.Y;
            if (dy * dy < _ds2) return true;

            return false;
        }

        #region Extent.Delta.Helpers  =========================================
        // assume (X,Y) represents (dx,dy) from Extent.Delta
        internal int Diagonal { get { return (X * X) + (Y * Y); } }
        internal float Slope { get { float d = (X == 0) ? 0.4f : X; return (Y / d); } }
        internal int Quad { get { return (X >= 0) ? ((Y >= 0) ? (1) : (4)) : ((Y >= 0) ? (2) : (3)); } }
        #endregion
    }
}
