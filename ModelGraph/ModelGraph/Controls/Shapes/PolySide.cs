﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolySide : Polygon
    {
        internal PolySide()
        {
            Radius1 = Radius2 = 50;
            Dimension = 3;
            CreatePoints();
        }
        internal PolySide(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {

            var D = Dimension;
            DXY = new List<(float dx, float dy)>(D);
            var da = FullRadians / D;
            var (r1, r2, f1) = GetRadius();
            var a = RadiansStart;
            for (int i = 0; i < D; i++)
            {
                DXY.Add(Limit((r1 * (float)Math.Cos(a), r1 * (float)Math.Sin(a))));
                a += da;
            }
        }

        #region PrivateConstructor  ===========================================
        private PolySide(Shape shape)
        {
            CopyData(shape);
        }
        private PolySide(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new PolySide(this);
        internal override Shape Clone(Vector2 center) => new PolySide(this, center);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Major | HasSlider.Dim;
        #endregion
    }
}
