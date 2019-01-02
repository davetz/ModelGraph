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

        public Fill_Stroke FillStroke { get { return (Fill_Stroke)FS; } set { FS = (byte)value; } }
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

        #region HighLight  ====================================================
        internal static void HighLight(CanvasDrawingSession ds, float width, int index)
        {
            var hw = width / 2;
            var y1 = index * width;
            var y2 = y1 + width;
            ds.DrawLine(hw, y1, hw, y2, Colors.SlateGray, width);
        }
        #endregion

        #region RequiredMethods  ==============================================
        internal abstract Shape Clone();
        internal abstract Shape Clone(Vector2 Center);
        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth);

        protected abstract void GetVector(List<Vector2> list);
        protected abstract void SetVector(List<Vector2> list);

        protected abstract void GetPoints(List<(float dx, float dy)> list);
        protected abstract void SetPoints(List<(float dx, float dy)> list);

        #endregion

        #region StaticMethods  ================================================
        static float PMIN = sbyte.MinValue;
        static float PMAX = sbyte.MaxValue;
        static private int LIM1(float v) => (int)Math.Round(v);
        static private sbyte LIM2(int v) => (v < sbyte.MinValue) ? sbyte.MinValue : (v > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)v;
        static protected (sbyte dx, sbyte dy) Round(float x, float y) => (LIM2(LIM1(x)), LIM2(LIM1(y)));
        static protected (sbyte dx, sbyte dy) Round((float x, float y) p) => Round(p.x, p.y);

        static private bool GetAllPoints(IEnumerable<Shape> shapes, List<(float dx, float dy)> points)
        {
            foreach (var shape in shapes) { shape.GetPoints(points); }
            return points.Count > 0;
        }
        static private (float dx1, float dy1, float dx2, float dy2, float cdx, float cdy) GetExtent(List<(float dx, float dy)> points)
        {
            if (points.Count == 0) return (0, 0, 0, 0, 0, 0);

            var x1 = PMAX;
            var y1 = PMAX;
            var x2 = PMIN;
            var y2 = PMIN;

            foreach (var (dx, dy) in points)
            {
                if (dx < x1) x1 = dx;
                if (dy < y1) y1 = dy;

                if (dx > x2) x2 = dx;
                if (dy > y2) y2 = dy;
            }
            return (x1, y1, x2, y2, (x1 + x2) / 2, (y1 + y2) / 2);
        }
        static internal void SetCenter(IEnumerable<Shape> shapes, Vector2 cp)
        {
            var points = new List<(float dx, float dy)>();
            if (GetAllPoints(shapes, points))
            {
                var (dx1, dy1, dx2, dy2, cdx, cdy) = GetExtent(points);
                var ex = cp.X - cdx;
                var ey = cp.Y - cdy;

                foreach (var shape in shapes)
                {
                    points.Clear();
                    shape.GetPoints(points);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var (tx, ty) = points[i];
                        points[i] = (tx + ex, ty + ey);
                    }
                    shape.SetPoints(points);
                }
            }
        }
        static internal void MoveCenter(IEnumerable<Shape> shapes, Vector2 dcp)
        {
            var points = new List<(float dx, float dy)>();
            if (GetAllPoints(shapes, points))
            {
                var (dx1, dy1, dx2, dy2, cdx, cdy) = GetExtent(points);
                var ex = dcp.X;
                var ey = dcp.Y;

                foreach (var shape in shapes)
                {
                    points.Clear();
                    shape.GetPoints(points);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var (tx, ty) = points[i];
                        points[i] = (tx + ex, ty + ey);
                    }
                    shape.SetPoints(points);
                }
            }
        }

        #region DrawTargets  ==================================================
        static internal void DrawTargets(IEnumerable<Shape> shapes, CanvasDrawingSession ds, float scale, Vector2 center)
        {
            var points = new List<(float dx, float dy)>();
            if (GetAllPoints(shapes, points))
            {
                var (dx1, dy1, dx2, dy2, cdx, cdy) = GetExtent(points);

                Draw(new Vector2(dx1, dy1) * scale + center, true);
                Draw(new Vector2(dx2, dy2) * scale + center, true);
                Draw(new Vector2(cdx, cdy) * scale + center, true);

                void Draw(Vector2 cp, bool drawHash = false)
                {
                    ds.DrawCircle(cp, 5, Colors.White, 2);
                    ds.DrawCircle(cp, 7, Colors.Black, 2);

                    if (drawHash)
                    {
                        DrawHash(t11, t12, Colors.White);
                        DrawHash(t13, t14, Colors.Black);

                        DrawHash(-t11, -t12, Colors.White);
                        DrawHash(-t13, -t14, Colors.Black);

                        DrawHash(t21, t22, Colors.White);
                        DrawHash(t23, t24, Colors.Black);

                        DrawHash(-t21, -t22, Colors.White);
                        DrawHash(-t23, -t24, Colors.Black);
                    }

                    void DrawHash(Vector2 vt1, Vector2 vt2, Color color)
                    {
                        ds.DrawLine(cp + vt1, cp + vt2, color, 2);
                    }
                }
            }
        }
        private static Vector2 t11 = new Vector2(6, 0);
        private static Vector2 t12 = new Vector2(12, 0);
        private static Vector2 t13 = new Vector2(6, 2);
        private static Vector2 t14 = new Vector2(12, 2);

        private static Vector2 t21 = new Vector2(0, 6);
        private static Vector2 t22 = new Vector2(0, 12);
        private static Vector2 t23 = new Vector2(2, 6);
        private static Vector2 t24 = new Vector2(2, 12);
        #endregion

        #endregion
    }
}
