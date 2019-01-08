using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Ellipes : Central
    {
        internal Ellipes()
        {
            P1 = 50;
            P2 = 75;
            DXY = new List<(sbyte dx, sbyte dy)>() { (0, 0) };
        }
        internal Ellipes(int I, byte[] data) : base(I, data)
        {
        }

        #region PrivateConstructor  ===========================================
        private Ellipes(Shape shape)
        {
            CopyData(shape);
        }
        private Ellipes(Shape shape, Vector2 center)
        {
            CopyData(shape);
            Center = center;
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new Ellipes(this);
        internal override Shape Clone(Vector2 center) => new Ellipes(this, center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var radius = Radius * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillEllipse(Center * scale + center, radius.X, radius.Y, Color);
            else
                ds.DrawEllipse(Center * scale + center, radius.X, radius.Y, Color, strokeWidth, StrokeStyle());
        }
        #endregion
    }
}
