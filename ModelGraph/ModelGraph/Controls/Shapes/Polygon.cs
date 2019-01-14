using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace ModelGraph.Controls
{
    public enum PolyDimension : byte
    {
        D2 = 2,
        D3 = 3,
        D4 = 4,
        D5 = 5,
        D6 = 6,
        D8 = 8,
    }

    internal abstract class Polygon : Polyline
    {
        internal Polygon() { }
        internal Polygon(int I, byte[] data) : base(I, data) { }

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth, Coloring coloring = Coloring.Normal)
        {
            var color = GetColor(coloring);
            var points = GetDrawingPoints(center, scale);

            using (var geo = CanvasGeometry.CreatePolygon(cc, points))
            {
                if (FillStroke == Fill_Stroke.Filled)
                    ds.FillGeometry(geo, color);
                else
                    ds.DrawGeometry(geo, color, strokeWidth, StrokeStyle());
            }
        }
    }
}
