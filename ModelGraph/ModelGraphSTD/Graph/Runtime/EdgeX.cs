﻿namespace ModelGraphSTD
{
    public class EdgeX
    {
        public (int X, int Y)[] Bends;
        public Face Face1;
        public Face Face2;
        public FacetOf Facet1;
        public FacetOf Facet2;

        public bool HasBends => (Bends != null && Bends.Length > 0);
    }
}
