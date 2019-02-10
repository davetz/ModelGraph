using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ModelGraphSTD;
using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolyWave : PolySpline
    {
        internal PolyWave(bool deserializing = false)
        {
            if (deserializing) return; // properties to be loaded from serialized data

            Radius1 = 0.75f;
            Radius2 = 0.35f;
            Dimension = 6;
            CreatePoints();
        }

        #region PrivateConstructor  ===========================================
        private PolyWave(Shape shape)
        {
            CopyData(shape);
        }
        private PolyWave(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region CreatePoints  =================================================
        protected override void CreatePoints()
        {
            var D = Dimension;
            var (r1, r2, f1) = GetRadius();

            var n = 0;
            var N = 1 + D * 2; // number of points per spline
            DXY = new List<(float dx, float dy)>(N);
            float dx = -r1, dy = r2, adx = r1 / D;

            Add(dx, 0);

            for (int i = 0; i < N; i++)
            {
                if (AddLobe()) break;
            }
            TransformPoints(Matrix3x2.CreateRotation(RadiansStart));

            bool AddLobe()
            {
                dx += adx;
                if (Add(dx, dy)) return true;
                dx += adx;
                if(Add(dx, dy)) return true;
                dx += adx;
                if (Add(dx, dy)) return true;
                dx += adx;
                if (Add(dx, 0)) return true;
                dy = -1 * dy;
                return false;
            }
            bool Add(float x, float y)
            {
                DXY.Add(Limit(x, y));
                return (++n >= N); 
            }
        }
        #endregion

        #region OverideAbstract  ==============================================
        internal override Shape Clone() => new PolyWave(this);
        internal override Shape Clone(Vector2 center) => new PolyWave(this, center);
        protected override (int min, int max) MinMaxDimension => (1, 22);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Minor | HasSlider.Major | HasSlider.Dim;
        protected override byte TypeCode => (byte)ShapeType.PolyWave;
        #endregion
    }
}
