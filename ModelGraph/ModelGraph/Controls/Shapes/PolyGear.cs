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
            R1 = 20;
            R2 = 50;
            PD = 4;
            CreatePoints();
        }
        internal PolyGear(int I, byte[] data) : base(I, data) { }

        protected override void CreatePoints()
        {
            var N = 3 * PD; //number of points
            var M = 2 * PD; //number of angles
            DXY = new List<(sbyte dx, sbyte dy)>(N);

            var da = Math.PI * 2 / M;
            var ta = da * R3 / 200 ;
            var a = - da;
            for (int i = 0; i < M; i++)
            {
                DXY.Add(Round((R1 * (float)Math.Cos(a), R1 * (float)Math.Sin(a))));
                a += da;
                DXY.Add(Round((R2 * (float)Math.Cos(a - ta), R2 * (float)Math.Sin(a - ta))));
                DXY.Add(Round((R2 * (float)Math.Cos(a + ta), R2 * (float)Math.Sin(a + ta))));
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
