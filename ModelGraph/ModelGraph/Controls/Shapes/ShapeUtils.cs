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
        static private int LIM1(float v) => (int)Math.Round(v);
        static private sbyte LIM2(int v) => (v < sbyte.MinValue) ? sbyte.MinValue : (v > sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)v;
        static protected (sbyte dx, sbyte dy) Round(float x, float y) => (LIM2(LIM1(x)), LIM2(LIM1(y)));
        static protected (sbyte dx, sbyte dy) Round((float x, float y) p) => Round(p.x, p.y);

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
        private float DegreesToRadians(float angle) => angle * (float)Math.PI / 180;

        private void MoveCenter(float dx, float dy)
        {
            for (int i = 0; i < DXY.Count; i++)
            {
                var (tx, ty) = DXY[i];
                DXY[i] = Round(tx + dx, ty + dy);
            }
        }
        private void RotateLeft()
        {
            A0 = (byte)((A0 - 1) & 0xF);
            TransformPoints(Matrix3x2.CreateRotation(DegreesToRadians(-22.5f)));
        }
        private void RotateRight()
        {
            A0 = (byte)((A0 + 1) & 0xF);
            TransformPoints(Matrix3x2.CreateRotation(DegreesToRadians(22.5f)));
        }
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
                DXY[i] = Round(p.X, p.Y);
            }
        }
    }
}
