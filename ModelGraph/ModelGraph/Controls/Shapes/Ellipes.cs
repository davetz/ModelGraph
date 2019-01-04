using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Ellipes : Central
    {
        private static (sbyte dx, sbyte dy)[] DefaultDXY = { (0, 0), (40, 30) };

        internal Ellipes(int I, byte[] data) : base(I, data)
        {
        }

        internal Ellipes(Shape shape)
        {
            CopyData(shape);
        }

        internal Ellipes(Vector2 center)
        {
            DXY = new List<(sbyte dx, sbyte dy)>(DefaultDXY);
            Center = center;
        }

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new Ellipes(this);
        internal override Shape Clone(Vector2 Center) => new Ellipes(Center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var radius = Radius * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillEllipse(Center * scale + center, radius.X, radius.Y, Color);
            else
                ds.DrawEllipse(Center * scale + center, radius.X, radius.Y, Color, strokeWidth);
        }
        #endregion
    }
}
