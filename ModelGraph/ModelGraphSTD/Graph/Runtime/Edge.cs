
using System;
using System.Numerics;

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

        public (float X, float Y) SP1;
        public (float X, float Y) FP1;
        public (float X, float Y) TP1;

        public (float X, float Y) SP2;
        public (float X, float Y) FP2;
        public (float X, float Y) TP2;

        public short Tm1; // index of terminal point 1
        public short Bp1; // index of closest bend point after Tm1 (to the right) 
        public short Bp2; // index of closest bend point after Tm2 (to the left)
        public short Tm2; // index of terminal point 2

        public Facet Facet1;
        public Facet Facet2;

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

        internal Connect Connect1 { get { return QueryX.PathParm.Connect1; } }
        internal Connect Connect2 { get { return QueryX.PathParm.Connect2; } }

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

        #region RotateFlip  ===================================================
        internal void RotateFlip((float x, float y) focus, FlipRotate flip)
        {
            var len = Points.Length;
            for (int i = 0; i < len; i++)
            {
                Points[i] = XYPair.RotateFlip(Points[i], focus, flip);
            }

            len = (Bends == null) ? 0 : Bends.Length;
            for (int i = 0; i < len; i++)
            {
                Bends[i] = XYPair.RotateFlip(Bends[i], focus, flip);
            }
        }
        internal void RotateFlip((float x, float y) focus, FlipRotate flip, int index1, int index2)
        {
            if (HasBends)
            {
                for (int i = index1; i < index2; i++)
                {
                    var j = i - Tm1 - 1;
                    if (j >= 0 && j < Bends.Length)
                    {
                        Bends[j] = XYPair.RotateFlip(Bends[j], focus, flip);
                    }
                }
            }
            for (int i = index1; i < index2; i++)
            {
                Points[i] = XYPair.RotateFlip(Points[i], focus, flip);
            }
        }
        #endregion

        #region OtherBendAttachHorz  ==========================================
        internal (Node other, (float, float) bend, Attach atch, bool horz) OtherBendAttachHorz(Node node)
        {
            if (Points == null) Refresh();
            var l = Points.Length - 1;

            if (node.Aspect == Aspect.Point && (Node1.IsGraphSymbol || Node2.IsGraphSymbol))
            {
                var dx1 = Points[0].X - Points[Tm1].X;
                var dy1 = Points[0].Y - Points[Tm1].Y;
                var dx2 = Points[l].X - Points[Tm2].X;
                var dy2 = Points[l].Y - Points[Tm2].Y;

                return (node == Node1) ?
                    (Node2, (Node2.Aspect == Aspect.Point) ? Points[l] : Points[Bp1], Graph.GetAttach(Node2), (dx2 * dx2 > dy2 * dy2)) :
                    (Node1, (Node1.Aspect == Aspect.Point) ? Points[0] : Points[Bp2], Graph.GetAttach(Node1), (dx1 * dx1 > dy1 * dy1));
            }
            else
            {
                return (node == Node1) ?
                    (Node2, (Node2.Aspect == Aspect.Point) ? Points[l] : Points[Bp1], Graph.GetAttach(Node2), false) :
                    (Node1, (Node1.Aspect == Aspect.Point) ? Points[0] : Points[Bp2], Graph.GetAttach(Node1), false) ;
            }
        }
        #endregion

        #region TargetOtherBendAttachHorz  ====================================
        // horz: means the far terminal attaches horizontaly
        // revr: means reverse the sort comparison
        internal (Target targ, Node other, (float, float) bend, Attach atch, bool horz, bool revr) TargetOtherBendAttachHorzRevr(Node node)
        {
            if (Points == null) Refresh();
            var l = Points.Length - 1;

            if (node.Aspect == Aspect.Point && (Node1.IsGraphSymbol || Node2.IsGraphSymbol))
            {
                var dx1 = Points[0].X - Points[Tm1].X;
                var dy1 = Points[0].Y - Points[Tm1].Y;
                var dx2 = Points[l].X - Points[Tm2].X;
                var dy2 = Points[l].Y - Points[Tm2].Y;

                return (node == Node1) ?
                    (QueryX.PathParm.Target1, Node2, (Node2.Aspect == Aspect.Point) ? Points[l] : Points[Bp1], Graph.GetAttach(Node2), (dx2 * dx2) > (dy2 * dy2), (dx2 + dy2) > 0) :
                    (QueryX.PathParm.Target2, Node1, (Node1.Aspect == Aspect.Point) ? Points[0] : Points[Bp2], Graph.GetAttach(Node1), (dx1 * dx1) > (dy1 * dy1), (dx1 + dy1) > 0);
            }
            else
            {
                return (node == Node1) ?
                    (QueryX.PathParm.Target1, Node2, (Node2.Aspect == Aspect.Point) ? Points[l] : Points[Bp1], Graph.GetAttach(Node2), false, false) :
                    (QueryX.PathParm.Target2, Node1, (Node1.Aspect == Aspect.Point) ? Points[0] : Points[Bp2], Graph.GetAttach(Node1), false, false);
            }
        }
        #endregion

        #region GetConnect  ===================================================
        internal Connect GetConnect(Node node) => (node == Node1) ? Connect1 :  Connect2;
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
        internal void SetFace(Node node, (float x, float y) d) => SetFace(node, d, d, d);
        internal void SetFace(Node node, (float x, float y) d1, (float x, float y) d3) => SetFace(node, d1, d1, d3);
        internal void SetFace(Node node, (float x, float y) d1, (float x, float y) d2, (float x, float y) d3)
        {
            if (node == Node1)
            { SP1 = d1; FP1 = d2; TP1 = d3; }
            else
            { SP2 = d1; FP2 = d2; TP2 = d3; }

            // do the refresh after both faces have changed
            if (NeedsRefresh) Refresh();
            NeedsRefresh = !NeedsRefresh;
        }
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
            var P = new(float X, float Y)[len];        // line coordinate values (x,y), (x,y),..

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

            P[sp1] = SP1;
            P[fp1] = FP1;
            P[tp1] = TP1;

            P[sp2] = SP2;
            P[fp2] = FP2;
            P[tp2] = TP2;

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
                if (TP1.X > FP1.X)
                {
                    if (TP1.Y > FP1.Y)
                    {//==================================== off south-east corner
                        NotImplemented();
                    }
                    else if (TP1.Y < FP1.Y)
                    {//==================================== off north-east corner
                        var x = P[fp1].X;
                        var y = P[fp1].Y;
                        for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                        {
                            P[j].X = x + facet1.DXY[i].X + facet1.DXY[i].Y;
                            P[j].Y = y + facet1.DXY[i].Y + facet1.DXY[i].X;
                        }
                    }
                    else
                    {//==================================== off east side
                        var x = P[fp1].X;
                        var y = P[fp1].Y;
                        for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                        {
                            P[j].X = x + facet1.DXY[i].X;
                            P[j].Y = y + facet1.DXY[i].Y;
                        }
                    }
                }
                else if (TP1.X < FP1.X)
                {
                    if (TP1.Y > FP1.Y)
                    {//==================================== off south-west corner
                        NotImplemented();
                    }
                    else if (TP1.Y < FP1.Y)
                    {//==================================== off north-west corner
                        NotImplemented();
                    }
                    else
                    {//==================================== off west side
                        var x = P[fp1].X;
                        var y = P[fp1].Y;
                        for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                        {
                            P[j].X = x - facet1.DXY[i].X;
                            P[j].Y = y - facet1.DXY[i].Y;
                        }
                    }
                }
                else
                {
                    if (TP1.Y > FP1.Y)
                    {//==================================== off south side
                        var x = P[fp1].X;
                        var y = P[fp1].Y;
                        for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                        {
                            P[j].X = x + facet1.DXY[i].Y;
                            P[j].Y = y + facet1.DXY[i].X;
                        }
                    }
                    else if (TP1.Y < FP1.Y)
                    {//==================================== off north side
                        var x = P[fp1].X;
                        var y = P[fp1].Y;
                        for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                        {
                            P[j].X = x - facet1.DXY[i].Y;
                            P[j].Y = y - facet1.DXY[i].X;
                        }
                    }
                    else
                    {//==================================== all balled-up in a knot
                        NotImplemented();
                    }
                }

                void NotImplemented()
                {
                    var x = P[fp1].X;
                    var y = P[fp1].Y;
                    for (int i = 0, j = (fp1 + 1); i < len1; j++, i++)
                    {
                        P[j].X = x;
                        P[j].Y = y;
                    }
                }
            }
            #endregion

            #region Facet2 Points  ============================================
            if (len2 > 0)
            {
                if (TP2.X > FP2.X)
                {
                    if (TP2.Y > FP2.Y)
                    {//==================================== off south-east corner
                        NotImplemented();
                    }
                    else if (TP2.Y < FP2.Y)
                    {//==================================== off north-east corner
                        NotImplemented();
                    }
                    else
                    {//==================================== off east side
                        var x = P[fp2].X;
                        var y = P[fp2].Y;
                        for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                        {
                            P[j].X = x + facet2.DXY[i].X;
                            P[j].Y = y + facet2.DXY[i].Y;
                        }
                    }
                }
                else if (TP2.X < FP2.X)
                {
                    if (TP2.Y > FP2.Y)
                    {//==================================== off south-west corner
                        NotImplemented();
                    }
                    else if (TP2.Y < FP2.Y)
                    {//==================================== off north-west corner
                        NotImplemented();
                    }
                    else
                    {//==================================== off west side
                        var x = P[fp2].X;
                        var y = P[fp2].Y;
                        for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                        {
                            P[j].X = x - facet2.DXY[i].X;
                            P[j].Y = y - facet2.DXY[i].Y;
                        }
                    }
                }
                else
                {
                    if (TP2.Y > FP2.Y)
                    {//==================================== off south side
                        var x = P[fp2].X;
                        var y = P[fp2].Y;
                        for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                        {
                            P[j].X = x + facet2.DXY[i].Y;
                            P[j].Y = y + facet2.DXY[i].X;
                        }
                    }
                    else if (TP2.Y < FP2.Y)
                    {//==================================== off north side
                        var x = P[fp2].X;
                        var y = P[fp2].Y;
                        for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                        {
                            P[j].X = x - facet2.DXY[i].Y;
                            P[j].Y = y - facet2.DXY[i].X;
                        }
                    }
                    else
                    {//==================================== all balled-up in a knot
                        NotImplemented();
                    }
                }

                void NotImplemented()
                {
                    var x = P[fp2].X;
                    var y = P[fp2].Y;
                    for (int i = 0, j = (fp2 - 1); i < len2; j--, i++)
                    {
                        P[j].X = x;
                        P[j].Y = y;
                    }
                }
            }
            #endregion

            Extent.SetExtent(P, 2);
        }
        #endregion
    }
}