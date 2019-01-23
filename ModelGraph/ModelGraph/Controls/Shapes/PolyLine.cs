using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Polyline : Shape
    {
        private static (float, float)[] POINTS_2 = { (-0.25f, 0), (0.25f, 0) };
        private static (float, float)[] POINTS_4 = { (-0.25f, 0), (0, 0.25f), (0.25f, 0), };
        private static (float, float)[][] POINTS = { POINTS_2, POINTS_4 };

        internal Polyline() { }
        internal Polyline(int n)
        {
            DXY = new List<(float dx, float dy)>(POINTS[n & 1]);
        }
        internal Polyline(int I, byte[] data) : base(I, data) { }

        #region PrivateConstructor  ===========================================
        private Polyline(Shape shape)
        {
            CopyData(shape);
        }
        private Polyline(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter( new Shape[] { this }, center);
        }
        #endregion


        #region Polyline Methods  =============================================
        internal Vector2[] GetDrawingPoints(Vector2 center, float scale)
        {
            var list = new List<Vector2>(DXY.Count);
            foreach (var (dx, dy) in DXY)
            {
                list.Add(new Vector2(dx, dy) * scale + center);
            }
            return list.ToArray();
        }

        internal void MovePoint(int index, Vector2 ds)
        {
            if (index < 0) return;
            if (index < DXY.Count)
            {
                var (dx, dy) = DXY[index];
                DXY[index] = Limit(dx + ds.X, dy + ds.Y);
            }
        }
        internal bool TryDeletePoint(int index)
        {
            if (index < 0) return false;
            if (index < DXY.Count && DXY.Count > 2)
            {
                DXY.RemoveAt(index);
                return true;
            }
            return false;
        }
        internal bool TryAddPoint(int index, Vector2 point)
        {
            if (index < 0) return false;
            if (index < DXY.Count)
            {
                DXY.Insert(index + 1, (point.X, point.Y));
                return true;
            }
            else if (index == DXY.Count)
            {
                DXY.Add((point.X, point.Y));
                return true;
            }
            return false;
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() => new Polyline(this);
        internal override Shape Clone(Vector2 center) => new Polyline(this, center);
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var x1 = 1f;
            var y1 = 1f;
            var x2 = -1f;
            var y2 = -1f;

            foreach (var (dx, dy) in DXY)
            {
                if (dx < x1) x1 = dx;
                if (dy < y1) y1 = dy;

                if (dx > x2) x2 = dx;
                if (dy > y2) y2 = dy;
            }
            return (x1 == 1) ? (0, 0, 0, 0) : (x1, y1, x2, y2);
        }
        protected override void Scale(Vector2 scale) => TransformPoints(Matrix3x2.CreateScale(scale)); 
        internal override void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var points = GetDrawingPoints(center, scale);

            if (points.Length == 2)
            {
                ds.DrawLine(points[0], points[1], color, strokeWidth, StrokeStyle());
            }
            else if (points.Length > 2)
            {
                using (var pb = new CanvasPathBuilder(ctl))
                {
                    pb.BeginFigure(points[0]);
                    for (int i = 1; i < points.Length; i++)
                    {
                        pb.AddLine(points[i]);
                    }
                    pb.EndFigure(CanvasFigureLoop.Open);

                    using (var geo = CanvasGeometry.CreatePath(pb))
                    {
                        if (FillStroke == Fill_Stroke.Filled)
                            ds.FillGeometry(geo, color);
                        else
                            ds.DrawGeometry(geo, color, strokeWidth, StrokeStyle());
                    }
                }
            }
        }
        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, FlipState flip)
        {
            var color = GetColor(Coloring.Normal);
            var points = GetDrawingPoints(flip, scale, center);

            if (points.Length == 2)
            {
                ds.DrawLine(points[0], points[1], color, StrokeWidth, StrokeStyle());
            }
            else if (points.Length > 2)
            {
                using (var pb = new CanvasPathBuilder(cc))
                {
                    pb.BeginFigure(points[0]);
                    for (int i = 1; i < points.Length; i++)
                    {
                        pb.AddLine(points[i]);
                    }
                    pb.EndFigure(CanvasFigureLoop.Open);

                    using (var geo = CanvasGeometry.CreatePath(pb))
                    {
                        if (FillStroke == Fill_Stroke.Filled)
                            ds.FillGeometry(geo, color);
                        else
                            ds.DrawGeometry(geo, color, StrokeWidth, StrokeStyle());
                    }
                }
            }
        }
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert;
        protected override (int min, int max) MinMaxDimension => (1, 18);
        #endregion
    }
}
