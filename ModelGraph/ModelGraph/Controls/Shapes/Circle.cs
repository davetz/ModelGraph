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

        internal Circle(Vector2 delta)
        {
            StrokeWidth = 2;
            Move(delta);
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
        internal override void Move(Vector2 delta)
        {
            var d = ValideDelta(delta);
            _dx += d.X;
            _dy += d.Y;
        }

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            ds.DrawCircle(new Vector2(_dx, _dy) * scale + center, _radius * scale, Color, strokeWidth);
        }

        internal override void GetPoints(List<(float dx, float dy)> points)
        {
            points.Add((_dx, _dy));
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

        #endregion

        #region ValidateDelta  ================================================
        internal override Vector2 ValideDelta(Vector2 delta)
        {
            var dxmin = HALFSIZE;
            var dymin = HALFSIZE;

            var dxmax = -HALFSIZE;
            var dymax = -HALFSIZE;

            var dx = _radius + _dx;
            var dy = _radius + _dy;

            if (dx > dxmax) dxmax = dx;
            if (dx < dxmin) dxmin = dx;
            if (dy > dymax) dymax = dy;
            if (dy < dymin) dymin = dy;

            dxmin = -(HALFSIZE + dxmin);
            dymin = -(HALFSIZE + dymin);
            dxmax = (HALFSIZE - dxmax);
            dymax = (HALFSIZE - dymax);
            dx = delta.X;
            dy = delta.Y;
            if (dx < dxmin) dx = dxmin;
            if (dx > dxmax) dx = dxmax;
            if (dy < dymin) dy = dymin;
            if (dy > dymax) dy = dymax;
            return new Vector2(dx, dy);
        }
        #endregion
    }
}
