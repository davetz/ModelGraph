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
        static protected float PMIN = sbyte.MinValue;
        static protected float PMAX = sbyte.MaxValue;
        static private float LIM(float v) => (v < PMIN) ? PMIN : (v > PMAX) ? PMAX : v;
        static protected (float dx, float dy) Limit(float x, float y) => (LIM(x), LIM(y));
        static protected (float dx, float dy) Limit((float x, float y) p) => Limit(p.x, p.y);

        #region GetMaxRadius  =================================================
        static private (byte r1, byte r2, byte r3) GetMaxRadius(IEnumerable<Shape> shapes)
        {
            byte r1 = 0, r2 = 0, r3 = 0;

            foreach (var shape in shapes)
            {
                if (shape.R1 > r1) r1 = shape.R1;
                if (shape.R2 > r2) r2 = shape.R2;
                if (shape.R3 > r3) r3 = shape.R3;
            }
            return (r1, r2, r3);
        }
        #endregion

        #region GetMinMaxDimension  ===========================================
        static private (byte min, byte max, byte dim) GetDimension(IEnumerable<Shape> shapes)
        {
            byte min = 0, max = 100, dim = 0;

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
            var x1 = PMAX;
            var y1 = PMAX;
            var x2 = PMIN;
            var y2 = PMIN;

            foreach (var shape in shapes)
            {
                var (dx1, dy1, dx2, dy2) = shape.GetExtent();

                if (dx1 < x1) x1 = dx1;
                if (dy1 < y1) y1 = dy1;

                if (dx2 > x2) x2 = dx2;
                if (dy2 > y2) y2 = dy2;
            }
            return (x1 == PMAX) ? (0, 0, 0, 0, 0, 0, 0, 0) : (x1, y1, x2, y2, (x1 + x2) / 2, (y1 + y2) / 2, (x2 - x1), (y2 - y1));
        }
        #endregion

        #region Rotation  =====================================================
        protected static float FullRadians = (float)(2 * Math.PI);
        protected static float DeltaRadians0 = (float)(Math.PI / 8);
        protected static float DeltaRadians1 = (float)(Math.PI / 6);
        protected float RotateLeftRadians0 => -DeltaRadians0;
        protected float RotateRightRadians0 => DeltaRadians0;
        protected float RotateLeftRadians1 => -DeltaRadians1;
        protected float RotateRightRadians1 => DeltaRadians1;
        protected float RadiansStart => (A0 % 16) * DeltaRadians0 + (A1 % 12) * DeltaRadians1;

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
                A1 = (byte)((A1 - 1) & 0xF);
                TransformPoints(Matrix3x2.CreateRotation(RotateLeftRadians1));
            }
            else
            {
                A0 = (byte)((A0 - 1) & 0xF);
                TransformPoints(Matrix3x2.CreateRotation(RotateLeftRadians0));
            }
        }
        private void RotateRight(bool useAlternate = false)
        {
            if (useAlternate)
            {
                A1 = (byte)((A1 - 1) & 0xF);
                TransformPoints(Matrix3x2.CreateRotation(RotateRightRadians1));
            }
            else
            {
                A0 = (byte)((A0 - 1) & 0xF);
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
        #endregion
    }
}
