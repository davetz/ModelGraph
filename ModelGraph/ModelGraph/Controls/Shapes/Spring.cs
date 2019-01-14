using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraph.Controls
{
    internal enum SpringType
    {
        L1 = 1,
        L2 = 2,
        L3 = 3,
        L4 = 4,
    }   
    internal class Spring : Polyline
    {
        internal Spring()
        {
            R1 = 60;
            R2 = 60;
            PD = (byte)SpringType.L2;
            CreatePoints();
        }
        internal Spring(int I, byte[] data) : base(I, data) { }

        #region PrivateConstructor  ===========================================
        private Spring(Shape shape)
        {
            CopyData(shape);
        }
        private Spring(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        protected override void CreatePoints()
        {
            var N = 1 + PD * 12; // number of points per loop
            DXY = new List<(sbyte dx, sbyte dy)>(N);
            float dx = -R1, dy = R2, adx, bdx, cdx;

            switch ((SpringType)PD)
            {
                case SpringType.L1:
                    SetDelta(2 * R1 / 6);

                    AddHalfLoop(adx, -dy);  // 1
                    AddHalfLoop(bdx, dy);   // 2
                    AddHalfLoop(adx, -dy);  // 3
                    break;

                case SpringType.L2:
                    SetDelta(2 * R1 / 9);
                    
                    AddHalfLoop(adx, -dy);  // 1
                    AddHalfLoop(bdx, dy);   // 2
                    AddHalfLoop(cdx, -dy);  // 3
                    AddHalfLoop(bdx, dy);   // 4
                    AddHalfLoop(adx, -dy);  // 5
                    break;
                case SpringType.L3:
                    SetDelta(2 * R1 / 12);

                    AddHalfLoop(adx, -dy);  // 1
                    AddHalfLoop(bdx, dy);   // 2
                    AddHalfLoop(cdx, -dy);  // 3
                    AddHalfLoop(bdx, dy);   // 4
                    AddHalfLoop(cdx, -dy);  // 5
                    AddHalfLoop(bdx, dy);   // 6
                    AddHalfLoop(adx, -dy);  // 7
                    break;

                case SpringType.L4:
                    SetDelta(2 * R1 / 14);

                    AddHalfLoop(adx, -dy);  // 1
                    AddHalfLoop(bdx, dy);   // 2
                    AddHalfLoop(cdx, -dy);  // 3
                    AddHalfLoop(bdx, dy);   // 4
                    AddHalfLoop(cdx, -dy);  // 5
                    AddHalfLoop(bdx, dy);   // 6
                    AddHalfLoop(cdx, -dy);  // 7
                    AddHalfLoop(bdx, dy);   // 8
                    AddHalfLoop(adx, -dy);  // 9
                    break;
            }
            void SetDelta(float ds)
            {
                adx = ds * 2;
                bdx = ds * -1;
                cdx = ds * 3.5f;
                DXY.Add(Round(dx, 0));
            }
            void AddHalfLoop(float tdx, float tdy)
            {
                DXY.Add(Round(dx, tdy));
                dx += tdx;
                DXY.Add(Round(dx, tdy));
                dx += tdx;
                DXY.Add(Round(dx, tdy));
                DXY.Add(Round(dx, 0));
            }
        }


        #region OverideAbstract  ==============================================
        internal override Shape Clone() => new Spring(this);
        internal override Shape Clone(Vector2 center) => new Spring(this, center);
        internal override void Draw(CanvasControl ctl, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var points = GetDrawingPoints(center, scale);
            var (r1, r2, r3) = GetRadius(scale);

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
                    ds.DrawGeometry(geo, color, strokeWidth, StrokeStyle());
                }
            }
        }
        private static float K1 = FullRadians / 4;
        private static float K2 = FullRadians * 0.9f;
        private static float K3 = FullRadians / 4;

        #endregion


    }
}
