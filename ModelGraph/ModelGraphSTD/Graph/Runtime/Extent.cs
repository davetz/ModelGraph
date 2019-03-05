using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class Extent
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        #region Constructor  ==================================================
        public Extent(int s = 0)
        {
            X1 = X2 = Y1 = Y2 = s;
        }
        public Extent(Extent e)
        {
            X1 = e.X1;
            Y1 = e.Y1;
            X2 = e.X2;
            Y2 = e.Y2;
        }
        public Extent Clone => new Extent(this);

        public Extent((float X, float Y) p)
        {
            X1 = X2 = p.X;
            Y1 = Y2 = p.Y;
        }

        public Extent(float x, float y)
        {
            X1 = X2 = x;
            Y1 = Y2 = y;
        }

        public Extent(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Extent((float x1, float y1, float x2, float y2) e, float ds)
        {
            X1 = e.x1 - ds;
            Y1 = e.y1 - ds;
            X2 = e.x2 + ds;
            Y2 = e.y2 + ds;
        }

        public Extent((float X, float Y) p, float ds)
        {
            X1 = p.X + 1 - ds;
            X2 = p.X + 1 + ds;
            Y1 = p.Y + 1 - ds;
            Y2 = p.Y + 1 + ds;
        }

        public static Extent Create (IEnumerable<Node> nodes, int margin)
        {
            var e = new Extent();
            return e.SetExtent(nodes, margin);
        }
        public Extent((float X, float Y) p1, (float X, float Y) p2)
        {
            if (p1.X < p2.X)
            {
                X1 = p1.X;
                X2 = p2.X;
            }
            else
            {
                X1 = p2.X;
                X2 = p1.X;
            }
            if (p1.Y < p2.Y)
            {
                Y1 = p1.Y;
                Y2 = p2.Y;
            }
            else
            {
                Y1 = p2.Y;
                Y2 = p1.Y;
            }
        }
        #endregion

        #region Move  =========================================================
        public void Move((float X, float Y) delta)
        {
            X1 += delta.X;
            X2 += delta.X;
            Y1 += delta.Y;
            Y2 += delta.Y;
        }
        #endregion

        #region Shape  ========================================================
        public bool IsTall { get { return DY > DX; } }
        public bool IsWide { get { return DX > DY; } }
        public bool IsEmpty { get { return (IsVertical || IsHorizontal); } }
        public bool HasArea { get { return (X1 != X2 && Y1 != Y2); } }
        public bool IsVertical => (X2 - X1 < 0) ? ((X2 - X1 > -4) ? true : false) : (X2 - X1 < 4) ? true : false;
        public bool IsHorizontal => (Y2 - Y1 < 0) ? ((Y2 - Y1 > -4) ? true : false) : (Y2 - Y1 < 4) ? true : false;
        #endregion

        #region Center  =======================================================
        public float CenterX => (X2 + X1) / 2;
        public float CenterY => (Y2 + Y1) / 2;
        public (float X, float Y) Center => (CenterX, CenterY);
        #endregion

        #region Points  =======================================================
        public void Points((float X, float Y) p1, (float X, float Y) p2) { Point1 = p1; Point2 = p2; }
        public (float X, float Y) Point1 { get { return (X1, Y1); } set { X1 = value.X; Y1 = value.Y; } }
        public (float X, float Y) Point2 { get { return (X2, Y2); } set { X2 = value.X; Y2 = value.Y; } }
        public void Record((float X, float Y) p) { Point1 = Point2; Point2 = p; }

        public void Record((float X, float Y) p, float scale) { Point1 = Point2; SetPoint2(p, scale); }
        public void SetPoint1((float X, float Y) p, float scale) { X1 = (int)(p.X * scale); Y1 = (int)(p.Y * scale); }
        public void SetPoint2((float X, float Y) p, float scale) { X2 = (int)(p.X * scale); Y2 = (int)(p.Y * scale); }
        #endregion

        #region Expand  =======================================================
        public void Expand(int margin)
        {
            X1 -= margin;
            Y1 -= margin;
            X2 += margin;
            Y2 += margin;
        }

        public void Expand(float x, float y)
        {
            if (x < X1) X1 = x;
            if (y < Y1) Y1 = y;
            if (x > X2) X2 = x;
            if (y > Y2) Y2 = y;
        }
        public void Expand((float X, float Y) p)
        {
            if (p.X < X1) X1 = p.X;
            if (p.Y < Y1) Y1 = p.Y;
            if (p.X > X2) X2 = p.X;
            if (p.Y > Y2) Y2 = p.Y;
        }
        #endregion

        #region Clear  ========================================================
        public void Clear()
        {
            X1 = 0;
            Y1 = 0;
            X2 = 0;
            Y2 = 0;
        }
        #endregion

        #region Diagonal  =====================================================
        public float DX => X2 - X1;
        public float DY => Y2 - Y1;
        public (float X, float Y) Delta => (DX, DY);
        public float Length => (float)Math.Sqrt(Diagonal);
        public float Diagonal => XYTuple.Diagonal(Delta);
        public (float x, float y) OrthoginalDisplacedPoint(float ds) => XYTuple.OrthoginalDisplacedPoint(DX, DY, X2, Y2, ds);
        public (Quad quad, Sect sect, double slope) QuadSectSlope => XYTuple.QuadSectSlope((X1, Y1), (X2, Y2));
        public bool TryGetDelta(out (float X, float Y) delta)
        {
            delta = Delta;
            if (X2 == X1 && Y2 == Y1)
                return false;

            X1 = X2;
            Y1 = Y2;
            return true;
        }
        #endregion

        #region Normalize  ====================================================
        // enforce  (X1 < X2) and  (Y1 < Y2)

        public void Normalize()
        {
            Normalize(Point1, Point2);
        }

        public void Normalize((float X, float Y) p1, (float X, float Y) p2)
        {
            if (p2.X < p1.X)
            {
                X1 = p2.X;
                X2 = p1.X;
            }
            else
            {
                X1 = p1.X;
                X2 = p2.X;
            }

            if (p2.Y < p1.Y)
            {
                Y1 = p2.Y;
                Y2 = p1.Y;
            }
            else
            {
                Y1 = p1.Y;
                Y2 = p2.Y;
            }
        }
        #endregion

        #region SetExtent  ====================================================
        // enforce  (X1 < X2) and  (Y1 < Y2)
        public void SetExtent(Node node, int margin)
        {
            var (x, y, w, h) = node.Values();

            X1 = x - w - margin;
            Y1 = y - h - margin;
            X2 = x + w + margin;
            Y2 = y + h + margin;
        }
        public Extent SetExtent(IEnumerable<Node> nodeList, int margin)
        {
            X1 = Y1 = int.MaxValue;
            X2 = Y2 = int.MinValue;
            foreach (var node in nodeList)
            {
                var e = node.Extent;
                if (e.X1 < X1) X1 = e.X1;
                if (e.Y1 < Y1) Y1 = e.Y1;

                if (e.X2 > X2) X2 = e.X2;
                if (e.Y2 > Y2) Y2 = e.Y2;
            }
            if (X1 == int.MaxValue)
            {
                X1 = -margin;
                Y1 = -margin;
                X2 = margin;
                Y2 = margin;
            }
            else
            {
                X1 -= margin;
                Y1 -= margin;
                X2 += margin;
                Y2 += margin;
            }
            return this;
        }

        public Extent SetExtent((float X, float Y)[] points, int margin)
        {
            var N = (points == null) ? 0 : points.Length;
            if (N == 0)
            {
                X1 = Y1 = X2 = Y2 = 0;
            }
            else
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = int.MinValue;
                for (int i = 0; i < N; i++)
                {
                    if (points[i].X < X1) X1 = points[i].X;
                    if (points[i].Y < Y1) Y1 = points[i].Y;

                    if (points[i].X > X2) X2 = points[i].X;
                    if (points[i].Y > Y2) Y2 = points[i].Y;
                }
                X1 -= margin;
                Y1 -= margin;
                X2 += margin;
                Y2 += margin;
            }
            return this;
        }
        #endregion

        #region Rectangle  ====================================================
        // independant of order (x1,y1), (x2,y2)
        public float Xmin => (X1 < X2) ? X1 : X2;
        public float Ymin => (Y1 < Y2) ? Y1 : Y2;
        public float Xmax => (X2 > X1) ? X2 : X1;
        public float Ymax => (Y2 > Y1) ? Y2 : Y1;

        public float Width => (Xmax - Xmin);
        public float Hieght => (Ymax - Ymin);
        public (float X, float Y) TopLeft => (Xmin, Ymin);
        public (float X, float Y) TopRight => (Xmax, Ymin);
        public (float X, float Y) BottomLeft => (Xmin, Ymax);
        public (float X, float Y) BottomRight => (Xmax, Ymax);
        #endregion

        #region RoundedRectanglePoints  =======================================
        internal List<(float X, float Y)> RoundedRectanglePoints()
        {
            const int r = 4;
            const int c = 1;
            var list = new List<(float X, float Y)>(12);
            var tl = TopLeft;
            var tr = TopRight;
            var bl = BottomLeft;
            var br = BottomRight;

            list.Add((tl.X, tl.Y + r));
            list.Add((tl.X + c, tl.Y + c));
            list.Add((tl.X + r, tl.Y));

            list.Add((tr.X - r, tl.Y));
            list.Add((tr.X - c, tr.Y + c));
            list.Add((tr.X, tr.Y + r));

            list.Add((br.X, br.Y - r));
            list.Add((br.X - c, br.Y - c));
            list.Add((br.X - r, br.Y));

            list.Add((bl.X + r, bl.Y));
            list.Add((bl.X + c, bl.Y - c));
            list.Add((bl.X, bl.Y - r));

            return list;
        }
        #endregion

        #region Comparison  ===================================================
        public bool IsLessThan(Extent e) => Diagonal < (e.Diagonal);
        public bool IsGreaterThan(Extent e) => Diagonal > (e.Diagonal); 
        #endregion

        #region Contains  =====================================================
        public bool ContainsX(float X)
        {
            if (X < X1) return false;
            if (X > X2) return false;
            return true;
        }

        public bool ContainsY(float Y)
        {
            if (Y < Y1) return false;
            if (Y > Y2) return false;
            return true;
        }

        public bool Contains((float X, float Y) p)
        {
            if (p.X < X1 && p.X < X2) return false;
            if (p.Y < Y1 && p.Y < Y2) return false;
            if (p.X > X2 && p.X > X1) return false;
            if (p.Y > Y2 && p.Y > Y1) return false;
            return true;
        }
        //public bool Contains(Vector2 p)
        //{
        //    if (p.X < X1) return false;
        //    if (p.Y < Y1) return false;
        //    if (p.X > X2) return false;
        //    if (p.Y > Y2) return false;
        //    return true;
        //}

        public bool Contains(ref Extent e)
        {
            if (e.X1 < X1) return false;
            if (e.Y1 < Y1) return false;
            if (e.X2 > X2) return false;
            if (e.Y2 > Y2) return false;
            return true;
        }
        #endregion

        #region HitTest  ======================================================
        // p as input is the point we are testing
        // E is the target range arround point p
        // p as output is the closest point that lies on the line defined by my Point1 and my Point2
        //
        //          my  Point1 o - - - -
        //                     :\      : <- my extent
        //            : - - - - -\ - : : 
        //       E -> :        :  p  : :
        //            :    p   :   \ : :
        //            :        :    \: :
        //            : - - - - - - -\ :
        //                     :      \:
        //                     - - - - o my Point2
        //
        public bool HitTest(ref (float X, float Y) p, Extent E)
        {
            if (Intersects(E))  // my extent intersects with E
            {
                if (IsHorizontal)   // my Y1 == my Y2
                {
                    p.Y = Y1;
                    return true; ;
                }
                else if (IsVertical) // my X1 == my X2
                {
                    p.X = X1;
                    return true;
                }
                else
                {
                    var dx = (double)DX;
                    var dy = (double)DY;

                    int xi = (int)(X1 + (dx * (p.Y - Y1)) / dy);
                    if (E.ContainsX(xi))
                    {
                        p.X = xi;
                        return true;
                    }
                    xi = (int)(X2 + (dx * (p.Y - Y2)) / dy);
                    if (E.ContainsX(xi))
                    {
                        p.X = xi;
                        return true;
                    }

                    int yi = (int)(Y1 + (dy * (p.X - X1)) / dx);
                    if (E.ContainsY(yi))
                    {
                        p.Y = yi;
                        return true;
                    }
                    yi = (int)(Y2 + (dy * (p.X - X2)) / dx);
                    if (E.ContainsY(yi))
                    {
                        p.Y = yi;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Intersection  =================================================
        public bool Intersects(Extent e)
        {
            if (IsVertical)
            {
                if (e.IsVertical)
                {
                    if (X1 != e.X1) return false;
                    if (ContainsY(e.Y1)) return true;
                    if (ContainsY(e.Y2)) return true;
                    if (e.ContainsY(Y1)) return true;
                }
                else if (e.IsHorizontal)
                {
                    if (e.ContainsX(X1) && ContainsY(e.Y1)) return true;
                }
                else
                {
                    if (!e.ContainsX(X1)) return false;
                    if (ContainsY(e.Y1)) return true;
                    if (ContainsY(e.Y2)) return true;
                    if (e.ContainsY(Y1)) return true;
                }
            }
            else if (IsHorizontal)
            {
                if (e.IsVertical)
                {
                    if (e.ContainsY(Y1) && ContainsX(e.X1)) return true;
                }
                else if (e.IsHorizontal)
                {
                    if (Y1 != e.Y1) return false;
                    if (ContainsX(e.X1)) return true;
                    if (ContainsX(e.X2)) return true;
                    if (e.ContainsX(X1)) return true;
                }
                else
                {
                    if (!e.ContainsY(Y1)) return false;
                    if (ContainsX(e.X1)) return true;
                    if (ContainsX(e.X2)) return true;
                    if (e.ContainsX(X1)) return true;
                }
            }
            else
            {
                if (e.IsVertical)
                {
                    if (!ContainsX(e.X1)) return false;
                    if (ContainsY(e.Y1)) return true;
                    if (ContainsY(e.Y2)) return true;
                    if (e.ContainsY(Y1)) return true;

                }
                else if (e.IsHorizontal)
                {
                    if (!ContainsY(e.Y1)) return false;
                    if (ContainsX(e.X1)) return true;
                    if (ContainsX(e.X2)) return true;
                    if (e.ContainsX(X1)) return true;
                }
                else
                {
                    if (Contains(e.TopLeft)) return true;
                    if (Contains(e.TopRight)) return true;
                    if (Contains(e.BottomLeft)) return true;
                    if (Contains(e.BottomRight)) return true;

                    if (e.Contains(TopLeft)) return true;
                    if (e.Contains(TopRight)) return true;
                    if (e.Contains(BottomLeft)) return true;
                    if (e.Contains(BottomRight)) return true;
                }
            }
            return false;
        }

        #endregion
    }
}
