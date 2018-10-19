
using System.Collections.Generic;
using System.Diagnostics;

namespace ModelGraphSTD
{
    public class Region
    {
        public Extent Normal;
        public Extent Extent;
        public HashSet<Node> Nodes = new HashSet<Node>();   // nodes inside the region
        public List<Extent> DotExtents = new List<Extent>(0);

        #region Constructor  ==================================================
        public Region((int X, int Y) p)
        {
            Extent = new Extent(p);
            Normal = new Extent(p);
        }
        public Region(Node node)
        {
            Nodes.Add(node);
            Normal = Extent = new Extent();
            Extent.SetExtent(Nodes, GraphDefault.RegionExtentMargin);
        }
        #endregion

        #region Properties  ===================================================
        public bool AnyHits { get { return (Nodes.Count > 0); } }
        public bool IsHorizontal => Extent.IsHorizontal;
        #endregion

        #region Add  ==========================================================
        // try to add another point to the region
        public void Add((int X, int Y) p)
        {
            Extent.Expand(p);
            Normal.Point2 = p;
        }
        #endregion

        #region Move  =========================================================
        public void Move((int X, int Y) delta)
        {
            Extent.Move(delta);
        }
        #endregion

        #region RotateFlip  ===================================================
        public void RotateFlip((int X, int Y) focus, FlipRotate flip)
        {
            Extent.RotateFlip(focus, flip);
        }
        #endregion

        #region HitTest  ======================================================
        public bool HitTest((int X, int Y) p) =>  Extent.Contains(p);

        public (bool gotHit, bool isInterior, int index1, int index2)  HitTest(Edge edge)
        {
            if (Nodes.Contains(edge.Node1) && Nodes.Contains(edge.Node2)) return (true, true, 0, 0);

            if (Nodes.Contains(edge.Node1))
            {
                if (edge.HasBends)
                {
                    for (int i = edge.Tm1 + 1; i < edge.Tm2; i++)
                    {
                        if (Extent.Contains(edge.Points[i])) continue;
                        return (true, false, 0, i);
                    }
                }
                return (true, false, 0, edge.Tm2);
            }
            else if (Nodes.Contains(edge.Node2))
            {
                if (edge.HasBends)
                {
                    for (int i = edge.Tm2 - 1; i > edge.Tm1; i--)
                    {
                        if (Extent.Contains(edge.Points[i])) continue;
                        return (true, false, i, edge.Points.Length);
                    }
                }
                return (true, false, edge.Tm1 + 1, edge.Points.Length);
            }
            return (false, false, 0, 0);
        }
        #endregion

        #region SetExtent  ====================================================
        internal void SetExtent()
        {
            Extent.SetExtent(Nodes, GraphDefault.RegionExtentMargin);
            DotExtents.Clear();
        }
        #endregion
    }
}
