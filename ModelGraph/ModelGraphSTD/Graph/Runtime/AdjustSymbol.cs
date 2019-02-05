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
            var targetCount = symbol.Target_Contacts.Count;

            var bestFlip = FlipState.None;
            var bestCost = float.MaxValue;
            var testCost = 0f;

            var cx = node.X;
            var cy = node.Y;
            var scale = GraphX.SymbolScale;

            var edgeSlopeSector = new List<(int ei, float ds, float m, int six)>(edgeCount);
            var targetContacts = new List<(Target trg, byte tix, Contact con, (float dx, float dy) pnt)>(targetCount);

            var edgeIsDone = new bool[edgeCount];

            var testResult = NewTestResult();
            var bestResult = testResult;

            #region FindBestArrangement  ======================================
            foreach (var flip in _allFlipStates)
            {
                symbol.GetFlipTargetConnect(flip, cx, cy, scale, targetContacts);
                var penalty = symbol.GetFlipTargetPenalty(flip);

                testCost = 0;
                for (int i = 0; i < edgeCount; i++) { edgeIsDone[i] = false; }
                for (int i = 0; i < targetCount; i++) { testResult[i].Clear(); }

                for (int ti = 0; ti < targetContacts.Count; ti++)
                {
                    var (x, y) = targetContacts[ti].pnt; //actual target coordinate
                    InitEdgeSlopeSector(x, y); //list of (edge-index, slope-to-target, sector-of-slope-0..15)

                    foreach (var (ei, ds, m, six) in edgeSlopeSector)
                    {
                        var (trg, tix, con, pnt) = targetContacts[ti];
                        var (edge, etrg, Other, bend, atch, horz) = E[ei];

                        if ((etrg & trg) == 0) continue; // this edge refuses to mate with target point

                        var pi = penalty[six][tix]; // [from direction sector index] [to target location index]
                        var cs = ds * _penaltyFactor[pi];
                        testCost += cs;

                        edgeIsDone[ei] = true;
                        testResult[ti].Add((ei, cs, m, six)); // add the edge connection to the list

                        if (con == Contact.One) break; //allow only one edge to connect to the target
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

                foreach( var (ei, c, m, s) in bestResult[ti])
                {
                    E[ei].edge.SetFace(node, (x, y));
                }
            }

            #region NewTestResult  ============================================
            List<(int ei, float c, float m, int s)>[] NewTestResult()
            {
                var tr = new List<(int ei, float c, float m, int s)>[targetCount];
                for (int i = 0; i < targetCount; i++)
                {
                    tr[i] = new List<(int ei, float c, float m, int s)>(5);
                }
                return tr;
            }
            #endregion

            #region InitEdgeSlopeSector  ======================================
            // populate the edgeSect list for given target point
            void InitEdgeSlopeSector(float x1, float y1)
            {
                const float a = 0.4142135623730950f; //tan(22.5)
                const float b = 1.0f;                //tan(45.0)
                const float c = 2.4142135623730950f; //tan(67.5)

                edgeSlopeSector.Clear();
                for (int i = 0; i < edgeCount; i++)
                {
                    if (edgeIsDone[i]) continue;

                    var (x2, y2) = E[i].bend;
                    var dx = x2 - x1;
                    var dy = y2 - y1; 

                    bool isVert = (int)(dx + 0.5) == 0;
                    bool isHorz = (int)(dy + 0.5) == 0;

                    if (isVert)
                    {
                        if (isHorz)
                            edgeSlopeSector.Add((i, 0, 0f, 0));
                        else if (dy > 0)
                            edgeSlopeSector.Add((i, dy, 1023f, 3));
                        else
                            edgeSlopeSector.Add((i, -dy, -1023f, 12));
                    }
                    else
                    {
                        if (isHorz)
                        {
                            if (dx > 0)
                                edgeSlopeSector.Add((i, dx, 0, 0));
                            else
                                edgeSlopeSector.Add((i, -dx, 0, 7));
                        }
                        else
                        {
                            var m = dy / dx;
                            if (dx < 0)
                            {
                                if (dy < 0)
                                    edgeSlopeSector.Add((i, (-dx - dy), m, (m < a) ? 8 : (m < b) ? 9 : (m < c) ? 10 : 11));
                                else
                                    edgeSlopeSector.Add((i, (-dx + dy), m, (m < -c) ? 4 : (m < -b) ? 5 : (m < -a) ? 6 : 7));
                            }
                            else
                            {
                                if (dy < 0)
                                    edgeSlopeSector.Add((i, (dx - dy), m, (m < -c) ? 12 : (m < -b) ? 13 : (m < -a) ? 14 : 15));
                                else
                                    edgeSlopeSector.Add((i, (dx + dy), m, (m < a) ? 0 : (m < b) ? 1 : (m < c) ? 2 : 3));
                            }
                        }
                    }
                }
                edgeSlopeSector.Sort(CompareEntry);

                int CompareEntry((int ei, float ds, float m, int s) u, (int ei, float ds, float m, int s) v)
                {
                    return (u.s < v.s) ? -1 : (u.s > v.s) ? 1 : (u.ds < v.ds) ? -1 : (u.ds > v.ds) ? 1 : 0; 
                }
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
