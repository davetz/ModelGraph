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

        protected (Vector2 cp, float r1, float r2) GetCenterRadius(Vector2 center, float scale)
        {
            var (r1, r2, f1) = GetRadius(scale);
            var (dx, dy) = DXY[0];
            return (new Vector2(center.X + scale * dx, center.Y + scale * dy), r1, r2);
        }

        #region OverideAbstract  ==============================================
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var r1 = Radius1;
            var r2 = Radius2;
            var (dx, dy) = DXY[0];
            return (dx - r1, dy - r2, dx + r1, dy + r2);
        }
        protected override void Scale(Vector2 scale)
        {
            Radius1 *= scale.X;
            Radius2 *= scale.Y;
        }
        #endregion
    }
}
