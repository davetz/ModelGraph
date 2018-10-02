using System.Collections.Generic;
using System.Linq;

namespace ModelGraphSTD
{
    public class Selector
    {
        public Graph Graph;   // reference the graphs Node and Edge lists

        public List<Region> Regions = new List<Region>();

        public Node PrevNode;
        public Edge PrevEdge;
        public HitLocation PrevLocation;

        public Node HitNode;
        public Edge HitEdge;
        public int HitBend;  // index of bend point (relative to edge.bends)
        public int HitIndex; // index of start of the hit segment (relative to edge.point)
        public (int x, int y) HitPoint; // the refined hit point location
        public Region HitRegion; // hit this specific region
        public HitLocation HitLocation; // hit location details

        public Edge BendEdge;     // when bending an edge, remember which edge it is
        public int BendIndex;     // when moving an bend point, remember which point it is

        public List<EdgeCut> HitNodeEdgeCuts;  // edges cut by region(s)

        private bool _enableSnapShot;

        #region Constructor  ==================================================
        public Selector(Graph graph)
        {
            Graph = graph;
        }
        #endregion

        #region Properties  ===================================================
        public bool IsVoidHit => (HitLocation == HitLocation.Void);
        public bool IsNodeHit => ((HitLocation & HitLocation.Node) != 0);
        public bool IsEdgeHit => ((HitLocation & HitLocation.Edge) != 0);
        public bool IsRegionHit => ((HitLocation & HitLocation.Region) != 0);

        public bool IsChanged => (PrevNode != HitNode) || (PrevEdge != HitEdge) || (PrevLocation != HitLocation);

        public bool HitTop => ((HitLocation & HitLocation.Top) != 0);
        public bool HitLeft => ((HitLocation & HitLocation.Left) != 0);
        public bool HitRignt => ((HitLocation & HitLocation.Right) != 0);
        public bool HitBottom => ((HitLocation & HitLocation.Bottom) != 0);
        public bool HitCenter => ((HitLocation & HitLocation.Center) != 0);
        public bool HitSideOf => ((HitLocation & HitLocation.SideOf) != 0);

        public bool HitNearEnd1 => ((HitLocation & HitLocation.End1) != 0);
        public bool HitNearEnd2 => ((HitLocation & HitLocation.End2) != 0);
        public bool HitBendPoint => ((HitLocation & HitLocation.Bend) != 0);

        // Cached results of GetUnion for efficient repeated access 
        public HashSet<Node> Nodes = new HashSet<Node>();     // nodes inside the union of regions
        public HashSet<Edge> Edges = new HashSet<Edge>();     // edges completely inside the union of regions
        public HashSet<Edge> Chops = new HashSet<Edge>();     // edges with only one end inside the union of regions
        public List<EdgeCut> EdgeCuts = new List<EdgeCut>();  // cut edge points inside the union of regions
        #endregion

        #region TryAddRegion  =================================================
        public void TryAddRegion(Region region)
        {
            if (region.IsViable)
            {
                region.Nodes.Clear();
                foreach (var node in Graph.Nodes)
                {
                    if (region.HitTest(node.Center))
                    {
                        Nodes.Add(node);
                        region.Nodes.Add(node);
                    }
                }
                var anyHits = (region.Nodes.Count > 0);

                foreach (var edge in Graph.Edges)
                {
                    if (region.HitTest(edge, out int index1, out int index2, out bool isInterior))
                    {
                        anyHits = true;
                        if (isInterior)
                        {
                            Edges.Add(edge);
                        }
                        else
                        {
                            if (Chops.Contains(edge))
                            {
                                for (int i = 0; i < EdgeCuts.Count; i++)
                                {
                                    if (EdgeCuts[i].Edge != edge) continue;

                                    if (index1 < EdgeCuts[i].Index1) EdgeCuts[i] = new EdgeCut(edge, index1, EdgeCuts[i].Index2);
                                    if (index2 > EdgeCuts[i].Index2) EdgeCuts[i] = new EdgeCut(edge, EdgeCuts[i].Index1, index2);
                                    break;
                                }
                            }
                            else
                            {
                                Chops.Add(edge);
                                EdgeCuts.Add(new EdgeCut(edge, index1, index2));
                            }
                        }
                    }
                }

                if (anyHits)
                {
                    Regions.Add(region);

                    int last = EdgeCuts.Count - 1;
                    for (int i = last; i >= 0; i--)
                    {
                        var edge = EdgeCuts[i].Edge;
                        if (Edges.Contains(edge))
                        {
                            EdgeCuts.RemoveAt(i);
                            Chops.Remove(edge);
                        }
                    }
                }
            }
        }
        #endregion

