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

        #region CostSideSector  ===============================================
        //=====================================================================
        //      sect        quad         side
        //    5\6|7/8        3|4           N
        //    ~~~+~~~        ~+~         W + E
        //    4/3|2\1        2|1           S
        //=====================================================================
        // Orchestrate the face contact connection assignment loop
        // The are many permutations, but these are the rules and cost of assigning connections.
        // We pick the permutation with the cheapest overall cost.

        static (short cost, Side side, Sect sect)[] _costSideSector = new (short cost, Side side, Sect sect)[]
        {
            (0, Side.East, Sect.S8),
            (0, Side.East, Sect.S1),

            (0, Side.South, Sect.S2),
            (0, Side.South, Sect.S3),

            (0, Side.West, Sect.S4),
            (0, Side.West, Sect.S5),

            (0, Side.North, Sect.S6),
            (0, Side.North, Sect.S7),

            (10, Side.East, Sect.S2),
            (10, Side.South, Sect.S4),
            (10, Side.West, Sect.S6),
            (10, Side.North, Sect.S8),

            (10, Side.East, Sect.S7),
            (10, Side.South, Sect.S1),
            (10, Side.West, Sect.S3),
            (10, Side.North, Sect.S5),

            (20, Side.East, Sect.S3),
            (20, Side.South, Sect.S5),
            (20, Side.West, Sect.S7),
            (20, Side.North, Sect.S1),

            (20, Side.East, Sect.S6),
            (20, Side.South, Sect.S8),
            (20, Side.West, Sect.S2),
            (20, Side.North, Sect.S4),

            (40, Side.East, Sect.S4),
            (40, Side.South, Sect.S6),
            (40, Side.West, Sect.S8),
            (40, Side.North, Sect.S2),

            (40, Side.East, Sect.S5),
            (40, Side.South, Sect.S7),
            (40, Side.West, Sect.S1),
            (40, Side.North, Sect.S3),
        };                              // Sect   S0     S1     S2    S3    S4    S5    S6     S7     S8
        static bool[] _sectInsert = new bool[] { false, false, true, true, true, true, false, false, true };
        #endregion

        private void AdjustSymbol(Node node)
        {
            var (count, nquad, nsect, E) = Layout.SortedEdges(node);
            if (count == 0) return;

            var sectEdge = new int[10];
            BuildSectEdge();

            var isym = node.Symbol - 2;
            var symbol = Symbols[isym];
            var symFlip = new SymbolFlip(symbol);

            var sideEdge = new List<Edge>[] { new List<Edge>(count), new List<Edge>(count), new List<Edge>(count), new List<Edge>(count) };
            var bestSideEdge = new List<Edge>[] { new List<Edge>(count), new List<Edge>(count), new List<Edge>(count), new List<Edge>(count) };
            var unusedEdge = new List<Edge>(count);

            int bestCost = int.MaxValue;
            int bestFlip = 0;
            for (int flip = 0; flip < 8; flip++)
            {
                InitializeFlip(flip);

                int cost = 0;
                foreach (var (delta, side, sect) in _costSideSector)
                {
                    int sideI = (int)side;
                    int sectI = (int)sect;
                    var e1 = sectEdge[sectI];
                    var e2 = sectEdge[sectI + 1];
                    if (e1 == e2) continue;

                    for (int e = e1; e < e2; e++)
                    {
                        if (SkipConnect(e, sideI)) continue;

                        if (_sectInsert[(int)E[e].sect])
                            sideEdge[sideI].Insert(0, E[e].edge);
                        else
                            sideEdge[sideI].Add(E[e].edge);

                        cost += delta;
                        if (cost > bestCost) break;
                    }
                    if (cost > bestCost) break;
                }
                for (int i = 0; i < count; i++)
                {
                    if (E[i].conf.HasNotBeenUsed) cost += 5;
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestFlip = flip;
                    for (int i = 0; i < 4; i++)
                    {
                        bestSideEdge[i].Clear();
                        bestSideEdge[i].AddRange(sideEdge[i]);
                    }

                    unusedEdge.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        if (E[i].conf.HasNotBeenUsed) unusedEdge.Add(E[i].edge);
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

            #region AssignEdgeConnectors  =========================
            for (var s = 0; s < 4; s++)
            {
                var n = bestSideEdge[s].Count;
                if (n == 0) continue;
                switch (s)
                {
                    case 0: //east
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            bestSideEdge[s][i].SetFace(node, (dx1, ds1), (dx2, ds2), (dx3, ds2));
                        }
                        break;

                    case 1: //south
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            bestSideEdge[s][i].SetFace(node, (ds1, dy1), (ds2, dy2), (ds2, dy2));
                        }
                        break;

                    case 2: //west
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            bestSideEdge[s][i].SetFace(node, (-dx1, ds1), (-dx2, ds2), (-dx3, ds2));
                        }
                        break;

                    case 3: //north
                        for (int i = 0; i < n; i++)
                        {
                            ds1 = (int)((dsx / n) * Layout.Offset(i, n));
                            ds2 = tmSpc * Layout.Offset(i, n);
                            bestSideEdge[s][i].SetFace(node, (ds1, -dy1), (ds2, -dy2), (ds2, -dy2));
                        }
                        break;
                }
            }
            foreach (var edge in unusedEdge)
            {
                edge.SetFace(node, (0, 0), (0, 0));
            }
            #endregion

            #region BuildSectEdge  ============================================
            void BuildSectEdge()
            {
                var sP = 0;     //prevous edge sector index
                for (int eI = 0; eI < count; eI++)
                {
                    var sI = (int)E[eI].sect;
                    if (sI != sP)
                    {
                        sectEdge[sI] = eI;
                        sP = sI;
                    }
                    for (int s = sP + 1; s < 10; s++) { sectEdge[s] = eI + 1; }
                }
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
