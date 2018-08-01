namespace ModelGraphSTD
{
    public struct EdgeParm
    {
        internal readonly Edge Edge;
        private readonly ((int X, int Y)[] Points, (int X, int Y)[] Bends, Face Face1, Face Face2, FacetOf Facet1, FacetOf Facet2, Attach Attatch1, Attach Attatch2) _parms;

        internal EdgeParm(Edge edge) { Edge = edge; _parms = edge.Parms; }
        internal void Restore() { Edge.Parms = _parms; }
    }
}