        #region GetChangedItems  ==============================================
        public List<Item> GetChangedItems()
        {
            List<Item> items = null;
            if ((HitLocation & HitLocation.Region) != 0)
            {
                items = new List<Item>(Nodes.Count + Edges.Count + Chops.Count);
                foreach (var item in Nodes) { items.Add(item); }
                foreach (var item in Edges) { items.Add(item); }
                foreach (var item in Chops) { items.Add(item); }
            }
            else if (HitNode != null)
            {
                if (HitNodeEdgeCuts == null)
                {
                    items = new List<Item>(1) { HitNode };
                }
                else
                {
                    items = new List<Item>(1 + HitNodeEdgeCuts.Count) { HitNode };
                    foreach (var cut in HitNodeEdgeCuts) { items.Add(cut.Edge); }
                }
            }
            else if (HitEdge != null)
            {
                items = new List<Item> { HitEdge };
            }
            else
            {
                items = new List<Item>(0);
            }
            return items;
        }
        #endregion

        #region HitTest  ======================================================
        public void HitTest((int x, int y) p)
        {
            PrevNode = HitNode;
            PrevEdge = HitEdge;
            PrevLocation = HitLocation;

            // clear previous results
            HitNode = null;
            HitEdge = null;
            HitBend = -1;
            HitIndex = -1;
            HitPoint = p;
            HitRegion = null;
            HitNodeEdgeCuts = null;
            HitLocation = HitLocation.Void;

            // test all regions
            foreach (var region in Regions)
            {
                if (!region.HitTest(p)) continue;

                HitRegion = region;
                HitLocation |= HitLocation.Region;
                break;
            }

            // test prev node
            if (PrevNode != null && PrevNode.HitTest(p))
            {
                var (hit, pnt) = PrevNode.RefinedHit(p);
                HitLocation |= hit;
                HitPoint = pnt;

                HitNode = PrevNode;
                if (Graph.Node_Edges.TryGetValue(HitNode, out List<Edge> nodeEdges))
                {
                    var len = nodeEdges.Count;
                    HitNodeEdgeCuts = new List<EdgeCut>(len);
                    for (int i = 0; i < len; i++)
                    {
                        var edge = nodeEdges[i];
                        if (edge.Node1 == PrevNode)
                            HitNodeEdgeCuts.Add(new EdgeCut(edge, 0, edge.Tm1 + 1));
                        else
                            HitNodeEdgeCuts.Add(new EdgeCut(edge, edge.Tm2, edge.Points.Length));
                    }
                }
                return;  // we are done;
            }

            // test all nodes
            foreach (var node in Graph.Nodes)
            {
                // eliminate unqualified nodes
                if (!node.HitTest(p)) continue;

                // now refine the hit test results
                // node.RefineHitTest(p, ref HitLocation, ref HitPoint);
                var (hit, pnt) = node.RefinedHit(p);
                HitLocation |= hit;
                HitPoint = pnt;

                HitNode = node;
                if (Graph.Node_Edges.TryGetValue(HitNode, out List<Edge> nodeEdges))
                {
                    var len = nodeEdges.Count;
                    HitNodeEdgeCuts = new List<EdgeCut>(len);
                    for (int i = 0; i < len; i++)
                    {
                        var edge = nodeEdges[i];
                        if (edge.Node1 == node)
                            HitNodeEdgeCuts.Add(new EdgeCut(edge, 0, edge.Tm1 + 1));
                        else
                            HitNodeEdgeCuts.Add(new EdgeCut(edge, edge.Tm2, edge.Points.Length));
                    }
                }
                return;  // we are done;
            }

            // test all edges
            foreach (var edge in Graph.Edges)
            {
                // eliminate unqualified edges
                if (!edge.HitTest(p)) continue;

                // now refine the hit test results
                if (!edge.HitTest(p, ref HitLocation, ref HitBend, ref HitIndex, ref HitPoint)) continue;

                HitEdge = edge;
                return;  // we are done
            }
        }
        #endregion

        #region Clear  ========================================================
        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
            Chops.Clear();
            EdgeCuts.Clear();
            Regions.Clear();
        }
        #endregion

