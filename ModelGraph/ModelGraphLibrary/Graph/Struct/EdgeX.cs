namespace ModelGraphLibrary
{
    public struct EdgeX
    {
        internal XYPoint[] Bends;
        internal Face Face1;
        internal Face Face2;
        internal FacetOf Facet1;
        internal FacetOf Facet2;

        internal bool HasBends => (Bends != null && Bends.Length > 0);
    }
}
