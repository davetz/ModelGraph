namespace ModelGraphLibrary
{
    public struct EdgeX
    {
        public XYPoint[] Bends;
        public Face Face1;
        public Face Face2;
        public FacetOf Facet1;
        public FacetOf Facet2;

        public bool HasBends => (Bends != null && Bends.Length > 0);
    }
}
