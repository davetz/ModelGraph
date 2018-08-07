namespace ModelGraphSTD
{
    public class FacetDXY
    {
        internal FacetDXY(int[] dxy)
        {
            var len = (dxy == null) ? 0 : dxy.Length;

            if ((len % 2) == 1) len -= 1; // ensure even number

            DXY = new sbyte[len];

            for (int i = 0; i < len; i++)
            {
                var val = dxy[i];
                DXY[i] = (val < sbyte.MinValue) ? sbyte.MinValue :
                    ((val > sbyte.MaxValue) ? sbyte.MaxValue :
                    (sbyte)val);
            }
        }

        // enforce reasonable limits on the (dx, dy) values
        internal readonly sbyte[] DXY;

        internal int Length { get { return DXY.Length; } }


        #region Width  ========================================================
        /// <summary>
        /// Calculate the facet's width
        /// </summary>
        internal double Width()
        {
            int y, ymin, ymax;
            y = ymin = ymax = 0;

            for (int i = 1; i < DXY.Length; i += 2)
            {
                y += DXY[i];

                if (y < ymin) ymin = y;
                else if (y > ymax) ymax = y;
            }
            var w = (ymax - ymin);
            return (w < 2) ? 2 : w;
        }
        #endregion
    }
}
