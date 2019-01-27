using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraph.Controls
{
    internal abstract partial class Shape
    {
        static private float LIM(float v) => (v < -1) ? -1 : (v > 1) ? 1 : v;
        static protected (float dx, float dy) Limit(float x, float y) => (LIM(x), LIM(y));
        static protected (float dx, float dy) Limit((float x, float y) p) => Limit(p.x, p.y);
        static protected Vector2 ToVector((float x, float y) p) => new Vector2(p.x, p.y);

        #region GetMaxRadius  =================================================
        static private (float r1, float r2, float f1) GetMaxRadius(IEnumerable<Shape> shapes)
        {
            float r1 = 0, r2 = 0, f1 = 0;

            foreach (var shape in shapes)
            {
                if (shape.Radius1 > r1) r1 = shape.Radius1;
                if (shape.Radius2 > r2) r2 = shape.Radius2;
                if (shape.AuxFactor > f1) f1 = shape.AuxFactor;
            }
            return (r1, r2, f1);
        }
        #endregion

        #region GetHasSlider  =================================================
        static private (bool, HasSlider) GetHasSlider(IEnumerable<Shape> shapes)
        {
            var locked = false;
            var slider = HasSlider.None;
            foreach (var shape in shapes)
            {
                slider |= shape.Sliders;
                locked |= shape.IsLocked;
            }
            return (locked, slider);
        }
        #endregion

        #region GetMinMaxDimension  ===========================================
        static private (int min, int max, int dim) GetDimension(IEnumerable<Shape> shapes)
        {
            int min = 1, max = 100, dim = 0;

            foreach (var shape in shapes)
            {
                var pd = shape.PD;
                var (d1, d2) = shape.MinMaxDimension;
                if (d1 > min) min = d1;
                if (d2 < max) max = d2;
                dim = (pd < min) ? min : (pd > max) ? max : pd;
            }
            return (min, max, dim);
        }
        #endregion

        #region GetExtent  ====================================================
        static private (float dx1, float dy1, float dx2, float dy2, float cdx, float cdy, float dx, float dy) GetExtent(IEnumerable<Shape> shapes)
        {
            var x1 = 1f;
            var y1 = 1f;
            var x2 = -1f;
            var y2 = -1f;

            foreach (var shape in shapes)
            {
                var (dx1, dy1, dx2, dy2) = shape.GetExtent();

                if (dx1 < x1) x1 = dx1;
                if (dy1 < y1) y1 = dy1;

                if (dx2 > x2) x2 = dx2;
                if (dy2 > y2) y2 = dy2;
            }
            return (x1 == 1) ? (0, 0, 0, 0, 0, 0, 0, 0) : (x1, y1, x2, y2, (x1 + x2) / 2, (y1 + y2) / 2, (x2 - x1), (y2 - y1));
        }
        #endregion

        #region Rotation  =====================================================

        private void MoveCenter(float dx, float dy)
        {
            for (int i = 0; i < DXY.Count; i++)
            {
                var (tx, ty) = DXY[i];
                DXY[i] = Limit(tx + dx, ty + dy);
            }
        }
        private void RotateLeft(bool useAlternate = false)
        {
            if (useAlternate)
            {
                RotateStartLeft1();
                TransformPoints(Matrix3x2.CreateRotation(RotateLeftRadians1));
            }
            else
            {
                RotateStartLeft0();
                TransformPoints(Matrix3x2.CreateRotation(RotateLeftRadians0));
            }
        }
        private void RotateRight(bool useAlternate = false)
        {
            if (useAlternate)
            {
                RotateStartRight1();
                TransformPoints(Matrix3x2.CreateRotation(RotateRightRadians1));
            }
            else
            {
                RotateStartRight0();
                TransformPoints(Matrix3x2.CreateRotation(RotateRightRadians0));
            }
        }
        #endregion

        #region Flip/TransformPoints  =========================================
        private void VerticalFlip()
        {
            TransformPoints(Matrix3x2.CreateScale(new Vector2(1, -1)));
        }
        private void HorizontalFlip()
        {
            TransformPoints(Matrix3x2.CreateScale(new Vector2(-1, 1)));
        }
        protected void TransformPoints(Matrix3x2 m)
        {
            for (int i = 0; i < DXY.Count; i++)
            {
                var (dx, dy) = DXY[i];
                var p = new Vector2(dx, dy);
                p = Vector2.Transform(p, m);
                DXY[i] = Limit(p.X, p.Y);
            }
        }
        protected Vector2[] GetDrawingPoints(FlipState flip, float scale, Vector2 center)
        {
            var N = DXY.Count;
            var points = new Vector2[N];
            
            switch (flip)
            {
                case FlipState.None:
                    return ConvertedPoints();

                case FlipState.VertFlip:
                    var mv = Matrix3x2.CreateScale(1, -1);
                    return TransformedPoints(mv);

                case FlipState.HorzFlip:
                    var mh = Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoints(mh);

                case FlipState.VertHorzFlip:
                    var mb  = Matrix3x2.CreateScale(-1, -1);
                    return TransformedPoints(mb);

                case FlipState.LeftRotate:
                    var ml = Matrix3x2.CreateRotation(FullRadians / 4);
                    return TransformedPoints(ml);

                case FlipState.LeftHorzFlip:
                    var mlh = Matrix3x2.CreateRotation(FullRadians / 4) * Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoints(mlh);

                case FlipState.RightRotate:
                    var mr = Matrix3x2.CreateRotation(-FullRadians / 4);
                    return TransformedPoints(mr);

                case FlipState.RightHorzFlip:
                    var mlr = Matrix3x2.CreateRotation(-FullRadians / 4) * Matrix3x2.CreateScale(-1, 1);
                    return TransformedPoints(mlr);
            }
            return null;

            Vector2[] ConvertedPoints()
            {
                for (int i = 0; i < N; i++)
                {
                    var (dx, dy) = DXY[i];
                    var p = new Vector2(dx, dy);
                    points[i] = center + p * scale;
                }
                return points;
            }
            Vector2[] TransformedPoints(Matrix3x2 m)
            {
                for (int i = 0; i < N; i++)
                {
                    var (dx, dy) = DXY[i];
                    var p = new Vector2(dx, dy);
                    p = Vector2.Transform(p, m);
                    points[i] = center + p * scale;
                }
                return points;
            }
        }
        #endregion
    }
}
