using Microsoft.Graphics.Canvas;
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
            R1 = R2 = 50;
            PD = 3;
            CreatePoints();
        }
        internal PolySide(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            DXY = new List<(float dx, float dy)>(PD);

            var da = FullRadians / PD;
            var a = RadiansStart;
            for (int i = 0; i < PD; i++)
            {
                DXY.Add(Limit((R1 * (float)Math.Cos(a), R1 * (float)Math.Sin(a))));
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
        #endregion
    }
}
