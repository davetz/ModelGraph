namespace ModelGraphSTD
{
    public class FacetDXY
    {
        internal readonly (sbyte X, sbyte Y)[] DXY;
        internal int Length { get { return DXY.Length; } }

        internal FacetDXY((float x, float y)[] dxy)
        {
            var len = (dxy == null) ? 0 : dxy.Length;

            DXY = new (sbyte X, sbyte y)[len];

            for (int i = 0; i < len; i++)
            {
                var (x, y) = dxy[i];

                // enforce reasonable limits on the (dx, dy) values
                x = (x < sbyte.MinValue) ? sbyte.MinValue : (x > sbyte.MaxValue) ? sbyte.MaxValue : x;
                y = (y < sbyte.MinValue) ? sbyte.MinValue : (y > sbyte.MaxValue) ? sbyte.MaxValue : y;

                DXY[i] = ((sbyte)x, (sbyte)y);
            }
        }

        #region Width  ========================================================
        /// <summary>
        /// Calculate the facet's width
        /// </summary>
        internal double Width()
        {
            int y, ymin, ymax;
            y = ymin = ymax = 0;

            for (int i = 1; i < DXY.Length; i++)
            {
                y += DXY[i].Y;

                if (y < ymin) ymin = y;
                else if (y > ymax) ymax = y;
            }
            var w = (ymax - ymin);
            return (w < 2) ? 2 : w;
        }
        #endregion
    }
}
