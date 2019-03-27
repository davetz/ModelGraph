
using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Graph
    {
        private void AdjustNode(Node node)
        {
            if (!Node_Edges.TryGetValue(node, out List<Edge> edges)) return;
            var N = edges.Count;
            if (N == 0) return;

            var E = new ((float x, float y) bend, Node other, int order, Target targ, Attach atch, Direction odir)[N];  // primary edge data
            var F = new ((float dx, float dy) delta, float slope, byte slice, byte tix, byte tsiz, Direction tdir)[N];  // secondary edge target data
            var I = new List<int>(N);                                                                                   // sorted edge index list

            #region PopulateEdgeDataArrays  ===================================
            var node_tuple = new Dictionary<Node, bool>(N);
            var haveTuples = false;

            for (int i = 0; i < N; i++)
            {
                I.Add(i); // the edge index list will be sorted based on comparison of edge data

                var (other, bend, targ, atch, tdir) = edges[i].OtherBendTargetAttachDirection(node);
                var (dx, dy, slope, slice) = XYTuple.SlopeSlice(node.Center, bend);

                E[i] = (bend, other, 0, targ, atch, tdir);
                F[i] = ((dx, dy), slope, (byte)slice, 0, 0, Direction.Any);

                var isTuple = node_tuple.ContainsKey(other);
                node_tuple[other] = isTuple;
                haveTuples |= isTuple;
            }
            if (haveTuples)
            {
                for (int i = 0; i < N; i++)
                {
                    if (node_tuple[E[i].other]) E[i].order = edges[i].GetHashCode(); // set the edge tuple sort criteria
                }
            }
            #endregion

            var gx = node.Graph.GraphX;
            var spSkew = 8;// gx.SurfaceSkew;
            var tpSkew = gx.TerminalSkew;
            var tmLen = gx.TerminalLength;
            var tmSpc = gx.TerminalSpacing / 2f;
            var barSize = ((node.BarWidth == BarWidth.Thin) ? gx.ThinBusSize : (node.BarWidth == BarWidth.Wide) ? gx.WideBusSize : gx.ExtraBusSize) / 2;

            var si = node.Symbol - 2;
            var symbols = node.Graph.Symbols;

            var (ncx, ncy, ndx, ndy) = node.Values(); //node's center x,y and node's half width,height

            if (si >= 0 && si < symbols.Length)
            {
                #region AdjustSymbol  =========================================
                var symbol = Symbols[si];
                var targetCount = symbol.TargetContacts.Count;

                var targetContacts = new List<(Target trg, byte tix, Contact con, (float dx, float dy) pnt)>(targetCount);

                var testResult = new List<(int ei, float c, float m, int x1, int y1, int x2, int y2, int s)>[targetCount];

                for (int i = 0; i < targetCount; i++)
                {
                    testResult[i] = new List<(int ei, float c, float m, int x1, int y1, int x2, int y2, int s)>(N);
                }

                var bestFlip = FlipState.None;
                var bestCost = float.MaxValue;

                var scale = GraphX.SymbolScale;

                ClearTestResults();
                byte[][] penalty; // [edge sector index] [symbol target index]

                #region FindBestArrangement  ==================================
                foreach (var (flip, autoFlip) in _allFlipStates)
                {
                    if (flip != FlipState.None && (symbol.AutoFlip & autoFlip) == 0) continue; //this autoflip is not allowed

                    var testCost = 0f;
                    penalty = symbol.GetFlipTargetPenalty(flip);
                    symbol.GetFlipTargetContacts(flip, ncx, ncy, scale, tmLen, targetContacts);
                    (float cost, float slope, int x1, int y1, int x2, int y2, int six) bestTarget;
                    var bestTi = -1;

                    testCost = 0;
                    for (int ei = 0; ei < N; ei++)
                    {
                        bestTi = -1;
                        bestTarget = (float.MaxValue, 0, 0, 0, 0, 0, 0);

                        for (int ti = 0; ti < targetCount; ti++)
                        {
                            if (!SetBestTarget(ei, ti)) continue;
                        }
                        if (!AddToTestResult(ei, bestTi)) break;
                    }
                    SetBestResult();

                    testCost = 0;
                    for (int ei = N - 1; ei >= 0; ei--)
                    {
                        bestTi = -1;
                        bestTarget = (float.MaxValue, 0, 0, 0, 0, 0, 0);

                        for (int ti = targetCount - 1; ti >= 0; ti--)
                        {
                            if (!SetBestTarget(ei, ti)) continue;
                        }
                        if (!AddToTestResult(ei, bestTi)) break;
                    }
                    SetBestResult();

                    #region SetBestTarget  ========================================
                    bool SetBestTarget(int cei, int cti)
                    {
                        if ((targetContacts[cti].trg & E[cei].targ) == 0) return false; // skip targets that won't mate with the edge
                        if (targetContacts[cti].con == Contact.One && testResult[cti].Count > 0) return false; //skip targes that are already at capacity

                        var testTarget = EdgetTarget_CostSlopSectorIndex(cei, cti);
                        if (testTarget.cost < bestTarget.cost)
                        {
                            bestTi = cti;
                            bestTarget = testTarget;
                        }
                        return true;
                    }
                    #endregion

                    #region SetBestResult  ========================================
                    void SetBestResult()
                    {
                        if (testCost < bestCost)
                        {
                            bestFlip = flip;
                            bestCost = testCost;

                            for (int ti = 0; ti < targetCount; ti++)
                            {
                                var result = testResult[ti];
                                if (result.Count == 0) continue;

                                var (sdx, sdy, tsiz, tix, tdir) = symbol.GetFlipTarget(ti, bestFlip, scale);
                                foreach (var (ei, cost, slope, x1, y1, x2, y2, slice) in result)
                                {
                                    F[ei] = ((sdx, sdy), slope, (byte)slice, tix, tsiz, tdir);
                                }
                            }
                        }
                        ClearTestResults();
                    }
                    #endregion

                    #region AddToTestResult  ======================================
                    bool AddToTestResult(int cei, int cti)
                    {
                        if (cti >= 0 && cti < targetCount) 
                        {
                            var factor = CrissCrossFactor(cei);
                            var (cost, slope, x1, y1, x2, y2, six) = bestTarget;
                            cost *= factor;
                            testCost += cost;
                            if (testCost > bestCost) return false; //abort, this is a bad choice of flip state

                            for (int t = 0; t < targetCount; t++)
                            {
                                var pre = testResult[t];
                                if (pre.Count == 0) continue;
                                for (int j = pre.Count - 1; j >= 0; j--)
                                {
                                    if (pre[j].ei == cei)
                                        pre.RemoveAt(j); // get rid of duplicate edge from a previous try
                                }
                            }
                            testResult[cti].Add((cei, cost, slope, x1, y1, x2, y2, six));
                        }
                        return true;
                    }
                    #endregion

                    #region CrissCrossFactor  =====================================
                    float CrissCrossFactor(int cei)
                    {
                        var p = bestTarget;
                        for (int t = 0; t < targetCount; t++)
                        {
                            var pre = testResult[t];
                            foreach (var q in pre)
                            {
                                if (cei == q.ei) continue; //skip if duplicate edge from a previous best try
                                if (NoIntersection()) continue;
                                return 20f;

                                #region NoIntersection  =======================
                                bool NoIntersection()
                                {
                                    int pxmin, pymin, pxmax, pymax, qxmin, qymin, qxmax, qymax;
                                    if (p.x1 < p.x2) { pxmin = p.x1; pxmax = p.x2; } else { pxmin = p.x2; pxmax = p.x1; }
                                    if (q.x1 < q.x2) { qxmin = q.x1; qxmax = q.x2; } else { qxmin = q.x2; qxmax = q.x1; }
                                    if (qxmax < pxmin || qxmin > pxmax) return true; //non intersecting boundry boxes

                                    if (p.y1 < p.y2) { pymin = p.y1; pymax = p.y2; } else { pymin = p.y2; pymax = p.y1; }
                                    if (q.y1 < q.y2) { qymin = q.y1; qymax = q.y2; } else { qymin = q.y2; qymax = q.y1; }
                                    if (qymax < pymin || qymin > pymax) return true; //non intersecting boundry boxes

                                    var pdx = p.x2 - p.x1;
                                    var pdy = p.y2 - p.y1;
                                    if (pdx == 0 && pdy == 0) return true; //line segment has zero length

                                    var qdx = q.x2 - q.x1;
                                    var qdy = q.y2 - q.y1;
                                    if (qdx == 0 && qdy == 0) return true; //line segment has zero length

                                    if (p.x1 == q.x1 && p.y1 == q.y1) return false; // cooincident end points
                                    if (p.x1 == q.x2 && p.y1 == q.y2) return false; // cooincident end points
                                    if (p.x2 == q.x1 && p.y2 == q.y1) return false; // cooincident end points
                                    if (p.x2 == q.x2 && p.y2 == q.y2) return false; // cooincident end points

                                    if (pdy == 0 && qdy == 0) return true; // parallel vartical line segments
                                    if (pdx == 0 && qdx == 0) return true; // parallel horizontal line segments

                                    if (pdx == 0 && qdy == 0 && p.x1 > qxmin && p.x1 < qxmax && q.y1 > pymin && q.y1 < pymax) return false; //crossing perpendicular sline segments
                                    if (pdy == 0 && qdx == 0 && p.y1 > qymin && p.y1 < qymax && q.x1 > pxmin && q.x1 < pxmax) return false; //crossing perpendicular sline segments

                                    var pm = (pdx == 0) ? (pdy > 0) ? 8192f : -8192f : pdy / (float)pdx;
                                    var qm = (qdx == 0) ? (qdy > 0) ? 8192f : -8192f : qdy / (float)qdx;

                                    var pb = p.y1 - pm * p.x1;
                                    var qb = q.y1 - qm * q.x1;

                                    var rm = (qm == 0) ? 8192f : pm / qm;

                                    var y = (-rm * qb + pb) / (1 - rm);
                                    if (y < pymin) return true;
                                    if (y > pymax) return true;
                                    if (y < qymin) return true;
                                    if (y > qymax) return true;

                                    var x = (y - pb) / pm;
                                    if (x < pxmin) return true;
                                    if (x > pxmax) return true;
                                    if (x < qxmin) return true;
                                    if (x > qxmax) return true;

                                    return false;
                                }
                                #endregion
                            }
                        }
                        return 1f;
                    }
                    #endregion
                }
                #endregion

                #region AssignEdgeContacts  ===================================
                node.FlipState = bestFlip;
                if (bestFlip < FlipState.LeftRotate)
                {
                    node.DX = (byte)(symbol.Width * scale / 2);
                    node.DY = (byte)(symbol.Height * scale / 2);
                }
                else
                {
                    node.DY = (byte)(symbol.Width * scale / 2);
                    node.DX = (byte)(symbol.Height * scale / 2);
                }

                SetFacePoints();
                #endregion

                #region ClearTestResults  =====================================
                void ClearTestResults()
                {
                    foreach (var list in testResult) { list.Clear(); }
                }
                #endregion

                #region EdgetTarget_CostSlopeSectorIndex  =====================
                // populate the edgeSect list for given target point
                (float cost, float slope, int x1, int y1, int x2, int y2, int six) EdgetTarget_CostSlopSectorIndex(int ei, int ti)
                {
                    var (dx, dy, slope, six) = XYTuple.SlopeSlice(targetContacts[ti].pnt, E[ei].bend);
                    var tix = targetContacts[ti].tix;

                    var pi = penalty[tix][six]; // [from direction sector index] [to target location index]
                    var cost = ((dx * dx) + (dy * dy)) * _penaltyFactor[pi]; //weighted cost

                    var (x1, y1) = targetContacts[ti].pnt;
                    var (x2, y2) = E[ei].bend;

                    return (cost, slope, (int)x1, (int)y1, (int)x2, (int)y2, six);
                }
                #endregion
                #endregion
            }
            else if (node.Sizing == Sizing.Auto || node.Aspect == Aspect.Point)
            {
                #region AdjustAutoNode  =======================================
                if (node.Aspect == Aspect.Vertical)
                {
                    #region Vertical  =============================================
                    int nE = 0, nW = 0;
                    for (int i = 0; i < N; i++)
                    {
                        var s = F[i].slice;
                        if (s < 4 || s > 11)
                        {
                            nE++;
                            F[i].tdir = Direction.E;
                        }
                        else
                        {
                            nW++;
                            F[i].tdir = Direction.W;
                        }
                    }

                    var width = barSize;
                    var height = tmSpc * (nE > nW ? nE : nW);
                    node.SetSize(width, height);

                    SetFaceOffset();
                    #endregion
                }
                else if (node.Aspect == Aspect.Horizontal)
                {
                    #region Horizontal  ===========================================
                    int nN = 0, nS = 0;
                    for (int i = 0; i < N; i++)
                    {
                        var s = F[i].slice;
                        if (s < 8)
                        {
                            nS++;
                            F[i].tdir = Direction.S;
                        }
                        else
                        {
                            nN++;
                            F[i].tdir = Direction.N;
                        }
                    }

                    var height = barSize;
                    var width = tmSpc * (nN > nS ? nN : nS);
                    node.SetSize(width, height);

                    SetFaceOffset();
                    #endregion
                }
                else if (node.Aspect == Aspect.Central)
                {
                    #region Central  ==============================================
                    int nN = 0, nS = 0, nE = 0, nW = 0;
                    for (int i = 0; i < N; i++)
                    {
                        var s = F[i].slice;
                        if (s < 2 || s > 13)
                        {
                            nE++;
                            F[i].tdir = Direction.E;
                        }
                        else if (s > 4 && s < 10)
                        {
                            nW++;
                            F[i].tdir = Direction.W;
                        }
                        else if (s > 9 && s < 14)
                        {
                            nN++;
                            F[i].tdir = Direction.N;
                        }
                        else
                        {
                            nS++;
                            F[i].tdir = Direction.S;
                        }
                    }

                    var width = tmSpc * (nN > nS ? nN : nS);
                    if (width < barSize) width = barSize;

                    var height = tmSpc * (nE > nW ? nE : nW);
                    if (height < barSize) height = barSize;

                    node.SetSize(width, height);

                    SetFaceOffset();
                    #endregion
                }
                else
                {
                    #region Point  ================================================
                    for (int i = 0; i < N; i++)
                    {
                        var ((dx, dy), slope, slice, tix, tsiz, tdir) = F[i];
                        var quad = (slice / 4) + 1;

                        if (E[i].atch == Attach.RightAngle)
                        {
                            if (E[i].odir == Direction.E || E[i].odir == Direction.W)
                                edges[i].SetFace(node, (0, 0), (0, dy), Direction.Any);
                            else
                                edges[i].SetFace(node, (0, 0), (dx, 0), Direction.Any);
                        }
                        else if (E[i].atch == Attach.SkewedAngle)
                        {
                            if (E[i].odir == Direction.E || E[i].odir == Direction.W)
                            {
                                if (quad == 4 || quad == 1)
                                    edges[i].SetFace(node, (0, 0), (tpSkew, dy), Direction.Any);
                                else
                                    edges[i].SetFace(node, (0, 0), (-tpSkew, dy), Direction.Any);
                            }
                            else
                            {
                                if (quad == 2 || quad == 1)
                                    edges[i].SetFace(node, (0, 0), (dx, tpSkew), Direction.Any);
                                else
                                    edges[i].SetFace(node, (0, 0), (dx, -tpSkew), Direction.Any);
                            }
                        }
                        else
                        {
                            edges[i].SetFace(node, (0, 0), Direction.Any);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region AdjustManualNode  =====================================
                var (x, y, w, h) = node.Values();
                var d0 = 2;
                var xE = x + w - d0; // east side
                var xW = x - w + d0; // west side
                var yS = y + h - d0; // south side
                var yN = y - h + d0; // north side

                var dx0 = w - d0;
                var dx1 = w;
                var dx2 = dx1 + spSkew;
                var dx3 = dx2 + tmLen;
                var dx4 = dx3 + spSkew;

                var dy0 = h - d0;
                var dy1 = h;
                var dy2 = dy1 + spSkew;
                var dy3 = dy2 + tmLen;
                var dy4 = dy3 + spSkew;
                /*
                 * delta dx1,dy1 determine the surface of the node
                 * ================================================
                    (-dx4, -dy4)  o             o  (+dx4, -dy4)
                     (-dx3, -dy3)  o           o  (+dx3, -dy3)
                      (-dx2, -dy2)  o         o  (+dx2, -dy2)
                       (-dx1, -dy1)  o-------o  (+dx1, -dy1) <== upper left,right corner of node
                        (-dx0, -dy0) |o     o| (+dx0, -dy0)
                                     |       |
                                     | (x,y) |  <== center of node (x,y)
                                     |       |
                        (-dx0, +dy0) |o     o| (+dx0, +dy0)
                       (-dx1, +dy1)  o-------o  (+dx1, +dy1) <== lower left,right corner of node
                      (-dx2, +dy2)  o         o  (+dx2, +dy2)
                     (-dx3, +dy3)  o           o  (+dx3, +dy3)
                    (-dx4, +dy4)  o             o  (+dx4, +dy4)                          
                 * ================================================
                 */
                for (int i = 0; i < N; i++)
                {
                    var (x2, y2) = E[i].bend; // closest edge bend point towards the destination node

                    var dx = x2 - x;
                    var dy = y2 - y;

                    var dxE = (double)(xE - x2);
                    var dxW = (double)(xW - x2);
                    var dyS = (double)(yS - y2);
                    var dyN = (double)(yN - y2);

                    if (node.Aspect == Aspect.Vertical)
                    {
                        #region Vertical  =========================================
                        if (x2 > xE)
                        {//========================== on the east side of major axis
                            if (y2 > yS)
                            {//========================== going south
                                if (dyS / dxE < 1)
                                {
                                    F[i].delta = (dx1, dy0);
                                    F[i].tdir = Direction.E;
                                }
                                else
                                {
                                    F[i].delta = (0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                            }
                            else if (y2 < yN)
                            {//========================== going north
                                if (dyN / dxE > -1)
                                {
                                    F[i].delta = (dx1, dy0);
                                    F[i].tdir = Direction.E;
                                }
                                else
                                {
                                    F[i].delta = (0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                            }
                            else
                            {
                                F[i].delta = (dx, dy1);
                                F[i].tdir = Direction.E;
                            }
                        }
                        else if (x2 < xW)
                        {//========================== on the west side of major axis
                            if (y2 > yS)
                            {//========================== going south
                                if (dyS / dxW > -1)
                                {
                                    F[i].delta = (-dx, dy0);
                                    F[i].tdir = Direction.W;
                                }
                                else
                                {
                                    F[i].delta = (0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                            }
                            else if (y2 < yN)
                            {//========================== going north
                                if (dyN / dxW < 1)
                                {
                                    F[i].delta = (-dx1, dy0);
                                    F[i].tdir = Direction.W;
                                }
                                else
                                {
                                    F[i].delta = (0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                            }
                            else
                            {
                                F[i].delta = (-dx1, dy);
                                F[i].tdir = Direction.W;
                            }
                        }
                        else
                        {//========================== directly on the major axis
                            if (y2 > yS)
                            {
                                F[i].delta = (0, dy1);
                                F[i].tdir = Direction.S;
                            }
                            else
                            {
                                F[i].delta = (0, -dy1);
                                F[i].tdir = Direction.N;
                            }
                        }
                        #endregion
                    }
                    else if (node.Aspect == Aspect.Horizontal)
                    {
                        #region Horizontal  =======================================
                        if (y2 > yS)
                        {//========================== on the south side of major axis
                            if (x2 > xE)
                            {//========================== heading east
                                if (dyS / dxE < 1)
                                {
                                    F[i].delta = (dx1, 0);
                                    F[i].tdir = Direction.E;
                                }
                                else
                                {
                                    F[i].delta = (dx0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                            }
                            else if (x2 < xW)
                            {//========================== heading west
                                if (dyN / dxE > -1)
                                {
                                    F[i].delta = (-dx1, 0);
                                    F[i].tdir = Direction.W;
                                }
                                else
                                {
                                    F[i].delta = (-dx0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                            }
                            else
                            {
                                F[i].delta = (dx, dy1);
                                F[i].tdir = Direction.S;
                            }
                        }
                        else if (y2 < yN)
                        {//========================== on the north side of major axis
                            if (x2 > xE)
                            {//========================== heading east
                                if (dyN / dxE < -1)
                                {
                                    F[i].delta = (dx0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                                else
                                {
                                    F[i].delta = (dx1, 0);
                                    F[i].tdir = Direction.E;
                                }
                            }
                            else if (x2 < xW)
                            {//========================== heading west
                                if (dyN / dxW < 1)
                                {
                                    F[i].delta = (-dx1, 0);
                                    F[i].tdir = Direction.W;
                                }
                                else
                                {
                                    F[i].delta = (-dx0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                            }
                            else
                            {
                                F[i].delta = (dx, -dy1);
                                F[i].tdir = Direction.N;
                            }
                        }
                        else
                        {//========================== directly on the major axis
                            if (x2 > xE)
                            {
                                F[i].delta = (dx1, 0);
                                F[i].tdir = Direction.E;
                            }
                            else
                            {
                                F[i].delta = (-dx1, 0);
                                F[i].tdir = Direction.W;
                            }
                        }
                        #endregion
                    }
                    else if (node.Aspect == Aspect.Central)
                    {
                        #region Central  ==========================================
                        if (x2 > xE)
                        {//========================== on the east side of major axis
                            if (y2 > yS)
                            {//========================== going south
                                if (dyS / dxE < 1)
                                {
                                    F[i].delta = (dx1, dy0);
                                    F[i].tdir = Direction.E;
                                }
                                else
                                {
                                    F[i].delta = (dx0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                            }
                            else if (y2 < yN)
                            {//========================== going north
                                if (dyN / dxE > -1)
                                {
                                    F[i].delta = (dx1, -dy0);
                                    F[i].tdir = Direction.E;
                                }
                                else
                                {
                                    F[i].delta = (dx0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                            }
                            else
                            {
                                F[i].delta = (dx1, dy);
                                F[i].tdir = Direction.E;
                            }
                        }
                        else if (x2 < xW)
                        {//========================== on the west side of major axis
                            if (y2 > yS)
                            {//========================== going south
                                if (dyS / dxW < -1)
                                {
                                    F[i].delta = (-dx0, dy1);
                                    F[i].tdir = Direction.S;
                                }
                                else
                                {
                                    F[i].delta = (-dx1, dy0);
                                    F[i].tdir = Direction.W;
                                }
                            }
                            else if (y2 < yN)
                            {//========================== going north
                                if (dyN / dxW < 1)
                                {
                                    F[i].delta = (-dx1, dy0);
                                    F[i].tdir = Direction.W;
                                }
                                else
                                {
                                    F[i].delta = (-dx0, -dy1);
                                    F[i].tdir = Direction.N;
                                }
                            }
                            else
                            {
                                F[i].delta = (-dx1, dy);
                                F[i].tdir = Direction.W;
                            }
                        }
                        else
                        {//========================== directly on the major axis
                            if (y2 > yS)
                            {
                                F[i].delta = (dx, dy1);
                                F[i].tdir = Direction.S;
                            }
                            else
                                {
                                F[i].delta = (dx, -dy1);
                                F[i].tdir = Direction.N;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        F[i].delta = (0, 0);
                        F[i].tdir = Direction.Any;
                    }
                }

                SetFacePoints();
                #endregion
            }

            #region SetFaceOffset  ============================================
            void SetFaceOffset()
            {
                for (int i = 0; i < N; i++)
                {
                    var dx = (float)node.DX;
                    var dy = (float)node.DY;

                    switch (F[i].tdir)
                    {
                        case Direction.Any: F[i].delta = (0, 0); break;
                        case Direction.E: F[i].delta = (dx, 0); break;
                        case Direction.S: F[i].delta = (0, dy); break;
                        case Direction.W: F[i].delta = (-dx, 0); break;
                        case Direction.N: F[i].delta = (0, -dy); break;
                        case Direction.SEC: F[i].delta = (dx, dy); break;
                        case Direction.SWC: F[i].delta = (-dx, dy); break;
                        case Direction.NWC: F[i].delta = (-dx, -dy); break;
                        case Direction.NEC: F[i].delta = (dx, -dy); break;
                    }
                }

                SetFacePoints();
            }
            #endregion

            #region SetFacePoints  ==========================================
            void SetFacePoints()
            {
                I.Sort(ComapreEdgeData); // sort the edge index list

                var k = 0;
                var n = 0;
                var dir = Direction.Any;
                byte tix = 255;
                float d1, d2, w1, w2, os, o1, o2;
                d1 = d2 = w1 = w2 = os = o1 = o2 = 0;

                for (int i = 0; i < N; i++)
                {
                    if (F[I[i]].tix != tix || F[I[i]].tdir != dir)
                    {
                        dir = F[I[i]].tdir;
                        tix = F[I[i]].tix;

                        k = n = 0; 
                        for (int j = i; j < N; j++)
                        {
                            if (F[I[j]].tix != tix || F[I[j]].tdir != dir) break;
                            n++;
                        }
                        w1 = F[I[i]].tsiz;
                        w2 = tmSpc;   // required width using terminal spacing
                        d1 = (w2 > w1) ? n : 1;
                        if (node.Sizing == Sizing.Auto || w1 > w2) w1 = w2;
                        d2 = d1 + tmLen;
                    }
                    var (odx, ody) = _terminalOutwardDirection[(int)dir];
                    var (tdx, tdy) = _terminalTangentDirection[(int)dir];
                    var (dx, dy) = F[I[i]].delta;
                    os = (k++ * 2) + 1 - n;
                    o1 = w1 * os;
                    o2 = w2 * os;

                    var dx1 = dx + tdx * o1;
                    var dy1 = dy + tdy * o1;
                    var dx2 = dx + d1 * odx + tdx * o2;
                    var dy2 = dy + d1 * ody + tdy * o2;
                    var dx3 = dx + d2 * odx + tdx * o2;
                    var dy3 = dy + d2 * ody + tdy * o2;

                    edges[I[i]].SetFace(node, (dx1, dy1), (dx2, dy2), (dx3, dy3), dir);
                }
            }
            #endregion

            #region CompEdgeData  =============================================
            int ComapreEdgeData(int a, int b)
            {
                if (F[a].tix < F[b].tix) return -1;
                if (F[a].tix > F[b].tix) return 1;
                var adir = (int)F[a].tdir;
                var bdir = (int)F[a].tdir;
                if (adir < bdir) return -1;
                if (adir > bdir) return 1;
                var ord1 = E[a].order;
                if (ord1 != 0)
                {
                    var ord2 = E[b].order;
                    if (ord2 != 0 && E[a].other == E[b].other)
                        return CompareF(ord1, ord2);
                }
                switch (adir)
                {
                    case 1:
                    case 5:
                    case 6:
                    case 7:
                        return CompareF(F[a].slope, F[b].slope);
                    case 2:
                        if (F[a].slice > F[b].slice) return -1;
                        if (F[a].slice < F[b].slice) return 1;
                        return CompareR(F[a].slope, F[b].slope);
                    case 3:
                        return CompareR(F[a].slope, F[b].slope);
                    case 4:
                        if (F[a].slice < F[b].slice) return -1;
                        if (F[a].slice > F[b].slice) return 1;
                        return CompareF(F[a].slope, F[b].slope);
                    default:
                        return 0;
                }
            }
            #endregion
        }

        #region StaticValues  =================================================
        private static readonly byte _maxPenalty = SymbolX.MaxPenalty;
        private static readonly float[] _penaltyFactor = SymbolX.PenaltyFactor;

        // indexed by (int)Direction
        private static readonly (float dx, float dy)[] _terminalOutwardDirection =
        {
            (0, 0),   //Any
            (1, 0),   //E
            (0, 1),   //S
            (-1, 0),  //W
            (0, -1),  //N
            (Q, Q),   //SEC
            (-Q, Q),  //SWC
            (-Q, -Q), //NWC
            (Q, -Q),  //NEC
        };
        // indexed by (int)Direction
        private static readonly (float dx, float dy)[] _terminalTangentDirection =
        {
            (0, 0),   //Any
            (0, 1),   //E
            (1, 0),   //S
            (0, 1),   //W
            (1, 0),   //N
            (-Q, -Q), //SEC
            (Q, Q),   //SWC
            (-Q, Q),  //NWC
            (Q, Q),   //NEC
        };
        private const float Q = 0.7071067811865f; // 1 / SQRT(2)

        private readonly static (FlipState, AutoFlip)[] _allFlipStates =
        {
            (FlipState.None, AutoFlip.None), (FlipState.VertFlip, AutoFlip.VertFlip),
            (FlipState.HorzFlip, AutoFlip.HorzFlip), (FlipState.VertHorzFlip, AutoFlip.BothFlip),
            (FlipState.LeftRotate, AutoFlip.RotateLeft), (FlipState.LeftHorzFlip, AutoFlip.LeftHorzFlip),
            (FlipState.RightRotate, AutoFlip.RotateRight), (FlipState.RightHorzFlip, AutoFlip.RightHorzFlip),
        };

        private static int CompareF(int a, int b) => (a < b) ? -1 : (a > b) ? 1 : 0;
        private static int CompareF(float a, float b) => (a < b) ? -1 : (a > b) ? 1 : 0;
        private static int CompareR(int a, int b) => (a < b) ? 1 : (a > b) ? -1 : 0;
        private static int CompareR(float a, float b) => (a < b) ? 1 : (a > b) ? -1 : 0;
        #endregion
    }
}
