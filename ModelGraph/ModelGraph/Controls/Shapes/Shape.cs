using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        internal Shape() { }
        internal Shape(int I, byte[] data) { ReadData(I, data); }

        #region Enums  ========================================================
        internal enum ShapeType : byte
        {
            Line = 1,
            Circle = 3,
            Ellipse = 4,
            PolySide = 5,
            PolyStar = 6,
            PolyGear = 7,
            Polyline = 8,
            Rectangle = 7,
            RoundedRectangle = 8,
            InvalidShapeType = 9,
        }
        #endregion

        #region Properties  ===================================================
        internal ShapeDimension Dimension { get { return (ShapeDimension)P3; } set { P3 = (byte)value; CreatePoints(); } }

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

        public float StrokeWidth { get { return SW; } set { SW = (byte)((value < 1) ? 1 : (value > 20) ? 20 : value); } }
        public Color Color => Color.FromArgb(A, R, G, B);
        public string ColorCode { get { return $"#{A}{R}{G}{B}"; } set { SetColor(value); } }

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

        #endregion

        #region HighLight  ====================================================
        internal static void HighLight(CanvasDrawingSession ds, float width, int index)
        {
            var hw = width / 2;
            var y1 = index * width;
            var y2 = y1 + width;
            ds.DrawLine(hw, y1, hw, y2, Colors.SlateBlue, width);
        }
        #endregion

        #region RequiredMethods  ==============================================
        protected virtual void CreatePoints() { }
        internal abstract Shape Clone();
        internal abstract Shape Clone(Vector2 Center);
        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth);

        protected abstract void Rotate(float radians, Vector2 center);
        protected abstract void Scale(Vector2 scale);

        protected abstract (float dx1, float dy1, float dx2, float dy2) GetExtent();
        #endregion

        #region HelperMethods  ================================================
        static protected float PMIN = sbyte.MinValue;
        static protected float PMAX = sbyte.MaxValue;
        static private int LIM1(float v) => (int)Math.Round(v);
        static private sbyte LIM2(int v) => (v < sbyte.MinValue) ? sbyte.MinValue : (v > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)v;
        static protected (sbyte dx, sbyte dy) Round(float x, float y) => (LIM2(LIM1(x)), LIM2(LIM1(y)));
        static protected (sbyte dx, sbyte dy) Round((float x, float y) p) => Round(p.x, p.y);

        static private (float dx1, float dy1, float dx2, float dy2, float cdx, float cdy, float dx, float dy) GetExtent(IEnumerable<Shape> shapes)
        {
            var x1 = PMAX;
            var y1 = PMAX;
            var x2 = PMIN;
            var y2 = PMIN;

            foreach (var shape in shapes)
            {
                var (dx1, dy1, dx2, dy2) = shape.GetExtent();

                if (dx1 < x1) x1 = dx1;
                if (dy1 < y1) y1 = dy1;

                if (dx2 > x2) x2 = dx2;
                if (dy2 > y2) y2 = dy2;
            }
            return (x1 == PMAX) ? (0, 0, 0, 0, 0, 0, 0, 0) : (x1, y1, x2, y2, (x1 + x2) / 2, (y1 + y2) / 2, (x2 - x1), (y2 - y1));
        }
        internal static float DegreesToRadians(float angle) => angle * (float)Math.PI / 180;

        private void MoveCenter(float dx, float dy)
        {
            for (int i = 0; i < DXY.Count; i++)
            {
                var (tx, ty) = DXY[i];
                DXY[i] = Round(tx + dx, ty + dy);
            }
        }
        #endregion

        #region StaticMethods  ================================================

        #region Flip/Rotate  ==================================================
        static internal void RotateLeft(IEnumerable<Shape> shapes)
        {
            var radians = DegreesToRadians(22.5f);
            foreach (var shape in shapes) { shape.Rotate(radians, Vector2.Zero); }
        }
        static internal void RotateRight(IEnumerable<Shape> shapes)
        {
            var radians = DegreesToRadians(-22.5f);
            foreach (var shape in shapes) { shape.Rotate(radians, Vector2.Zero); }
        }
        static internal void VerticalFlip(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.Scale(new Vector2(1, -1)); }
        }
        static internal void HorizontalFlip(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.Scale(new Vector2(-1, 1)); }
        }
        #endregion

        #region SetCenter  ====================================================
        static internal void SetCenter(IEnumerable<Shape> shapes, Vector2 cp)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);

            if (dx + dy > 0)
            {
                var ex = cp.X - cdx;
                var ey = cp.Y - cdy;

                foreach (var shape in shapes) { shape.MoveCenter(ex, ey); }
            }
        }
        #endregion

        #region MoveCenter  ===================================================
        static internal void MoveCenter(IEnumerable<Shape> shapes, Vector2 ds)
        {
            foreach (var shape in shapes) { shape.MoveCenter(ds.X, ds.Y); }
        }
        #endregion

        #region Resize  =======================================================
        const float SIZE = 2.56f;
        internal static void ResizeCentral(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);

            if (dx + dy > 0)
            {
                var actualSize = (dx > dy) ? dx : dy;
                var desiredSize = SIZE * factor;
                var ratio = desiredSize / actualSize;
                var scale = new Vector2(ratio, ratio);
                foreach (var shape in shapes)
                {
                    shape.Scale(scale);
                }
            }
        }
        internal static void ResizeRadius1(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);

            if (dx + dy > 0)
            {
                var actualSize = dx2 - dx1;
                var desiredSize = SIZE * factor;
                var ratio = desiredSize / actualSize;
                var scale = new Vector2(ratio, 1);
                foreach (var shape in shapes)
                {
                    shape.Scale(scale);
                }
            }
        }
        internal static void ResizeRadius2(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);

            if (dx + dy > 0)
            {

                var actualSize = dy2 - dy1;
                var desiredSize = SIZE * factor;
                var ratio = desiredSize / actualSize;
                var scale = new Vector2(1, ratio);
                foreach (var shape in shapes)
                {
                    shape.Scale(scale);
                }
            }
        }
        #endregion

        #region DrawTargets  ==================================================
        static internal (float Cent, float Vert, float Horz) DrawTargets(IEnumerable<Shape> shapes, bool recordTargets, bool recordPointTargets, List<Vector2> targets, CanvasDrawingSession ds, float scale, Vector2 center)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dw, dh) = GetExtent(shapes);
            if (dw + dh > 0)
            {
                if (recordTargets) targets.Clear();

                var h = dw / SIZE;
                var v = dh / SIZE;
                var s = (h > v) ? h : v;


                DrawTarget(new Vector2(cdx, cdy) * scale + center);

                if (recordPointTargets && recordTargets  && shapes.First() is Polyline polyline)
                {
                    var points = polyline.GetDrawingPoints( center, scale);
                    foreach (var point in points)
                    {
                        DrawTarget(point);
                    }
                }
                return (s, v, h);

                void DrawTarget(Vector2 c)
                {
                    if (recordTargets) targets.Add(c);

                    ds.DrawCircle(c, 5, Colors.White, 2);
                    ds.DrawCircle(c, 7, Colors.Black, 2);
                }
            }
            return (1, 1, 1);
        }
        private static Vector2 dsx = new Vector2(2, 0);
        private static Vector2 dsy = new Vector2(0, 2);
        #endregion

        #endregion
    }
}
