﻿using System.Collections.Generic;
using System.Linq;

namespace ModelGraphLibrary
{
    public class Selector
    {
        internal Graph Graph;   // reference the graphs Node and Edge lists

        internal List<Region> Regions = new List<Region>();

        internal Node PrevNode;
        internal Edge PrevEdge;
        internal HitLocation PrevLocation;

        internal Node HitNode; // index of node
        internal Edge HitEdge; // index of edge
        internal int HitBend;  // index of bend point (relative to edge.Core.bends)
        internal int HitIndex; // index of start of the hit segment (relative to edge.point)
        internal XYPoint HitPoint; // the refined hit point location
        internal Region HitRegion; // hit this specific region
        internal HitLocation HitLocation; // nuanced hit location details

        internal Edge BendEdge;     // when bending an edge, remember which edge it is
        internal int BendIndex;     // when moving an bend point, remember which point it is

        internal EdgeCut[] HitNodeEdgeCuts;  // edges cut by region(s)

        private bool _enableSnapShot;

        #region Constructor  ==================================================
        internal Selector(Graph graph)
        {
            Graph = graph;
        }
        #endregion

        #region Properties  ===================================================
        internal bool IsVoidHit { get { return (HitLocation == HitLocation.Void); } }
        internal bool IsNodeHit { get { return ((HitLocation & HitLocation.Node) != 0); } }
        internal bool IsEdgeHit { get { return ((HitLocation & HitLocation.Edge) != 0); } }
        internal bool IsRegionHit { get { return ((HitLocation & HitLocation.Region) != 0); } }

        internal bool IsChanged { get { return PrevNode != HitNode || PrevEdge != HitEdge || PrevLocation != HitLocation; } }

        internal bool HitTop { get { return ((HitLocation & HitLocation.Top) != 0); } }
        internal bool HitLeft { get { return ((HitLocation & HitLocation.Left) != 0); } }
        internal bool HitRignt { get { return ((HitLocation & HitLocation.Right) != 0); } }
        internal bool HitBottom { get { return ((HitLocation & HitLocation.Bottom) != 0); } }
        internal bool HitCenter { get { return ((HitLocation & HitLocation.Center) != 0); } }
        internal bool HitSideOf { get { return ((HitLocation & HitLocation.SideOf) != 0); } }

        internal bool HitNearEnd1 { get { return ((HitLocation & HitLocation.End1) != 0); } }
        internal bool HitNearEnd2 { get { return ((HitLocation & HitLocation.End2) != 0); } }
        internal bool HitBendPoint { get { return ((HitLocation & HitLocation.Bend) != 0); } }

        // Cached results of GetUnion for efficient repeated access 
        internal HashSet<Node> Nodes = new HashSet<Node>();     // nodes inside the union of regions
        internal HashSet<Edge> Edges = new HashSet<Edge>();     // edges completely inside the union of regions
        internal HashSet<Edge> Chops = new HashSet<Edge>();     // edges with only one end inside the union of regions
        internal List<EdgeCut> EdgeCuts = new List<EdgeCut>();  // cut edge points inside the union of regions
        #endregion

