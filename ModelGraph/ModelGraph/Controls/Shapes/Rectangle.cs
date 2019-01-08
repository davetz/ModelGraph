using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Rectangle : Central
    {
        internal Rectangle()
        {
            P1 = 50;
            P2 = 75;
            DXY = new List<(sbyte dx, sbyte dy)>() { (0, 0) };
        }
        internal Rectangle(int I, byte[] data) : base(I, data)
        {
        }

        #region PrivateConstructor  ===========================================
        private Rectangle(Shape shape)
        {
            CopyData(shape);
        }
        private Rectangle(Shape shape, Vector2 center)
        {
            CopyData(shape);
            Center = center;
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new Rectangle(this);
        internal override Shape Clone(Vector2 center) => new Rectangle(this, center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var min = center + (Center - Radius) * scale;
            var len = Radius * 2 * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRectangle( min.X, min.Y, len.X, len.Y, Color);
            else
                ds.DrawRectangle(min.X, min.Y, len.X, len.Y, Color, strokeWidth, StrokeStyle());
        }
        #endregion
    }
}