        #region Move  =========================================================
        public void Move((int X, int Y) delta)
        {
            if ((HitLocation & (HitLocation.Region | HitLocation.Node)) != 0)
            {
                if (_enableSnapShot) TakeSnapShot();

                if ((HitLocation & HitLocation.Region) == 0)
                {
                    HitNode?.Move(delta); //BUG - HitNode was Null !
                    if (HitNodeEdgeCuts != null)
                        foreach (var cut in HitNodeEdgeCuts) { cut.Edge.Move(delta, cut.Index1, cut.Index2); }
                }
                else
                {
                    foreach (var reg in Regions) { reg.Move(delta); }
                    foreach (var node in Nodes) { node.Move(delta); }
                    foreach (var edge in Edges) { edge.Move(delta); }
                    foreach (var cut in EdgeCuts) { cut.Edge.Move(delta, cut.Index1, cut.Index2); }
                }
            }
        }

        public void AdjustGraph()
        {
            Graph.AdjustGraph(this);
            _enableSnapShot = true;
        }
        #endregion

        #region Gravity  ======================================================
        internal void ApplyGravity()
        {
            if ((HitLocation & HitLocation.Node) != 0)
            {
                var nodeHash = new HashSet<Node>(Nodes); // working copy of node hash

                Node node1 = HitNode;   // focal node
                Node node2 = null;      // next focal node
                Edge edge = null;
                while (TryGetNextNodeEdge())
                {

                }

                bool TryGetNextNodeEdge()
                {
                    if (Graph.Node_Edges.TryGetValue(node1, out List<Edge> list))
                    {
                        foreach (var trialEdge in list)
                        {
                            if (nodeHash.Contains(edge.Node1) && nodeHash.Contains(edge.Node2))
                            {
                                edge = trialEdge; // take only one edge
                                node2 = (edge.Node1 == node1) ? edge.Node2 : edge.Node1;
                                nodeHash.Remove(node1); //this node has been traversed, so remove it from the work hash
                                return true;
                            }
                        }
                    }
                    nodeHash.Remove(node1); //this node has been traversed, so remove it from the work hash
                    return false;
                }
            }
        }
        #endregion

        #region FlipRotate  ===================================================
        public void Rotate() => RotateFlip(FlipRotate.RotateClockWise);
        public void RotateFlip(FlipRotate flip) => RotateFlip(HitPoint, flip);
        public void RotateFlip((int X, int Y) focus, FlipRotate flip)
        {
            if ((HitLocation & HitLocation.Region) != 0)
            {
                foreach (var reg in Regions) { reg.RotateFlip(focus, flip); }
                foreach (var node in Nodes) { node.RotateFlip(focus, flip); }
                foreach (var edge in Edges) { edge.RotateFlip(focus, flip); }
                foreach (var cut in EdgeCuts) { cut.Edge.RotateFlip(focus, flip, cut.Index1, cut.Index2); }
            }
        }
        #endregion

        #region Align  ========================================================
        public void AlignVertical()
        {
            if (HitNode != null)
            {
                var x = HitNode.X;
                TakeSnapShot();
                foreach (var node in Nodes)
                {
                    node.AlignVertical(x);
                }
            }
        }
        public void AlignHorizontal()
        {
            if (HitNode != null)
            {
                var y = HitNode.Y;
                TakeSnapShot();
                foreach (var node in Nodes)
                {
                    node.AlignHorizontal(y);
                }
            }
        }
        #endregion

        #region TakeSnapShot  =================================================
        public void TakeSnapShot()
        {
            _enableSnapShot = false;

            var nodeCopy = new List<NodeParm>(Nodes.Count);
            var edgeCopy = new List<EdgeParm>(Edges.Count + Chops.Count);

            if (HitEdge != null)
            {
                edgeCopy.Add(new EdgeParm(HitEdge));
            }
            else if (HitNode != null)
            {
                nodeCopy.Add(new NodeParm(HitNode));
                if (HitNodeEdgeCuts != null)
                {
                    foreach (var cut in HitNodeEdgeCuts)
                    {
                        edgeCopy.Add(new EdgeParm(cut.Edge));
                    }
                }
            }
            else
            {
                foreach (var node in Nodes) { nodeCopy.Add(new NodeParm(node)); }
                foreach (var edge in Edges) { edgeCopy.Add(new EdgeParm(edge)); }
                foreach (var edge in Chops) { edgeCopy.Add(new EdgeParm(edge)); }
            }
            Graph.PushSnapShot(new ParmCopy(nodeCopy, edgeCopy));
        }
        #endregion
    }
}
