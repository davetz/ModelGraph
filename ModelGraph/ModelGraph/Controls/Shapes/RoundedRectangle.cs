using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class RoundedRectangle : Central
    {
        internal RoundedRectangle()
        {
            R1 = 50;
            R2 = 30;
            DXY = new List<(sbyte dx, sbyte dy)>() { (0, 0) };
        }
        internal RoundedRectangle(int I, byte[] data) : base(I, data)
        {
        }
        internal float Corner => 12f;

        #region PrivateConstructor  ===========================================
        internal RoundedRectangle(Shape shape)
        {
            CopyData(shape);
        }

        internal RoundedRectangle(Shape shape, Vector2 center)
        {
            CopyData(shape);
            Center = center;
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new RoundedRectangle(this);
        internal override Shape Clone(Vector2 center) => new RoundedRectangle(this, center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var min = center + (Center - Radius) * scale;
            var len = Radius * 2 * scale;
            var corner = Corner * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRoundedRectangle( min.X, min.Y, len.X, len.Y, corner, corner, color);
            else
                ds.DrawRoundedRectangle(min.X, min.Y, len.X, len.Y, corner, corner, color, strokeWidth, StrokeStyle());
        }
        #endregion
    }
}
