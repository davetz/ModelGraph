namespace ModelGraphSTD
{
    public class EdgeCopy
    {
        internal readonly Edge Edge;
        private readonly (int X, int Y)[] _points;
        private readonly ((int X, int Y)[] Bends, Face Face1, Face Face2, FacetOf Facet1, FacetOf Facet2, Attach Attatch1, Attach Attatch2) _parms;

        internal EdgeCopy(Edge edge) { Edge = edge; _points = edge.Points; _parms = edge.Parms; }
        internal void Restore() { Edge.Points = _points; Edge.Parms = _parms; }
    }
}
