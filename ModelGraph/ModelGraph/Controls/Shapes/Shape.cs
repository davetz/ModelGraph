﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
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

        #region Abstract/Virtual  =============================================
        internal abstract Shape Clone();
        internal abstract Shape Clone(Vector2 Center);

        internal abstract void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal);

        protected abstract (float dx1, float dy1, float dx2, float dy2) GetExtent();
        protected abstract void Scale(Vector2 scale);

        protected virtual void CreatePoints() { }
        protected virtual (int min, int max) MinMaxDimension => (1, 100);
        #endregion

        #region Flip/Rotate  ==================================================
        static internal void RotateLeft(IEnumerable<Shape> shapes, bool useAlternate = false)
        {
            foreach (var shape in shapes) { shape.RotateLeft(useAlternate); }
        }
        static internal void RotateRight(IEnumerable<Shape> shapes, bool useAlternate = false)
        {
            foreach (var shape in shapes) { shape.RotateRight(useAlternate); }
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
                shape.F1 = (byte)factor;
                shape.CreatePoints();
            }
            SetCenter(shapes, new Vector2(cdx, cdy));
        }
        internal static void SetDimension(IEnumerable<Shape> shapes, float pd)
        {
            var (min, max, dim) = GetDimension(shapes);
            if (pd < min) pd = min;
            if (pd > max) pd = max;
            foreach (var shape in shapes)
            {
                shape.PD = (byte)pd;
                shape.CreatePoints();
            }
        }
        #endregion

        #region GetSliders  ===================================================
        internal static (float min, float max, float dim, float aux, float major, float minor, float cent, float vert, float horz) GetSliders(IEnumerable<Shape> shapes)
        {
            var (dx1, dy1, dx2, dy2, cdx, cdy, dx, dy) = GetExtent(shapes);
            var (r1, r2, r3) = GetMaxRadius(shapes);
            var (min, max, dim) = GetDimension(shapes);

            var horz = Limited(dx1, dx2);
            var vert = Limited(dy1, dy2);
            var cent = Larger(vert, horz);
            var major = Factor(r1);
            var minor = Factor(r2);
            var aux = Factor(r3);
            return (min, max, dim, aux, major, minor, cent, vert, horz);

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

                DrawTarget(new Vector2(cdx, cdy) * scale + center, true);

                if (shapes.Count() == 1  && shapes.First() is Polyline polyline)
                {
                    var points = polyline.GetDrawingPoints( center, scale);
                    foreach (var point in points)
                    {
                        DrawTarget(point);
                    }
                }

                void DrawTarget(Vector2 c, bool highlight = false)
                {
                    targets.Add(c);

                    ds.DrawCircle(c, 7, Colors.White, 3);
                    if  (highlight)
                        ds.DrawCircle(c, 9, Colors.Red, 3);
                }
            }
        }
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
    }
}
