using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract class Polyline : Shape
    {
        internal Polyline() { }
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
        #endregion

        #region OverideAbstract  ==============================================
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var x1 = PMAX;
            var y1 = PMAX;
            var x2 = PMIN;
            var y2 = PMIN;

            foreach (var (dx, dy) in DXY)
            {
                if (dx < x1) x1 = dx;
                if (dy < y1) y1 = dy;

                if (dx > x2) x2 = dx;
                if (dy > y2) y2 = dy;
            }
            return (x1 == PMAX) ? (0, 0, 0, 0) : (x1, y1, x2, y2);
        }
        protected override void Scale(Vector2 scale) => TransformPoints(Matrix3x2.CreateScale(scale)); 
        internal override void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var points = GetDrawingPoints(center, scale);

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
                    ds.DrawGeometry(geo, color, strokeWidth, StrokeStyle());
                }
            }
        }
        protected override (int min, int max) MinMaxDimension => (1, 18);
        #endregion
    }
}
