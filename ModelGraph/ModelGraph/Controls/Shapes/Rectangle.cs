using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Rectangle : Central
    {
        internal Rectangle(bool deserializing = false)
        {
            if (deserializing) return; // properties to be loaded from serialized data

            Radius1 = Radius2 = 0.30f;
            DXY = new List<(float dx, float dy)>() { (0, 0) };
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

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var min = center + (Center - Radius) * scale;
            var len = Radius * 2 * scale;

            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRectangle( min.X, min.Y, len.X, len.Y, color);
            else
                ds.DrawRectangle(min.X, min.Y, len.X, len.Y, color, strokeWidth, StrokeStyle());
        }
        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, FlipState flip)
        {
            var color = GetColor(Coloring.Normal);
            var (cp, r1, r2) = GetCenterRadius(center, scale);
            var radius = new Vector2(r1, r2);
            var min = cp - radius;
            var len = radius * 2;

            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRectangle(min.X, min.Y, len.X, len.Y, color);
            else
                ds.DrawRectangle(min.X, min.Y, len.X, len.Y, color, StrokeWidth, StrokeStyle());
        }
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert;
        protected override byte TypeCode => (byte)ShapeType.Rectangle;

        #endregion
    }
}
