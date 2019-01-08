using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Circle : Central
    {
        internal Circle()
        {
            P1 = P2 = 25;
            DXY = new List<(sbyte dx, sbyte dy)>() { (0, 0) };
        }
        internal Circle(int I, byte[] data) : base(I, data)
        {
        }

        #region PrivateConstructor  ===========================================
        private Circle(Shape shape)
        {
            CopyData(shape);
        }
        private Circle(Shape shape, Vector2 center)
        {
            CopyData(shape);
            Center = center;
        }
        #endregion

        #region RequiredMethods  ==============================================
        internal override Shape Clone() =>new Circle(this);
        internal override Shape Clone(Vector2 center) => new Circle(this, center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var radius = (P1 * scale);
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillCircle( Center * scale + center, radius, Color);
            else
                ds.DrawCircle(Center * scale + center, radius, Color, strokeWidth, StrokeStyle());
        }
        protected override void Scale(Vector2 scale)
        {
            if (scale.X == 1)
            {
                P1 = P2 = (byte)(P1 * scale.Y);
            }
            else
            {
                P1 = P2 = (byte)(P1 * scale.X);
            }
        }
        #endregion
    }
}
