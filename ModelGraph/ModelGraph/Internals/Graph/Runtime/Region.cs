
using System.Collections.Generic;

namespace ModelGraph.Internals
{
    public class Region
    {
        private int _ds;                 // minimum line segment length
        private double _closingLength;   // length of segment that would close the polygon
        private double _perimeterLength; // polygon's perimeter length;

        public HashSet<Node> Nodes = new HashSet<Node>();   // nodes inside the region
        public List<XYPoint> Points; // region's boundry
        public Extent Delta;
        public Extent Extent;
        public Extent Normal;
        public Extent Closing;

        public bool AnyHits { get { return (Nodes.Count > 0); } }
        public bool IsPolygon { get { return (Points.Count > 2 && _closingLength < .15 * _perimeterLength); } }
        public bool IsViable { get { return (Points.Count > 1 && Extent.HasArea); } }

        #region Constructor  ==================================================
        public Region(XYPoint p, int minSpacing = -1)
        {
            _ds = (minSpacing < 0) ? GraphParm.PolygonPointSpacing : minSpacing;

            Delta = new Extent(p);
            Extent = new Extent(p);
            Normal = new Extent(p);
            Closing = new Extent(p);

            Points = new List<XYPoint>();
            Points.Add(p);
        }
        #endregion

        #region Add, Close  ===================================================
        // try to add another point to the region
        public bool Add(XYPoint p)
        {
            if (_ds == 0)
            {
                AddPoint(p);
                return true;
            }
            else
            {
                Delta.Point2 = p;
                var ds = Delta.Length;

                if (ds > _ds)
                {
                    Points.Add(p);
                    Delta.Record(p);
                    Closing.Point2 = p;
                    _closingLength = Closing.Length;
                    _perimeterLength += ds;
                }
                Extent.Expand(p);
                Normal.Point2 = p;
                return true;
            }
        }
        // common method adds XYPair to Points
        private void AddPoint(XYPoint p)
        {
            Points.Add(p);
            Extent.Expand(p);
            Normal.Point2 = p;
        }

        // enter the last point and close the region
        public void Close(XYPoint p)
        {
            Add(p);
            if (IsPolygon) return;

            Points = new List<XYPoint>(2);
            Points.Add(Normal.TopLeft);
            Points.Add(Normal.BottomRight);
            Extent = Normal;
        }
        #endregion

        #region Move  =========================================================
        public void Move(XYPoint delta)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new XYPoint((Points[i].X + delta.X), (Points[i].Y + delta.Y));
            }
            Normal.Move(delta);
            Extent.Move(delta);
        }
        #endregion

        #region HitTest  ======================================================
        // hit test the point
        public bool HitTest(XYPoint p)
        {
            if (IsPolygon)
            {
                var len = Points.Count;
                int k = len - 1;
                var x1 = Points[k].X;
                var y1 = Points[k].Y;

                // count the number times that the point's positive
                // horizontal ray intersects with the polygon 
                int count = 0;
                for (int j = 0; j < len; j++)
                {
                    var x2 = Points[j].X;
                    var y2 = Points[j].Y;
                    if (p.Y > ((y1 < y2) ? (y1) : (y2)))
                    {
                        if (p.Y <= ((y1 > y2) ? (y1) : (y2)))
                        {
                            if (p.X <= ((x1 > x2) ? (x1) : (x2)))
                            {
                                if (y1 != y2)
                                {
                                    var xi = (y2 - y1) * (x2 - x1) / (y2 - y1) + x1;
                                    if (x1 == x2 || p.X <= xi) count += 1;
                                }
                            }
                        }
                    }
                    x1 = x2;
                    y1 = y2;
                }
                // is the point interior to polygon path
                // (if count is odd) Yes, (if count is even) No
                return ((count % 2) != 0);
            }
            else return Normal.Contains(p);
        }

        public bool HitTest(Edge edge, out int index1, out int index2, out bool isInterior)
        {
            isInterior = false;
            index1 = index2 = -1;

            if (Nodes.Contains(edge.Node1))
            {
                if (Nodes.Contains(edge.Node2))
                {
                    isInterior = true;
                    return true;
                }
                else
                {
                    index1 = 0;
                    index2 = edge.Tm2;
                    for (int i = (edge.Tm1 + 1); i < edge.Tm2; i++)
                    {
                        if (HitTest(edge.Points[i])) index2 = (i + 1);
                    }
                    return true;
                }
            }
            else if (Nodes.Contains(edge.Node2))
            {
                index1 = edge.Tm1 + 1;
                index2 = edge.Points.Length;
                for (int i = (edge.Tm2 - 1); i > edge.Tm1; i--)
                {
                    if (HitTest(edge.Points[i])) index1 = i;
                }
                return true;
            }
            else if (edge.Core.HasBends)
            {
                var bends = edge.Core.Bends;
                for (int i = 0, j = (edge.Tm1 + 1), k = (edge.Tm1 + 2); i < bends.Length; i++)
                {
                    if (HitTest(bends[i]))
                    {
                        if (index1 == -1)
                            index1 = (i + j);
                        else
                            index2 = (i + k);
                    }
                }
                return (index2 > 0);
            }
            else
            {
                index1 = index2 = 0;
                return false;
            }
        }
        #endregion
    }
}
