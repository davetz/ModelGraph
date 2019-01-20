using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolySpline : Polyline
    {
        internal PolySpline()
        {
            Radius1 = 60;
            Radius2 = 60;
            Dimension = 6;
            CreatePoints();
        }
        internal PolySpline(int I, byte[] data) : base(I, data) { }

        #region PrivateConstructor  ===========================================
        private PolySpline(Shape shape)
        {
            CopyData(shape);
        }
        private PolySpline(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region CreatePoints  =================================================
        private enum PS { L1, L2, L3, L4}
        protected override void CreatePoints()
        {
            var D = Dimension;
            var (r1, r2, f1) = GetRadius();

            var n = 0;
            var N = 1 + D * 2; // number of points per spline
            DXY = new List<(float dx, float dy)>(N);
            float dx = -r1, dy = r2, adx, bdx, cdx;
           
            var ps = (D < 7) ? PS.L1 : (D < 11) ? PS.L2 : (D < 15) ? PS.L3 : PS.L4;
            switch (ps)
            {
                case PS.L1:
                    SetDelta(2 * r1 / 6);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(adx, -dy)) break;  // 3
                    break;

                case PS.L2:
                    SetDelta(2 * r1 / 9);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(adx, -dy)) break;  // 5
                    break;
                case PS.L3:
                    SetDelta(2 * r1 / 12);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(cdx, -dy)) break;  // 5
                    if (AddLobe(bdx, dy)) break;   // 6
                    if (AddLobe(adx, -dy)) break;  // 7
                    break;

                case PS.L4:
                    SetDelta(2 * r1 / 14);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(cdx, -dy)) break;  // 5
                    if (AddLobe(bdx, dy)) break;   // 6
                    if (AddLobe(cdx, -dy)) break;  // 7
                    if (AddLobe(bdx, dy)) break;   // 8
                    if (AddLobe(adx, -dy)) break;  // 9
                    break;
            }
            TransformPoints(Matrix3x2.CreateRotation(RadiansStart));

            void SetDelta(float ds)
            {
                adx = ds * 2;
                bdx = ds * -1;
                cdx = ds * 2.5f;
                Add(dx, 0);
            }
            bool AddLobe(float tdx, float tdy)
            {
                if (Add(dx, tdy)) return true;
                dx += tdx;
                if(Add(dx, tdy)) return true;
                dx += tdx;
                if (Add(dx, tdy)) return true;
                if (Add(dx, 0)) return true;
                return false;
            }
            bool Add(float x, float y)
            {
                DXY.Add(Limit(x, y));
                return (++n >= N); 
            }
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() => new PolySpline(this);
        internal override Shape Clone(Vector2 center) => new PolySpline(this, center);
        protected override (int min, int max) MinMaxDimension => (1, 18);

        internal override void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var points = GetDrawingPoints(center, scale);

            using (var pb = new CanvasPathBuilder(ctl))
            {
                pb.BeginFigure(points[0]);
                var N = DXY.Count;
                for (var i = 0; i < N - 2;)
                {
                    pb.AddCubicBezier(points[i], points[++i], points[++i]);
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
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Minor | HasSlider.Major | HasSlider.Dim;
        #endregion
    }
}
