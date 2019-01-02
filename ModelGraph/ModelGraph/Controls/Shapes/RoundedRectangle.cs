using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class RoundedRectangle : Central
    {
        private static (sbyte dx, sbyte dy)[] DefaultDXY = { (0, 0), (50, 30), (12, 12) };

        #region Corner  =======================================================
        internal Vector2 Corner
        {
            get
            {
                var (x, y) = DXY[2];
                return new Vector2(x, y);
            }
            set
            {
                DXY[2] = ((sbyte)value.X, (sbyte)value.Y);
            }
        }
        #endregion

        internal RoundedRectangle(int I, byte[] data) : base(I, data)
        {
        }

        internal RoundedRectangle(Shape shape, Vector2 center)
        {
            DXY = new List<(sbyte dx, sbyte dy)>(DefaultDXY);
            CopyData(shape);
            Center = center;
        }

        internal RoundedRectangle(Vector2 center)
        {
            DXY = new List<(sbyte dx, sbyte dy)>(DefaultDXY);
            Center = center;
        }

        #region OverideAbstract  ==============================================
        internal override Shape Clone() =>new RoundedRectangle(this, Vector2.Zero);
        internal override Shape Clone(Vector2 Center) => new RoundedRectangle(this, Center);

        internal override void Draw(CanvasControl cc, CanvasDrawingSession ds, float scale, Vector2 center, float strokeWidth)
        {
            var min = center + (Center - Radius) * scale;
            var len = Radius * 2 * scale;
            var corner = Corner * scale;
            if (FillStroke == Fill_Stroke.Filled)
                ds.FillRoundedRectangle( min.X, min.Y, len.X, len.Y, corner.X, corner.Y, Color);
            else
                ds.DrawRoundedRectangle(min.X, min.Y, len.X, len.Y, corner.X, corner.Y, Color, strokeWidth);
        }
        #endregion
    }
}
