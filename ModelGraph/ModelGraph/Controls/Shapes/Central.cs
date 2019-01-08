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
            DXY = new List<(sbyte dx, sbyte dy)>() { (0, 0) };
            var (r1, r2) = values[1];
            P1 = (byte)r1;
            P2 = (byte)r2;
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
                DXY[0] = Round(value.X, value.Y);
            }
        }
        protected Vector2 Radius => new Vector2(P1, P2);

        #region OverideAbstract  ==============================================
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var (dx, dy) = DXY[0];
            return (dx - P1, dy - P2, dx + P1, dy + P2);
        }
        protected override void Scale(Vector2 scale)
        {
            P1 = (byte)(P1 * scale.X);
            P2 = (byte)(P1 * scale.Y);
        }
        protected override void Rotate(float radians, Vector2 center)
        {
            var m = Matrix3x2.CreateRotation(radians, center);
            Center = Vector2.Transform(Center, m);
        }
        #endregion
    }
}
