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
            Radius1 = Radius2 = 25;
            DXY = new List<(float dx, float dy)>() { (0, 0) };
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

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var (cp, r1, r2) = GetCenterRadius(center, scale);

            if (FillStroke == Fill_Stroke.Filled)
                ds.FillCircle( cp, r1, color);
            else
                ds.DrawCircle(cp, r1, color, strokeWidth, StrokeStyle());
        }
        protected override void Scale(Vector2 scale)
        {
            if (scale.X == 1)
                Radius1 = Radius2 = (Radius1 * scale.Y);
            else
                Radius1 = Radius2 = (Radius1 * scale.X);
        }
        #endregion
    }
}
