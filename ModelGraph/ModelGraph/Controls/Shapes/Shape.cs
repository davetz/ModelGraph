using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        internal Shape() { }
        internal Shape(int I, byte[] data) { ReadData(I, data); }

        #region Properties  ===================================================
        internal enum Coloring { Gray, Light, Normal};
        internal double MajorAxis { get { return R2; } set { R2 = (byte)value; CreatePoints(); } }
        internal double MinorAxis { get { return R1; } set { R1 = (byte)value; CreatePoints(); } }
        internal double TernaryAxis { get { return R3; } set { R3 = (byte)value; CreatePoints(); } }
        internal PolyDimension Dimension { get { return (PolyDimension)PD; } set { PD = (byte)value; CreatePoints(); } }

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
        internal Color GetColor(Coloring c) => (c == Coloring.Normal) ? Color : (c == Coloring.Light) ? Color.FromArgb(0x60, R, G, B) : Color.FromArgb(0x60, 0X80, 0X80, 0X80);

        protected Vector2 Radius => new Vector2(R1, R2);
        protected (float r1, float r2, float r3) GetRadius(float scale) => (R1 * scale, R2 * scale, R3 * scale);
        protected (float r1, float r2) GetRadius() => (R1, R2);

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
        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal);

        protected abstract (float dx1, float dy1, float dx2, float dy2) GetExtent();
        protected abstract void Scale(Vector2 scale);

        #endregion

        #region StaticMethods  ================================================

        #region Flip/Rotate  ==================================================
        static internal void RotateLeft(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.RotateLeft(); }
        }
        static internal void RotateRight(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.RotateRight(); }
        }
        static internal void VerticalFlip(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.VerticalFlip(); }
        }
        static internal void HorizontalFlip(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes) { shape.HorizontalFlip(); }
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
        const float SIZE = 2.56f; // 1% of the maximum width, height of the shape
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
                    SetCenter(shapes, new Vector2(cdx, cdy));
                }
            }
        }
        internal static void ResizeVertical(IEnumerable<Shape> shapes, float factor)
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
                    SetCenter(shapes, new Vector2(cdx, cdy));
                }
            }
        }
        internal static void ResizeHorizontal(IEnumerable<Shape> shapes, float factor)
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
                    SetCenter(shapes, new Vector2(cdx, cdy));
                }
            }
        }
        internal static void ResizeMajorAxis(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);
            foreach (var shape in shapes)
            {
                shape.R1 = (byte)(factor * SIZE / 2);
                shape.CreatePoints();
            }
            SetCenter(shapes, new Vector2(cdx, cdy));
        }
        internal static void ResizeMinorAxis(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);
            foreach (var shape in shapes)
            {
                shape.R2 = (byte)(factor * SIZE / 2);
                shape.CreatePoints();
            }
            SetCenter(shapes, new Vector2(cdx, cdy));
        }
        internal static void ResizeTernaryAxis(IEnumerable<Shape> shapes, float factor)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);
            foreach (var shape in shapes)
            {
                shape.R3 = (byte)factor;
                shape.CreatePoints();
            }
            SetCenter(shapes, new Vector2(cdx, cdy));
        }
        #endregion

        #region GetSliders  ===================================================
        internal static (float cent, float vert, float horz, float major, float minor, float ternary) GetSliders(IEnumerable<Shape> shapes)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);
            var (r1, r2, r3) = GetMaxRadius(shapes);

            var horz = Limited(dx1, dx2);
            var vert = Limited(dy1, dy2);
            var cent = Larger(vert, horz);
            var major = Factor(r1);
            var minor = Factor(r2);
            var ternary = Factor(r3);

            return (cent, vert, horz, major, minor, ternary);

            float Larger(float p, float q) => (p > q) ? p : q;
            float Limited(float a, float b) => Larger(Factor(a), Factor(b));
            float Factor(float v) => (float)System.Math.Round(100 * ((v < 0) ?  ((v < PMIN) ? 1 : v / PMIN) : ((v > PMAX) ? 1 : v / PMAX)));
        }
        #endregion

        #region DrawTargets  ==================================================
        static internal  void DrawTargets(IEnumerable<Shape> shapes, List<Vector2> targets, CanvasDrawingSession ds, float scale, Vector2 center)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dw, dh) = GetExtent(shapes);
            if (dw + dh > 0)
            {
                var h = dw / SIZE;
                var v = dh / SIZE;
                var s = (h > v) ? h : v;

                DrawTarget(new Vector2(cdx, cdy) * scale + center);

                if (shapes.Count() == 1  && shapes.First() is Polyline polyline)
                {
                    var points = polyline.GetDrawingPoints( center, scale);
                    foreach (var point in points)
                    {
                        DrawTarget(point);
                    }
                }

                void DrawTarget(Vector2 c)
                {
                    targets.Add(c);

                    ds.DrawCircle(c, 7, Colors.White, 3);
                }
            }
        }
        #endregion

        #endregion
    }
}
