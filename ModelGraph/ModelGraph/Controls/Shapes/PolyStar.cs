﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolyStar : Polygon
    {
        internal PolyStar()
        {
            Radius1 = 0.5f;
            Radius2 = 0.2f;
            Dimension = 6;
            CreatePoints();
        }
        internal PolyStar(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            var D = Dimension;
            var N = 2 * D;
            DXY = new List<(float dx, float dy)>(N);
            var (r1, r2, f1) = GetRadius();
            var da = FullRadians / N;
            var a = RadiansStart;
            for (int i = 0; i < D; i++)
            {
                DXY.Add(Limit((r1 * (float)Math.Cos(a), r1 * (float)Math.Sin(a))));
                a += da;
                DXY.Add(Limit((r2 * (float)Math.Cos(a), r2 * (float)Math.Sin(a))));
                a += da;
            }
        }

        #region PrivateConstructor  ===========================================
        private PolyStar(Shape shape)
        {
            CopyData(shape);
        }
        private PolyStar(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new PolyStar(this);
        internal override Shape Clone(Vector2 center) => new PolyStar(this, center);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Minor | HasSlider.Major | HasSlider.Dim;
        #endregion
    }
}
