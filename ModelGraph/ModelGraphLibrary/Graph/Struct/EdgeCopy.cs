namespace ModelGraphSTD
{
    public struct EdgeCopy
    {
        internal Edge Edge;
        internal XYPoint[] Points;
        internal EdgeX Core;

        internal EdgeCopy(Edge edge)
        {
            Edge = edge;
            Core = edge.Core;
            Points = edge.Points;
        }
        internal void Restore()
        {
            Edge.Points = Points;
            Edge.Core = Core;
        }
    }
}
