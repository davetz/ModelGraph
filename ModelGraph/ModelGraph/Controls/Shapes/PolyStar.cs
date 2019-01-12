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
            R1 = 50;
            R2 = 20;
            PD = 6;
            CreatePoints();
        }
        internal PolyStar(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            var N = 2 * PD;
            DXY = new List<(sbyte dx, sbyte dy)>(N);

            var da = FullRadians / N;
            var a = RadiansStart;
            for (int i = 0; i < PD; i++)
            {
                DXY.Add(Round((R1 * (float)Math.Cos(a), R1 * (float)Math.Sin(a))));
                a += da;
                DXY.Add(Round((R2 * (float)Math.Cos(a), R2 * (float)Math.Sin(a))));
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
