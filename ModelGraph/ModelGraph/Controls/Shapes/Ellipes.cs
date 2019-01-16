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
            R1 = 50;
            R2 = 75;
            DXY = new List<(float dx, float dy)>() { (0, 0) };
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

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var radius = Radius * scale;

            if (FillStroke == Fill_Stroke.Filled)
                ds.FillEllipse(Center * scale + center, radius.X, radius.Y, color);
            else
                ds.DrawEllipse(Center * scale + center, radius.X, radius.Y, color, strokeWidth, StrokeStyle());
        }
        #endregion
    }
}
