using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class Extent
    {
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;

        #region Constructor  ==================================================
        public Extent(int s = 0)
        {
            X1 = X2 = Y1 = Y2 = s;
        }

        public Extent((int X, int Y) p)
        {
            X1 = X2 = p.X;
            Y1 = Y2 = p.Y;
        }

        public Extent(int x, int y)
        {
            X1 = X2 = x;
            Y1 = Y2 = y;
        }

        public Extent(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Extent((int X, int Y) p, int ds)
        {
            X1 = p.X + 1 - ds;
            X2 = p.X + 1 + ds;
            Y1 = p.Y + 1 - ds;
            Y2 = p.Y + 1 + ds;
        }

        public Extent((int X, int Y) p1, (int X, int Y) p2)
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
        public void Move((int X, int Y) delta)
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
        public bool IsVertical { get { return (X1 == X2); } }
        public bool IsHorizontal { get { return (Y1 == Y2); } }
        #endregion

        #region Center  =======================================================
        public int CenterX => (X2 + X1) / 2;
        public int CenterY => (Y2 + Y1) / 2;
        public (int X, int Y) Center => (CenterX, CenterY);
        #endregion

        #region Points  =======================================================
        public void Points((int X, int Y) p1, (int X, int Y) p2) { Point1 = p1; Point2 = p2; }
        public (int X, int Y) Point1 { get { return (X1, Y1); } set { X1 = value.X; Y1 = value.Y; } }
        public (int X, int Y) Point2 { get { return (X2, Y2); } set { X2 = value.X; Y2 = value.Y; } }
        public void Record((int X, int Y) p) { Point1 = Point2; Point2 = p; }

        public void Record((int X, int Y) p, float scale) { Point1 = Point2; SetPoint2(p, scale); }
        public void SetPoint1((int X, int Y) p, float scale) { X1 = (int)(p.X * scale); Y1 = (int)(p.Y * scale); }
        public void SetPoint2((int X, int Y) p, float scale) { X2 = (int)(p.X * scale); Y2 = (int)(p.Y * scale); }
        #endregion

        #region Expand  =======================================================
        public void Expand(int margin)
        {
            X1 -= margin;
            Y1 -= margin;
            X2 += margin;
            Y2 += margin;
        }

        public void Expand(int x, int y)
        {
            if (x < X1) X1 = x;
            if (y < Y1) Y1 = y;
            if (x > X2) X2 = x;
            if (y > Y2) Y2 = y;
        }
        public void Expand((int X, int Y) p)
        {
            if (p.X < X1) X1 = p.X;
            if (p.Y < Y1) Y1 = p.Y;
            if (p.X > X2) X2 = p.X;
            if (p.Y > Y2) Y2 = p.Y;
        }
        #endregion

        #region Diagonal  =====================================================
        public int DX => X2 - X1;
        public int DY => Y2 - Y1;
        public (int X, int Y) Delta => (DX, DY);
        public int Length => (int)Math.Sqrt(Diagonal);
        public int Diagonal => XYPoint.Diagonal(Delta);
        public float Slope => XYPoint.Slope(Delta);
        public int Quad => XYPoint.Quad(Delta);

        public bool TryGetDelta(out (int X, int Y) delta)
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

        public void Normalize((int X, int Y) p1, (int X, int Y) p2)
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
        public Extent SetExtent(List<Node> nodeList, int margin)
        {
            if (nodeList.Count > 0)
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = int.MinValue;
                foreach (var node in nodeList)
                {
                    var e = node.Core.Extent;
                    if (e.X1 < X1) X1 = e.X1;
                    if (e.Y1 < Y1) Y1 = e.Y1;

                    if (e.X2 > X2) X2 = e.X2;
                    if (e.Y2 > Y2) Y2 = e.Y2;
                }
            }
            X1 -= margin;
            Y1 -= margin;
            X2 += margin;
            Y2 += margin;
            return this;
        }

        public Extent SetExtent((int X, int Y)[] points, int margin)
        {
            var len = (points == null) ? 0 : points.Length;
            if (len == 0)
            {
                X1 = Y1 = X2 = Y2 = 0;
            }
            else
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = int.MinValue;
                for (int i = 0; i < len; i++)
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
        public int Xmin => (X1 < X2) ? X1 : X2;
        public int Ymin => (Y1 < Y2) ? Y1 : Y2;
        public int Xmax => (X2 > X1) ? X2 : X1;
        public int Ymax => (Y2 > Y1) ? Y2 : Y1;

        public int Width => (Xmax - Xmin);
        public int Hieght => (Ymax - Ymin);
        public (int X, int Y) TopLeft => (Xmin, Ymin);
        public (int X, int Y) TopRight => (Xmax, Ymin);
        public (int X, int Y) BottomLeft => (Xmin, Ymax);
        public (int X, int Y) BottomRight => (Xmax, Ymax);
        #endregion

        #region Comparison  ===================================================
        public bool IsLessThan(Extent e) => Diagonal < (e.Diagonal);
        public bool IsGreaterThan(Extent e) => Diagonal > (e.Diagonal); 
        #endregion

        #region Contains  =====================================================
        public bool ContainsX(int X)
        {
            if (X < X1) return false;
            if (X > X2) return false;
            return true;
        }

        public bool ContainsY(int Y)
        {
            if (Y < Y1) return false;
            if (Y > Y2) return false;
            return true;
        }

        public bool Contains((int X, int Y) p)
        {
            if (p.X < X1) return false;
            if (p.Y < Y1) return false;
            if (p.X > X2) return false;
            if (p.Y > Y2) return false;
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
        public bool HitTest(ref (int X, int Y) p, Extent E)
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
