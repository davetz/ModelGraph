using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal abstract class Central : Shape
    {
        internal Central() { }

        protected Vector2 Center { get { return ToVector(DXY[0]); } set { DXY[0] = Limit(value.X, value.Y); } }

        #region GetCenterRadius  ==============================================
        protected (Vector2 cp, float r1, float r2) GetCenterRadius(Vector2 center, float scale)
        {
            var (r1, r2, f1) = GetRadius(scale);
            return (center + Center * scale, r1, r2);
        }

        protected (Vector2 cp, float r1, float r2) GetCenterRadius(FlipState flip, Vector2 center, float scale)
        {
            var (r1, r2, f1) = GetRadius(scale);

            switch (flip)
            {
                case FlipState.None:
                    return ConvertedPoint();

                case FlipState.VertFlip:
                    var mv = Matrix3x2.CreateScale(1, -1);
                    return TransformedPoint(mv);

                case FlipState.HorzFlip:
                    var mh = Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoint(mh);

                case FlipState.VertHorzFlip:
                    var mb = Matrix3x2.CreateScale(-1, -1);
                    return TransformedPoint(mb);

                case FlipState.LeftRotate:
                    SwapRadius();
                    var ml = Matrix3x2.CreateRotation(FullRadians / 4);
                    return TransformedPoint(ml);

                case FlipState.LeftHorzFlip:
                    SwapRadius();
                    var mlh = Matrix3x2.CreateRotation(FullRadians / 4) * Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoint(mlh);

                case FlipState.RightRotate:
                    SwapRadius();
                    var mr = Matrix3x2.CreateRotation(-FullRadians / 4);
                    return TransformedPoint(mr);

                case FlipState.RightHorzFlip:
                    SwapRadius();
                    var mlr = Matrix3x2.CreateRotation(-FullRadians / 4) * Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoint(mlr);
            }
            return (Vector2.Zero, r1, r2);

            void SwapRadius()
            {
                var t = r1;
                r1 = r2;
                r2 = t;
            }
            (Vector2 cp, float r1, float r2) ConvertedPoint()
            {
                var (dx, dy) = DXY[0];
                var p = new Vector2(dx, dy);
                p = center + p * scale;
                return (p, r1, r2);
            }
            (Vector2 cp, float r1, float r2) TransformedPoint(Matrix3x2 m)
            {
                var (dx, dy) = DXY[0];
                var p = new Vector2(dx, dy);
                p = Vector2.Transform(p, m);
                p = center + p * scale;

                return (p, r1, r2);
            }
        }
        #endregion

        #region OverideAbstract  ==============================================
        protected override (float dx1, float dy1, float dx2, float dy2) GetExtent()
        {
            var r1 = Radius1;
            var r2 = Radius2;
            var (dx, dy) = DXY[0];
            return (dx - r1, dy - r2, dx + r1, dy + r2);
        }
        protected override void Scale(Vector2 scale)
        {
            Radius1 = scale.X;
            Radius2 = scale.Y;
        }
        #endregion
    }
}
