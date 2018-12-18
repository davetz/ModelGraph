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
    internal class PolyLine : Shape
    {
        internal enum Profile { Line, PolyLine, Polygon}
        private static (float dx, float dy)[] _defaultPoints1 = { (-50, -30), (50, 30) };
        private static (float dx, float dy)[] _defaultPoints2 = { (-80, -30), (-40, 30), (0, -30), (40, -30), (80, 80) };
        private static (float dx, float dy)[] _defaultPoints3 = { (-50, -50), (-90, -10), (90, 90), (50, 30) };

        private List<(float dx, float dy)> _points = new List<(float dx, float dy)> ();
        private Profile _profile;

        internal PolyLine(Vector2 delta, Profile profile = Profile.PolyLine)
        {
            StrokeWidth = 2;
            _profile = profile;
            switch (profile)
            {
                case Profile.Line:
                    _points.AddRange(_defaultPoints1);
                    break;
                case Profile.PolyLine:
                    _points.AddRange(_defaultPoints2);

                    break;
                case Profile.Polygon:
                    _points.AddRange(_defaultPoints3);
                    break;
            }
            Move(delta);
        }
        private PolyLine(Profile profile, (float dx, float dy)[] points)
        {
            _profile = profile;
            _points.AddRange(points);
        }

        #region OverrideAbstract  =============================================
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
        internal override Shape Clone() => CopyToClone(new PolyLine(_profile, _points.ToArray()));

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var transformedPoints = from p in _points
                                    select new Vector2(p.dx, p.dy) * scale + center;

            var tp = transformedPoints.ToArray();

            if (_profile == Profile.Polygon)
            {
                using (var geo = CanvasGeometry.CreatePolygon(cc, tp) )
                {
                    ds.DrawGeometry(geo, Color, strokeWidth, StrokeStyle());
                }
            }
            else
            {
                using (var pb = new CanvasPathBuilder(cc))
                {
                    pb.BeginFigure(tp[0]);
                    for (int i = 1; i < _points.Count; i++)
                    {
                        pb.AddLine(tp[i]);
                    }
                    pb.EndFigure(CanvasFigureLoop.Open);

                    using (var geo = CanvasGeometry.CreatePath(pb))
                    {
                        ds.DrawGeometry(geo, Color, strokeWidth, StrokeStyle());
                    }
                }
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
