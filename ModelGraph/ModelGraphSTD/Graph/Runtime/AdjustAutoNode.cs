
using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Graph
    {
        private void AdjustAutoNode(Node node)
        {
            var E = Layout.ConnectedEdges(node);
            var N = (E is null) ? 0 : E.Length;
            if (N == 0) return;

            var (x, y) = node.Center;

            var gx = node.Graph.GraphX;
            var tmSpc = gx.TerminalSpacing / 2;
            var tmLen = gx.TerminalLength;
            var tmsku = gx.TerminalSkew;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraBusSize) / 2;

            var EIN = new List<int>(N); //sorted edge index list north side
            var EIS = new List<int>(N); //sorted edge index list south side
            var EIE = new List<int>(N); //sorted edge index list east side
            var EIW = new List<int>(N); //sorted edge index list west side

            #region EdgeSlopesSortCriteria  ===================================
            // this is used when sorting the edge index lists
            // index (0:15) identifies the sector containing the edge end point
            var F = new (float dx, float dy, float slope, int index)[N];
            for (int i = 0; i < N; i++)
            {
                F[i] = XYTuple.SlopeIndex((x, y), E[i].bend);
            }
            #endregion

            if (node.Aspect == Aspect.Vertical)
            {
                #region Vertical  =============================================
                for (int i = 0; i < N; i++)
                {
                    var ix = F[i].index;
                    if (ix < 4 || ix > 11) EIE.Add(i);
                    else EIW.Add(i);
                }
                EIE.Sort(CompE);
                EIW.Sort(CompW);

                var width = barSize;
                var height = tmSpc * (EIE.Count > EIW.Count ? EIE.Count : EIW.Count);
                node.SetSize(width, height);

                #region SetFace  ==============================================
                var d1 = width;
                var d2 = width + tmLen;
                var n = EIE.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIE[i]].edge.SetFace(node, (x + d1, y + ds), (x + d2, y + ds), TupleSort.East);
                }
                n = EIW.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIW[i]].edge.SetFace(node, (x - d1, y + ds), (x - d2, y + ds), TupleSort.West);
                }
                #endregion
                #endregion
            }
            else if (node.Aspect == Aspect.Horizontal)
            {
                #region Horizontal  ===========================================
                for (int i = 0; i < N; i++)
                {
                    var ix = F[i].index;
                    if (ix < 8) EIS.Add(i);
                    else EIN.Add(i);
                }
                EIN.Sort(CompN);
                EIS.Sort(CompS);

                var height = barSize;
                var width = tmSpc * (EIN.Count > EIS.Count ? EIN.Count : EIS.Count);
                node.SetSize(width, height);

                #region SetFace  ==============================================
                var d1 = height;
                var d2 = height + tmLen;
                var n = EIN.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIN[i]].edge.SetFace(node, (x + ds, y - d1), (x + ds, y - d2), TupleSort.North);
                }
                n = EIS.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIS[i]].edge.SetFace(node, (x + ds, y + d1), (x + ds, y + d2), TupleSort.South);
                }
                #endregion

                #endregion
            }
            else if (node.Aspect == Aspect.Central)
            {
                #region Central  ==============================================
                for (int i = 0; i < N; i++)
                {
                    var ix = F[i].index;
                    if (ix < 2 || ix > 13) EIE.Add(i);
                    else if (ix > 4 && ix < 10) EIW.Add(i);
                    else if (ix > 9 && ix < 14) EIN.Add(i);
                    else EIS.Add(i);
                }
                EIN.Sort(CompN);
                EIS.Sort(CompS);
                EIE.Sort(CompE);
                EIW.Sort(CompW);

                var width = tmSpc * (EIN.Count > EIS.Count ? EIN.Count : EIS.Count);
                if (width < barSize) width = barSize;
                var height = tmSpc * (EIE.Count > EIW.Count ? EIE.Count : EIW.Count);
                if (height < barSize) height = barSize;
                node.SetSize(width, height);

                #region SetFace  ==============================================
                var d1 = width;
                var d2 = d1 + tmLen;
                var n = EIE.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIE[i]].edge.SetFace(node, (x + d1, y + ds), (x + d2, y + ds), TupleSort.East);
                }
                n = EIW.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIW[i]].edge.SetFace(node, (x - d1, y + ds), (x - d2, y + ds), TupleSort.West);
                }
                d1 = height;
                d2 = d1 + tmLen;
                n = EIN.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIN[i]].edge.SetFace(node, (x + ds, y - d1), (x + ds, y - d2), TupleSort.North);
                }
                n = EIS.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIS[i]].edge.SetFace(node, (x + ds, y + d1), (x + ds, y + d2), TupleSort.South);
                }
                #endregion
                #endregion
            }
            else
            {
                #region Point  ================================================
                for (int i = 0; i < N; i++)
                {
                    var (dx, dy, slope, index) = F[i];
                    var quad = (index / 4) + 1;

                    if (E[i].atch == Attach.RightAngle)
                    {
                        if (E[i].tsort == TupleSort.East || E[i].tsort == TupleSort.West)
                            E[i].edge.SetFace(node, (x, y), (x, y + dy), TupleSort.Any);
                        else
                            E[i].edge.SetFace(node, (x, y), (x + dx, y), TupleSort.Any);
                    }
                    else if (E[i].atch == Attach.SkewedAngle)
                    {
                        if (E[i].tsort == TupleSort.East || E[i].tsort == TupleSort.West)
                        {
                            if (quad == 4 || quad == 1)
                                E[i].edge.SetFace(node, (x, y), (x + tmsku, y + dy), TupleSort.Any);
                            else
                                E[i].edge.SetFace(node, (x, y), (x - tmsku, y + dy), TupleSort.Any);
                        }
                        else
                        {
                            if (quad == 2 || quad == 1)
                                E[i].edge.SetFace(node, (x, y), (x + dx, y + tmsku), TupleSort.Any);
                            else
                                E[i].edge.SetFace(node, (x, y), (x + dx, y - tmsku), TupleSort.Any);
                        }
                    }
                    else
                    {
                        E[i].edge.SetFace(node, (x, y), TupleSort.Any);
                    }
                }
                #endregion
            }

            #region Compare  ==============================================
            int CompE(int a, int b)
            {
                var tup1 = E[a].tuple;
                if (tup1 != 0)
                {
                    var tup2 = E[b].tuple;
                    if (tup2 == tup1)
                        return TupleCompE[(int)E[a].tsort](E[a].order, E[b].order);
                }
                if (F[a].slope < F[b].slope) return -1;
                if (F[a].slope > F[b].slope) return 1;
                return 0;
            }
            int CompS(int a, int b)
            {
                var tup1 = E[a].tuple;
                if (tup1 != 0)
                {
                    var tup2 = E[b].tuple;
                    if (tup2 == tup1)
                        return TupleCompS[(int)E[a].tsort](E[a].order, E[b].order);
                }
                if (F[a].index > F[b].index) return -1;
                if (F[a].index < F[b].index) return 1;
                if (F[a].slope > F[b].slope) return -1;
                if (F[a].slope < F[b].slope) return 1;
                return 0;
            }
            int CompW(int a, int b)
            {
                var tup1 = E[a].tuple;
                if (tup1 != 0)
                {
                    var tup2 = E[b].tuple;
                    if (tup2 == tup1)
                        return TupleCompW[(int)E[a].tsort](E[a].order, E[b].order);
                }
                if (F[a].slope > F[b].slope) return -1;
                if (F[a].slope < F[b].slope) return 1;
                return 0;
            }
            int CompN(int a, int b)
            {
                var tup1 = E[a].tuple;
                if (tup1 != 0)
                {
                    var tup2 = E[b].tuple;
                    if (tup2 == tup1)
                        return TupleCompN[(int)E[a].tsort](E[a].order, E[b].order);
                }
                if (F[a].index < F[b].index) return -1;
                if (F[a].index > F[b].index) return 1;
                if (F[a].slope < F[b].slope) return -1;
                if (F[a].slope > F[b].slope) return 1;
                return 0;
            }
            #endregion
        }
        private static Func<int, int, int>[] TupleCompE = new Func<int, int, int>[]
        {
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.Any
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.East
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.South
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.West
            (a, b) =>(a > b) ? -1 : (a < b) ? 1 : 0, // TupleSort.North
        };
        private static Func<int, int, int>[] TupleCompS = new Func<int, int, int>[]
        {
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.Any
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.East
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.South
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.West
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.North
        };
        private static Func<int, int, int>[] TupleCompW = new Func<int, int, int>[]
        {
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.Any
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.East
            (a, b) =>(a > b) ? -1 : (a < b) ? 1 : 0, // TupleSort.South
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.West
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.North
        };
        private static Func<int, int, int>[] TupleCompN = new Func<int, int, int>[]
        {
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.Any
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.East
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.South
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.West
            (a, b) =>(a < b) ? -1 : (a > b) ? 1 : 0, // TupleSort.North
        };
    }
}
