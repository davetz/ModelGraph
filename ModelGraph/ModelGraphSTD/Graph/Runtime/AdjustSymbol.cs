using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        internal void AdjustSymbol(Node node)
        {
            var E = Layout.ConnectedEdges(node);
            var edgeCount = (E is null) ? 0 : E.Length;
            if (edgeCount == 0) return;

            var symbol = Symbols[node.Symbol - 2];
            var targetCount = symbol.TargetContacts.Count;
            var targetContacts = new List<(Target trg, byte tix, Contact con, (float dx, float dy) pnt)>(targetCount);
            var rejectIndex = targetCount; //references lists of edges that didn't mate with a contact

            var bestFlip = FlipState.None;
            var bestCost = float.MaxValue;

            var cx = node.X;
            var cy = node.Y;
            var scale = GraphX.SymbolScale;

            var tmSpc = node.Graph.GraphX.TerminalSpacing;
            var tmLen = node.Graph.GraphX.TerminalLength;

            var testResult = NewTestResult();
            var bestResult = testResult;
            byte[][] penalty; // [edge sector index] [symbol target index]

            #region FindBestArrangement  ======================================
            foreach (var (flip, autoFlip) in _allFlipStates)
            {
                if (flip != FlipState.None && (symbol.AutoFlip & autoFlip) == 0) continue; //this arrangement is not allowed

                var testCost = 0f;
                penalty = symbol.GetFlipTargetPenalty(flip);
                symbol.GetFlipTargetContacts(flip, cx, cy, scale, tmLen, targetContacts);
                (float cost, float slope, int x1, int y1, int x2, int y2, int six) bestTarget;
                var bestTi = -1;

                testCost = 0;
                for (int ei = 0; ei < edgeCount; ei++)
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
                for (int ei = edgeCount - 1; ei >= 0; ei--)
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
                        bestResult = testResult;
                    }
                    testResult = NewTestResult();
                }
                #endregion

                #region AddToTestResult  ======================================
                bool AddToTestResult(int cei, int cti)
                {
                    if (cti < 0) // no targets will mate with this edge 
                    {
                        testResult[rejectIndex].Add((cei, 0, 0, 0, 0, 0, 0, -1));
                    }
                    else  //add this edge to the target's edge list
                    {
                        var factor = CrissCrossFactor(cei);
                        var (cost, slope, x1, y1, x2, y2, six) = bestTarget;
                        cost *= factor;
                        testCost += cost;
                        if (testCost > bestCost) return false; //abort, this is a bad choice of flip state

                        foreach (var pre in testResult)
                        {
                            if (pre.Count == 0) continue;
                            for (int j = pre.Count - 1; j >= 0; j--)
                            {
                                if (pre[j].ei == cei)
                                    pre.RemoveAt(j); // get rid of duplicate edge from a previous best try
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
                    foreach (var pre in testResult)
                    {
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

            #region AssignEdgeContacts  =======================================
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

            var targetSurface = symbol.GetFlipTargetSurface(bestFlip, cx, cy, scale);

            for (int ti = 0; ti < targetCount; ti++)
            {
                var n = bestResult[ti].Count;
                if (n == 0) continue;

                var (x, y, dx, dy, w1, ts) = targetSurface[ti];
                if (n > 1)
                {
                    if ((dx > 0 && dy ==0) || (dy > 0 && dx == 0))
                        bestResult[ti].Sort(FromEastSouth);
                    else
                        bestResult[ti].Sort(FromWestNorth);
                }


                var w2 = n * tmSpc / 2; // required width using terminal spacing 

                var d1 = (w2 > w1) ? n : 1;

                if (w1 > w2) w1 = w2;

                var d2 = d1 + tmLen;

                for (int i = 0; i < n; i++)
                {

                    var ei = bestResult[ti][i].ei;
                    var os = Layout.Offset(i, n);
                    var o1 = w1 * os;
                    var o2 = w2 * os;

                    var x1 = x - dy * o1;
                    var y1 = y + dx * o1;
                    var x2 = (x + d1 * dx) - dy * o2;
                    var y2 = (y + d1 * dy) + dx * o2;
                    var x3 = (x + d2 * dx) - dy * o2;
                    var y3 = (y + d2 * dy) + dx * o2;

                    E[ei].edge.SetFace(node, (x1, y1), (x2, y2), (x3, y3), ts);
                }
                int FromEastSouth((int ei, float c, float m, int x1, int y1, int x2, int y2, int s) a, (int ei, float c, float m, int x1, int y1, int x2, int y2, int s) b)
                {
                    var tup1 = E[a.ei].tuple;
                    if (tup1 != 0)
                    {
                        var tup2 = E[b.ei].tuple;
                        if (tup2 == tup1)
                        {
                            var ord1 = E[a.ei].order;
                            var ord2 = E[b.ei].order;
                            return (ord1 < ord2) ? -1 : (ord1 > ord2) ? 1 : 0;
                        }
                    }
                    if (E[a.ei].tsort == TupleSort.West || E[a.ei].tsort == TupleSort.North)
                        return (a.s < b.s) ? -1 : (a.s > b.s) ? 1 : (a.m < b.m) ? -1 : (a.m > b.m) ? 1 : 0;
                    else
                        return (a.s < b.s) ? 1 : (a.s > b.s) ? -1 : (a.m < b.m) ? 1 : (a.m > b.m) ? -1 : 0;
                }
                int FromWestNorth((int ei, float c, float m, int x1, int y1, int x2, int y2, int s) a, (int ei, float c, float m, int x1, int y1, int x2, int y2, int s) b)
                {
                    var tup1 = E[a.ei].tuple;
                    if (tup1 != 0)
                    {
                        var tup2 = E[b.ei].tuple;
                        if (tup2 == tup1)
                        {
                            var ord1 = E[a.ei].order;
                            var ord2 = E[b.ei].order;
                            return (ord1 < ord2) ? 1 : (ord1 > ord2) ? -1 : 0;
                        }
                    }
                    if (E[a.ei].tsort == TupleSort.West || E[a.ei].tsort == TupleSort.North)
                        return (a.s < b.s) ? 1 : (a.s > b.s) ? -1 : (a.m < b.m) ? 1 : (a.m > b.m) ? -1 : 0;
                    else
                        return (a.s < b.s) ? -1 : (a.s > b.s) ? 1 : (a.m < b.m) ? -1 : (a.m > b.m) ? 1 : 0;
                }
            }
            #endregion

            #region NewTestResult  ============================================
            List<(int ei, float c, float m, int x1, int y1, int x2, int y2, int s)>[] NewTestResult()
            {
                var plusOneCount = targetCount + 1;
                var tr = new List<(int ei, float c, float m, int x1, int y1, int x2, int y2, int s)>[plusOneCount];
                for (int i = 0; i < plusOneCount; i++)
                {
                    tr[i] = new List<(int ei, float c, float m, int x1, int y1, int x2, int y2, int s)>(5);
                }
                return tr;
            }
            #endregion

            #region EdgetTarget_CostSlopeSectorIndex  =========================
            // populate the edgeSect list for given target point
            (float cost, float slope, int x1, int y1, int x2, int y2, int six) EdgetTarget_CostSlopSectorIndex(int ei, int ti)
            {
                var (dx, dy, slope, six) = XYTuple.SlopeIndex(targetContacts[ti].pnt, E[ei].bend);
                var tix = targetContacts[ti].tix;

                var pi = penalty[tix][six]; // [from direction sector index] [to target location index]
                var cost = ((dx * dx) + (dy * dy)) * _penaltyFactor[pi]; //weighted cost

                var (x1, y1) = targetContacts[ti].pnt;
                var (x2, y2) = E[ei].bend;

                return (cost, slope, (int)x1, (int)y1, (int)x2, (int)y2, six);
            }
            #endregion
        }

        #region StaticValues  =================================================
        private static readonly byte _maxPenalty = SymbolX.MaxPenalty;
        private static readonly float[] _penaltyFactor = SymbolX.PenaltyFactor;

        private readonly static (FlipState, AutoFlip)[] _allFlipStates =
        {
            (FlipState.None, AutoFlip.None), (FlipState.VertFlip, AutoFlip.VertFlip),
            (FlipState.HorzFlip, AutoFlip.HorzFlip), (FlipState.VertHorzFlip, AutoFlip.VertHorzFlip),
            (FlipState.LeftRotate, AutoFlip.LeftRotate), (FlipState.LeftHorzFlip, AutoFlip.LeftHorzFlip),
            (FlipState.RightRotate, AutoFlip.RightRotate), (FlipState.RightHorzFlip, AutoFlip.RightHorzFlip),
        };
        #endregion
    }
}
