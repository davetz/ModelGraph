using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Circle : Central
    {
        private static (sbyte dx, sbyte dy)[] DefaultDXY = { (0, 0), (25, 25) };

        internal Circle(int I, byte[] data) : base(I, data)
        {
        }

        internal Circle(Shape shape)
        {
            CopyData(shape);
        }

        internal Circle(Vector2 center)
        {
            DXY = new List<(sbyte dx, sbyte dy)>(DefaultDXY);
            Center = center;
        }

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new Circle(this);
        internal override Shape Clone(Vector2 Center) => new Circle(Center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var radius = (Radius * scale);
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillCircle( Center * scale + center, radius.X, Color);
            else
                ds.DrawCircle(Center * scale + center, radius.X, Color, strokeWidth);
        }
        #endregion
    }
}
