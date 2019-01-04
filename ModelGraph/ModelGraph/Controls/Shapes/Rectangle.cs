using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Rectangle : Central
    {
        private static (sbyte dx, sbyte dy)[] DefaultDXY = { (0, 0), (50, 70)};

        internal Rectangle(int I, byte[] data) : base(I, data)
        {
        }

        internal Rectangle(Shape shape)
        {
            CopyData(shape);
        }

        internal Rectangle(Vector2 center)
        {
            DXY = new List<(sbyte dx, sbyte dy)>(DefaultDXY);
            Center = center;
        }

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new Rectangle(this);
        internal override Shape Clone(Vector2 Center) => new Rectangle(Center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var min = center + (Center - Radius) * scale;
            var len = Radius * 2 * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRectangle( min.X, min.Y, len.X, len.Y, Color);
            else
                ds.DrawRectangle(min.X, min.Y, len.X, len.Y, Color, strokeWidth);
        }
        #endregion
    }
}
