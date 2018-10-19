using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ModelGraphSTD
{
    public class Selector
    {
        public Graph Graph;   // reference the graphs Node and Edge lists

        public Node PrevNode;
        public Edge PrevEdge;
        public HitLocation PrevLocation;

        public Node HitNode;
        public Edge HitEdge;
        public int HitBend;  // index of bend point (relative to edge.bends)
        public int HitIndex; // index of start of the hit segment (relative to edge.point)
        public (int x, int y) HitPoint; // the refined hit point location
        public HitLocation HitLocation; // hit location details

        public Edge BendEdge;     // when bending an edge, remember which edge it is
        public int BendIndex;     // when moving an bend point, remember which point it is

        public HashSet<Node> Nodes = new HashSet<Node>();   // interior nodes
        public HashSet<Edge> Edges = new HashSet<Edge>();   // interior edges
        public Dictionary<Edge, (int I1, int I2)> Points = new Dictionary<Edge, (int I1, int I2)>(); // chopped edge interior points

        public Extent Extent = new Extent(); // selector rectangle

        public List<Extent> Included = new List<Extent>();  // extents of included nodes
        public List<Extent> Occluded = new List<Extent>();  // extents of occluded nodes
        
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
        #endregion

        #region SelectorRectangle  ============================================
        public void StartPoint((int X, int Y) p) => Extent.Point1 = Extent.Point2 = p;
        public void NextPoint((int X, int Y) p) => Extent.Point2 = p;
        public (int X, int Y, int W, int H) Rectangle => (Extent.Xmin, Extent.Ymin, Extent.Width, Extent.Hieght);
        #endregion

        #region TryAdd  =======================================================
        public void TryAdd()
        {
            if (Extent.HasArea)
            {
                var count = Nodes.Count;
                foreach (var node in Graph.Nodes)
                {
                    if (Nodes.Contains(node)) continue;
                    if (Extent.Contains(node.Center)) Nodes.Add(node);
                }
                if (count != Nodes.Count)
                {
                    Edges.Clear();

                    var badBend = new HashSet<Edge>();
                    foreach (var e in Points)
                    {
                        var edge = e.Key;
                        if (Nodes.Contains(edge.Node1) && Nodes.Contains(edge.Node2)) badBend.Add(edge);
                        if (!Nodes.Contains(edge.Node1) && !Nodes.Contains(edge.Node2)) badBend.Add(edge);
                    }
                    foreach (var edge in badBend) { Points.Remove(edge); }

                    foreach (var edge in Graph.Edges)
                    {
                        if (Nodes.Contains(edge.Node1) && Nodes.Contains(edge.Node2))
                        {
                            Edges.Add(edge);
                        }
                        else if (edge.HasBends)
                        {
                            if (Nodes.Contains(edge.Node1) || Nodes.Contains(edge.Node2))
                            {
                                var j = 0;
                                var k = 0;
                                for (int i = edge.Tm1 + 1; i < edge.Tm2; i++)
                                {
                                    if (Extent.Contains(edge.Points[i]))
                                    {
                                        if (j == 0) j = i;
                                    }
                                    else
                                    {
                                        if (k == 0) k = i;
                                        break;
                                    }
                                }
                                if (j > 0 && k > 0) Points.Add(edge, (j, k));                                
                            }
                        }
                    }
                    UpdateExtents();
                }
            }
        }
        #endregion

        #region UpdateExtents  ================================================
        public void UpdateExtents()
        {
            Included.Clear();
            Occluded.Clear();
            foreach (var node in Nodes)
            {
                Included.Add(new Extent(node.Extent, GraphDefault.RegionExtentMargin));
            }
            foreach (var e in Points)
            {
                var edge = e.Key;
                var (j, k) = e.Value;
                var ext = new Extent(edge.Points[j]);
                for (int i = j + 1; i < k; i++)
                {
                    ext.Expand(edge.Points[j]);
                }
                ext.Expand(GraphDefault.RegionExtentMargin);
                Included.Add(ext);
            }
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
            HitLocation = HitLocation.Void;

            // test prev node
            if (PrevNode != null && PrevNode.HitTest(p))
            {
                var (hit, pnt) = PrevNode.RefinedHit(p);
                HitLocation |= hit;
                HitPoint = pnt;

                HitNode = PrevNode;
                return;  // we're done;
            }

            // test near by nodes
            var nearByNodes = Graph.HitMap.NearByNodes(p, 4);
            if (nearByNodes != null)
            {
                foreach (var node in nearByNodes)
                {
                    // eliminate unqualified nodes
                    if (!node.HitTest(p)) continue;

                    // now refine the hit test results
                    // node.RefineHitTest(p, ref HitLocation, ref HitPoint);
                    var (hit, pnt) = node.RefinedHit(p);
                    HitLocation |= hit;
                    HitPoint = pnt;

                    HitNode = node;
                    return;  // we are done;
                }
            }

            // test near by edges
            var nearByEdges = Graph.HitMap.NearByEdges(p, 8);
            if (nearByEdges != null)
            {
                foreach (var edge in nearByEdges)
                {
                    // eliminate unqualified edges
                    if (!edge.HitTest(p)) continue;

                    // now refine the hit test results
                    if (!edge.HitTest(p, ref HitLocation, ref HitBend, ref HitIndex, ref HitPoint)) continue;

                    HitEdge = edge;
                    return;  // we are done
                }
            }
        }
        #endregion

        #region Clear  ========================================================
        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
            Points.Clear();
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
                }
                else
                {
                    foreach (var ext in Included) { ext.Move(delta); }
                    foreach (var node in Nodes) { node.Move(delta); }
                    foreach (var edge in Edges) { edge.Move(delta); }
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
        public void ApplyGravity()
        {
            if ((HitLocation & HitLocation.Region) == 0)
            {
                if ((HitLocation & HitLocation.Node) != 0)
                {
                }
                HitLocation |= HitLocation.Region;
            }

            if ((HitLocation & HitLocation.Node) != 0)
            {
                if (Graph.Node_Edges.TryGetValue(HitNode, out List<Edge> edges))
                {
                   
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
                foreach (var ext in Included) { ext.RotateFlip(focus, flip); }
                foreach (var node in Nodes) { node.RotateFlip(focus, flip); }
                foreach (var edge in Edges) { edge.RotateFlip(focus, flip); }
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
            var edgeCopy = new List<EdgeParm>(Edges.Count + Points.Count);

            if (HitEdge != null)
            {
                edgeCopy.Add(new EdgeParm(HitEdge));
            }
            else if (HitNode != null)
            {
                nodeCopy.Add(new NodeParm(HitNode));
            }
            else
            {
                foreach (var node in Nodes) { nodeCopy.Add(new NodeParm(node)); }
                foreach (var edge in Edges) { edgeCopy.Add(new EdgeParm(edge)); }
            }
            Graph.PushSnapShot(new ParmCopy(nodeCopy, edgeCopy));
        }
        #endregion
    }
}
