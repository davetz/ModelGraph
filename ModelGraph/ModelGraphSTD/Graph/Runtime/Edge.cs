using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphSTD
{
    public class Edge : Item
    {

        private Item _key;
        public Node Node1;
        public Node Node2;

        public (int X, int Y)[] Points;
        public Extent Extent = new Extent(); // all points are withing this extent
        public short Tm1; // index of terminal point 1
        public short Bp1; // index of closes bend point after Tm1 (to the right) 
        public short Bp2; // index of closes bend point after Tm2 (to the left)
        public short Tm2; // index of terminal point 2

        public EdgeX Core = new EdgeX();


        #region Constructors  =================================================
        internal Edge(Item key)
        {
            Owner = null;
            Trait = Trait.Edge;
            _key = key;
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal Graph Graph { get { return Owner as Graph; } }
        internal GraphX GraphX { get { return (Owner == null) ? null : Owner.Owner as GraphX; } }
        internal QueryX QueryX { get { return _key as QueryX; } }

        internal Connect Connect1 { get { return QueryX.Connect1; } }
        internal Connect Connect2 { get { return QueryX.Connect2; } }
        #endregion


        #region Move  =========================================================
        internal void Move((int X, int Y) delta)
        {
            if (Core.HasBends)
            {
                for (int i = 0; i < Core.Bends.Length; i++)
                {
                    Core.Bends[i].X = Core.Bends[i].X + delta.X;
                    Core.Bends[i].Y = Core.Bends[i].Y + delta.Y;
                }
            }
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].X = Points[i].X + delta.X;
                Points[i].Y = Points[i].Y + delta.Y;
            }
        }
        internal void Move((int X, int Y) delta, int index1, int index2)
        {
            if (Core.HasBends)
            {
                for (int i = index1; i < index2; i++)
                {
                    var j = i - Tm1 - 1;
                    if (j >= 0 && j < Core.Bends.Length)
                    {
                        Core.Bends[j].X = Core.Bends[j].X + delta.X;
                        Core.Bends[j].Y = Core.Bends[j].Y + delta.Y;
                    }
                }
            }
            for (int i = index1; i < index2; i++)
            {
                Points[i].X = Points[i].X + delta.X;
                Points[i].Y = Points[i].Y + delta.Y;
            }
        }
        #endregion

        #region Flip, Rotate  =================================================
        internal void Rotate(int x, int y)
        {
            var len = Points.Length;
            for (int i = 0; i < len;)
            {
                Points[i] = XYPoint.Rotate(Points[i], (x, y));
            }

            len = (Core.Bends == null) ? 0 : Core.Bends.Length;
            for (int i = 0; i < len;)
            {
                Core.Bends[i] = XYPoint.Rotate(Core.Bends[i], (x, y));
            }
        }

        internal void VerticalFlip(int y)
        {
            var len = Points.Length;
            for (int i = 0; i < len;)
            {
                Points[i] = XYPoint.VerticalFlip(Points[i], y);
            }

            len = (Core.Bends == null) ? 0 : Core.Bends.Length;
            for (int i = 0; i < len;)
            {
                Core.Bends[i] = XYPoint.VerticalFlip(Core.Bends[i], y);
            }
        }

        internal void HorizontalFlip(int x)
        {
            var len = Points.Length;
            for (int i = 0; i < len;)
            {
                Points[i] = XYPoint.HorizontalFlip(Points[i], x);
            }

            len = (Core.Bends == null) ? 0 : Core.Bends.Length;
            for (int i = 0; i < len;)
            {
                Core.Bends[i] = XYPoint.HorizontalFlip(Core.Bends[i], x);
            }
        }
        #endregion

        #region LineOrder  ====================================================
        internal (int X, int Y) GetClosestBend(Node node)
        {
            if (Points == null) Refresh();
            if (node == Node1)
                return Points[Bp1];
            else
                return Points[Bp2];
        }
        #endregion

        #region GetConnect  ===================================================
        internal Connect GetConnect(Node node)
        {
            if (node == Node1)
                return Connect1;
            else
                return Connect2;
        }
        #endregion

        #region HitTest  ======================================================
        // [node1]o----o-----o---edge----o-----o----o[node2] 
        //            Tm1   Bp1         Bp2   Tm2
        static int _ds = GraphParm.HitMargin;
        static int _ds2 = GraphParm.HitMarginSquared;

        internal void SetExtent((int X, int Y)[] points, int margin)
        {
            Extent = Extent.SetExtent(points, margin);
        }


        // quickly eliminate edges that don't qaulify
        internal bool HitTest((int X, int Y) p)
        {
            return Extent.Contains(p);
        }

        internal bool HitTest((int X, int Y) p, ref HitLocation hit, ref int hitBend, ref int hitIndex, ref (int X, int Y) hitPoint)
        {
            (int X, int Y) p1 = (0, 0);    // used for testing line segments
            (int X, int Y) p2 = (0, 0);    // used for testing line segments
            var E = new Extent(p, _ds); // extent of hit point sensitivity

            var gotHit = false;
            var len = Points.Length;
            var sp2 = len - 1;
            for (int i = 0; i < len; i++)
            {
                if (E.Contains(Points[i]))
                {
                    if (i <= Tm1)
                        hit |= HitLocation.Term | HitLocation.End1;
                    else if (i >= Tm2)
                        hit |= HitLocation.Term | HitLocation.End2;
                    else
                    {
                        hitBend = i;
                        hit |= HitLocation.Bend;
                    }

                    hitPoint = Points[i];
                    gotHit = true;
                    break;
                }

                p2 = Points[i];
                if (i == Tm1)
                {
                    var t1 = new Extent(Points[0], p2);
                    if (t1.Intersects(E))
                    {
                        gotHit = true;
                        hitPoint = Points[i];
                        hit |= HitLocation.Term | HitLocation.End1;
                        break;
                    }
                }
                else if (i == sp2)
                {
                    var t2 = new Extent(Points[Tm2], p2);
                    if (t2.Intersects(E))
                    {
                        gotHit = true;
                        hitPoint = Points[i];
                        hit |= HitLocation.Term | HitLocation.End1;
                        break;
                    }
                }
                else if (i > Tm1 && i <= Tm2)
                {
                    var e = new Extent(p1, p2);
                    if (e.Intersects(E))
                    {
                        if (e.IsHorizontal)
                        {
                            gotHit = true;
                            hitIndex = i;
                            hitPoint = (p.X, p2.Y);
                            break;
                        }
                        else if (e.IsVertical)
                        {
                            gotHit = true;
                            hitIndex = i;
                            hitPoint = (p1.X, p.Y);
                            break;
                        }
                        else
                        {
                            var dx = (double)(p2.X - p1.X);
                            var dy = (double)(p2.Y - p1.Y);

                            int xi = (int)(p1.X + (dx * (p.Y - p1.Y)) / dy);
                            if (E.ContainsX(xi))
                            {
                                gotHit = true;
                                hitIndex = i;
                                hitPoint = (xi, p.Y);
                                break;
                            }
                            xi = (int)(p2.X + (dx * (p.Y - p2.Y)) / dy);
                            if (E.ContainsX(xi))
                            {
                                gotHit = true;
                                hitIndex = i;
                                hitPoint = (xi, p.Y);
                                break;
                            }

                            int yi = (int)(p1.Y + (dy * (p.X - p1.X)) / dx);
                            if (E.ContainsY(yi))
                            {
                                gotHit = true;
                                hitIndex = i;
                                hitPoint = (p.X, yi);
                                break;
                            }
                            yi = (int)(p2.Y + (dy * (p.X - p2.X)) / dx);
                            if (E.ContainsY(yi))
                            {
                                gotHit = true;
                                hitIndex = i;
                                hitPoint = (p.X, yi);
                                break;
                            }
                        }

                    }
                }
                p1 = p2;
            }
            if (gotHit)
            {
                var e1 = new Extent(Points[Tm1], hitPoint);
                var e2 = new Extent(Points[Tm2], hitPoint);

                if (e1.IsLessThan(e2))
                    hit |= (HitLocation.Edge | HitLocation.End1);
                else
                    hit |= (HitLocation.Edge | HitLocation.End2);

                return true;
            }
            return false;
        }
        #endregion

        #region Options  ======================================================
        internal Node OtherNode(Node node) { return (node == Node1) ? Node2 : Node1; }

        internal void SetFace(Node node, Side direction)
        {
            if (node == Node1)
                Core.Face1.Assign(direction);
            else
                Core.Face2.Assign(direction);
        }
        internal void SetFace(Node node, Side direction, int index, int count)
        {
            if (node == Node1)
                Core.Face1.Assign(direction, index, count);
            else
                Core.Face2.Assign(direction, index, count);
        }
        #endregion

        #region Facets  =======================================================
        static Facet[] Facets =
        {
            new Facet(new int[0]),
            new Facet(new int[] { 0, 2,    3, 2,    6,0,   3,-2,   0,-2,    0, 0,    6, 0 }),
            new Facet(new int[] { 3, 0,    7,-4,   11,0,   7, 4,   3, 0,    7,-4,   11, 0 }),
            new Facet(new int[] { 2, 0,   12,-3,    8,0,  12, 3,   2, 0,   12, 0 })
        };
        static Facet NoFacet = new Facet(new int[0]);
        #endregion

        #region Refresh  ======================================================
        /// <summary>
        ///  Populate the edge bendPoint and edgePoints arrays
        /// </summary>
        /// <remarks>
        ///         facet1         bend points          facet2 
        /// node1 |o------o----------------------------o------o| node2
        ///        sp1    tm1                         tm2   sp2
        /// </remarks>
        internal void Refresh()
        {
            var facet1 = Node1.Core.IsPointNode ? NoFacet : Facets[(int)Core.Facet1];
            var facet2 = Node2.Core.IsPointNode ? NoFacet : Facets[(int)Core.Facet2];

            var tmLen = GraphParm.TerminalLength;
            var tmSpc = GraphParm.TerminalSpacing / 2;
            var tmSkf = GraphParm.TerminalSkew;

            if (Owner != null && Owner.IsGraph)
            {
                var gx = GraphX;
                tmLen = gx.TerminalLength;
                tmSpc = gx.TerminalSpacing / 2;
                tmSkf = gx.TerminalSkew;
            }

            var bendCount = (Core.Bends == null) ? 0 : Core.Bends.Length;

            var len1 = facet1.Length / 2;
            var len2 = facet2.Length / 2;

            var len = len1 + bendCount + len2 + 4;  // allow for pseudo points sp1 tp1 tp2 sp2 (x,y)
            var points = new(int X, int Y)[len];        // line coordinate values x,y,x,y,...

            var sp1 = 0;               // index of surface point 1 value
            var tm1 = len1 + 1;        // index of terminal point 1 value
            var tm2 = len - 2 - len2;  // index of terminal point 2 value
            var sp2 = len - 1;         // index of surface point 2 value
            var bp1 = tm1 + 1;         // index of bend point 1 value
            var bp2 = tm2 - 1;         // index of bend point 2 value

            Tm1 = (short)tm1;
            Bp1 = (short)bp1;
            Bp2 = (short)bp2;
            Tm2 = (short)tm2;

            Points = points;

            (int cx1, int cy1, int w1, int h1) = Node1.Core.Values();
            (int cx2, int cy2, int w2, int h2) = Node2.Core.Values();

            var y1T = cy1 - h1;     // node 1 top Y
            var x1L = cx1 - w1;     // node 1 left X
            var x1R = cx1 + w1;     // node 1 right X
            var y1B = cy1 + h1;     // node 1 bottom Y

            var y2T = cy2 - h2;     // node 2 top Y
            var x2L = cx2 - w2;     // node 2 left X
            var x2R = cx2 + w2;     // node 2 right X
            var y2B = cy2 + h2;     // node 2 bottom Y

            #region Initialize Bend Points  ===================================
            if (bendCount > 0)
            {
                for (int i = 0, j = (tm1 + 1); i < bendCount; i++, j++)
                {
                    points[j] = Core.Bends[i];
                }
            }
            else
            {
                #region FallBack Values  ======================================
                switch (Core.Face1.Side)
                {
                    case Side.East:
                        points[bp1].X = x2R;
                        points[bp1].Y = cy2;
                        break;

                    case Side.West:
                        points[bp1].X = x2L;
                        points[bp1].Y = cy2;
                        break;

                    case Side.South:
                        points[bp1].X = cx2;
                        points[bp1].Y = y2B;
                        break;

                    case Side.North:
                        points[bp1].X = cx2;
                        points[bp1].Y = y2T;
                        break;
                }
                switch (Core.Face2.Side)
                {
                    case Side.East:
                        points[bp2].X = x1R;
                        points[bp2].Y = cy1;
                        break;

                    case Side.West:
                        points[bp2].X = x1L;
                        points[bp2].Y = cy1;
                        break;

                    case Side.South:
                        points[bp2].X = cx1;
                        points[bp2].Y = y1B;
                        break;

                    case Side.North:
                        points[bp2].X = cx1;
                        points[bp2].Y = y1T;
                        break;
                }
                #endregion
            }
            #endregion

            if (Node1.Core.IsPointNode)
            {
                #region PointNode  ============================================
                if (Core.Face1.Side == Side.East)
                {
                    points[sp1].X = cx1;
                    points[tm1].X = x1R;
                    points[sp1].Y = points[tm1].Y = cy1;
                }
                else if (Core.Face1.Side == Side.West)
                {
                    points[sp1].X = cx1;
                    points[tm1].X = x1L;
                    points[sp1].Y = points[tm1].Y = cy1;
                }
                else if (Core.Face1.Side == Side.South)
                {
                    points[sp1].Y = cy1;
                    points[tm1].Y = y1B;
                    points[sp1].X = points[tm1].X = cx1;
                }
                else
                {
                    points[sp1].Y = cy1;
                    points[tm1].Y = y1T;
                    points[sp1].X = points[tm1].X = cx1;
                }
                #endregion
            }
            if (Node2.Core.IsPointNode)
            {
                #region PointNode  ============================================
                if (Core.Face2.Side == Side.East)
                {
                    points[sp2].X = cx2;
                    points[tm2].X = x2R;
                    points[sp2].Y = points[tm2].Y = cy2;
                }
                else if (Core.Face2.Side == Side.West)
                {
                    points[sp2].X = cx2;
                    points[tm2].X = x2L;
                    points[sp2].Y = points[tm2].Y = cy2;
                }
                else if (Core.Face2.Side == Side.South)
                {
                    points[sp2].Y = cy2;
                    points[tm2].Y = y2B;
                    points[sp2].X = points[tm2].X = cx2;
                }
                else
                {
                    points[sp2].Y = cy2;
                    points[tm2].Y = y2T;
                    points[sp2].X = points[tm2].X = cx2;
                }
                #endregion
            }

            if (Node1.Core.IsAutoSizing)
            {
                #region AutoSpacing  ==========================================
                if (Core.Face1.Side == Side.East)
                {
                    var x = points[sp1].X = x1R;
                    points[tm1].X = x1R + tmLen;
                    var y = points[sp1].Y = points[tm1].Y = cy1 + Core.Face1.Offset * tmSpc;

                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x + dx;
                        points[j].Y = y + dy;
                    }
                }
                else if (Core.Face1.Side == Side.West)
                {
                    var x = points[sp1].X = x1L;
                    points[tm1].X = cx1 - (w1 + tmLen);
                    var y = points[sp1].Y = points[tm1].Y = cy1 + Core.Face1.Offset * tmSpc;

                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x - dx;
                        points[j].Y = y - dy;
                    }
                }
                else if (Core.Face1.Side == Side.South)
                {
                    var y = points[sp1].Y = y1B;
                    points[tm1].Y = y1B + tmLen;
                    var x = points[sp1].X = points[tm1].X = cx1 + Core.Face1.Offset * tmSpc;

                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x + dy;
                        points[j].Y = y + dx;
                    }
                }
                else
                {
                    var y = points[sp1].Y = y1T;
                    points[tm1].Y = y1T - tmLen;
                    var x = points[sp1].X = points[tm1].X = cx1 + Core.Face1.Offset * tmSpc;

                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x - dy;
                        points[j].Y = y - dx;
                    }
                }
                #endregion
            }
            if (Node2.Core.IsAutoSizing)
            {
                #region AutoSpacing  ==========================================
                if (Core.Face2.Side == Side.East)
                {
                    var x = points[sp2].X = x2R;
                    points[tm2].X = x2R + tmLen;
                    var y = points[sp2].Y = points[tm2].Y = cy2 + Core.Face2.Offset * tmSpc;

                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = x + dx;
                        points[j].Y = y + dy;
                    }
                }
                else if (Core.Face2.Side == Side.West)
                {
                    var x = points[sp2].X = x2L;
                    points[tm2].X = x2L - tmLen;
                    var y = points[sp2].Y = points[tm2].Y = cy2 + Core.Face2.Offset * tmSpc;

                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = x - dx;
                        points[j].Y = y - dy;
                    }
                }
                else if (Core.Face2.Side == Side.South)
                {
                    var y = points[sp2].Y = y2B;
                    points[tm2].Y = y2B + tmLen;
                    var x = points[sp2].X = points[tm2].X = cx2 + Core.Face2.Offset * tmSpc;

                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = x + dy;
                        points[j].Y = y + dx;
                    }
                }
                else
                {
                    var y = points[sp2].Y = y2T;
                    points[tm2].Y = y2T - tmLen;
                    var x = points[sp2].X = points[tm2].X = cx2 + Core.Face2.Offset * tmSpc;

                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = x - dy;
                        points[j].Y = y - dx;
                    }
                }
                #endregion
            }

            if (Node1.Core.IsManualSizing)
            {
                #region ManualSpacing  ========================================
                var x = points[bp1].X;
                var y = points[bp1].Y;

                var f1W = facet1.Width();

                if (Node1.Core.IsVertical)
                {
                    if (y < (y1T + f1W))
                    {
                        y = points[sp1].Y = y1T;
                        x = points[sp1].X = points[tm1].X = cx1;
                        points[tm1].Y = y - tmLen;

                        Core.Face1.Side = Side.North;
                    }
                    else if (y > (y1B - f1W))
                    {
                        y = points[sp1].Y = y1B;
                        x = points[sp1].X = points[tm1].X = cx1;
                        points[tm1].Y = y + tmLen;

                        Core.Face1.Side = Side.South;
                    }
                    else
                    {
                        points[tm1].Y = points[sp1].Y = y;
                        if (x < cx1)
                        {
                            x = points[sp1].X = x1L;
                            points[tm1].X = x - tmLen;
                            Core.Face1.Side = Side.West;
                        }
                        else
                        {
                            x = points[sp1].X = x1R;
                            points[tm1].X = x + tmLen;
                            Core.Face1.Side = Side.East;
                        }
                    }
                }
                else if (Node1.Core.IsHorizontal)
                {
                    if (x < (x1L + f1W))
                    {
                        x = points[sp1].X = x1L;
                        y = points[sp1].Y = points[tm1].Y = cy1;
                        points[tm1].X = x - tmLen;

                        Core.Face1.Side = Side.West;
                    }
                    else if (x > (x1R - f1W))
                    {
                        x = points[sp1].X = x1R;
                        y = points[sp1].Y = points[tm1].Y = cy1;
                        points[tm1].X = x + tmLen;

                        Core.Face1.Side = Side.East;
                    }
                    else
                    {
                        points[tm1].X = points[sp1].X = x;
                        if (y < cy1)
                        {
                            y = points[sp1].Y = y1T;
                            points[tm1].Y = y - tmLen;
                            Core.Face1.Side = Side.North;
                        }
                        else
                        {
                            y = points[sp1].Y = y1B;
                            points[tm1].Y = y + tmLen;
                            Core.Face1.Side = Side.South;
                        }
                    }
                }

                if (Core.Face1.Side == Side.East)
                {
                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x + dx;
                        points[j].Y = y + dy;
                    }
                }
                else if (Core.Face1.Side == Side.West)
                {
                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x - dx;
                        points[j].Y = y + dy;
                    }
                }
                else if (Core.Face1.Side == Side.South)
                {
                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x + dy;
                        points[j].Y = y + dx;
                    }
                }
                else
                {
                    for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
                    {
                        var dx = facet1.DXY[i++];
                        var dy = facet1.DXY[i++];

                        points[j].X = x - dy;
                        points[j].Y = y - dx;
                    }
                }
                #endregion
            }
            if (Node2.Core.IsManualSizing)
            {
                #region ManualSpacing  ========================================
                var x = points[bp2].X;
                var y = points[bp2].Y;

                var f2W = facet2.Width();

                if (Node2.Core.IsVertical)
                {
                    if (y < (y2T + f2W))
                    {
                        y = points[sp2].Y = y2T;
                        x = points[sp2].X = points[tm2].X = cx2;
                        points[tm2].Y = y - tmLen;

                        Core.Face2.Side = Side.North;
                    }
                    else if (y > (y2B - f2W))
                    {
                        y = points[sp2].Y = y2B;
                        x = points[sp2].X = points[tm2].X = cx2;
                        points[tm2].Y = y + tmLen;

                        Core.Face2.Side = Side.South;
                    }
                    else
                    {
                        points[tm2].Y = points[sp2].Y = y;
                        if (x < cx2)
                        {
                            x = points[sp2].X = x2L;
                            points[tm2].X = x - tmLen;
                            Core.Face2.Side = Side.West;
                        }
                        else
                        {
                            x = points[sp2].X = x2R;
                            points[tm2].X = x + tmLen;
                            Core.Face2.Side = Side.East;
                        }
                    }
                }
                else if (Node2.Core.IsHorizontal)
                {
                    if (x < (x2L + f2W))
                    {
                        x = points[sp2].X = x2L;
                        y = points[sp2].Y = points[tm2].Y = cy2;
                        points[tm2].X = x - tmLen;

                        Core.Face2.Side = Side.West;
                    }
                    else if (x > (x2R - f2W))
                    {
                        x = points[sp2].X = x2R;
                        y = points[sp2].Y = points[tm2].Y = cy2;
                        points[tm2].X = x + tmLen;

                        Core.Face2.Side = Side.East;
                    }
                    else
                    {
                        points[tm2].X = points[sp2].X = x;
                        if (y < cy2)
                        {
                            y = points[sp2].Y = y2T;
                            points[tm2].Y = y - tmLen;
                            Core.Face2.Side = Side.North;
                        }
                        else
                        {
                            y = points[sp2].Y = y2B;
                            points[tm2].Y = y + tmLen;
                            Core.Face2.Side = Side.South;
                        }
                    }
                }

                if (Core.Face2.Side == Side.East)
                {
                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = y + dy;
                        points[j].Y = x + dx;
                    }
                }
                else if (Core.Face2.Side == Side.West)
                {
                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = y + dy;
                        points[j].Y = x - dx;
                    }
                }
                else if (Core.Face2.Side == Side.South)
                {
                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = y + dx;
                        points[j].Y = x + dy;
                    }
                }
                else
                {
                    for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                    {
                        var dx = facet2.DXY[i++];
                        var dy = facet2.DXY[i++];

                        points[j].X = y - dx;
                        points[j].Y = x - dy;
                    }
                }
                #endregion
            }

            if (Node1.Core.IsSymbol && Node2.Core.IsNode)
            {
                #region SkewFactor  ===========================================
                //                 tm o--------------------------------o| Node1
                //                   /
                //                  /--skew angle(skewFactor)
                //          Face2  /
                //  Node2 |o------o bp
                //
                var bx = points[bp1].X;
                var by = points[bp1].Y;

                var dx = points[tm1].X - bx;
                var dy = points[tm1].Y - by;

                var adx = (dx > 0) ? dx : -dx; // absolute value dx
                var ady = (dy > 0) ? dy : -dy; // absolute value dy

                switch (Core.Face1.Side)
                {
                    case Side.East:
                        if (dx < 0 && adx > tmSkf * ady) points[tm1].X = bx - (ady * tmSkf);
                        break;
                    case Side.West:
                        if (dx > 0 && adx > tmSkf * ady) points[tm1].X = bx + (ady * tmSkf);
                        break;
                    case Side.South:
                        if (dy < 0 && ady > tmSkf * adx) points[tm1].Y = by - (adx * tmSkf);
                        break;
                    case Side.North:
                        if (dy > 0 && ady > tmSkf * adx) points[tm1].Y = by + (adx * tmSkf);
                        break;
                }
                #endregion
            }

            if (Node1.Core.IsSymbol && Node2.Core.IsNode)
            {
                #region SkewFactor  ===========================================
                //                 tm o--------------------------------o| Node2
                //                   /
                //                  /--skew angle(skewFactor)
                //          Face1  /
                //  Node1 |o------o bp
                //
                var bx = points[bp2].X;
                var by = points[bp2].Y;

                var dx = points[tm2].X - bx;
                var dy = points[tm2].Y - by;

                var adx = (dx > 0) ? dx : -dx; // absolute value dx
                var ady = (dy > 0) ? dy : -dy; // absolute value dy

                switch (Core.Face1.Side)
                {
                    case Side.East:
                        if (dx < 0 && adx > tmSkf * ady) points[tm1].X = bx - (ady * tmSkf);
                        break;
                    case Side.West:
                        if (dx > 0 && adx > tmSkf * ady) points[tm1].X = bx + (ady * tmSkf);
                        break;
                    case Side.South:
                        if (dy < 0 && ady > tmSkf * adx) points[tm1].Y = by - (adx * tmSkf);
                        break;
                    case Side.North:
                        if (dy > 0 && ady > tmSkf * adx) points[tm1].Y = by + (adx * tmSkf);
                        break;
                }
                #endregion
            }

            SetExtent(points, GraphParm.HitMargin);
        }
        #endregion
    }
}