        #region TryAddRegion  =================================================
        internal void TryAddRegion(Region region)
        {
            if (region.IsViable)
            {
                region.Nodes.Clear();
                foreach (var node in Graph.Nodes)
                {
                    if (region.HitTest(node.Core.Center))
                    {
                        Nodes.Add(node);
                        region.Nodes.Add(node);
                    }
                }
                var anyHits = (region.Nodes.Count > 0);

                int index1, index2;
                bool isInterior;

                foreach (var edge in Graph.Edges)
                {
                    if (region.HitTest(edge, out index1, out index2, out isInterior))
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
        internal Item[] GetChangedItems()
        {
            Item[] items = null;
            if ((HitLocation & HitLocation.Region) != 0)
            {
                var n = Nodes.Count + Edges.Count + Chops.Count;
                items = new Item[n];
                int i = 0;
                foreach (var item in Nodes) { items[i++] = item; }
                foreach (var item in Edges) { items[i++] = item; }
                foreach (var item in Chops) { items[i++] = item; }
            }
            else if (HitNode != null)
            {
                if (HitNodeEdgeCuts == null)
                {
                    items = new Item[] { HitNode };
                }
                else
                {
                    var n = 1 + HitNodeEdgeCuts.Length;
                    items = new Item[n];
                    items[0] = HitNode;
                    for (int i = 0, j = 1; j < n; i++, j++)
                    {
                        items[j] = HitNodeEdgeCuts[i].Edge;
                    }
                }
            }
            else if (HitEdge != null)
            {
                items = new Item[] { HitEdge };
            }
            else
            {
                items = new Item[0];
            }
            return items;
        }
        #endregion

        #region HitTest  ======================================================
        internal void HitTest(XYPoint p)
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
            if (PrevNode != null && PrevNode.Core.HitTest(p))
            {
                var r = PrevNode.Core.RefinedHit(p);
                HitLocation |= r.hit;
                HitPoint = r.pnt;

                HitNode = PrevNode;
                List<Edge> nodeEdges;
                if (Graph.Node_Edges.TryGetValue(HitNode, out nodeEdges))
                {
                    var len = nodeEdges.Count;
                    HitNodeEdgeCuts = new EdgeCut[len];
                    for (int i = 0; i < len; i++)
                    {
                        var edge = nodeEdges[i];
                        if (edge.Node1 == PrevNode)
                            HitNodeEdgeCuts[i] = new EdgeCut(edge, 0, edge.Tm1 + 1);
                        else
                            HitNodeEdgeCuts[i] = new EdgeCut(edge, edge.Tm2, edge.Points.Length);
                    }
                }
                return;  // we are done;
            }

            // test all nodes
            foreach (var node in Graph.Nodes)
            {
                // quick eliminate unqualified nodes
                if (!node.Core.HitTest(p)) continue;

                // now refine the hit test results
                //node.RefineHitTest(p, ref HitLocation, ref HitPoint);
                var r = node.Core.RefinedHit(p);
                HitLocation |= r.hit;
                HitPoint = r.pnt;

                HitNode = node;
                List<Edge> nodeEdges;
                if (Graph.Node_Edges.TryGetValue(HitNode, out nodeEdges))
                {
                    var len = nodeEdges.Count;
                    HitNodeEdgeCuts = new EdgeCut[len];
                    for (int i = 0; i < len; i++)
                    {
                        var edge = nodeEdges[i];
                        if (edge.Node1 == node)
                            HitNodeEdgeCuts[i] = new EdgeCut(edge, 0, edge.Tm1 + 1);
                        else
                            HitNodeEdgeCuts[i] = new EdgeCut(edge, edge.Tm2, edge.Points.Length);
                    }
                }
                return;  // we are done;
            }

            // test all edges
            foreach (var edge in Graph.Edges)
            {
                // quickly eliminate unqualified edges
                if (!edge.HitTest(p)) continue;

                // now refine the hit test results
                if (!edge.HitTest(p, ref HitLocation, ref HitBend, ref HitIndex, ref HitPoint)) continue;

                HitEdge = edge;
                return;  // we are done
            }
        }
        #endregion

        #region Clear  ========================================================
        internal void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
            Chops.Clear();
            EdgeCuts.Clear();
            Regions.Clear();
        }
        #endregion

        #region Move  =========================================================
        internal void Move(XYPoint delta)
        {
            if (_enableSnapShot) TakeSnapShot();

            if ((HitLocation & HitLocation.Region) == 0)
            {
                HitNode.Move(delta);
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

        internal void AdjustGraph()
        {
            Graph.AdjustGraph(this);
            _enableSnapShot = true;
        }
        #endregion

        #region Align  ========================================================
        internal void AlignVertical()
        {
            if (HitNode != null)
            {
                var x = HitNode.Core.X;
                TakeSnapShot();
                foreach (var node in Nodes)
                {
                    node.Core.AlignVertical(x);
                }
            }
        }
        internal void AlignHorizontal()
        {
            if (HitNode != null)
            {
                var y = HitNode.Core.Y;
                TakeSnapShot();
                foreach (var node in Nodes)
                {
                    node.Core.AlignHorizontal(y);
                }
            }
        }
        #endregion

        #region TakeSnapShot  =================================================
        internal void TakeSnapShot()
        {
            _enableSnapShot = false;

            var nodeCopy = new List<NodeCopy>(Nodes.Count);
            var edgeCopy = new List<EdgeCopy>(Edges.Count);

            if (HitEdge != null)
            {
                edgeCopy.Add(new EdgeCopy(HitEdge));
            }
            else if (HitNode != null)
            {
                nodeCopy.Add(new NodeCopy(HitNode));
                if (HitNodeEdgeCuts != null)
                {
                    foreach (var cut in HitNodeEdgeCuts)
                    {
                        edgeCopy.Add(new EdgeCopy(cut.Edge));
                    }
                }
            }
            else
            {
                foreach (var node in Nodes)
                {
                    nodeCopy.Add(new NodeCopy(node));
                }
                foreach (var edge in Edges)
                {
                    edgeCopy.Add(new EdgeCopy(edge));
                }
                foreach (var edge in Chops)
                {
                    edgeCopy.Add(new EdgeCopy(edge));
                }
            }
            Graph.PushSnapShot(new ParmCopy(nodeCopy.ToArray(), edgeCopy.ToArray()));
        }
        #endregion
    }
}
