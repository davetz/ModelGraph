using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract class Polyline : Shape
    {
        internal Polyline() { }
        internal Polyline(int I, byte[] data) : base(I, data) { }

        internal  Vector2[] GetDrawingPoints(Vector2 center, float scale)
        {
            var list = new List<Vector2>(DXY.Count);
            foreach (var (dx, dy) in DXY)
            {
                list.Add(new Vector2(dx, dy) * scale + center);
            }
            return list.ToArray();
        }
        protected override void Rotate(float radians, Vector2 center)
        {
            Transform(Matrix3x2.CreateRotation(radians, center));
        }
        protected override void Scale(Vector2 scale)
        {
            Transform(Matrix3x2.CreateScale(scale));
        }
        private void Transform(Matrix3x2 m)
        {
            for (int i = 0; i < DXY.Count; i++)
            {
                var (dx, dy) = DXY[i];
                var p = new Vector2(dx, dy);
                p = Vector2.Transform(p, m);
                DXY[i] = Round(p.X, p.Y);
            }
        }

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
        #endregion
    }
}
