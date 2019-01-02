using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract class Central : Shape
    {
        internal Central() { }
        internal Central(int I, byte[] data) : base(I, data) { }

        #region Center/Radius  ====================================================
        internal Vector2 Center
        {
            get
            {
                var (dx, dy) = DXY[0];
                return new Vector2(dx, dy);
            }
            set
            {
                DXY[0] = Round(value.X, value.Y);
            }
        }
        internal Vector2 Radius
        {
            get
            {
                var (x, y) = DXY[1];
                return new Vector2(x, y);
            }
            set
            {
                DXY[1] = Round(value.X, value.Y);
            }
        }
        #endregion

        #region OverideAbstract  ==============================================
        protected override void GetVector(List<Vector2> list)
        {
            list.Add(Center);
        }
        protected override void SetVector(List<Vector2> list)
        {
            Center = list[0];
        }
        protected override void GetPoints(List<(float dx, float dy)> list)
        {
            var (dx, dy) = DXY[0];
            var (rx, ry) = DXY[1];
            list.Add((dx, dy));
            list.Add((dx - rx, dy - ry));
            list.Add((dx + rx, dy + ry));
        }
        protected override void SetPoints(List<(float dx, float dy)> list)
        {
            DXY[0] = Round(list[0]);
        }
        #endregion
    }
}
