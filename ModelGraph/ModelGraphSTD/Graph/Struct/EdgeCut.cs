
namespace ModelGraphSTD
{
    public struct EdgeCut
    {
        internal Edge Edge;     // the edge that was cut by the region
        internal int Index1;    // bend1 index inside a region
        internal int Index2;    // bend2 index inside some other region
        internal EdgeCut(Edge edge, int index1, int index2)
        {
            Edge = edge;
            Index1 = index1;
            Index2 = index2;
        }
    }
}
