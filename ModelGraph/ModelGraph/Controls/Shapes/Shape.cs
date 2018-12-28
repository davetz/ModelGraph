using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        internal const float FULLSIZE = 200;    // full size of the symbol's graphic definition
        internal const float HALFSIZE = 100;    // half size (radius) of the symbol's graphic definition
        internal Shape() { }
        internal Shape(int I, byte[] data) { ReadData(I, data); }

        #region Enums  ========================================================
        internal enum ShapeType : byte
        {
            Arc = 0,
            Line = 1,
            Spline = 2,
            Circle = 3,
            Ellipse = 4,
            Polygon = 5,
            Polyline = 6,
            ClosedPolyline = 7,
            RoundedRectangle = 8,
            InvalidShapeType = 9,
        }
        #endregion

        #region SerializedData  ===============================================
        private const int HeaderPointCountIndex = 16;
        protected byte ST = 0;  // shapte type code
        protected byte A = 255;
        protected byte R = 255;
        protected byte G = 255;
        protected byte B = 255;
        protected byte SW = 1;  // stroke width
        protected byte SC = 0;  // startCap
        protected byte EC = 0;  // endCap
        protected byte DC = 0;  // dashCap
        protected byte LJ = 3;  // line join
        protected byte DS = 0;  // dash style
        protected byte FS = 0;  // fill stroke
        protected byte R1 = 0;  // major axis radius
        protected byte R2 = 0;  // minor axis radius
        protected byte PS = 3;  // number of polygon sides
        protected byte RF = 0;  // rotate flip state code
        protected List<(sbyte dx, sbyte dy)> DXY;  // zero or more points
        //=====================================================================
        //working buffer flip rotate drawing points
        internal List<Vector2> Points = new List<Vector2>();
        #endregion

        #region CommonProperties  =============================================
        internal bool IsSelected;

        public CanvasStrokeStyle StrokeStyle()
        {            
            var ss = _strokeStyle;
            ss.DashStyle = DashStyle;
            ss.StartCap = StartCap;
            ss.EndCap = EndCap;
            ss.DashCap = DashCap;
            ss.LineJoin = LineJoin; ;
            return ss;
        }
        private CanvasStrokeStyle _strokeStyle = new CanvasStrokeStyle();

        public CanvasDashStyle DashStyle { get { return (CanvasDashStyle)DS; } set { DS = (byte)value; } }
        public CanvasCapStyle StartCap { get { return (CanvasCapStyle)SC; } set { SC = (byte)value; } }
        public CanvasCapStyle EndCap { get { return (CanvasCapStyle)EC; } set { EC = (byte)value; } }
        public CanvasLineJoin LineJoin { get { return (CanvasLineJoin)LJ; } set { LJ = (byte)value; } }
        public CanvasCapStyle DashCap { get { return (CanvasCapStyle)DC; } set { DC = (byte)value; } }

        public  Fill_Stroke FillStroke { get { return (Fill_Stroke)FS; } set { FS = (byte)value; } }
        public PolygonSides PolygonSide { get { return (PolygonSides)PS; } set { PS = (byte)value; } }

        public float StrokeWidth { get { return SW; } set { SW = (byte)((value < 1) ? 1 : (value > 20) ? 20 : value); } }
        public Color Color => Color.FromArgb(A, R, G, B);
        public string ColorCode { get { return $"#{A}{R}{G}{B}"; } set { SetColor(value); } }
        #endregion

        #region SetColor  =====================================================
        private void SetColor(string code)
        {
            var (a, r, g, b) = GetARGB(code);
            A = a;
            R = r;
            G = g;
            B = b;
        }

        private static (byte, byte, byte, byte) GetARGB(string argbStr)
        {
            if (IsInvalid(argbStr)) return _invalidColor;
            var argb = _invalidColor; // default color when there is a bad color string

            var ca = argbStr.ToLower().ToCharArray();
            if (ca[0] != '#') return _invalidColor;

            var N = _argbLength;
            int[] va = new int[N];
            for (int j = 1; j < N; j++)
            {
                va[j] = _hexValues.IndexOf(ca[j]);
                if (va[j] < 0) return _invalidColor;
            }
            return ((byte)((va[1] << 4) | va[2]), (byte)((va[3] << 4) | va[4]), (byte)((va[5] << 4) | va[6]), (byte)((va[7] << 4) | va[8]));
        }
        static bool IsValid(string argbStr) => !IsInvalid(argbStr);
        static bool IsInvalid(string argbStr) => (string.IsNullOrWhiteSpace(argbStr) || argbStr.Length != _argbLength);
        static readonly (byte, byte, byte, byte) _invalidColor = (0x88, 0x87, 0x86, 0x85);
        static readonly string _hexValues = "0123456789abcdef";
        const int _argbLength = 9;
        #endregion

        #region CopyToClone  ==================================================
        protected Shape CopyToClone(Shape clone)
        {
            clone.A = A;
            clone.R = R;
            clone.G = G;
            clone.B = B;
            clone.SW = SW;
            clone.SC = SC;
            clone.EC = EC;
            clone.DC = DC;
            clone.LJ = LJ;
            clone.DS = DS;
            return clone;
        }
        #endregion

        #region HighLight  ====================================================
        internal static void HighLight(CanvasDrawingSession ds, float width, int index)
        {
            var hw = width / 2;
            var y1 = index * width;
            var y2 = y1 + width;
            ds.DrawLine(hw, y1, hw, y2, Colors.SlateGray, width);
        }
        #endregion

        #region GetCenter  ====================================================
        internal static (float dx, float dy) GetCenter(List<(float dx, float dy)> points)
        {
            var N = points.Count;
            if (N == 0) return (0, 0);

            float dxmin, dxmax, dymin, dymax;

            dxmin = dxmax = points[0].dx;
            dymin = dymax = points[0].dy;
            for (int i = 1; i < N; i++)
            {
                var (dx, dy) = points[i];
                if (dx < dxmin) dxmin = dx;
                if (dy < dymin) dymin = dy;

                if (dx > dxmax) dxmax = dx;
                if (dy > dymax) dymax = dy;
            }
            return ((dxmax + dxmin) / 2, (dymax + dymin) / 2);
        }
        internal static (float dx1, float dy1, float dx2, float dy2, float cdx, float cdy)  GetExtentCenter(IEnumerable<Shape> shapes, List<(float dx, float dy)> points)
        {
            float dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

            points.Clear();
            foreach (var shape in shapes)
            {
                shape.GetPoints(points);
            }
            foreach (var (tx, ty) in points)
            {
                if (tx < dx1) dx1 = tx;
                if (ty < dy1) dy1 = ty;

                if (tx > dx2) dx2 = tx;
                if (ty > dy2) dy2 = ty;
            }
            return (dx1, dy1, dx2, dy2, (dx1 + dx2) / 2, (dy1 + dy2) / 2);
        }
        internal (float dx, float dy, float cdx, float cdy) ValidateMove(float dx, float dy, IEnumerable<Shape> shapes, List<(float dx, float dy)> points)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy) = GetExtentCenter(shapes, points);

            while ((int)dx != 0)
            {
                if (dx < 0)
                {
                    var d = dx + 1;
                    if (IsNotValid(MoveTD(dx1, d), MoveTD(dx2, d))) break;
                    dx = d;
                }
                else if (dx > 0)
                {
                    var d = dx - 1;
                    if (IsNotValid(MoveTD(dx1, d), MoveTD(dx2, d))) break;
                    dx = d;
                }
            }
            while ((int)dy != 0)
            {
                if (dy < 0)
                {
                    var d = dy + 1;
                    if (IsNotValid(MoveTD(dy1, d), MoveTD(dy2, d))) break;
                    dy = d;
                }
                else if (dy > 0)
                {
                    var d = dy - 1;
                    if (IsNotValid(MoveTD(dy1, d), MoveTD(dy2, d))) break;
                    dy = d;
                }
            }

            return (dx, dy, cdx, cdy);
        }
        internal static (float dx, float dy, float cdx, float cdy) ValidateScale (float dx, float dy, IEnumerable<Shape> shapes, List<(float dx, float dy)> points)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy) = GetExtentCenter(shapes, points);

            while ((int)dx != 0 && IsNotValid(ScaleTD(dx1, dx), ScaleTD(dx2, dx)))
            {
                if (dx < 0)
                {
                    var d = dx + 1;
                    if (IsNotValid(ScaleTD(dx1, d), ScaleTD(dx2, d))) break;
                    dx = d;
                }
                else if (dx > 0)
                {
                    var d = dx - 1;
                    if (IsNotValid(ScaleTD(dx1, d), ScaleTD(dx2, d))) break;
                    dx = d;
                }
            }
            while ((int)dy != 0 && IsNotValid(ScaleTD(dy1, dy), ScaleTD(dy2, dy)))
            {
                if (dy < 0)
                {
                    var d = dy + 1;
                    if (IsNotValid(ScaleTD(dy1, d), ScaleTD(dy2, d))) break;
                    dy = d;
                }
                else if (dy > 0)
                {
                    var d = dy - 1;
                    if (IsNotValid(ScaleTD(dy1, d), ScaleTD(dy2, d))) break;
                    dy = d;
                }
            }
            return (dx, dy, cdx, cdy);
        }
        protected static bool IsValid(float t1, float t2) => !IsNotValid(t1, t2);
        protected static bool IsNotValid(float t1, float t2) => (t1 < -HALFSIZE) || (t2 > HALFSIZE) || (t2 - t1) < 1;
        protected static float ScaleTD(float t, float d) => t < 0 ? t - d : t + d;
        protected static float MoveTD(float t, float d) => t + d;

        #endregion

        #region RequiredMethods  ==============================================
        internal abstract Shape Clone();
        internal abstract Func<Vector2, Shape> CreateShape { get; }

        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth);

        internal abstract void GetPoints(List<(float dx, float dy)> points);
        internal abstract void SetPoints(List<(float dx, float dy)> points);

        internal abstract void Move(float dx, float dy);
        internal abstract void Scale(float dx, float dy);
        #endregion
    }
}
