using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {

        #region SymbolFlip  ================================================
        private class SymbolFlip
        {
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

            #region Constructors  =============================================
            internal SymbolFlip(SymbolX symbol)
            {
                n = N = (byte)symbol.TopContact;
                s = S = (byte)symbol.BottomContact;
                w = W = (byte)symbol.LeftContact;
                e = E = (byte)symbol.RightContact;

                dx = DX = (byte)(symbol.Width / 2);
                dy = DY = (byte)(symbol.Height / 2);
            }
            #endregion

            internal void SetFlip(int value) { _action[value & 7](this); }

            internal Contact Contact(int index) { return _contact[index & 3](this); } // (E, S, W, N)

            #region VectorTables  =============================================
            private static Func<SymbolFlip, Contact>[] _contact = new Func<SymbolFlip, Contact>[]
            {
                (r) => { return (Contact)r.E; },
                (r) => { return (Contact)r.S; },
                (r) => { return (Contact)r.W; },
                (r) => { return (Contact)r.N; },
            };

            private static Action<SymbolFlip>[] _action = new Action<SymbolFlip>[]
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

            private static void FlipRotateNone(SymbolFlip r)
            {
                r.N = r.n;
                r.S = r.s;
                r.W = r.w;
                r.E = r.e;
                r.DX = r.dx;
                r.DY = r.dy;
            }
            private static void FlipVertical(SymbolFlip r)
            {
                r.N = r.s;
                r.S = r.n;
                r.W = r.w;
                r.E = r.e;
            }
            private static void FlipHorizontal(SymbolFlip r)
            {
                r.N = r.n;
                r.S = r.s;
                r.W = r.e;
                r.E = r.w;
            }
            private static void FlipBothWays(SymbolFlip r)
            {
                r.N = r.s;
                r.S = r.n;
                r.W = r.e;
                r.E = r.w;
            }
            private static void RotateClockWise(SymbolFlip r)
            {
                r.N = r.w;
                r.S = r.e;
                r.W = r.s;
                r.E = r.n;
                r.DX = r.dy;
                r.DY = r.dx;
            }
            private static void RotateFlipVertical(SymbolFlip r)
            {
                r.N = r.e;
                r.S = r.w;
                r.W = r.s;
                r.E = r.n;
            }
            private static void RotateFlipHorizontal(SymbolFlip r)
            {
                r.N = r.w;
                r.S = r.e;
                r.W = r.n;
                r.E = r.s;
            }
            private static void RotateFlipBothWays(SymbolFlip r)
            {
                r.N = r.e;
                r.S = r.w;
                r.W = r.n;
                r.E = r.s;
            }
            #endregion
        }
        #endregion

        #region CostSide  =====================================================
        //=====================================================================
        //      sect        quad         side
        //    5\6|7/8        3|4           N
        //    ~~~+~~~        ~+~         W + E
        //    4/3|2\1        2|1           S
        //=====================================================================
        // Orchestrate the face contact connection assignment loop
        // The are many permutations, but these are the rules and cost of assigning connections.
        // We pick the permutation with the cheapest overall cost.

        static (byte, Side)[] _sect1 = new (byte, Side)[]
        {
            (0, Side.East),
            (10, Side.South),
            (20, Side.North),
            (40, Side.West),
        };
        static (byte, Side)[] _sect2 = new (byte, Side)[]
        {
            (0, Side.South),
            (10, Side.East),
            (20, Side.West),
            (40, Side.North),
        };
        static (byte, Side)[] _sect3 = new (byte, Side)[]
        {
            (0, Side.South),
            (10, Side.West),
            (20, Side.East),
            (40, Side.North),
        };
        static (byte, Side)[] _sect4 = new (byte, Side)[]
        {
            (0, Side.West),
            (10, Side.South),
            (20, Side.North),
            (40, Side.East),
        };
        static (byte, Side)[] _sect5 = new (byte, Side)[]
        {
            (0, Side.West),
            (10, Side.North),
            (20, Side.South),
            (40, Side.East),
        };
        static (byte, Side)[] _sect6 = new (byte, Side)[]
        {
            (0, Side.North),
            (10, Side.West),
            (20, Side.East),
            (40, Side.South),
        };
        static (byte, Side)[] _sect7 = new (byte, Side)[]
        {
            (0, Side.North),
            (10, Side.East),
            (20, Side.West),
            (40, Side.South),
        };
        static (byte, Side)[] _sect8 = new (byte, Side)[]
        {
            (0, Side.East),
            (10, Side.North),
            (20, Side.South),
            (40, Side.West),
        };
        static (byte cost, Side side)[][] _sectCostSide = new (byte cost, Side side)[][]
        {
            null, _sect1, _sect2, _sect3, _sect4, _sect5, _sect6, _sect7, _sect8
        };
        private int flipIndex;
        #endregion

        private void AdjustSymbol(Node node)
        {
            var (count, nquad, nsect, E) = Layout.SortedEdges(node);
            if (count == 0) return;

            var isym = node.Symbol - 2;
            var symbol = Symbols[isym];
            var symFlip = new SymbolFlip(symbol);

            var sideEdge = new List<int>[] { new List<int>(count), new List<int>(count), new List<int>(count), new List<int>(count) };
            var bestSideEdge = new List<int>[] { new List<int>(count), new List<int>(count), new List<int>(count), new List<int>(count) };
            var unusedEdge = new List<Edge>(count);

            int bestCost = int.MaxValue;
            int bestFlip = 0;

            #region CompareQuadSlope  =========================================

            int CompareEastQuadSlope(int i, int j)
            {
                if (E[i].quad == Quad.Q1)
                {
                    if (E[j].quad == Quad.Q2)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q2)
                {
                    if (E[j].quad == Quad.Q1)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q3)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q4)
                        return -1;
                }
                else if (E[i].quad == Quad.Q4)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q3)
                        return -1;
                }
                if (E[i].slope < E[j].slope) return -1;
                if (E[i].slope > E[j].slope) return +1;
                return 0;
            }
            int CompareSouthQuadSlope(int i, int j)
            {
                if (E[i].quad == Quad.Q1)
                {
                    if (E[j].quad == Quad.Q2)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q2)
                {
                    if (E[j].quad == Quad.Q1)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q3)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q4)
                        return -1;
                }
                else if (E[i].quad == Quad.Q4)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q3)
                        return -1;
                }
                if (E[i].slope > E[j].slope) return -1;
                if (E[i].slope < E[j].slope) return +1;
                return 0;
            }
            int CompareWestQuadSlope(int i, int j)
            {
                if (E[i].quad == Quad.Q1)
                {
                    if (E[j].quad == Quad.Q2)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q2)
                {
                    if (E[j].quad == Quad.Q1)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q3)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q4)
                        return -1;
                }
                else if (E[i].quad == Quad.Q4)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q3)
                        return -1;
                }
                if (E[i].slope > E[j].slope) return -1;
                if (E[i].slope < E[j].slope) return +1;
                return 0;
            }
            int CompareNorthQuadSlope(int i, int j)
            {
                if (E[i].quad == Quad.Q1)
                {
                    if (E[j].quad == Quad.Q2)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q2)
                {
                    if (E[j].quad == Quad.Q1)
                        return 1;
                    else if (E[j].quad == Quad.Q3)
                        return 1;
                    else if (E[j].quad == Quad.Q4)
                        return 1;
                }
                else if (E[i].quad == Quad.Q3)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q4)
                        return -1;
                }
                else if (E[i].quad == Quad.Q4)
                {
                    if (E[j].quad == Quad.Q1)
                        return -1;
                    else if (E[j].quad == Quad.Q2)
                        return -1;
                    else if (E[j].quad == Quad.Q3)
                        return -1;
                }
                if (E[i].slope < E[j].slope) return -1;
                if (E[i].slope > E[j].slope) return +1;
                return 0;
            }
            #endregion

            for (int flipI = 0; flipI < 8; flipI++)
            {
                InitializeFlip(flipI);
                var done = 0;
                var cost = 0;

                for (int edgeI = 0; edgeI < count; edgeI++)
                {
                    int sectI = (int)E[edgeI].sect;
                    if (E[edgeI].conf.IsPriority1 == true)
                    {
                        foreach (var (delta, side) in _sectCostSide[sectI])
                        {
                            int sideI = (int)side;
                            if (SkipConnect(edgeI, sideI)) continue;

                            sideEdge[sideI].Add(edgeI);

                            done++;
                            cost += delta;
                            if (cost > bestCost) break;
                        }
                    }
                }
                for (int edgeI = 0; edgeI < count; edgeI++)
                {
                    int sectI = (int)E[edgeI].sect;
                    if (E[edgeI].conf.IsPriority1 == false)
                    {
                        foreach (var (delta, side) in _sectCostSide[sectI])
                        {
                            int sideI = (int)side;
                            if (SkipConnect(edgeI, sideI)) continue;
                            
                            sideEdge[sideI].Add(edgeI);

                            done++;
                            cost += delta;
                            if (cost > bestCost) break;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].conf.HasNotBeenUsed) cost += 5;
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestFlip = flipI;
                    for (int sideI = 0; sideI < 4; sideI++)
                    {
                        bestSideEdge[sideI].Clear();
                        bestSideEdge[sideI].AddRange(sideEdge[sideI]);
                    }

                    unusedEdge.Clear();
                    for (int edgeI = 0; edgeI < count; edgeI++)
                    {
                        if (E[edgeI].conf.HasNotBeenUsed) unusedEdge.Add(E[edgeI].edge);
                    }
                }
            }

            InitializeFlip(bestFlip);

            node.DX = symFlip.DX;
            node.DY = symFlip.DY;
            node.FlipRotate = (FlipRotate)bestFlip;

            var d1 = 8;
            var tmSpc = node.Graph.GraphX.TerminalSpacing;
            var tmLen = node.Graph.GraphX.TerminalLength;

            var dx1 = node.DX + 1;
            var dx2 = dx1 + d1;
            var dx3 = dx2 + tmLen;

            var dy1 = node.DY + 1;
            var dy2 = dy1 + d1;
            var dy3 = dy2 + tmLen;

            var dsx = (double)dx1 / 2;
            var dsy = (double)dy1 / 2;

            var ds1 = 0;
            var ds2 = 0;

            bestSideEdge[0].Sort(CompareEastQuadSlope);
            bestSideEdge[1].Sort(CompareSouthQuadSlope);
            bestSideEdge[2].Sort(CompareWestQuadSlope);
            bestSideEdge[3].Sort(CompareNorthQuadSlope);

            #region AssignEdgeConnectors  =========================
            for (var sideI = 0; sideI < 4; sideI++)
            {
                var n = bestSideEdge[sideI].Count;
                if (n == 0) continue;
                switch (sideI)
                {
                    case 0: //east
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            E[bestSideEdge[sideI][i]].edge.SetFace(node, (dx1, ds1), (dx2, ds2), (dx3, ds2));
                        }
                        break;

                    case 1: //south
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            E[bestSideEdge[sideI][i]].edge.SetFace(node, (ds1, dy1), (ds2, dy2), (ds2, dy2));
                        }
                        break;

                    case 2: //west
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            E[bestSideEdge[sideI][i]].edge.SetFace(node, (-dx1, ds1), (-dx2, ds2), (-dx3, ds2));
                        }
                        break;

                    case 3: //north
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            E[bestSideEdge[sideI][i]].edge.SetFace(node, (ds1, -dy1), (ds2, -dy2), (ds2, -dy2));
                        }
                        break;
                }
            }
            foreach (var edge in unusedEdge)
            {
                edge.SetFace(node, (0, 0), (0, 0));
            }
            #endregion

            #region InitializeFlip  ===========================================
            void InitializeFlip(int flip)
            {
                symFlip.SetFlip(flip);
                for (int i = 0; i < 4; i++) { sideEdge[i].Clear(); }
                for (int i = 0; i < count; i++) { E[i].conf.SetFlip(flip); }
            }
            #endregion

            #region SkipConnect  ==============================================
            bool SkipConnect(int e, int s)
            {
                if (symFlip.Contact(s) == Contact.None) return true;
                if (symFlip.Contact(s) == Contact.One && sideEdge[s].Count > 0) return true;
                if (!E[e].conf.CanConnect(s)) return true;

                return false;
            }
            #endregion
        }
    }
}