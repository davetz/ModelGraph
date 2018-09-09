using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        internal static (int count, int[] nquad, int[] nsect, int[] sectEdge, (Edge edge, Node other, EdgeRotator conn, (int x, int y) bend, double slope, short ord1, short ord2, short tuple, Quad quad, Sect sect)[]) 
            FarNodeParms(Node n1)
        {/*
            Construct an optimumly ordered edge list for the given node.

            Consider the given node is at the center of a circle and the edge
            connections are a sequence of radial vectors.  
            Order the edges so that the radial vectors progress arround the circle
            in a clockwise direction. The circle has 8 sectors and 4 qaudrants as
            shown below. Keep track of the number of lines in each quadrant and
            sector.    
                            sect         quad       side
                           =======       ====       ======
                           5\6|7/8        3|4         N
                           ~~~+~~~        ~+~       W + E
                           4/3|2\1        2|1         S
        */
            var (count, edge) = n1.Graph.ConnectedEdges(n1);
            if (count == 0) return (count, null, null, null, null);

            var nquad = new int[5];
            var nsect = new int[9];
            var sectEdge = new int[10];
            var E = new (Edge edge, Node other, EdgeRotator conn, (int x, int y) bend, double slope, short ord1, short ord2, short tuple, Quad quad, Sect sect)[count];


            #region Populate edge array and locate parallel edges  ============
            var otherEdgeCount = new Dictionary<Node, int>(count);
            for (int i = 0; i < count; i++)
            {
                E[i].ord2 = (short)i;
                E[i].edge = edge[i];
                E[i].conn = new EdgeRotator(edge[i].GetConnect(n1));

                var (other, bend) = edge[i].OtherBend(n1);
                E[i].ord1 = (short)n1.Graph.Nodes.IndexOf(other);
                E[i].other = other;
                E[i].bend = bend;

                if (otherEdgeCount.TryGetValue(other, out int c))
                    otherEdgeCount[other] = c + 1;
                else
                    otherEdgeCount.Add(other, 1);
            }

            var indexOfParallelEdge = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                if (otherEdgeCount[E[i].other] == 1) continue;
                indexOfParallelEdge.Add(i);
            }
            #endregion

            #region Set parallel edge bend point  ============================= 
            if (indexOfParallelEdge.Count > 0)
            {
                indexOfParallelEdge.Sort(CompareParallelEdges);

                var ip = 0;
                var np = indexOfParallelEdge.Count;
                var tc = (byte)0;
                var ext = new Extent(n1.Center);
                while (ip < np)
                {
                    var n2 = E[indexOfParallelEdge[ip]].other;
                    var (x2, y2) = ext.Point2 = n2.GetCenter();

                    var pc = InParallelCount(ip);
                    if (n2.Sizing == Sizing.Auto && n2.Aspect != Aspect.Point)
                    {
                        tc++;
                        for (int i = ip, j = 0; j < pc; i++, j++)
                        {
                            var ds = 7 * Offset(j, pc);
                            E[indexOfParallelEdge[i]].tuple = tc;
                            E[indexOfParallelEdge[i]].bend = ext.OrthoginalDisplacedPoint(ds);
                        }
                    }
                    ip += pc;

                    int InParallelCount(int t)
                    {
                        while (t < np && n2 == E[indexOfParallelEdge[t]].other) { t++; }
                        return t - ip;
                    }
                }
            }
            #endregion

            #region Calculate quad, qect, slope  ==============================
            for (int i = 0; i < count; i++)
            {
                var (quad, sect, slope) = XYPair.QuadSectSlope(n1.Center, E[i].bend);

                E[i].quad = quad;
                E[i].sect = sect;
                E[i].slope = slope;
            }
            #endregion

            #region Order edges based on quad and slope  ======================
            var edgeIndex = new List<int>(count);
            for (int i = 0; i < count; i++) { edgeIndex.Add(i); }
            edgeIndex.Sort(CompareQuadSlope);


            for (int i = 0; i < count; i++)
            {
                var j = edgeIndex[i];
                if (j == i) continue;

                var t0 = edgeIndex[i]; edgeIndex[i] = edgeIndex[j]; edgeIndex[j] = t0;
                var t1 = E[i]; E[i] = E[j]; E[j] = t1;
            }
            #endregion

            #region AssignTupleQuadSect  ======================================
            var tupleT = (short)0;
            var firstT = 0;
            var countT = 0;
            for (int i = 0; i < count; i++)
            {
                if (E[i].tuple == 0)
                {
                    if (tupleT != 0) AssignTupleQuadSect();
                }
                else
                {
                    if (E[i].tuple == tupleT)
                    {
                        countT++;
                    }
                    else
                    {
                        if (tupleT != 0) AssignTupleQuadSect();

                        tupleT = E[i].tuple;
                        firstT = i;
                        countT = 1;
                    }
                }
            }
            if (tupleT != 0) AssignTupleQuadSect();

            void AssignTupleQuadSect()
            {
                if (tupleT != 0 && countT > 1)
                {
                    var nd2 = E[firstT].other;
                    var ex = new Extent(n1.Center);
                    ex.Point2 = nd2.Center;
                    var (quad, sect, slope) = XYPair.QuadSectSlope(n1.Center, nd2.Center);
                    for (int i = 0, j = firstT; i < countT; i++, j++)
                    {
                        E[j].quad = quad;
                        E[j].sect = sect;
                    }
                }
                tupleT = 0;
                firstT = countT = 0;
            }
            #endregion

            #region Calculate nquad and nsect  ================================
            for (int i = 0; i < count; i++)
            {
                nquad[(int)E[i].quad] += 1;
                nsect[(int)E[i].sect] += 1;
            }
            #endregion

            #region Compute sectEdge  =========================================
            int h = 0, k = 0,  N = 10;
            for (int i = 0; i < count; i++)
            {
                if (k == (int)E[i].sect) continue;
                k = (int)E[i].sect;
                for (; h <= k; h++) { sectEdge[h] = i; }
            }
            for (; h < N; h++) { sectEdge[h] = count; }
            #endregion

            return (count, nquad, nsect, sectEdge, E);

            #region CompareParallelEdges  =====================================
            int CompareParallelEdges(int i, int j)
            {
                if (E[i].ord1 < E[j].ord1) return -1;
                if (E[i].ord1 > E[j].ord1) return 1;
                if (E[i].ord2 < E[j].ord2) return -1;
                if (E[i].ord2 > E[j].ord2) return 1;
                return 0;
            }
            #endregion

            #region CompareQuadSlope  =========================================
            int CompareQuadSlope(int i, int j)
            {
                if (E[i].tuple == 0 || E[i].tuple != E[j].tuple)
                {
                    if (E[i].quad < E[j].quad) return -1;
                    if (E[i].quad > E[j].quad) return +1;
                }
                else if (E[i].quad == Quad.Q3 && E[j].quad == Quad.Q4)
                    return -1;
                else if (E[i].quad == Quad.Q4 && E[j].quad == Quad.Q3)
                    return +1;
                else if (E[i].quad == Quad.Q1 && E[j].quad == Quad.Q2)
                    return -1;
                else if (E[i].quad == Quad.Q2 && E[j].quad == Quad.Q1)
                    return +1;

                if (E[i].slope < E[j].slope) return -1;
                if (E[i].slope > E[j].slope) return +1;
                return 0;
            }
            #endregion
        }

        #region Offset  =======================================================
        internal static int Offset(int index, int count)
        {/*
            The index is 0 based, the totalCount is 1's based 
            for examle:    0,  1,  2,  3,  4 with the totalCount = 5
                offset:   -4, -2,  0,  2,  4  
                         . . . . . | . . . . . 
             or examle:  0,  1,  2,  3,  4,  5 with the totalCount = 6
                offset: -5, -3, -1,  1,  3,  5
           
            The diference between succesive offset values is always 2
         */
            return 2 * index - (count - 1);
        }
        #endregion
    }
}