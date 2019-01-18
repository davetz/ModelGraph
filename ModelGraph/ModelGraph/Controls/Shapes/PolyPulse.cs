using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolyPulse : Polyline
    {
        internal PolyPulse()
        {
            Radius1 = Radius2 = 25;
            Dimension = 1;
            CreatePoints();
        }
        internal PolyPulse(int I, byte[] data) : base(I, data)
        {
        }

        #region PrivateConstructor  ===========================================
        private PolyPulse(Shape shape)
        {
            CopyData(shape);
        }
        private PolyPulse(Shape shape, Vector2 center)
        {
            CopyData(shape);
        }
        #endregion


        #region CreatePoints  =================================================
        protected override void CreatePoints()
        {
            var D = Dimension;
            var n = 0;
            var N = 1 + D;
            var (r1, r2, f1) = GetRadius();

            DXY = new List<(float dx, float dy)>(N);
            float dx = -r1, dy = r2, adx;


            void SetDelta(float ds)
            {
                adx = ds;
                Add(dx, 0);
            }
            bool AddPulse(float tdy)
            {
                dx += adx;
                if (Add(dx, 0)) return true;
                if (Add(dx, tdy)) return true;
                dx += adx;
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


        #region RequiredMethods  ==============================================
        internal override Shape Clone() =>new PolyPulse(this);
        internal override Shape Clone(Vector2 center) => new PolyPulse(this, center);
        #endregion
    }
}
