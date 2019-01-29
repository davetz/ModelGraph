using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolySpike : Polyline
    {
        internal PolySpike(bool deserializing = false)
        {
            if (deserializing) return; // properties to be loaded from serialized data

            Radius1 = 0.75f;
            Radius2 = 0.25f;
            AuxFactor = 0.50f;
            Dimension = 4;
            CreatePoints();
        }

        #region PrivateConstructor  ===========================================
        private PolySpike(Shape shape)
        {
            CopyData(shape);
        }
        private PolySpike(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
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

            var ax = r1 * 4 / D; //ax step size
            var bx = ax * f1; //ax step size

            var dx = -r1;
            var dy = r2;

            for (int i = 0; i < N; i++)
            {
                if (Add(dx, -dy)) return;
                if (Add(dx + bx, dy)) return;
                dx += ax;
            }
            TransformPoints(Matrix3x2.CreateRotation(RadiansStart));

            bool Add(float x, float y)
            {
                DXY.Add(Limit(x, y));
                return (++n >= N);
            }
        }
        #endregion

        #region RequiredMethods  ==============================================
        internal override Shape Clone() =>new PolySpike(this);
        internal override Shape Clone(Vector2 center) => new PolySpike(this, center);
        protected override (int min, int max) MinMaxDimension => (2, 20);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Major | HasSlider.Minor | HasSlider.Aux | HasSlider.Dim;
        protected override byte TypeCode => (byte)ShapeType.PolySpike;
        #endregion
    }
}
