using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract class Central : Shape
    {
        internal Central() { }
        internal Central(int I, byte[] data) : base(I, data) { }

        protected void InititializeDXY((sbyte dx, sbyte dy)[] values)
        {
            DXY = new List<(float dx, float dy)>() { (0, 0) };
            var (r1, r2) = values[1];
            R1 = (byte)r1;
            R2 = (byte)r2;
        }
        protected Vector2 Center
        {
            get
            {
                var (dx, dy) = DXY[0];
                return new Vector2(dx, dy);
            }
            set
            {
                DXY[0] = Limit(value.X, value.Y);
            }
        }

        #region OverideAbstract  ==============================================
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var (dx, dy) = DXY[0];
            return (dx - R1, dy - R2, dx + R1, dy + R2);
        }
        protected override void Scale(Vector2 scale)
        {
            R1 = (byte)(R1 * scale.X);
            R2 = (byte)(R2 * scale.Y);
        }
        #endregion
    }
}
