﻿using Microsoft.Graphics.Canvas;
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
    internal class PolySpline : Polyline
    {
        internal PolySpline()
        {
            R1 = 60;
            R2 = 60;
            PD = 6;
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
            if (PD > 18) PD = 18;

            var n = 0;
            var N = 1 + PD * 2; // number of points per spline
            DXY = new List<(float dx, float dy)>(N);
            float dx = -R1, dy = R2, adx, bdx, cdx;
           
            var ps = (PD < 7) ? PS.L1 : (PD < 11) ? PS.L2 : (PD < 15) ? PS.L3 : PS.L4;
            switch (ps)
            {
                case PS.L1:
                    SetDelta(2 * R1 / 6);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(adx, -dy)) break;  // 3
                    break;

                case PS.L2:
                    SetDelta(2 * R1 / 9);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(adx, -dy)) break;  // 5
                    break;
                case PS.L3:
                    SetDelta(2 * R1 / 12);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(cdx, -dy)) break;  // 5
                    if (AddLobe(bdx, dy)) break;   // 6
                    if (AddLobe(adx, -dy)) break;  // 7
                    break;

                case PS.L4:
                    SetDelta(2 * R1 / 14);

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
        protected override (byte min, byte max) MinMaxDimension => (1, 18);

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
                    if (FillStroke == Fill_Stroke.Filled)
                        ds.FillGeometry(geo, color);
                    else
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
