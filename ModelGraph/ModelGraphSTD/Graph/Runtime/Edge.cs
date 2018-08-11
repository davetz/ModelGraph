﻿
using System;

namespace ModelGraphSTD
{
    public class Edge : NodeEdge
    {
        private readonly QueryX _queryX;
        public Node Node1;
        public Node Node2;
        public Extent Extent = new Extent(); // all points are withing this extent+

        public (int X, int Y)[] Points;
        public (int X, int Y)[] Bends;
        
        public short Tm1; // index of terminal point 1
        public short Bp1; // index of closest bend point after Tm1 (to the right) 
        public short Bp2; // index of closest bend point after Tm2 (to the left)
        public short Tm2; // index of terminal point 2

        public DashStyle DashStyle;
        public LineStyle LineStyle;
        public byte LineColor;

        #region Parms  ========================================================
        public Face Face1;
        public Face Face2;

        internal ((int X, int Y)[] Points, (int X, int Y)[] Bends, Face Face1, Face Face2)
            Parms
        {
            get { return (Points, Bends, Face1, Face2); }
            set
            {
                Points = value.Points;
                Bends = value.Bends;
                Face1 = value.Face1;
                Face2 = value.Face2;
            }
        }
        #endregion


        #region Constructors  =================================================
        internal Edge(QueryX queryX)
        {
            Owner = null;
            Trait = Trait.Edge;
            _queryX = queryX;
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal bool HasBends => (Bends != null && Bends.Length > 0);
        internal Graph Graph { get { return Owner as Graph; } }
        internal GraphX GraphX { get { return (Owner == null) ? null : Owner.Owner as GraphX; } }
        internal QueryX QueryX => _queryX;

        internal Connect Connect1 { get { return QueryX.PathParm.Connect1; } }
        internal Connect Connect2 { get { return QueryX.PathParm.Connect2; } }
        #endregion


        #region Move  =========================================================
        internal void Move((int X, int Y) delta)
        {
            if (HasBends)
            {
                for (int i = 0; i < Bends.Length; i++)
                {
                    Bends[i].X = Bends[i].X + delta.X;
                    Bends[i].Y = Bends[i].Y + delta.Y;
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
            if (HasBends)
            {
                for (int i = index1; i < index2; i++)
                {
                    var j = i - Tm1 - 1;
                    if (j >= 0 && j < Bends.Length)
                    {
                        Bends[j].X = Bends[j].X + delta.X;
                        Bends[j].Y = Bends[j].Y + delta.Y;
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

            len = (Bends == null) ? 0 : Bends.Length;
            for (int i = 0; i < len;)
            {
                Bends[i] = XYPoint.Rotate(Bends[i], (x, y));
            }
        }

        internal void VerticalFlip(int y)
        {
            var len = Points.Length;
            for (int i = 0; i < len;)
            {
                Points[i] = XYPoint.VerticalFlip(Points[i], y);
            }

            len = (Bends == null) ? 0 : Bends.Length;
            for (int i = 0; i < len;)
            {
                Bends[i] = XYPoint.VerticalFlip(Bends[i], y);
            }
        }

        internal void HorizontalFlip(int x)
        {
            var len = Points.Length;
            for (int i = 0; i < len;)
            {
                Points[i] = XYPoint.HorizontalFlip(Points[i], x);
            }

            len = (Bends == null) ? 0 : Bends.Length;
            for (int i = 0; i < len;)
            {
                Bends[i] = XYPoint.HorizontalFlip(Bends[i], x);
            }
        }
        #endregion

        #region LineOrder  ====================================================
        internal (int X, int Y) GetClosestBend(Node node)
        {
            if (Points == null) Refresh();
            return (node == Node1) ? Points[Bp1] : Points[Bp2];
        }
        #endregion

        #region GetConnect  ===================================================
        internal Connect GetConnect(Node node) => (node == Node1) ? Connect1 :  Connect2;
        #endregion

        #region HitTest  ======================================================
        // [node1]o----o-----o---edge----o-----o----o[node2] 
        //            Tm1   Bp1         Bp2   Tm2
        static readonly int _ds = GraphDefault.HitMargin;

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
            (int X, int Y) p1 = (0, 0); // used for testing line segments
            (int X, int Y) p2 = (0, 0); // used for testing line segments
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

                hit |=  (e1.IsLessThan(e2)) ? (HitLocation.Edge | HitLocation.End1) : (HitLocation.Edge | HitLocation.End2);

                return true;
            }
            return false;
        }
        #endregion

        #region Options  ======================================================
        internal Node OtherNode(Node node) => (node == Node1) ? Node2 : Node1;

        internal void SetFace(Node node, Side side, int index = 0, int count = 1)
        {
            if (node == Node1)
            {
                Face1.Side = side;
                Face1.Index = (byte)((index > byte.MaxValue) ? byte.MaxValue : index);
                Face1.Count = (byte)((count > byte.MaxValue) ? byte.MaxValue : count);
            }
            else
            {
                Face2.Side = side;
                Face2.Index = (byte)((index > byte.MaxValue) ? byte.MaxValue : index);
                Face2.Count = (byte)((count > byte.MaxValue) ? byte.MaxValue : count);
            }
        }
        #endregion

        #region Facets  =======================================================
        static readonly FacetDXY[] Facets =
        {
            new FacetDXY(new int[0]),
            new FacetDXY(new int[] { 0, 1,    3, 1,    6,0,   3,-1,   0,-1,    0, 0,    6, 0 }),
            new FacetDXY(new int[] { 3, 0,    7,-4,   11,0,   7, 4,   3, 0,    7,-4,   11, 0 }),
            new FacetDXY(new int[] { 2, 0,   12,-3,    8,0,  12, 3,   2, 0,   12, 0 })
        };
        static readonly FacetDXY NoFacet = new FacetDXY(new int[0]);
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
            var facet1 = Node1.IsPointNode ? NoFacet : Facets[(int)(Face1.Facet & Facet.Mask)];
            var facet2 = Node2.IsPointNode ? NoFacet : Facets[(int)(Face2.Facet & Facet.Mask)];

            var tmLen = GraphDefault.TerminalLength;
            var tmSpc = GraphDefault.TerminalSpacing / 2;
            var tmSkf = GraphDefault.TerminalAngleSkew;

            if (Owner != null && Owner.IsGraph)
            {
                var gx = GraphX;
                tmLen = gx.TerminalLength;
                tmSpc = gx.TerminalSpacing / 2;
            }

            var bendCount = (Bends == null) ? 0 : Bends.Length;

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

            (int cx1, int cy1, int w1, int h1) = Node1.Values();
            (int cx2, int cy2, int w2, int h2) = Node2.Values();

            var y1T = cy1 - h1;     // node 1 top Y
            var x1L = cx1 - w1;     // node 1 left X
            var x1R = cx1 + w1;     // node 1 right X
            var y1B = cy1 + h1;     // node 1 bottom Y

            var y2T = cy2 - h2;     // node 2 top Y
            var x2L = cx2 - w2;     // node 2 left X
            var x2R = cx2 + w2;     // node 2 right X
            var y2B = cy2 + h2;     // node 2 bottom Y

            var at1 = QueryX.PathParm.Attach1;
            var at2 = QueryX.PathParm.Attach2;

            #region Initialize Bend Points  ===================================
            if (bendCount > 0)
            {
                for (int i = 0, j = (tm1 + 1); i < bendCount; i++, j++)
                {
                    points[j] = Bends[i];
                }
            }
            else
            {
                #region FallBack Values  ======================================
                switch (Face1.Side)
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
                switch (Face2.Side)
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

            #region PrevResolution  ===========================================
            //if (Node1.IsPointNode)
            //{
            //    #region PointNode  ============================================
            //    if (Face1.Side == Side.East)
            //    {
            //        points[sp1].X = cx1;
            //        points[tm1].X = x1R;
            //        points[sp1].Y = points[tm1].Y = cy1;
            //    }
            //    else if (Face1.Side == Side.West)
            //    {
            //        points[sp1].X = cx1;
            //        points[tm1].X = x1L;
            //        points[sp1].Y = points[tm1].Y = cy1;
            //    }
            //    else if (Face1.Side == Side.South)
            //    {
            //        points[sp1].Y = cy1;
            //        points[tm1].Y = y1B;
            //        points[sp1].X = points[tm1].X = cx1;
            //    }
            //    else
            //    {
            //        points[sp1].Y = cy1;
            //        points[tm1].Y = y1T;
            //        points[sp1].X = points[tm1].X = cx1;
            //    }
            //    #endregion
            //}
            //if (Node2.IsPointNode)
            //{
            //    #region PointNode  ============================================
            //    if (Face2.Side == Side.East)
            //    {
            //        points[sp2].X = cx2;
            //        points[tm2].X = x2R;
            //        points[sp2].Y = points[tm2].Y = cy2;
            //    }
            //    else if (Face2.Side == Side.West)
            //    {
            //        points[sp2].X = cx2;
            //        points[tm2].X = x2L;
            //        points[sp2].Y = points[tm2].Y = cy2;
            //    }
            //    else if (Face2.Side == Side.South)
            //    {
            //        points[sp2].Y = cy2;
            //        points[tm2].Y = y2B;
            //        points[sp2].X = points[tm2].X = cx2;
            //    }
            //    else
            //    {
            //        points[sp2].Y = cy2;
            //        points[tm2].Y = y2T;
            //        points[sp2].X = points[tm2].X = cx2;
            //    }
            //    #endregion
            //}

            //if (Node1.IsAutoSizing)
            //{
            //    #region AutoSpacing  ==========================================
            //    if (Face1.Side == Side.East)
            //    {
            //        var x = points[sp1].X = x1R;
            //        points[tm1].X = x1R + tmLen;
            //        var y = points[sp1].Y = points[tm1].Y = cy1 + Face1.Offset * tmSpc;

            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x + dx;
            //            points[j].Y = y + dy;
            //        }
            //    }
            //    else if (Face1.Side == Side.West)
            //    {
            //        var x = points[sp1].X = x1L;
            //        points[tm1].X = cx1 - (w1 + tmLen);
            //        var y = points[sp1].Y = points[tm1].Y = cy1 + Face1.Offset * tmSpc;

            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x - dx;
            //            points[j].Y = y - dy;
            //        }
            //    }
            //    else if (Face1.Side == Side.South)
            //    {
            //        var y = points[sp1].Y = y1B;
            //        points[tm1].Y = y1B + tmLen;
            //        var x = points[sp1].X = points[tm1].X = cx1 + Face1.Offset * tmSpc;

            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x + dy;
            //            points[j].Y = y + dx;
            //        }
            //    }
            //    else
            //    {
            //        var y = points[sp1].Y = y1T;
            //        points[tm1].Y = y1T - tmLen;
            //        var x = points[sp1].X = points[tm1].X = cx1 + Face1.Offset * tmSpc;

            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x - dy;
            //            points[j].Y = y - dx;
            //        }
            //    }
            //    #endregion
            //}
            //if (Node2.IsAutoSizing)
            //{
            //    #region AutoSpacing  ==========================================
            //    if (Face2.Side == Side.East)
            //    {
            //        var x = points[sp2].X = x2R;
            //        points[tm2].X = x2R + tmLen;
            //        var y = points[sp2].Y = points[tm2].Y = cy2 + Face2.Offset * tmSpc;

            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = x + dx;
            //            points[j].Y = y + dy;
            //        }
            //    }
            //    else if (Face2.Side == Side.West)
            //    {
            //        var x = points[sp2].X = x2L;
            //        points[tm2].X = x2L - tmLen;
            //        var y = points[sp2].Y = points[tm2].Y = cy2 + Face2.Offset * tmSpc;

            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = x - dx;
            //            points[j].Y = y - dy;
            //        }
            //    }
            //    else if (Face2.Side == Side.South)
            //    {
            //        var y = points[sp2].Y = y2B;
            //        points[tm2].Y = y2B + tmLen;
            //        var x = points[sp2].X = points[tm2].X = cx2 + Face2.Offset * tmSpc;

            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = x + dy;
            //            points[j].Y = y + dx;
            //        }
            //    }
            //    else
            //    {
            //        var y = points[sp2].Y = y2T;
            //        points[tm2].Y = y2T - tmLen;
            //        var x = points[sp2].X = points[tm2].X = cx2 + Face2.Offset * tmSpc;

            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = x - dy;
            //            points[j].Y = y - dx;
            //        }
            //    }
            //    #endregion
            //}

            //if (Node1.IsManualSizing)
            //{
            //    #region ManualSpacing  ========================================
            //    var x = points[bp1].X;
            //    var y = points[bp1].Y;

            //    var f1W = facet1.Width();

            //    if (Node1.IsVertical)
            //    {
            //        if (y < (y1T + f1W))
            //        {
            //            y = points[sp1].Y = y1T;
            //            x = points[sp1].X = points[tm1].X = cx1;
            //            points[tm1].Y = y - tmLen;

            //            Face1.Side = Side.North;
            //        }
            //        else if (y > (y1B - f1W))
            //        {
            //            y = points[sp1].Y = y1B;
            //            x = points[sp1].X = points[tm1].X = cx1;
            //            points[tm1].Y = y + tmLen;

            //            Face1.Side = Side.South;
            //        }
            //        else
            //        {
            //            points[tm1].Y = points[sp1].Y = y;
            //            if (x < cx1)
            //            {
            //                x = points[sp1].X = x1L;
            //                points[tm1].X = x - tmLen;
            //                Face1.Side = Side.West;
            //            }
            //            else
            //            {
            //                x = points[sp1].X = x1R;
            //                points[tm1].X = x + tmLen;
            //                Face1.Side = Side.East;
            //            }
            //        }
            //    }
            //    else if (Node1.IsHorizontal)
            //    {
            //        if (x < (x1L + f1W))
            //        {
            //            x = points[sp1].X = x1L;
            //            y = points[sp1].Y = points[tm1].Y = cy1;
            //            points[tm1].X = x - tmLen;

            //            Face1.Side = Side.West;
            //        }
            //        else if (x > (x1R - f1W))
            //        {
            //            x = points[sp1].X = x1R;
            //            y = points[sp1].Y = points[tm1].Y = cy1;
            //            points[tm1].X = x + tmLen;

            //            Face1.Side = Side.East;
            //        }
            //        else
            //        {
            //            points[tm1].X = points[sp1].X = x;
            //            if (y < cy1)
            //            {
            //                y = points[sp1].Y = y1T;
            //                points[tm1].Y = y - tmLen;
            //                Face1.Side = Side.North;
            //            }
            //            else
            //            {
            //                y = points[sp1].Y = y1B;
            //                points[tm1].Y = y + tmLen;
            //                Face1.Side = Side.South;
            //            }
            //        }
            //    }

            //    if (Face1.Side == Side.East)
            //    {
            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x + dx;
            //            points[j].Y = y + dy;
            //        }
            //    }
            //    else if (Face1.Side == Side.West)
            //    {
            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x - dx;
            //            points[j].Y = y + dy;
            //        }
            //    }
            //    else if (Face1.Side == Side.South)
            //    {
            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x + dy;
            //            points[j].Y = y + dx;
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0, j = (sp1 + 1); i < facet1.Length; j++)
            //        {
            //            var dx = facet1.DXY[i++];
            //            var dy = facet1.DXY[i++];

            //            points[j].X = x - dy;
            //            points[j].Y = y - dx;
            //        }
            //    }
            //    #endregion
            //}
            //if (Node2.IsManualSizing)
            //{
            //    #region ManualSpacing  ========================================
            //    var x = points[bp2].X;
            //    var y = points[bp2].Y;

            //    var f2W = facet2.Width();

            //    if (Node2.IsVertical)
            //    {
            //        if (y < (y2T + f2W))
            //        {
            //            y = points[sp2].Y = y2T;
            //            x = points[sp2].X = points[tm2].X = cx2;
            //            points[tm2].Y = y - tmLen;

            //            Face2.Side = Side.North;
            //        }
            //        else if (y > (y2B - f2W))
            //        {
            //            y = points[sp2].Y = y2B;
            //            x = points[sp2].X = points[tm2].X = cx2;
            //            points[tm2].Y = y + tmLen;

            //            Face2.Side = Side.South;
            //        }
            //        else
            //        {
            //            points[tm2].Y = points[sp2].Y = y;
            //            if (x < cx2)
            //            {
            //                x = points[sp2].X = x2L;
            //                points[tm2].X = x - tmLen;
            //                Face2.Side = Side.West;
            //            }
            //            else
            //            {
            //                x = points[sp2].X = x2R;
            //                points[tm2].X = x + tmLen;
            //                Face2.Side = Side.East;
            //            }
            //        }
            //    }
            //    else if (Node2.IsHorizontal)
            //    {
            //        if (x < (x2L + f2W))
            //        {
            //            x = points[sp2].X = x2L;
            //            y = points[sp2].Y = points[tm2].Y = cy2;
            //            points[tm2].X = x - tmLen;

            //            Face2.Side = Side.West;
            //        }
            //        else if (x > (x2R - f2W))
            //        {
            //            x = points[sp2].X = x2R;
            //            y = points[sp2].Y = points[tm2].Y = cy2;
            //            points[tm2].X = x + tmLen;

            //            Face2.Side = Side.East;
            //        }
            //        else
            //        {
            //            points[tm2].X = points[sp2].X = x;
            //            if (y < cy2)
            //            {
            //                y = points[sp2].Y = y2T;
            //                points[tm2].Y = y - tmLen;
            //                Face2.Side = Side.North;
            //            }
            //            else
            //            {
            //                y = points[sp2].Y = y2B;
            //                points[tm2].Y = y + tmLen;
            //                Face2.Side = Side.South;
            //            }
            //        }
            //    }

            //    if (Face2.Side == Side.East)
            //    {
            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = y + dy;
            //            points[j].Y = x + dx;
            //        }
            //    }
            //    else if (Face2.Side == Side.West)
            //    {
            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = y + dy;
            //            points[j].Y = x - dx;
            //        }
            //    }
            //    else if (Face2.Side == Side.South)
            //    {
            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = y + dx;
            //            points[j].Y = x + dy;
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
            //        {
            //            var dx = facet2.DXY[i++];
            //            var dy = facet2.DXY[i++];

            //            points[j].X = y - dx;
            //            points[j].Y = x - dy;
            //        }
            //    }
            //    #endregion
            //}

            //if (Node1.IsSymbol && Node2.IsNode)
            //{
            //    #region SkewOffset  ===========================================
            //    //                 tm o--------------------------------o| Node1
            //    //                   /|
            //    //                  / |
            //    //          Face2  /  | skew offset
            //    //  Node2 |o------o bp
            //    //
            //    var bx = points[bp1].X;
            //    var by = points[bp1].Y;

            //    var dx = points[tm1].X - bx;
            //    var dy = points[tm1].Y - by;

            //    switch (Face1.Side)
            //    {
            //        case Side.East:
            //            if (dx < 0) points[tm1].X = bx - tmSkf;
            //            break;
            //        case Side.West:
            //            if (dx > 0) points[tm1].X = bx + tmSkf;
            //            break;
            //        case Side.South:
            //            if (dy < 0) points[tm1].Y = by - tmSkf;
            //            break;
            //        case Side.North:
            //            if (dy > 0) points[tm1].Y = by + tmSkf;
            //            break;
            //    }
            //    #endregion
            //}

            //if (Node1.IsSymbol && Node2.IsNode)
            //{
            //    #region SkewFactor  ===========================================
            //    //                 tm o--------------------------------o| Node2
            //    //                   /
            //    //                  /--skew angle(skewFactor)
            //    //          Face1  /
            //    //  Node1 |o------o bp
            //    //
            //    var bx = points[bp2].X;
            //    var by = points[bp2].Y;

            //    var dx = points[tm2].X - bx;
            //    var dy = points[tm2].Y - by;

            //    switch (Face1.Side)
            //    {
            //        case Side.East:
            //            if (dx < 0) points[tm1].X = bx - tmSkf;
            //            break;
            //        case Side.West:
            //            if (dx > 0) points[tm1].X = bx + tmSkf;
            //            break;
            //        case Side.South:
            //            if (dy < 0) points[tm1].Y = by - tmSkf;
            //            break;
            //        case Side.North:
            //            if (dy > 0) points[tm1].Y = by + tmSkf;
            //            break;
            //    }
            //    #endregion
            //}

            #endregion

            #region NewResolution  ============================================
            if (Node1.IsNode)
            {
                if (Node2.IsNode)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            #region (IsNode, IsPoint) - (IsNode)
                            #region AssignFace1  ==============================
                            if (Face1.Side == Side.East)
                            {
                                points[sp1].X = cx1;
                                points[tm1].X = cx1 + 1;
                                points[sp1].Y = points[tm1].Y = cy1;
                            }
                            else if (Face1.Side == Side.West)
                            {
                                points[sp1].X = cx1;
                                points[tm1].X = cx1 - 1;
                                points[sp1].Y = points[tm1].Y = cy1;
                            }
                            else if (Face1.Side == Side.South)
                            {
                                points[sp1].Y = cy1;
                                points[tm1].Y = cy1 + 1;
                                points[sp1].X = points[tm1].X = cx1;
                            }
                            else
                            {
                                points[sp1].Y = cy1;
                                points[tm1].Y = cy1 - 1;
                                points[sp1].X = points[tm1].X = cx1;
                            }
                            #endregion

                            if (Node2.Orientation == Orientation.Point)
                            {
                                #region (IsNode, IsPoint) - (IsNode, IsPoint)
                                if (Face2.Side == Side.East)
                                {
                                    points[sp2].X = cx2;
                                    points[tm2].X = cx2 + 1;
                                    points[sp2].Y = points[tm2].Y = cy2;
                                }
                                else if (Face2.Side == Side.West)
                                {
                                    points[sp2].X = cx2;
                                    points[tm2].X = cx2 - 1;
                                    points[sp2].Y = points[tm2].Y = cy2;
                                }
                                else if (Face2.Side == Side.South)
                                {
                                    points[sp2].Y = cy2;
                                    points[tm2].Y = cy2 + 1;
                                    points[sp2].X = points[tm2].X = cx2;
                                }
                                else
                                {
                                    points[sp2].Y = cy2;
                                    points[tm2].Y = cy2 - 1;
                                    points[sp2].X = points[tm2].X = cx2;
                                }
                                #endregion
                            }
                            else
                            {
                                #region (IsNode, IsPoint) - (IsNode, IsNotPoint)
                                if (Node2.Resizing == Resizing.Auto)
                                {
                                    #region (IsNode, IsPoint) - (IsNode, IsNotPoint, IsAuto)
                                    if (Face2.Side == Side.East)
                                    {
                                        var x = points[sp2].X = x2R;
                                        points[tm2].X = x2R + tmLen;
                                        var y = points[sp2].Y = points[tm2].Y = cy2 + Face2.Offset * tmSpc;

                                        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                                        {
                                            var dx = facet2.DXY[i++];
                                            var dy = facet2.DXY[i++];

                                            points[j].X = x + dx;
                                            points[j].Y = y + dy;
                                        }
                                    }
                                    else if (Face2.Side == Side.West)
                                    {
                                        var x = points[sp2].X = x2L;
                                        points[tm2].X = x2L - tmLen;
                                        var y = points[sp2].Y = points[tm2].Y = cy2 + Face2.Offset * tmSpc;

                                        for (int i = 0, j = (sp2 - 1); i < facet2.Length; j--)
                                        {
                                            var dx = facet2.DXY[i++];
                                            var dy = facet2.DXY[i++];

                                            points[j].X = x - dx;
                                            points[j].Y = y - dy;
                                        }
                                    }
                                    else if (Face2.Side == Side.South)
                                    {
                                        var y = points[sp2].Y = y2B;
                                        points[tm2].Y = y2B + tmLen;
                                        var x = points[sp2].X = points[tm2].X = cx2 + Face2.Offset * tmSpc;

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
                                        var x = points[sp2].X = points[tm2].X = cx2 + Face2.Offset * tmSpc;

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
                                else
                                {
                                    #region (IsNode, IsPoint) - (IsNode, IsNotPoint, IsFixed)
                                    #endregion
                                }
                                break;
                                #endregion
                            }
                            #endregion
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else if (Node2.IsSymbol)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsNode, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            else if (Node1.IsSymbol)
            {
                if (Node2.IsNode)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else if (Node2.IsSymbol)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsSymbol, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            else
            {
                if (Node2.IsNode)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsNode, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else if (Node2.IsSymbol)
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsSymbol, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (Node1.Orientation)
                    {
                        case Orientation.Point:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsPoint, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Central:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsCentral, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Vertical:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsVertical, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Orientation.Horizontal:
                            switch (Node2.Orientation)
                            {
                                case Orientation.Point:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsAuto)
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsPoint, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Central:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsCentral, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Vertical:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsVertical, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                                case Orientation.Horizontal:
                                    if (Node1.Resizing == Resizing.Auto)
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsAuto) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (Node2.Resizing == Resizing.Auto)
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsAuto) 
                                            
                                        }
                                        else
                                        {//(IsEgress, IsHorizontal, IsFixed) - (IsEgress, IsHorizontal, IsFixed) 
                                            
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            #endregion

            SetExtent(points, GraphDefault.HitMargin);

        }
        #endregion
    }
}