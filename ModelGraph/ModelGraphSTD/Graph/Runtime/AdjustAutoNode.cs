
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

            var gx = node.Graph.GraphX;
            var tmSpc = gx.TerminalSpacing / 2;
            var tmLen = gx.TerminalLength;
            var tmsku = gx.TerminalSkew;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraBusSize) / 2;

            var (x, y) = node.Center;

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
                    if (ix < 3 || ix > 12) EIE.Add(i);
                    else if (ix > 4 && ix < 11) EIW.Add(i);
                    else if (ix > 10 && ix < 13) EIN.Add(i);
                    else EIS.Add(i);
                }
                EIN.Sort(CompN);
                EIS.Sort(CompS);
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
                    E[EIE[i]].edge.SetFace(node, (x + d1, y + ds), (x + d2, y + ds));
                }
                n = EIW.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIW[i]].edge.SetFace(node, (x - d1, y + ds), (x - d2, y + ds));
                }
                #endregion

                #region Compare  ==============================================
                int CompE(int a, int b)
                {
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompS(int a, int b)
                {
                    if (F[a].index < F[b].index) return -1;
                    if (F[a].index > F[b].index) return 1;
                    if (F[a].index < 4)
                        if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompN(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompW(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].slope > F[b].slope) return -1;
                    if (F[a].slope < F[b].slope) return 1;
                    return 0;
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
                    if (ix > 1 && ix < 7) EIS.Add(i);
                    else if (ix > 8 && ix < 15) EIN.Add(i);
                    else if (ix > 6 && ix < 9) EIW.Add(i);
                    else EIE.Add(i);
                }
                EIN.Sort(CompN);
                EIS.Sort(CompS);
                EIE.Sort(CompE);
                EIW.Sort(CompW);

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
                    E[EIN[i]].edge.SetFace(node, (x + ds, y - d1), (x + ds, y - d2));
                }
                n = EIS.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIS[i]].edge.SetFace(node, (x + ds, y + d1), (x + ds, y + d2));
                }
                #endregion

                #region Compare  ==============================================
                int CompN(int a, int b)
                {
                    if (F[a].index < F[b].index) return -1;
                    if (F[a].index > F[b].index) return 1;
                    if (F[a].index < 12)
                    {
                        if (F[a].slope < F[b].slope) return -1;
                        if (F[a].slope > F[b].slope) return 1;
                        return 0;
                    }
                    else
                    {
                        if (F[a].slope > F[b].slope) return -1;
                        if (F[a].slope < F[b].slope) return 1;
                        return 0;
                    }
                }
                int CompE(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompW(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].slope > F[b].slope) return -1;
                    if (F[a].slope < F[b].slope) return 1;
                    return 0;
                }
                int CompS(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].index < 4)
                    {
                        if (F[a].slope > F[b].slope) return -1;
                        if (F[a].slope < F[b].slope) return 1;
                        return 0;
                    }
                    else
                    {
                        if (F[a].slope < F[b].slope) return -1;
                        if (F[a].slope > F[b].slope) return 1;
                        return 0;
                    }
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
                    E[EIE[i]].edge.SetFace(node, (x + d1, y + ds), (x + d2, y + ds));
                }
                n = EIW.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIW[i]].edge.SetFace(node, (x - d1, y + ds), (x - d2, y + ds));
                }
                d1 = height;
                d2 = d1 + tmLen;
                n = EIN.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIN[i]].edge.SetFace(node, (x + ds, y - d1), (x + ds, y - d2));
                }
                n = EIS.Count;
                for (int i = 0; i < n; i++)
                {
                    var ds = tmSpc * Layout.Offset(i, n);
                    E[EIS[i]].edge.SetFace(node, (x + ds, y + d1), (x + ds, y + d2));
                }
                #endregion

                #region Compare  ==============================================
                int CompE(int a, int b)
                {
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompS(int a, int b)
                {
                    if (F[a].index < F[b].index) return -1;
                    if (F[a].index > F[b].index) return 1;
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompW(int a, int b)
                {
                    if (F[a].index < F[b].index) return -1;
                    if (F[a].index > F[b].index) return 1;
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                int CompN(int a, int b)
                {
                    if (F[a].index > F[b].index) return -1;
                    if (F[a].index < F[b].index) return 1;
                    if (F[a].slope < F[b].slope) return -1;
                    if (F[a].slope > F[b].slope) return 1;
                    return 0;
                }
                #endregion
                #endregion
            }
            else
            {
                #region Point  ================================================
                for (int i = 0; i < N; i++)
                {
                    E[i].edge.SetFace(node, (x, y));
                }
                #endregion
            }

            #region Legacy  ===================================================
            //int iEast, nEast, iWest, nWest, iSouth, nSouth, iNorth, nNorth;

            ////  Assign line end terminal points based  
            ////	on the just determined order of connections
            ////	and on the line end and node termination styles
            //if (node.Aspect == Aspect.Vertical)
            //{
            //    nEast = nquad[1] + nquad[4]; //the right side connection count
            //    nWest = nquad[2] + nquad[3]; //the left side  connection count
            //    iEast = 0;
            //    iWest = 0;

            //    var nVert = (nEast > nWest) ? nEast : nWest; //determine the resizable bar length
            //    var height = nVert * tmSpc;

            //    node.SetSize(barSize, height);
            //    dx1 = barSize;
            //    dx2 = dx1 + tmLen;
            //    dy1 = height;
            //    dy2 = dy1 + tmLen;

            //    for (int i = 0; i < count; i++)
            //    {
            //        if (E[i].quad == Quad.Q4) SetEast(i);
            //    }
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (E[i].quad == Quad.Q1) SetEast(i);
            //    }
            //    for (int i = last; i >= 0; i--)
            //    {
            //        if (E[i].quad == Quad.Q2 || E[i].quad == Quad.Q3) SetWest(i);
            //    }
            //}
            //else if (node.Aspect == Aspect.Horizontal)
            //{
            //    nSouth = nquad[1] + nquad[2];
            //    nNorth = nquad[3] + nquad[4];
            //    iSouth = 0;
            //    iNorth = 0;

            //    var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
            //    var width = nHorz * tmSpc;

            //    node.SetSize(width, barSize);
            //    dx1 = width;
            //    dx2 = dx1 + tmLen;
            //    dy1 = barSize;
            //    dy2 = dy1 + tmLen;

            //    for (int i = 0; i < count; i++)
            //    {
            //        if ((E[i].quad == Quad.Q3) || (E[i].quad == Quad.Q4)) SetNorth(i);
            //    }
            //    for (int i = last; i >= 0; i--)
            //    {
            //        if (E[i].quad == Quad.Q1 || E[i].quad == Quad.Q2) SetSouth(i);
            //    }
            //}
            //else if (node.Aspect == Aspect.Central)
            //{
            //    nSouth = nsect[2] + nsect[3];
            //    nWest = nsect[4] + nsect[5];
            //    nNorth = nsect[6] + nsect[7];
            //    nEast = nsect[8] + nsect[1];

            //    iSouth = 0;
            //    iWest = 0;
            //    iNorth = 0;
            //    iEast = 0;

            //    var nVert = (nEast > nWest) ? nEast : nWest;
            //    var height = (nVert * tmSpc);
            //    if (height < barSize) height = barSize;

            //    var nHorz = (nSouth > nNorth) ? nSouth : nNorth;
            //    var width = (nHorz * tmSpc);
            //    if (width < barSize) width = barSize;

            //    node.SetSize(width, height);
            //    dx1 = width;
            //    dx2 = dx1 + tmLen;
            //    dy1 = height;
            //    dy2 = dy1 + tmLen;

            //    for (int i = 0; i < count; i++)
            //    {
            //        if (E[i].sect == Sect.S6 || E[i].sect == Sect.S7) SetNorth(i);
            //        else if (E[i].sect == Sect.S8) SetEast(i);
            //    }
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (E[i].sect == Sect.S1) SetEast(i);
            //    }
            //    for (int i = last; i >= 0; i--)
            //    {
            //        if (E[i].sect == Sect.S2 || E[i].sect == Sect.S3) SetSouth(i);
            //        else if (E[i].sect == Sect.S4 || E[i].sect == Sect.S5) SetWest(i);
            //    }
            //}
            //else if (node.Aspect == Aspect.Point)
            //{
            //    for (int i = 0; i < count; i++)
            //    {
            //        var (bx, by) = E[i].bend;
            //        var dbx = bx - x;
            //        var dby = by - y;

            //        if (E[i].atch == Attach.RightAngle)
            //        {
            //            if (E[i].horz)
            //                E[i].edge.SetFace(node, (x, y), (x, y + dby));
            //            else
            //                E[i].edge.SetFace(node, (x, y), (x + dbx, y)); 
            //        }
            //        else if (E[i].atch == Attach.SkewedAngle)
            //        {
            //            if (E[i].horz)
            //            {
            //                if (E[i].quad == Quad.Q4 || E[i].quad == Quad.Q1)
            //                    E[i].edge.SetFace(node, (x, y), (x + tmsku, y + dby));
            //                else
            //                    E[i].edge.SetFace(node, (x, y), (x - tmsku, y + dby));
            //            }
            //            else
            //            {
            //                if (E[i].quad == Quad.Q2 || E[i].quad == Quad.Q1)
            //                    E[i].edge.SetFace(node, (x, y), (x + dbx, y + tmsku));
            //                else
            //                    E[i].edge.SetFace(node, (x, y), (x + dbx, y - tmsku));
            //            }
            //        }
            //        else
            //        {
            //            if (E[i].sect == Sect.S8 || E[i].sect == Sect.S1) E[i].edge.SetFace(node, (x, y));
            //            else if (E[i].sect == Sect.S2 || E[i].sect == Sect.S3) E[i].edge.SetFace(node, (x, y));
            //            else if (E[i].sect == Sect.S4 || E[i].sect == Sect.S5) E[i].edge.SetFace(node, (x, y));
            //            else if (E[i].sect == Sect.S6 || E[i].sect == Sect.S7) E[i].edge.SetFace(node, (x, y));
            //        }
            //    }
            //}
            //else
            //    throw new System.InvalidOperationException("Node has invalid aspect");

            //void SetEast(int e)
            //{
            //    var ds = (short)(tmSpc * Layout.Offset(iEast++, nEast));
            //    E[e].edge.SetFace(node, (x + dx1, y + ds), (x + dx2, y + ds));
            //}
            //void SetWest(int e)
            //{
            //    var ds = (short)(tmSpc * Layout.Offset(iWest++, nWest));
            //    E[e].edge.SetFace(node, (x - dx1, y + ds), (x - dx2, y + ds));
            //}
            //void SetSouth(int e)
            //{
            //    var ds = (short)(tmSpc * Layout.Offset(iSouth++, nSouth));
            //    E[e].edge.SetFace(node, (x + ds, y + dy1), (x + ds, y + dy2));
            //}
            //void SetNorth(int e)
            //{
            //    var ds = (short)(tmSpc * Layout.Offset(iNorth++, nNorth));
            //    E[e].edge.SetFace(node, (x + ds, y - dy1), (x + ds, y - dy2));
            //}
            #endregion
        }
    }
}
