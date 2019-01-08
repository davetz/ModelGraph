using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolyGear : Polygon
    {
        internal PolyGear()
        {
            P1 = 20;
            P2 = 75;
            P3 = 6;
            CreatePoints();
        }
        internal PolyGear(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            var N = 3 * P3;
            var M = 2 * P3;
            DXY = new List<(sbyte dx, sbyte dy)>(N);

            var da = Math.PI * 2 / M;
            var ta = da / 6;
            var a = (Math.PI / 2) - da;
            for (int i = 0; i < M; i++)
            {
                DXY.Add(Round((P2 * (float)Math.Cos(a - ta), P2 * (float)Math.Sin(a - ta))));
                DXY.Add(Round((P2 * (float)Math.Cos(a + ta), P2 * (float)Math.Sin(a + ta))));
                a += da;
                DXY.Add(Round((P1 * (float)Math.Cos(a), P1 * (float)Math.Sin(a))));
                a += da;
            }
        }

        #region PrivateConstructor  ===========================================
        private PolyGear(Shape shape)
        {
            CopyData(shape);
        }
        private PolyGear(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new PolyGear(this);
        internal override Shape Clone(Vector2 center) => new PolyGear(this, center);
        #endregion
    }
}
