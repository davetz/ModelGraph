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

            var testResult = NewTestResult();
            var bestResult = testResult;
            byte[][] penalty; // [edge sector index] [symbol target index]

            #region FindBestArrangement  ======================================
            foreach (var flip in _allFlipStates)
            {
                var testCost = 0f;
                penalty = symbol.GetFlipTargetPenalty(flip);
                symbol.GetFlipTargetContacts(flip, cx, cy, scale, targetContacts);

                for (int ei = 0; ei < edgeCount; ei++)
                {
                    (float cost, float slope, int six) bestParm = (float.MaxValue, 0, 0);
                    var bestTi = -1;
                    for (int ti = 0; ti < targetCount; ti++)
                    {
                        if ((targetContacts[ti].trg & E[ei].targ) == 0) continue; // skip targets that won't mate with the edge
                        if (targetContacts[ti].con == Contact.One && testResult[ti].Count > 0) continue; //skip targes that are already at capacity

                        var parm = EdgetTarget_CostSlopSectorIndex(ei, ti);
                        if (parm.cost < bestParm.cost)
                        {
                            bestTi = ti;
                            bestParm = parm;
                        }
                    }
                    if (bestTi < 0) // no targets will mathe with this edge 
                    {
                        testResult[rejectIndex].Add((ei, 0, 0, -1));   
                    }
                    else  //add this edge to the target's edge list
                    {
                        var (cost, slope, six) = bestParm;
                        testCost += cost;
                        if (testCost > bestCost) break; //abort, this is a bad choice of flip state

                        foreach (var pre in testResult)
                        {
                            if (pre.Count == 0) continue;
                            for (int j = pre.Count - 1; j >= 0; j--)
                            {
                                if (pre[j].ei == ei)
                                    pre.RemoveAt(j); // get rid of duplicate edge from a previous best try
                            }
                        }

                        testResult[bestTi].Add((ei, cost, slope, six));
                    }
                }
                if (testCost < bestCost)
                {
                    bestFlip = flip;
                    bestCost = testCost;
                    bestResult = testResult;

                    testResult = NewTestResult();
                }
            }
            #endregion

            #region AssignEdgeContacts  =======================================
            Debug.WriteLine($"Best Flip: {bestFlip}");
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

            var tmSpc = node.Graph.GraphX.TerminalSpacing;
            var tmLen = node.Graph.GraphX.TerminalLength;

            var targetSurface = symbol.GetFlipTargetSurface(bestFlip, cx, cy, scale);

            for (int ti = 0; ti < targetCount; ti++)
            {
                var (x, y, dx, dy, siz) = targetSurface[ti];

                if (bestResult[ti].Count > 1)
                    bestResult[ti].Sort((u, v) => (u.s < v.s) ? -1 : (u.s > v.s) ? 1 : (u.m < v.m) ? -1 : (u.m > v.m) ? 1 : 0);

                foreach ( var (ei, c, m, s) in bestResult[ti])
                {
                    E[ei].edge.SetFace(node, (x, y));
                }
            }
            #endregion

            #region NewTestResult  ============================================
            List<(int ei, float c, float m, int s)>[] NewTestResult()
            {
                var plusOneCount = targetCount + 1;
                var tr = new List<(int ei, float c, float m, int s)>[plusOneCount];
                for (int i = 0; i < plusOneCount; i++)
                {
                    tr[i] = new List<(int ei, float c, float m, int s)>(5);
                }
                return tr;
            }
            #endregion

            #region EdgetTarget_CostSlopeSectorIndex  =========================
            // populate the edgeSect list for given target point
            (float cost, float slope, int six) EdgetTarget_CostSlopSectorIndex(int ei, int ti)
            {
                const float a = 0.4142135623730950f; //tan(22.5)
                const float b = 1.0f;                //tan(45.0)
                const float c = 2.4142135623730950f; //tan(67.5)

                var tix = targetContacts[ti].tix;
                var (x1, y1) = targetContacts[ti].pnt;

                var (x2, y2) = E[ei].bend;
                var dx = x2 - x1;
                var dy = y2 - y1;

                bool isVert = (int)(dx + 0.5) == 0;
                bool isHorz = (int)(dy + 0.5) == 0;

                (float, float, int) costSlopeSix = (0, 0, 0);

                if (isVert)
                {
                    if (isHorz)
                    {
                        costSlopeSix = (0, 0, 0);
                    }
                    else if (dy > 0)
                        costSlopeSix = (dy, 1023, 3);
                    else
                        costSlopeSix = (-dy, -1023, 12);
                }
                else
                {
                    if (isHorz)
                    {
                        if (dx > 0)
                            costSlopeSix = (dx, 0, 0);
                        else
                            costSlopeSix = (-dx, 0, 7);
                    }
                    else
                    {
                        var m = dy / dx;
                        if (dx < 0)
                        {
                            if (dy < 0)
                                costSlopeSix = ((-dx - dy), m, (m < a) ? 8 : (m < b) ? 9 : (m < c) ? 10 : 11);
                            else
                                costSlopeSix = ((-dx + dy), m, (m < -c) ? 4 : (m < -b) ? 5 : (m < -a) ? 6 : 7);
                        }
                        else
                        {
                            if (dy < 0)
                                costSlopeSix = ((dx - dy), m, (m < -c) ? 12 : (m < -b) ? 13 : (m < -a) ? 14 : 15);
                            else
                                costSlopeSix = ((dx + dy), m, (m < a) ? 0 : (m < b) ? 1 : (m < c) ? 2 : 3);
                        }
                    }
                }
                var (cost, slope, six) = costSlopeSix;

                var pi = penalty[six][tix]; // [from direction sector index] [to target location index]
                cost = cost * _penaltyFactor[pi]; //weighted cost

                return (cost, slope, six);
            }
            #endregion
        }

        #region StaticValues  =================================================
        private static readonly byte _maxPenalty = SymbolX.MaxPenalty;
        private static readonly float[] _penaltyFactor = SymbolX.PenaltyFactor;

        private readonly static FlipState[] _allFlipStates =
            {
                FlipState.None, FlipState.VertFlip, FlipState.HorzFlip, FlipState.VertHorzFlip,
                FlipState.LeftRotate, FlipState.LeftHorzFlip, FlipState.RightRotate, FlipState.RightHorzFlip
            };
        #endregion
    }
}
