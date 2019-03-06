using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

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
        public (float x, float y) HitPoint; // the refined hit point location
        public HitLocation HitLocation; // hit location details

        public Edge BendEdge;     // when bending an edge, remember which edge it is
        public int BendIndex;     // when moving an bend point, remember which point it is

        public HashSet<Node> Nodes = new HashSet<Node>();   // interior nodes
        public HashSet<Edge> Edges = new HashSet<Edge>();   // interior edges
        public Dictionary<Edge, (int I1, int I2)> Points = new Dictionary<Edge, (int I1, int I2)>(); // chopped edge interior points

        public Extent Extent = new Extent(); // selector rectangle

        public List<Extent> Regions = new List<Extent>();  // extents of included nodes
        public List<Extent> Occluded = new List<Extent>();  // extents of occluded nodes
        
        private bool _enableSnapshot = true;

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
        public void StartPoint((float X, float Y) p) => Extent.Point1 = Extent.Point2 = p;
        public void NextPoint((float X, float Y) p) => Extent.Point2 = p;
        public (float X, float Y, float W, float H) Rectangle => (Extent.Xmin, Extent.Ymin, Extent.Width, Extent.Hieght);
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

            var fullExtents = new List<(float x1, float y1, float x2, float y2, float dx, float dy, Node node)>(Nodes.Count);
            #region PopulateFullExtents  ======================================
            foreach (var node in Graph.Nodes)
            {
                if (region.Contains(node.Center))
                {
                    var fex = node.FullExtent(GraphDefault.RegionExtentMargin);
                    if (Nodes.Contains(node))
                    {
                        fullExtents.Add(fex);

                        if (fex.DX > dx) dx = (int)fex.DX;
                        if (fex.DY > dy) dy = (int)fex.DY;
                    }
                    else
                        excluded.Add(node);
                }
            }
            #endregion

            var nr = (int)(1 + region.Width / dx);
            var nc = (int)(1 + region.Hieght / dy);

            var grid = new Dictionary<(int r, int c), List<Node>>();
            #region PopulateGrid  =============================================
            var x0 = (int)region.Xmin;
            var y0 = (int)region.Ymin;

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
        public void HitTest((float x, float y) p)
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
                return;  // we are done;
            }

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
            Points.Clear();
            Regions.Clear();
            Occluded.Clear();
        }
        #endregion

        #region Move  =========================================================
        public void Move((float X, float Y) delta)
        {
            if (IsRegionHit || IsNodeHit)
            {
                TakeSnapshot();

                if (IsRegionHit)
                {
                    foreach (var ext in Regions) { ext.Move(delta); }
                    foreach (var node in Nodes) { node.Move(delta); }
                    foreach (var edge in Edges) { edge.Move(delta); }
                    UpdateExtents();
                }
                else if (IsNodeHit)
                {
                    HitNode.Move(delta);
                    UpdateExtents();
                }
                Graph.AdjustGraph(this);
            }
        }
        #endregion

        #region Gravity  ======================================================
        public void GravityInside()
        {
           if (IsNodeHit && IsRegionHit)
           {
                var d = Graph.GraphX.TerminalLength + 1;
                var ds = IsVertical() ? (0, d) : (d, 0);



                UpdateExtents();
           }
        }
        public void GravityDisperse()
        {
            UpdateExtents();
        }

        #region IsForward  ====================================================
        bool IsForward(Node n1, Node n2, (int x, int y) d)
        {
            if (d.y == 0)
            {
                var dx = n2.X - n1.X;
                return dx > 0;
            }
            else
            {
                var dy = n2.Y - n1.Y;
                return dy > 0;
            }
        }
        #endregion

        #region IsVertical  ===================================================
        private bool IsVertical()
        {
            var e = new Extent();
            foreach (var node in Nodes)
            {
                e.Expand(node.Center);
            }
            return e.IsVertical;
        }
        #endregion
        #endregion

        #region Rotate  =======================================================
        public void RotateLeft45()
        {
            Rotate(XYTuple.RotateLeft45Matrix(HitPoint));
        }
        public void RotateRight45()
        {
            Rotate(XYTuple.RotateRight45Matrix(HitPoint));
        }
        public void RotateLeft90()
        {
            Rotate(XYTuple.RotateLeft90Matrix(HitPoint));
        }
        public void RotateRight90()
        {
            Rotate(XYTuple.RotateRight90Matrix(HitPoint));
        }
        public void Rotate(Matrix3x2 mx)
        {
            TakeSnapshot();
            if (IsRegionHit)
            {
                foreach (var node in Nodes) { node.Center = XYTuple.Transform(mx, node.Center); }
                foreach (var edge in Edges)
                {
                    if (edge.HasBends)
                    {
                        for (int i = 0; i < edge.Bends.Length; i++)
                        {
                            edge.Bends[i] = XYTuple.Transform(mx, edge.Bends[i]);
                        }
                    }
                }
            }
            UpdateExtents();
        }
        #endregion

        #region Align  ========================================================
        public void AlignVert()
        {
            var x = HitPoint.x;
            TakeSnapshot();
            foreach (var node in Nodes)
            {
                node.AlignVert(x);
            }
            UpdateExtents();
        }
        public void AlignHorz()
        {
            var y = HitPoint.y;
            TakeSnapshot();
            foreach (var node in Nodes)
            {
                node.AlignHorz(y);
            }
            UpdateExtents();
        }
        #endregion

        #region Flip  =========================================================
        public void FlipHorz()
        {
            var x = HitPoint.x;
            TakeSnapshot();
            foreach (var node in Nodes)
            {
                node.FlipHorz(x);
            }
            UpdateExtents();
        }
        public void FlipVert()
        {
            var y = HitPoint.y;
            TakeSnapshot();
            foreach (var node in Nodes)
            {
                node.FlipVert(y);
            }
            UpdateExtents();
        }
        #endregion

        #region Snapshot  =====================================================
        public void EnableSnapshot() => _enableSnapshot = true;

        public void TakeSnapshot()
        {
            if (_enableSnapshot)
            {
                _enableSnapshot = false;
                Graph.TakeSnapshot(this);
            }
        }
        #endregion
    }
}
