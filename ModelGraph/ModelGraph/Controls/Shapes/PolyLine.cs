using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
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
    internal class PolyLine : Shape
    {
        private List<(float dx, float dy)> _points = new List<(float dx, float dy)> { (-50, -30), (50, 30) };

        internal PolyLine()
        {
            Width = 3;
        }
        internal PolyLine(Vector2 delta)
        {
            Width = 3;
            Move(delta);
        }

        #region OverideAbstract  ==============================================
        internal override Func<Vector2, Shape> CreateShapeFunction => (delta) => new PolyLine(delta);
        internal override void Move(Vector2 delta)
        {
            var n = _points.Count;
            var d = ValideDelta(delta);

            for (int i = 0; i<n; i++)
            {
                var (dx, dy) = _points[i];
                _points[i] = (dx + d.X, dy + d.Y);
            }
        }

        internal override CanvasGeometry GetGeometry(ICanvasResourceCreator resourceCreator, float scale, Vector2 center)
        {
            var transformedPoints = from p in _points
                                    select new Vector2(p.dx, p.dy) * scale + center;
            var tp = transformedPoints.ToArray();

            return CanvasGeometry.CreatePolygon(resourceCreator, tp);
        }
        #endregion

        #region ValidateDelta  ================================================
        internal override Vector2 ValideDelta(Vector2 delta)
        {
            var dxmin = HalfSize;
            var dymin = HalfSize;

            var dxmax = -HalfSize;
            var dymax = -HalfSize;

            float dx, dy;
            foreach (var p in _points)
            {
                dx = p.dx;
                if (dx > dxmax) dxmax = dx;
                if (dx < dxmin) dxmin = dx;
                dy = p.dy;
                if (dy > dymax) dymax = dy;
                if (dy < dymin) dymin = dy;
            }
            dxmin = -(HalfSize + dxmin);
            dymin = -(HalfSize + dymin);
            dxmax = (HalfSize - dxmax);
            dymax = (HalfSize - dymax);
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
