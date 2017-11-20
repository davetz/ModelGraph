using System.Numerics;

namespace ModelGraph.Internals
{
    public struct XYPoint
    {
        public int X;
        public int Y;

        static int _ds = GraphParm.HitMargin;
        static int _ds2 = GraphParm.HitMarginSquared;

        public XYPoint(int x, int y) { X = x; Y = y; }

        //public XYPoint(Vector2 val) { X = (int)val.X; Y = (int)val.X; }

        public XYPoint(double x, double y) { X = (int)x; Y = (int)y; }

        //public Vector2 Vector2 { get { return new Vector2(X, Y); } }

        //public Vector2 GetVector2(int x, int y, float z)
        //{
        //    return new Vector2(z * (X - x), z * (Y - y));
        //}

        public XYPoint Move(XYPoint delta)
        {
            X += delta.X;
            Y += delta.Y;
            return new XYPoint(X, Y);
        }

        public XYPoint Rotate(int x, int y)
        {
            var px = X;
            var py = Y;
            X = x - (py - y);
            Y = (y + (px - x));
            return new XYPoint(X, Y);
        }

        public XYPoint VerticalFlip(int y)
        {
            Y = y + (y - Y);
            return new XYPoint(X, Y);
        }

        public XYPoint HorizontalFlip(int x)
        {
            X = x + (x - X);
            return new XYPoint(X, Y);
        }


        public bool HitTest(XYPoint p)
        {
            var dx = X - p.X;
            if (dx * dx < _ds2) return true;

            var dy = Y - p.Y;
            if (dy * dy < _ds2) return true;

            return false;
        }

        #region Extent.Delta.Helpers  =========================================
        // assume (X,Y) represents (dx,dy) from Extent.Delta
        public int Diagonal { get { return (X * X) + (Y * Y); } }
        public float Slope { get { float d = (X == 0) ? 0.4f : X; return (Y / d); } }
        public int Quad { get { return (X >= 0) ? ((Y >= 0) ? (1) : (4)) : ((Y >= 0) ? (2) : (3)); } }
        #endregion
    }
}
