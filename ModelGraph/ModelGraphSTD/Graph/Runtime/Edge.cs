namespace ModelGraphSTD
{
    public class Edge : NodeEdge
    {
        private readonly QueryX _queryX;
        public (float X, float Y)[] Points;
        public Extent Extent = new Extent(); // all points are withing this extent+

        public Node Node1;
        public Node Node2;

        public (float X, float Y)[] Bends;

        public short Tm1; // index of terminal point 1
        public short Bp1; // index of closest bend point after Tm1 (to the right) 
        public short Bp2; // index of closest bend point after Tm2 (to the left)
        public short Tm2; // index of terminal point 2

        internal (short dx, short dy) TP1; // terminal poiint displacement from facet point
        internal (short dx, short dy) TP2; // terminal poiint displacement from facet point

        public Facet Facet1;
        public Facet Facet2;

        internal Direction TD1; // outward direction of terminal-1
        internal Direction TD2; // outward direction of terminal-2

        internal (sbyte dx, sbyte dy) SP1; // surface point displacment from node center
        internal (sbyte dx, sbyte dy) FP1; // facet point displacment from surface point

        internal (sbyte dx, sbyte dy) SP2; // surface point displacment from node center
        internal (sbyte dx, sbyte dy) FP2; // facet point displacment from surface point
        
        public byte LineColor;


        #region Constructors  =================================================
        internal Edge(QueryX queryX)
        {
            Owner = null;
            Trait = Trait.Edge;
            _queryX = queryX;
        }
        #endregion

        #region Properties/Methods  ===========================================
        public DashStyle DashStyle => QueryX.PathParm.DashStyle;
        public LineStyle LineStyle => QueryX.PathParm.LineStyle;

        internal bool HasBends => !HasNoBends;
        internal bool HasNoBends => Bends == null || Bends.Length == 0;

        internal Graph Graph { get { return Owner as Graph; } }
        internal GraphX GraphX { get { return (Owner == null) ? null : Owner.Owner as GraphX; } }
        internal QueryX QueryX => _queryX;

        public override string ToString()
        {
            var chef = GetChef();
            var headName = chef.GetIdentity(Node1.Item, IdentityStyle.Double);
            var tailName = chef.GetIdentity(Node2.Item, IdentityStyle.Double);
            return $"{headName} --> {tailName}  ({LineColor})";
        }
        #endregion

        #region Snapshot  =====================================================
        internal ((float, float)[] bends, Facet facet1, Facet facet2) Snapshot
        {
            get
            {
                if (HasBends)
                {
                    var bends = new (float, float)[Bends.Length];
                    Bends.CopyTo(bends, 0);
                    return (bends, Facet1, Facet2);
                }
                else
                    return (null, Facet1, Facet2);
            }
            set
            {
                if (value.bends != null)
                {
                    Bends = new (float, float)[value.bends.Length];
                    value.bends.CopyTo(Bends, 0);
                }
                Facet1 = value.facet1;
                Facet2 = value.facet2;
            }
        }
        #endregion

        #region Move  =========================================================
        internal void Move((float X, float Y) delta)
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
        internal void Move((float X, float Y) delta, int index1, int index2)
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

        #region OtherBendTargetAttachDirection  ===============================
        internal (Node other, (float, float) bend, Target targ, Attach atch, Direction tdir) OtherBendTargetAttachDirection(Node node)
        {
            if (Points == null) Refresh();
            var symbols = Graph.Symbols;

            if (node == Node1)
            {
                var si = Node2.Symbol - 2;
                var atch = (si < 0 || si >= symbols.Length) ? Attach.Normal : symbols[si].Attach;
                var bend = Points[Bp1];
                var targ = QueryX.PathParm.Target1; 
                return (Node2, bend, targ, atch, TD2);
            }
            else
            {
                var si = Node1.Symbol - 2;
                var atch = (si < 0 || si >= symbols.Length) ? Attach.Normal : symbols[si].Attach;
                var bend = Points[Bp2];
                var targ = QueryX.PathParm.Target2;
                return (Node1, bend, targ, atch, TD1);
            }
        }
        #endregion

        #region HitTest  ======================================================
        // [node1]o----o-----o---edge----o-----o----o[node2] 
        //            Tm1   Bp1         Bp2   Tm2
        static readonly int _ds = GraphDefault.HitMargin;

        internal void SetExtent((float X, float Y)[] points, int margin)
        {
            Extent = Extent.SetExtent(points, margin);
        }


        // quickly eliminate edges that don't qaulify
        internal bool HitTest((float X, float Y) p)
        {
            return Extent.Contains(p);
        }

        internal bool HitTest((float X, float Y) p, ref HitLocation hit, ref int hitBend, ref int hitIndex, ref (float X, float Y) hitPoint)
        {
            (float X, float Y) p1 = (0, 0); // used for testing line segments
            (float X, float Y) p2 = (0, 0); // used for testing line segments
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

        #region SetFace  ======================================================
        internal void SetFace(Node node, (float dx, float dy) d, Direction td) => SetFace(node, d, d, d, td);
        internal void SetFace(Node node, (float dx, float dy) d1, (float dx, float dy) d3, Direction td) => SetFace(node, d1, d1, d3, td);
        internal void SetFace(Node node, (float dx, float dy) d1, (float dx, float dy) d2, (float dx, float dy) d3, Direction td)
        {
            if (node == Node1)
            {
                SP1 = ((sbyte)d1.dx, (sbyte)d1.dy);
                FP1 = ((sbyte)d2.dx, (sbyte)d2.dy);
                TP1 = ((short)d3.dx, (short)d3.dy);
                TD1 = td;
            }
            else
            {
                SP2 = ((sbyte)d1.dx, (sbyte)d1.dy);
                FP2 = ((sbyte)d2.dx, (sbyte)d2.dy);
                TP2 = ((short)d3.dx, (short)d3.dy);
                TD2 = td;
            }
            // do the refresh only once, after both faces have changed
            if (_needsRefresh) Refresh();
            _needsRefresh = !_needsRefresh;
        }
        private bool _needsRefresh;
        #endregion

        #region Facets  =======================================================
        static readonly FacetDXY[] Facets =
        {
            new FacetDXY(new (float, float)[0]),
            new FacetDXY(new (float, float)[] { (0, 1),    (3,  1),    (6, 0),   (3, -1),   (0, -1),    (0,  0),    (6, 0) }),
            new FacetDXY(new (float, float)[] { (3, 0),    (7, -4),   (11, 0),   (7,  4),   (3,  0),    (7, -4),   (11, 0) }),
            new FacetDXY(new (float, float)[] { (2, 0),   (12, -3),    (8, 0),   (12, 3),   (2,  0),   (12,  0) })
        };
        static readonly FacetDXY NoFacet = new FacetDXY(new (float, float)[0]);
        #endregion

        #region Refresh  ======================================================
        /// <summary>
        ///  Populate the edge Point array
        /// </summary>
        /// <remarks>
        ///           facet1    optional bend points   facet2 
        /// node1 |o--o-----o------o------o------o-----o-----o--o| node2
        ///      sp1 fp1  tm1    bp1    ...    bp2   tm2    fp2 sp2
        ///      
        /// sp:(surface point), fp:(facet point), tp:(terminal point), bp:(bend point)
        /// </remarks>
        internal void Refresh()
        {
            var facet1 = Node1.IsNodePoint ? NoFacet : Facets[(int)(Facet1 & Facet.Mask)];
            var facet2 = Node2.IsNodePoint ? NoFacet : Facets[(int)(Facet2 & Facet.Mask)];

            var bendCount = (Bends == null) ? 0 : Bends.Length;

            var len1 = facet1.Length;
            var len2 = facet2.Length;

            var len = len1 + bendCount + len2 + 6; // allow for pseudo points sp1 fp1 tp1 tp2 fp2 sp2 (x,y)
            var P = new (float X, float Y)[len];        // line coordinate values (x,y), (x,y),..

            var sp1 = 0;               // index of surface point 1 value
            var fp1 = 1;               // index of facet point 1 value
            var tp1 = len1 + 2;        // index of terminal point 1 value
            var tp2 = len - 3 - len2;  // index of terminal point 2 value
            var fp2 = len - 2;         // index of facet point 2 value
            var sp2 = len - 1;         // index of surface point 2 value
            var bp1 = tp1 + 1;         // index of bend point 1 value
            var bp2 = tp2 - 1;         // index of bend point 2 value

            Tm1 = (short)tp1;
            Bp1 = (short)bp1;
            Bp2 = (short)bp2;
            Tm2 = (short)tp2;

            Points = P;

            (float cx1, float cy1, float w1, float h1) = Node1.Values();
            (float cx2, float cy2, float w2, float h2) = Node2.Values();

            P[sp1] = (cx1 + SP1.dx, cy1 + SP1.dy); // surface point 1
            P[fp1] = (cx1 + FP1.dx, cy1 + FP1.dy); // facet point 1
            P[tp1] = (cx1 + TP1.dx, cy1 + TP1.dy); // terminal point 1

            P[sp2] = (cx2 + SP2.dx, cy2 + SP2.dy); // surface point 2
            P[fp2] = (cx2 + FP2.dx, cy2 + FP2.dy); // facet point 2
            P[tp2] = (cx2 + TP2.dx, cy2 + TP2.dy); // terminal point 2

            #region Bend Points  ==============================================
            if (bendCount > 0)
            {
                for (int i = 0, j = (tp1 + 1); i < bendCount; i++, j++)
                {
                    P[j] = Bends[i];
                }
            }
            #endregion

            #region Facet1 Points  ============================================
            if (len1 > 0)
            {
                var (Xdx, Xdy, Ydx, Ydy) = _facetDirectionVectors[(int)TD1];
                var x = P[fp1].X;
                var y = P[fp1].Y;
                for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                {
                    var (dx, dy) = facet1.DXY[i];

                    P[j].X = x + dx * Xdx + dy * Xdy;
                    P[j].Y = y + dx * Ydx + dy * Ydy;
                }
            }
            #endregion

            #region Facet2 Points  ============================================
            if (len2 > 0)
            {
                var (Xdx, Xdy, Ydx, Ydy) = _facetDirectionVectors[(int)TD2];
                var x = P[fp2].X;
                var y = P[fp2].Y;
                for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                {
                    var (dx, dy) = facet2.DXY[i];

                    P[j].X = x + dx * Xdx + dy * Xdy;
                    P[j].Y = y + dx * Ydx + dy * Ydy;
                }
            }
            #endregion

            Extent.SetExtent(P, 2);
        }
        #endregion

        #region FacetDirectionVectors  ========================================
        private const float q = 0.7071067811865f; // 1 / SQRT(2)
        private static (float Xdx, float Xdy, float Ydx, float Ydy)[] _facetDirectionVectors =
        {
            (0, 0, 0, 0),    // Direction.Any
            (1, 0, 0, 1),    // Direction.E
            (0, 1, -1, 0),   // Direction.S
            (-1, 0, 0, -1),  // Direction.W
            (0, -1, 1, 0),   // Direction.N
            (q, q, -q, q),   // Direction.SEC
            (-q, q, -q, -q), // Direction.SWC
            (-q, -q, q, -q), // Direction.NWC
            (q, -q, q, q),   // Direction.NEC
        };
        #endregion
    }
}