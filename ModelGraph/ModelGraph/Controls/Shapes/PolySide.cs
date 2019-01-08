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
            P1 = P2 = 50;
            P3 = 3;
            CreatePoints();
        }
        internal PolySide(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            DXY = new List<(sbyte dx, sbyte dy)>(P3);

            var da = Math.PI * 2 / P3;
            var a = da - Math.PI / 2;
            for (int i = 0; i < P3; i++)
            {
                DXY.Add(Round((P1 * (float)Math.Cos(a), P1 * (float)Math.Sin(a))));
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
