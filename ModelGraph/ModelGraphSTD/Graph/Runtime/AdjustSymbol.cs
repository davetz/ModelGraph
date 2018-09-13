﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {

        #region SymbolRotator  ================================================
        private class SymbolRotator
        {
            #region Fields  ===================================================
            private byte n;
            private byte s;
            private byte w;
            private byte e;

            private byte N;
            private byte S;
            private byte W;
            private byte E;

            private byte dx;
            private byte dy;
            internal byte DX;
            internal byte DY;
            #endregion

            #region Constructors  =============================================
            internal SymbolRotator(SymbolX symbol)
            {
                n = N = (byte)symbol.TopContact;
                s = S = (byte)symbol.BottomContact;
                w = W = (byte)symbol.LeftContact;
                e = E = (byte)symbol.RightContact;

                dx = DX = (byte)(symbol.Width / 2);
                dy = DY = (byte)(symbol.Height / 2);
            }
            #endregion

            internal void SetFlipRotate(int value) { _action[value & 7](this); }

            internal Contact Contact(int index) { return _contact[index & 3](this); } // (E, S, W, N)

            #region VectorTables  =============================================
            private static Func<SymbolRotator, Contact>[] _contact = new Func<SymbolRotator, Contact>[]
            {
                (r) => { return (Contact)r.E; },
                (r) => { return (Contact)r.S; },
                (r) => { return (Contact)r.W; },
                (r) => { return (Contact)r.N; },
            };

            private static Action<SymbolRotator>[] _action = new Action<SymbolRotator>[]
            {
                FlipRotateNone,
                FlipVertical,
                FlipHorizontal,
                FlipBothWays,
                RotateClockWise,
                RotateFlipVertical,
                RotateFlipHorizontal,
                RotateFlipBothWays,
            };

            private static void FlipRotateNone(SymbolRotator r)
            {
                r.N = r.n;
                r.S = r.s;
                r.W = r.w;
                r.E = r.e;
                r.DX = r.dx;
                r.DY = r.dy;
            }
            private static void FlipVertical(SymbolRotator r)
            {
                r.N = r.s;
                r.S = r.n;
                r.W = r.w;
                r.E = r.e;
            }
            private static void FlipHorizontal(SymbolRotator r)
            {
                r.N = r.n;
                r.S = r.s;
                r.W = r.e;
                r.E = r.w;
            }
            private static void FlipBothWays(SymbolRotator r)
            {
                r.N = r.s;
                r.S = r.n;
                r.W = r.e;
                r.E = r.w;
            }
            private static void RotateClockWise(SymbolRotator r)
            {
                r.N = r.w;
                r.S = r.e;
                r.W = r.s;
                r.E = r.n;
                r.DX = r.dy;
                r.DY = r.dx;
            }
            private static void RotateFlipVertical(SymbolRotator r)
            {
                r.N = r.e;
                r.S = r.w;
                r.W = r.s;
                r.E = r.n;
            }
            private static void RotateFlipHorizontal(SymbolRotator r)
            {
                r.N = r.w;
                r.S = r.e;
                r.W = r.n;
                r.E = r.s;
            }
            private static void RotateFlipBothWays(SymbolRotator r)
            {
                r.N = r.e;
                r.S = r.w;
                r.W = r.n;
                r.E = r.s;
            }
            #endregion
        }
        #endregion

        #region CostFaceSector  ===============================================
        //=====================================================================
        //      sect        quad       direction
        //    5\6|7/8        3|4           N
        //    ~~~+~~~        ~+~         W + E
        //    4/3|2\1        2|1           S
        //=====================================================================
        // Orchestrate the face contact connection assignment loop
        // The are many permutations, but these are the rules and cost of assigning connections.
        // We pick the permutation with the cheapest overall cost.
        private struct CostFaceSector
        {
            internal byte Cost;  // cost of this connecction.
            internal byte Face;  // which face (east,south,west,north)
            internal byte Sector;// the edge's radial sector direction being assigned (1-8)
            internal byte Invert;// insert edge into front of list instead of adding to end of the list

            internal CostFaceSector(byte cost, byte face, byte sect, byte invert = 0)
            {
                Cost = cost;
                Face = face;
                Sector = sect;
                Invert = invert;
            }
        }
        static CostFaceSector[] _costFaceSector = new CostFaceSector[]
        {
            new CostFaceSector(0, 0, 8),
            new CostFaceSector(0, 0, 1),

            new CostFaceSector(0, 1, 2, 1),
            new CostFaceSector(0, 1, 3, 1),

            new CostFaceSector(0, 2, 4, 1),
            new CostFaceSector(0, 2, 5, 1),

            new CostFaceSector(0, 3, 6),
            new CostFaceSector(0, 3, 7),

            new CostFaceSector(10, 0, 2),
            new CostFaceSector(10, 1, 4),
            new CostFaceSector(10, 2, 6),
            new CostFaceSector(10, 3, 8),

            new CostFaceSector(10, 0, 7, 1),
            new CostFaceSector(10, 1, 1, 1),
            new CostFaceSector(10, 2, 3, 1),
            new CostFaceSector(10, 3, 5, 1),

            new CostFaceSector(20, 0, 3),
            new CostFaceSector(20, 1, 5),
            new CostFaceSector(20, 2, 7),
            new CostFaceSector(20, 3, 1),

            new CostFaceSector(20, 0, 6, 1),
            new CostFaceSector(20, 1, 8, 1),
            new CostFaceSector(20, 2, 2, 1),
            new CostFaceSector(20, 3, 4, 1),

            new CostFaceSector(40, 0, 4),
            new CostFaceSector(40, 1, 6),
            new CostFaceSector(40, 2, 8),
            new CostFaceSector(40, 3, 2),

            new CostFaceSector(40, 0, 5, 1),
            new CostFaceSector(40, 1, 7, 1),
            new CostFaceSector(40, 2, 1, 1),
            new CostFaceSector(40, 3, 3, 1),
        };
        #endregion

        private void AdjustSymbol(Node node)
        {
            var (count, nquad, nsect, sectEdge, E) = Layout.SortedEdges(node);
            if (count == 0) return;


            var isym = node.Symbol - 2;
            var symbol = Symbols[isym];
            var sr = new SymbolRotator(symbol);

            var done = new bool[count];

            var FaceEdge = new List<Edge>[] // (E, S, W, N)
            {
                new List<Edge>(count),
                new List<Edge>(count),
                new List<Edge>(count),
                new List<Edge>(count),
            };

            bool allDone = false;
            int bestCost = int.MaxValue;
            for (int flipRotate = 0; flipRotate < 8; flipRotate++)
            {
                #region InitializeParameters  =================================
                // adjust the contact and connect for flip rotate
                // and also clear the line done flags
                sr.SetFlipRotate(flipRotate);
                for (int i = 0; i < count; i++)
                {
                    E[i].conn.SetFlipRotate(flipRotate);
                    done[i] = false;
                }

                // clear the east, south, west, and north face edge lists
                for (int i = 0; i < 4; i++) { FaceEdge[i].Clear(); }
                #endregion

                int Cost = 0;
                int delta = 0;
                for (int p = 0; p < _costFaceSector.Length; p++)
                {
                    #region CalculateCost  ====================================
                    var check = (delta != _costFaceSector[p].Cost);
                    var invert = (_costFaceSector[p].Invert != 0);
                    delta = _costFaceSector[p].Cost;
                    var f = _costFaceSector[p].Face;
                    var s = _costFaceSector[p].Sector;

                    if (invert)
                    {
                        int i1 = sectEdge[s];
                        int i2 = sectEdge[s + 1] - 1;
                        for (int i = i2; i >= i1; i--)
                        {
                            if (done[i]) continue;
                            if (sr.Contact(f) == Contact.None) continue;
                            if (!E[i].conn.CanConnect(f)) continue;
                            if (sr.Contact(f) == Contact.One && FaceEdge[f].Count > 0) continue;
                            Cost += delta;
                            done[i] = true;
                            FaceEdge[f].Insert(0, E[i].edge);
                        }
                    }
                    else
                    {
                        for (int i = sectEdge[s]; i < sectEdge[s + 1]; i++)
                        {
                            if (done[i]) continue;
                            if (sr.Contact(f) == Contact.None) continue;
                            if (!E[i].conn.CanConnect(f)) continue;
                            if (sr.Contact(f) == Contact.One && FaceEdge[f].Count > 0) continue;
                            Cost += delta;
                            done[i] = true;
                            FaceEdge[f].Add(E[i].edge);
                        }
                    }

                    if (check)
                    {
                        allDone = true;
                        for (int i = 0; i < count; i++) { if (done[i]) continue; allDone = false; break; }
                        if (allDone || Cost > bestCost) break;
                    }
                    #endregion
                }

                if (allDone)
                {
                    if (Cost < bestCost)
                    {
                        bestCost = Cost;
                        node.DX = sr.DX;
                        node.DY = sr.DY;
                        node.FlipRotate = (FlipRotate)flipRotate;

                        var d1 = 8;
                        var tmSpc = node.Graph.GraphX.TerminalSpacing;
                        var tmLen = node.Graph.GraphX.TerminalLength;

                        var dx1 = node.DX + 2;
                        var dx2 = dx1 + d1;
                        var dx3 = dx2 + tmLen;

                        var dy1 = node.DY + 2;
                        var dy2 = dy1 + d1;
                        var dy3 = dy2 + tmLen;

                        var dsx = (double)dx1 / 2;
                        var dsy = (double)dy1 / 2;

                        var ds1 = 0;
                        var ds2 = 0;
                        #region AssignEdgeConnectors  =========================
                        for (var f = 0; f < 4; f++)
                        {
                            var n = FaceEdge[f].Count;
                            if (n == 0) continue;
                            switch(f)
                            {
                                case 0: //east
                                    for (int i = 0; i < n; i++)
                                    {
                                        ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                                        ds2 = tmSpc * Layout.Offset(i, n);
                                        FaceEdge[f][i].SetFace(node, (dx1, ds1), (dx2, ds2), (dx3, ds2));
                                    }
                                    break;

                                case 1: //south
                                    for (int i = 0; i < n; i++)
                                    {
                                        ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                                        ds2 = tmSpc * Layout.Offset(i, n);
                                        FaceEdge[f][i].SetFace(node, (ds1, dy1), (ds2, dy2), (ds2, dy2));
                                    }
                                    break;

                                case 2: //west
                                    for (int i = 0; i < n; i++)
                                    {
                                        ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                                        ds2 = tmSpc * Layout.Offset(i, n);
                                        FaceEdge[f][i].SetFace(node, (-dx1, ds1), (-dx2, ds2), (-dx3, ds2));
                                    }
                                    break;

                                case 3: //north
                                    for (int i = 0; i < n; i++)
                                    {
                                        ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                                        ds2 = tmSpc * Layout.Offset(i, n);
                                        FaceEdge[f][i].SetFace(node, (ds1, -dy1), (ds2, -dy2), (ds2, -dy2));
                                    }
                                    break;
                            }
                        }
                        #endregion
                    }
                    if (Cost == 0) break;
                }
            }
        }
    }
}
