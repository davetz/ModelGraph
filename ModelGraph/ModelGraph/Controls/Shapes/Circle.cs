using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ModelGraph.Controls
{
    internal class Circle : Shape
    {
        private float _dx;
        private float _dy;
        private float _radius = 30;

        internal Circle(int I, byte[] data) : base(I, data)
        {
        }

        internal Circle()
        {
        }
        private Circle(float dx, float dy, float radius)
        {
            _dx = dx;
            _dy = dy;
            _radius = radius;
        }

        #region OverideAbstract  ==============================================
        internal override Func<Vector2, Shape> CreateShape => (delta) => new Circle(delta);
        internal override Shape Clone() => CopyToClone(new Circle(_dx, _dy, _radius));

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            ds.DrawCircle(new Vector2(_dx, _dy) * scale + center, _radius * scale, Color, strokeWidth);
        }

        internal override void GetPoints(List<(float dx, float dy)> points)
        {
            points.Add((_dx - _radius, _dy - _radius));
            points.Add((_dx + _radius, _dy + _radius));
        }
        internal override void SetPoints(List<(float dx, float dy)> points)
        {
            if (points.Count > 0)
            {
                var (dx, dy) = points[0];
                _dx = dx;
                _dy = dy;
            }
        }

        internal override void Move(float dx, float dy)
        {
            _dx += dx;
            _dy += dy;
        }

        internal override void Scale(float dx, float dy)
        {
            _radius += (dx < dy) ? dx : dy;
        }
        #endregion
    }
}
