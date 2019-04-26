using System.Collections.Generic;
using System.Numerics;

namespace ModelGraph.Controls
{
    internal class PolySpring : PolySpline
    {
        internal PolySpring(bool deserializing = false)
        {
            if (deserializing) return; // properties to be loaded from serialized data

            Radius1 = 0.75f;
            Radius2 = 0.35f;
            Dimension = 6;
            CreatePoints();
        }

        #region PrivateConstructor  ===========================================
        private PolySpring(Shape shape)
        {
            CopyData(shape);
        }
        private PolySpring(Shape shape, Vector2 center)
        {
            CopyData(shape);
            SetCenter(new Shape[] { this }, center);
        }
        #endregion

        #region CreatePoints  =================================================
        private enum PS { L1, L2, L3, L4 }
        protected override void CreatePoints()
        {
            var D = Dimension;
            var (r1, r2, f1) = GetRadius();

            var n = 0;
            var N = 1 + D * 2; // number of points per spline
            DXY = new List<(float dx, float dy)>(N);
            float dx = -r1, dy = r2, adx, bdx, cdx;

            var ps = (D < 7) ? PS.L1 : (D < 11) ? PS.L2 : (D < 15) ? PS.L3 : PS.L4;
            switch (ps)
            {
                case PS.L1:
                    SetDelta(2 * r1 / 6);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(adx, -dy)) break;  // 3
                    break;

                case PS.L2:
                    SetDelta(2 * r1 / 9);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(adx, -dy)) break;  // 5
                    break;
                case PS.L3:
                    SetDelta(2 * r1 / 12);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(cdx, -dy)) break;  // 5
                    if (AddLobe(bdx, dy)) break;   // 6
                    if (AddLobe(adx, -dy)) break;  // 7
                    break;

                case PS.L4:
                    SetDelta(2 * r1 / 14);

                    if (AddLobe(adx, -dy)) break;  // 1
                    if (AddLobe(bdx, dy)) break;   // 2
                    if (AddLobe(cdx, -dy)) break;  // 3
                    if (AddLobe(bdx, dy)) break;   // 4
                    if (AddLobe(cdx, -dy)) break;  // 5
                    if (AddLobe(bdx, dy)) break;   // 6
                    if (AddLobe(cdx, -dy)) break;  // 7
                    if (AddLobe(bdx, dy)) break;   // 8
                    if (AddLobe(adx, -dy)) break;  // 9
                    break;
            }
            TransformPoints(Matrix3x2.CreateRotation(RadiansStart));

            void SetDelta(float ds)
            {
                adx = ds * 2;
                bdx = ds * -1;
                cdx = ds * 2.5f;
                Add(dx, 0);
            }
            bool AddLobe(float tdx, float tdy)
            {
                if (Add(dx, tdy)) return true;
                dx += tdx;
                if (Add(dx, tdy)) return true;
                dx += tdx;
                if (Add(dx, tdy)) return true;
                if (Add(dx, 0)) return true;
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
        internal override Shape Clone() => new PolySpring(this);
        internal override Shape Clone(Vector2 center) => new PolySpring(this, center);
        protected override (int min, int max) MinMaxDimension => (1, 18);
        internal override HasSlider Sliders => HasSlider.Horz | HasSlider.Vert | HasSlider.Minor | HasSlider.Major | HasSlider.Dim;
        protected override byte TypeCode => (byte)ShapeType.PolySpring;

        #endregion
    }
}
