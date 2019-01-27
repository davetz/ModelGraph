using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class Line : Polyline
    {
        internal Line()
        {
            DXY = new List<(float dx, float dy)>() {(-0.25f, 0), (0.25f, 0)};
        }
        internal Line(int I, byte[] data) : base(I, data) { }

        #region PrivateConstructor  ===========================================
        private Line(Shape shape)
        {
            CopyData(shape);
        }
        private Line(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter( new Shape[] { this }, center);
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() => new Line(this);
        internal override Shape Clone(Vector2 center) => new Line(this, center);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert;
        protected override byte TypeCode => (byte)ShapeType.Line;
        #endregion
    }
}
