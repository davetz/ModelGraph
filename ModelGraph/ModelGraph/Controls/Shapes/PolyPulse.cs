using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolyPulse : Polyline
    {
        internal PolyPulse()
        {
            Radius1 = 64;
            Radius2 = 32;
            AuxFactor = 25;
            Dimension = 5;
            CreatePoints();
        }
        internal PolyPulse(int I, byte[] data) : base(I, data) { }

        #region PrivateConstructor  ===========================================
        private PolyPulse(Shape shape)
        {
            CopyData(shape);
        }
        private PolyPulse(Shape shape, Vector2 center)
        {
            CopyData(shape);
        }
        #endregion

        #region CreatePoints  =================================================
        protected override void CreatePoints()
        {
            var D = Dimension;
            var n = 0;
            var N = 1 + D;
            DXY = new List<(float dx, float dy)>(N);

            var (r1, r2, f1) = GetRadius();

            var nax = NAX[D]; // number of ax steps for given dimension
            var ax = r1 * 2 / nax; //ax step size
            var bx = ax * f1 / 200;
            var dx = -r1;
            var dy = r2;

            Action[] action = { SouthEast, South, NorthEast, North }; // loop sequence action

            action[LAC[D]](); // vector to the appropriate loop sequence
            TransformPoints(Matrix3x2.CreateRotation(RadiansStart));


            void NorthEast()
            {
                Add(dx + bx,-dy);
                for (int i = 0; i < D; i++)
                {
                    dx += ax;
                    if (Add(dx - bx, -dy)) return;
                    if (Add(dx + bx, dy)) return;
                    dx += ax;
                    if (Add(dx - bx, dy)) return;
                    if (Add(dx + bx, -dy)) return;
                }
            }
            void SouthEast()
            {
                Add(dx + bx, dy);
                for (int i = 0; i < D; i++)
                {
                    dx += ax;
                    if (Add(dx - bx, dy)) return;
                    if (Add(dx + bx, -dy)) return;
                    dx += ax;
                    if (Add(dx - bx, -dy)) return;
                    if (Add(dx  + bx, dy)) return;
                }
            }
            void North()
            {
                Add(dx - bx, dy);
                for (int i = 0; i < D; i++)
                {
                    if (Add(dx + bx, -dy)) return;
                    dx += ax;
                    if (Add(dx - bx, -dy)) return;
                    if (Add(dx + bx, dy)) return;
                    dx += ax;
                    if (Add(dx + bx, dy)) return;
                }
            }
            void South()
            {
                Add(dx - bx, -dy);
                for (int i = 0; i < D; i++)
                {
                    if (Add(dx + bx, dy)) return;
                    dx += ax;
                    if (Add(dx - bx, dy)) return;
                    if (Add(dx + bx, -dy)) return;
                    dx += ax;
                    if (Add(dx - bx, -dy)) return;
                }
            }

            bool Add(float x, float y)
            {
                DXY.Add(Limit(x, y));
                return (++n >= N);
            }
        }
        static private int[] NAX = { 0, 1, 1, 1, 2, 3, 3, 3, 4, 5, 5, 5, 6, 7, 7, 7, 8, 9, }; //number of ax steps for given dimension
        static private int[] LAC = { 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 0, 0, 1, 1, 2, 2, }; //loop action index for given dimension 
        #endregion

        #region RequiredMethods  ==============================================
        internal override Shape Clone() =>new PolyPulse(this);
        internal override Shape Clone(Vector2 center) => new PolyPulse(this, center);
        protected override (int min, int max) MinMaxDimension => (1, 15);
        internal override HasSlider Slider => HasSlider.Horz | HasSlider.Vert | HasSlider.Major | HasSlider.Minor | HasSlider.Aux | HasSlider.Dim;
        #endregion
    }
}
