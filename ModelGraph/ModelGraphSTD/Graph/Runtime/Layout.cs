using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        #region SortedEdges  ==================================================
        internal static (int count, int[] nquad, int[] nsect, int[] sectEdge, (Edge edge, Node other, EdgeRotator conn, (int x, int y) bend, Quad quad, Sect sect)[])
            SortedEdges(Node n1)
        {/*
            Construct an optimumly ordered edge list for the given node.

            Consider the given node is at the center of a circle and the edge
            connections are a sequence of radial vectors.  
            Order the edges so that the radial vectors progress arround the circle
            in a clockwise direction. The circle has 8 sectors and 4 qaudrants as
            shown below. Keep track of the number of edges in each quadrant and
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

            var E = new (Edge edge, Node node, EdgeRotator conn, (int x, int y) bend, double slope, short ord1, short ord2, bool isTuple, bool isFirst, Quad quad, Sect sect)[count];  // working edge array
            var F = new (Edge edge, Node node, EdgeRotator conn, (int x, int y) bend, Quad quad, Sect sect)[count];   // output edge array

            var P = new List<int>(count);  // ordered edge indexes for parralell edges 
            var O = new List<int>(count);  // ordered edge indexes for all non-parrallel edges, but including just one of the parallel edges

            #region Populate edge array and locate parallel edges  ============
            var other_Count = new Dictionary<Node, int>(count);
            for (int i = 0; i < count; i++)
            {
                E[i].ord2 = (short)i;
                E[i].edge = edge[i];
                E[i].conn = new EdgeRotator(edge[i].GetConnect(n1));

                var (other, bend) = edge[i].OtherBend(n1);
                E[i].ord1 = (short)n1.Graph.Nodes.IndexOf(other);
                E[i].node = other;
                E[i].bend = bend;

                var (quad, sect, slope) = XYPair.QuadSectSlope(n1.Center, E[i].bend);
                E[i].quad = quad;
                E[i].sect = sect;
                E[i].slope = slope;

                if (other_Count.TryGetValue(other, out int c))
                    other_Count[other] = c + 1;
                else
                    other_Count.Add(other, 1);
            }

            for (int i = 0; i < count; i++) { if (other_Count[E[i].node] > 1) P.Add(i); }
            #endregion

            #region Order edges based on quad and slope  ======================
            if (P.Count > 0)
            {
                P.Sort(CompareParallelEdges);

                var first = 0;
                Node n2 = null;
                for (int p = 0; p < P.Count; p++)
                {
                    var i = P[p];

                    E[i].isTuple = true;
                    if (n2 == E[i].node)
                    {
                        E[i].quad = E[first].quad;
                        E[i].sect = E[first].sect;
                        E[i].bend = E[first].bend;
                    }
                    else
                    {
                        n2 = E[i].node;
                        first = i;
                        E[i].isFirst = true;
                    }
                }
                for (int i = 0; i < count; i++) { if (!E[i].isTuple || E[i].isFirst) O.Add(i); }
                O.Sort(CompareQuadSlope);

                var j = 0;
                foreach (var i in O)
                {
                    if (E[i].isTuple)
                    {
                        n2 = E[i].node;
                        var Q = new List<int>(P.Count);
                        foreach (var p in P) { if (E[p].node == n2) Q.Add(p); }

                        var sectList = SectList(n2);
                        if (sectList.Contains(E[i].sect)) Q.Reverse();

                        foreach (var q in Q) { CopyToOutput(q, j++); }
                    }
                    else
                        CopyToOutput(i, j++);
                }
                Debug.Assert(j == count);
            }
            else
            {
                for (int i = 0; i < count; i++) { O.Add(i); }
                O.Sort(CompareQuadSlope);
                for (int i = 0; i < count; i++) { CopyToOutput(O[i], i); }
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
            int h = 0, k = 0, N = 10;
            for (int i = 0; i < count; i++)
            {
                if (k == (int)E[i].sect) continue;
                k = (int)E[i].sect;
                for (; h <= k; h++) { sectEdge[h] = i; }
            }
            for (; h < N; h++) { sectEdge[h] = count; }
            #endregion

            return (count, nquad, nsect, sectEdge, F);

            #region Copy E to F  ==============================================
            void CopyToOutput(int i, int j)
            {
                F[j].edge = E[i].edge;
                F[j].node = E[i].node;
                F[j].conn = E[i].conn;
                F[j].bend = E[i].bend;
                F[j].quad = E[i].quad;
                F[j].sect = E[i].sect;
            }
            #endregion

            #region SectList  =================================================
            List<Sect> SectList(Node n2)
            {/*
                The order of the parallel edge tuples betwee two nodes need to 
                be reversed for the specific apposing edge faces. For example
                consider the edges labeled A, B, and C, as show below.

                         /\     node-1   desired order  node-2     given order
                         |C        A|= = = = = = = = = =|A        A|
                         |B        B|= = = = = = = = = =|B        B|
                         |A        C|= = = = = = = = = =|C        C|
               given order                                        \/
  
                       ===Sector==    =========== For Each Node ============
                         5\6|7/8      Imagine the center of each node is at
                         ~~~+~~~   <- the + sign and the edges are ordered
                         4/3|2\1      clockwise arround that begining at sector-1
                              
                This mitigation strategy works well, but even still, it doesn't
                cover all the odd possible corner cases. (sigh)
             */
                if (n1.Aspect == n2.Aspect)
                {
                    switch (n1.Aspect)
                    {
                        case Aspect.Central: return new List<Sect>(0) { Sect.S2, Sect.S3, Sect.S4, Sect.S5, };
                        case Aspect.Vertical: return new List<Sect>(0) { Sect.S3, Sect.S4, Sect.S5, Sect.S6 };
                        case Aspect.Horizontal: return new List<Sect>(0) { Sect.S1, Sect.S2, Sect.S3, Sect.S4 };
                    }
                }
                else
                {
                    if (n1.Aspect == Aspect.Horizontal && n2.Aspect == Aspect.Vertical) return new List<Sect>(0) { Sect.S1, Sect.S2, Sect.S3, Sect.S4, Sect.S5, Sect.S6, Sect.S7, Sect.S8 };
                    else if (n1.Aspect == Aspect.Central && n2.Aspect == Aspect.Vertical) return new List<Sect>(0) { Sect.S1, Sect.S2, Sect.S3, Sect.S4, Sect.S5, Sect.S6, Sect.S7, Sect.S8 };
                    else if (n1.Aspect == Aspect.Central && n2.Aspect == Aspect.Horizontal) return new List<Sect>(0) { Sect.S1, Sect.S2, Sect.S3, Sect.S4, Sect.S5, Sect.S6, Sect.S7, Sect.S8 };
                }
                return new List<Sect>(0);
            }
            #endregion

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
                if (E[i].quad < E[j].quad) return -1;
                if (E[i].quad > E[j].quad) return +1;
                if (E[i].slope < E[j].slope) return -1;
                if (E[i].slope > E[j].slope) return +1;
                return 0;
            }
            #endregion
        }
        #endregion

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