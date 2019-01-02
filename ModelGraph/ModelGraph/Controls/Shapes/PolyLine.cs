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
    //internal class Polyline : Shape
    //{
    //    internal enum Profile { Line, PolyLine, Polygon}
    //    private static (float dx, float dy)[] _defaultPoints1 = { (-50, -0), (50, 0) };
    //    private static (float dx, float dy)[] _defaultPoints2 = { (-100, -0), (-75, -0), (-50, -40), (-25, 40), (-0, -40), (25, 40), (50, -40), (75, 0), (100, 0) };
    //    private static (float dx, float dy)[] _defaultPoints3 = { (-50, 50), (-50, -50), (50, -50), (50, 50) };

    //    private List<(float dx, float dy)> _points = new List<(float dx, float dy)> ();
    //    private Profile _profile;

    //    internal Polyline(Vector2 delta, Profile profile = Profile.PolyLine)
    //    {
    //        StrokeWidth = 2;
    //        _profile = profile;
    //        switch (profile)
    //        {
    //            case Profile.Line:
    //                _points.AddRange(_defaultPoints1);
    //                break;
    //            case Profile.PolyLine:
    //                _points.AddRange(_defaultPoints2);

    //                break;
    //            case Profile.Polygon:
    //                _points.AddRange(_defaultPoints3);
    //                break;
    //        }
    //        Move(delta.X, delta.Y);
    //    }
    //    private Polyline(Profile profile, (float dx, float dy)[] points)
    //    {
    //        _profile = profile;
    //        _points.AddRange(points);
    //    }

    //    #region OverrideAbstract  =============================================
    //    internal override Func<Vector2, Shape> CreateShape => (delta) => new Polyline(delta);

    //    internal override Shape Clone() => (new Polyline(_profile, _points.ToArray()));

    //    internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
    //    {
    //        var transformedPoints = from p in _points
    //                                select new Vector2(p.dx, p.dy) * scale + center;

    //        var tp = transformedPoints.ToArray();

    //        if (_profile == Profile.Polygon)
    //        {
    //            using (var geo = CanvasGeometry.CreatePolygon(cc, tp) )
    //            {
    //                ds.DrawGeometry(geo, Color, strokeWidth, StrokeStyle());
    //            }
    //        }
    //        else
    //        {
    //            using (var pb = new CanvasPathBuilder(cc))
    //            {
    //                pb.BeginFigure(tp[0]);
    //                for (int i = 1; i < _points.Count; i++)
    //                {
    //                    pb.AddLine(tp[i]);
    //                }
    //                pb.EndFigure(CanvasFigureLoop.Open);

    //                using (var geo = CanvasGeometry.CreatePath(pb))
    //                {
    //                    ds.DrawGeometry(geo, Color, strokeWidth, StrokeStyle());
    //                }
    //            }
    //        }
    //    }
    //    internal override void GetPoints(List<(float dx, float dy)> points)
    //    {
    //        points.AddRange(_points);
    //    }
    //    internal override void SetPoints(List<(float dx, float dy)> points)
    //    {
    //        _points.Clear();
    //        _points.AddRange(points);
    //    }

    //    internal override void Move(float dx, float dy)
    //    {
    //        var N = _points.Count;
    //        for (int i = 0; i < N; i++)
    //        {
    //            var (tx, ty) = _points[i];
    //            _points[i] = (tx + dx, ty + dy);
    //        }
    //    }

    //    internal override void Scale(float dx, float dy)
    //    {
    //        var N = _points.Count;
    //        for (int i = 0; i < N; i++)
    //        {
    //            var (tx, ty) = _points[i];
    //            _points[i] = (ScaleTD(tx, dx), ScaleTD(ty, dy));
    //        }
    //    }
    //    #endregion
    //}
}
