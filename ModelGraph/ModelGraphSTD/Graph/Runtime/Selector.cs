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
        public Extent HitRegion;
        public (int x, int y) HitPoint; // the refined hit point location
        public HitLocation HitLocation; // hit location details

        public Edge BendEdge;     // when bending an edge, remember which edge it is
        public int BendIndex;     // when moving an bend point, remember which point it is

        public HashSet<Node> Nodes = new HashSet<Node>();   // interior nodes
        public HashSet<Edge> Edges = new HashSet<Edge>();   // interior edges
        public Dictionary<Edge, (int I1, int I2)> Points = new Dictionary<Edge, (int I1, int I2)>(); // chopped edge interior points

        public Extent Extent = new Extent(); // selector rectangle

        public List<Extent> Regions = new List<Extent>();  // extents of included nodes
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

        public bool IsTopHit => ((HitLocation & HitLocation.Top) != 0);
        public bool IsLeftHit => ((HitLocation & HitLocation.Left) != 0);
        public bool IsRigntHit => ((HitLocation & HitLocation.Right) != 0);
        public bool IsBottomHit => ((HitLocation & HitLocation.Bottom) != 0);
        public bool IsCenterHit => ((HitLocation & HitLocation.Center) != 0);
        public bool IsSideHit => ((HitLocation & HitLocation.SideOf) != 0);

        public bool IsEnd1Hit => ((HitLocation & HitLocation.End1) != 0);
        public bool IsEnd2Hit => ((HitLocation & HitLocation.End2) != 0);
        public bool IsBendHit => ((HitLocation & HitLocation.Bend) != 0);
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
            var region = Extent.Create(Nodes, GraphDefault.RegionExtentMargin);

            var dx = int.MinValue;
            var dy = int.MinValue;
            var excluded = new List<Node>(Nodes.Count);

            var fullExtents = new List<(int x1, int y1, int x2, int y2, int dx, int dy, Node node)>(Nodes.Count);
            #region PopulateFullExtents  ======================================
            foreach (var node in Graph.Nodes)
            {
                if (region.Contains(node.Center))
                {
                    var fex = node.FullExtent(GraphDefault.RegionExtentMargin);
                    if (Nodes.Contains(node))
                    {
                        fullExtents.Add(fex);

                        if (fex.DX > dx) dx = fex.DX;
                        if (fex.DY > dy) dy = fex.DY;
                    }
                    else
                        excluded.Add(node);
                }
            }
            #endregion

            var nr = 1 + region.Width / dx;
            int nc = 1 + region.Hieght / dy;

            var grid = new Dictionary<(int r, int c), List<Node>>();
            #region PopulateGrid  =============================================
            var x0 = region.Xmin;
            var y0 = region.Ymin;

            foreach (var fex in fullExtents)
            {
                var nd = fex.node;
                for (int r = 0, xr = x0; r < nr; r++, xr += dx)
                {
                    var x = nd.X;
                    if (x < xr) continue;
                    if (x > xr + dx) continue;
                    for (int c = 0, yc = y0; c < nc; c++, yc += dy)
                    {
                        var y = nd.Y;
                        if (y < yc) continue;
                        if (y > yc + dy) continue;

                        if (grid.TryGetValue((r, c), out List<Node> list))
                            list.Add(nd);
                        else
                            grid[(r, c)] = new List<Node>(2) { nd };
                    }
                }
            }
            #endregion

            var vertKeys = new List<(int, int)>();
            var horzKeys = new List<(int, int)>();

            var vertNodes = new List<Node>();
            var horzNodes = new List<Node>();

            var vert = new List<Node[]>();
            var horz = new List<Node[]>();
            #region Populate<vert, horz>  =====================================
            while (grid.Count > 0)
            {
                var bestScore = 0;
                var bestKey = (0, 0);
                foreach (var key in grid.Keys)
                {
                    var score = VertScan(key) + HorzScan(key);

                    if (score <= bestScore) continue;
                    bestScore = score;
                    bestKey = key;                    
                }

                var vs = VertScan(bestKey);
                var hs = HorzScan(bestKey);
                if (hs < vs)
                {
                    vert.Add(vertNodes.ToArray());
                    foreach (var key in vertKeys)
                    {
                        grid.Remove(key);
                    }
                }
                else
                {
                    horz.Add(horzNodes.ToArray());
                    foreach (var key in horzKeys)
                    {
                        grid.Remove(key);
                    }
                }
            }
            #endregion

            Regions.Clear();
            #region PopulateIncluded  =========================================
            foreach (var h in horz)
            {
                Regions.Add(Extent.Create(h, GraphDefault.RegionExtentMargin));
            }
            foreach (var v in vert)
            {
                Regions.Add(Extent.Create(v, GraphDefault.RegionExtentMargin));
            }
            foreach (var g in grid)
            {
                if (g.Value.Count == 0) continue;
                Regions.Add(Extent.Create(g.Value, GraphDefault.RegionExtentMargin));
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
                Regions.Add(ext);
            }
            #endregion

            Occluded.Clear();
            #region PopulateOccluded  =========================================
            foreach (var nd in excluded)
            {
                foreach (var ex in Regions)
                {
                    if (ex.Contains(nd.Center))
                        Occluded.Add(new Extent(nd.Extent, GraphDefault.RegionExtentMargin));
                }
            }
            #endregion

            #region VertScan  =================================================
            int VertScan((int r, int c) key)
            {
                var c = key.c;

                vertKeys.Clear();
                vertNodes.Clear();

                for (int r = 0; r < nr; r++)
                {
                    if (grid.TryGetValue((r, c), out List<Node> nodes))
                    {
                        vertKeys.Add((r, c));
                        vertNodes.AddRange(nodes);
                    }
                }
                return vertKeys.Count + vertNodes.Count;
            }
            #endregion

            #region HorzScan  =================================================
            int HorzScan((int r, int c) key)
            {
                var r = key.r;

                horzKeys.Clear();
                horzNodes.Clear();

                for (int c = 0; c < nc; c++)
                {
                    if (grid.TryGetValue((r, c), out List<Node> nodes))
                    {
                        horzKeys.Add((r, c));
                        horzNodes.AddRange(nodes);
                    }
                }
                return horzKeys.Count + horzNodes.Count;
            }
            #endregion

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

            //test regions
            foreach (var r in Regions)
            {
                if (r.Contains(p))
                {
                    HitRegion = r;
                    HitLocation |= HitLocation.Region;
                    break;
                }
            }

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
            Regions.Clear();
            Occluded.Clear();
        }
        #endregion

        #region Move  =========================================================
        public void Move((int X, int Y) delta)
        {
            if (IsRegionHit || IsNodeHit)
            {
                if (_enableSnapShot) TakeSnapShot();

                if (IsNodeHit)
                {
                    HitNode.Move(delta);
                }
                else
                {
                    foreach (var ext in Regions) { ext.Move(delta); }
                    foreach (var node in Nodes) { node.Move(delta); }
                    foreach (var edge in Edges) { edge.Move(delta); }
                }
                AdjustGraph();
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
            if (IsNodeHit && !IsRegionHit)
            {
                var tm = Graph.GraphX.TerminalLength;

                Nodes.Add(HitNode);
                HitLocation |= HitLocation.Region;
                if (Graph.Node_Edges.TryGetValue(HitNode, out List<Edge> edges))
                {
                    foreach (var edge in edges)
                    {
                        var other = (edge.Node1 == HitNode) ? edge.Node2 : edge.Node1;
                        if (Nodes.Contains(other)) continue;
                        if (Graph.Node_Edges.TryGetValue(other, out List<Edge> otherEdges) && otherEdges.Count > 2) continue;

                        var ds = (HitNode.IsSymbolX && other.IsSymbolX) ? 2 * tm : tm;
                        Nodes.Add(other);
                        var (x1, y1) = HitNode.Center;
                        var (x2, y2) = other.Center;
                        var dx = x2 - x1;
                        var dy = y2 - y1;
                        if (dx * dx > dy * dy)
                        {
                            other.Y = HitNode.Y;
                            other.X = (dx > 0) ? HitNode.X + ds + HitNode.DX : HitNode.X - ds - HitNode.DX;
                        }
                        else
                        {
                            other.X = HitNode.X;
                            other.Y = (dy > 0) ? HitNode.Y + ds + HitNode.DX: HitNode.Y - ds - HitNode.DY;
                        } 
                    }
                }                
            }

            if (IsNodeHit && IsRegionHit)
            {
                if (Graph.Node_Edges.TryGetValue(HitNode, out List<Edge> edges))
                {
                   
                }
            }
            UpdateExtents();
        }
        #endregion

        #region FlipRotate  ===================================================
        public void Rotate() => RotateFlip(FlipRotate.RotateClockWise);
        public void RotateFlip(FlipRotate flip) => RotateFlip(HitPoint, flip);
        public void RotateFlip((int X, int Y) focus, FlipRotate flip)
        {
            if (IsRegionHit)
            {
                foreach (var ext in Regions) { ext.RotateFlip(focus, flip); }
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
