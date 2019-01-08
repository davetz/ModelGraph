using Microsoft.Graphics.Canvas;
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
            P1 = 25;
            P2 = 75;
            P3 = 5;
            CreatePoints();
        }
        internal PolyStar(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            var N = 2 * P3;
            DXY = new List<(sbyte dx, sbyte dy)>(N);

            var da = Math.PI * 2 / N;
            var a = -Math.PI /2;
            for (int i = 0; i < P3; i++)
            {
                DXY.Add(Round((P2 * (float)Math.Cos(a), P2 * (float)Math.Sin(a))));
                a += da;
                DXY.Add(Round((P1 * (float)Math.Cos(a), P1 * (float)Math.Sin(a))));
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
        #endregion
    }
}
