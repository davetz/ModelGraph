using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace ModelGraphLibrary
{
    public struct Extent
    {
        internal int X1;
        internal int Y1;
        internal int X2;
        internal int Y2;

        #region Constructor  ==================================================
        internal Extent(int s)
        {
            X1 = X2 = Y1 = Y2 = s;
        }

        internal Extent(XYPoint p)
        {
            X1 = X2 = p.X;
            Y1 = Y2 = p.Y;
        }

        internal Extent(int x, int y)
        {
            X1 = X2 = x;
            Y1 = Y2 = y;
        }

        internal Extent(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        internal Extent(XYPoint p, int ds)
        {
            X1 = p.X + 1 - ds;
            X2 = p.X + 1 + ds;
            Y1 = p.Y + 1 - ds;
            Y2 = p.Y + 1 + ds;
        }

        internal Extent(XYPoint p, Vector2 ds)
        {
            X1 = p.X - (int)ds.X;
            X2 = p.X + (int)ds.X;
            Y1 = p.Y - (int)ds.Y;
            Y2 = p.Y + (int)ds.Y;
        }

        internal Extent(XYPoint p1, XYPoint p2)
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
        internal void Move(XYPoint delta)
        {
            X1 += delta.X;
            X2 += delta.X;
            Y1 += delta.Y;
            Y2 += delta.Y;
        }
        #endregion

        #region Shape  ========================================================
        internal bool IsTall { get { return DY > DX; } }
        internal bool IsWide { get { return DX > DY; } }
        internal bool IsEmpty { get { return (IsVertical || IsHorizontal); } }
        internal bool HasArea { get { return (X1 != X2 && Y1 != Y2); } }
        internal bool IsVertical { get { return (X1 == X2); } }
        internal bool IsHorizontal { get { return (Y1 == Y2); } }
        #endregion

        #region Center  =======================================================
        internal int CenterX { get { return (X2 + X1) / 2; } }
        internal int CenterY { get { return (Y2 + Y1) / 2; } }
        internal XYPoint Center { get { return new XYPoint(CenterX, CenterY); } }
        #endregion

        #region Points  =======================================================
        internal void Points(XYPoint p1, XYPoint p2) { Point1 = p1; Point2 = p2; }
        internal XYPoint Point1 { get { return new XYPoint(X1, Y1); } set { X1 = value.X; Y1 = value.Y; } }
        internal XYPoint Point2 { get { return new XYPoint(X2, Y2); } set { X2 = value.X; Y2 = value.Y; } }
        internal void Record(XYPoint p) { Point1 = Point2; Point2 = p; }

        internal void Record(XYPoint p, float scale) { Point1 = Point2; SetPoint2(p, scale); }
        internal void SetPoint1(XYPoint p, float scale) { X1 = (int)(p.X * scale); Y1 = (int)(p.Y * scale); }
        internal void SetPoint2(XYPoint p, float scale) { X2 = (int)(p.X * scale); Y2 = (int)(p.Y * scale); }
        #endregion

        #region Expand  =======================================================
        internal void Expand(int margin)
        {
            X1 -= margin;
            Y1 -= margin;
            X2 += margin;
            Y2 += margin;
        }

        internal void Expand(int x, int y)
        {
            if (x < X1) X1 = x;
            if (y < Y1) Y1 = y;
            if (x > X2) X2 = x;
            if (y > Y2) Y2 = y;
        }
        internal void Expand(XYPoint p)
        {
            if (p.X < X1) X1 = p.X;
            if (p.Y < Y1) Y1 = p.Y;
            if (p.X > X2) X2 = p.X;
            if (p.Y > Y2) Y2 = p.Y;
        }
        #endregion

        #region Diagonal  =====================================================
        internal int DX { get { return X2 - X1; } }
        internal int DY { get { return Y2 - Y1; } }
        internal int Length { get { return (int)Math.Sqrt(Diagonal); } }
        internal int Diagonal { get { return Delta.Diagonal; } }
        internal float Slope { get { return Delta.Slope; } }
        internal int Quad { get { return Delta.Quad; } }
        internal XYPoint Delta { get { return new XYPoint(DX, DY); } }

        internal bool TryGetDelta(out XYPoint delta)
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

        internal void Normalize()
        {
            Normalize(Point1, Point2);
        }

        internal void Normalize(XYPoint p1, XYPoint p2)
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
        internal Extent SetExtent(List<Node> nodes, int margin)
        {
            if (nodes.Count > 0)
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = int.MinValue;
                foreach (var node in nodes)
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

        internal Extent SetExtent(XYPoint[] points, int margin)
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
        internal int Xmin { get { return (X1 < X2) ? X1 : X2; } }
        internal int Ymin { get { return (Y1 < Y2) ? Y1 : Y2; } }
        internal int Xmax { get { return (X2 > X1) ? X2 : X1; } }
        internal int Ymax { get { return (Y2 > Y1) ? Y2 : Y1; } }

        internal int Width { get { return (Xmax - Xmin); } }
        internal int Hieght { get { return (Ymax - Ymin); } }
        internal Rect Rect { get { return new Rect(Xmin, Ymin, Width, Hieght); } }
        internal Rect GetRect(int x, int y, float z)
        {
            var r = Rect;
            return new Rect(z * (r.X - x), z * (r.Y - y), z * r.Width, z * r.Height);
        }

        internal Vector2 UpperLeft { get { return new Vector2(Xmin, Ymin); } }
        internal Vector2 LowerRight { get { return new Vector2(Xmax, Xmax); } }
        internal XYPoint TopLeft { get { return new XYPoint(Xmin, Ymin); } }
        internal XYPoint TopRight { get { return new XYPoint(Xmax, Ymin); } }
        internal XYPoint BottomLeft { get { return new XYPoint(Xmin, Ymax); } }
        internal XYPoint BottomRight { get { return new XYPoint(Xmax, Ymax); } }

        internal void ScrollVertical(int ds) { Y1 += ds; Y2 += ds; }
        internal void ScrollHorizontal(int ds) { X1 += ds; X2 += ds; }
        #endregion

        #region Comparison  ===================================================
        internal bool IsLessThan(ref Extent e) { return Diagonal < (e.Diagonal); }
        internal bool IsGreaterThan(ref Extent e) { return Diagonal > (e.Diagonal); }
        #endregion

        #region Contains  =====================================================
        internal bool ContainsX(int X)
        {
            if (X < X1) return false;
            if (X > X2) return false;
            return true;
        }

        internal bool ContainsY(int Y)
        {
            if (Y < Y1) return false;
            if (Y > Y2) return false;
            return true;
        }

        internal bool Contains(XYPoint p)
        {
            if (p.X < X1) return false;
            if (p.Y < Y1) return false;
            if (p.X > X2) return false;
            if (p.Y > Y2) return false;
            return true;
        }
        internal bool Contains(Vector2 p)
        {
            if (p.X < X1) return false;
            if (p.Y < Y1) return false;
            if (p.X > X2) return false;
            if (p.Y > Y2) return false;
            return true;
        }

        internal bool Contains(ref Extent e)
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
        internal bool HitTest(ref XYPoint p, ref Extent E)
        {
            if (Intersects(ref E))  // my extent intersects with E
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
        internal bool Intersects(ref Extent e)
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
