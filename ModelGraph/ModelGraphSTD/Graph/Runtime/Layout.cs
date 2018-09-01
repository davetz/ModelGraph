using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        internal static (int count, Edge[] edge, int[] quad, int[] sect, float[] slope, int[] nquad, int[] nsect, int[] sectEdge, EdgeRotator[] conn) 
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
            if (count == 0) return (count, null, null, null, null, null, null, null, null);

            var other = new Node[count];
            var sect = new int[count];
            var quad = new int[count];
            var slope = new float[count];

            var bends = new (int X, int Y)[count];
            var ex = new Extent(n1.Center);

            var nquad = new int[5];
            var nsect = new int[9];

            var sectEdge = new int[10];
            var conn = new EdgeRotator[count];

            #region Locate Parallel Edges  ====================================
            var otherEdgeCount = new Dictionary<Node, int>(count);
            for (int i = 0; i < count; i++)
            {
                var p = edge[i].OtherBend(n1);
                other[i] = p.other;
                bends[i] = p.bend;

                if (otherEdgeCount.TryGetValue(p.other, out int c))
                    otherEdgeCount[p.other] = c + 1;
                else
                    otherEdgeCount.Add(p.other, 1);
            }

            var indexOfParallelEdge = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                if (otherEdgeCount[other[i]] == 1) continue;
                indexOfParallelEdge.Add(i);
            }
            #endregion

            #region Set Parallel Edge Bend Point  ============================= 
            if (indexOfParallelEdge.Count > 0)
            {
                indexOfParallelEdge.Sort(CompareParallelEdges);

                var ip = 0;
                var np = indexOfParallelEdge.Count;
                while (ip < np)
                {
                    var n2 = other[indexOfParallelEdge[ip]];
                    var (x2, y2) = ex.Point2 = n2.GetCenter();

                    var pc = InParallelCount(ip);
                    if (n2.Sizing == Sizing.Auto && n2.Orient != Orient.Point)
                    {
                        for (int i = ip, j = 0; j < pc; i++, j++)
                        {
                            var ds = 4 * Displacement(j, pc);
                            bends[indexOfParallelEdge[i]] = ex.OrthoginalDisplacedPoint(ds);
                        }
                    }
                    ip += pc;

                    int InParallelCount(int t)
                    {
                        while (t < np && n2 == other[indexOfParallelEdge[t]]) { t++; }
                        return t - ip;
                    }
                }
            }
            #endregion

            #region Calculate Quad, Sect, Slope  ==============================
            for (int i = 0; i < count; i++)
            {
                ex.Point2 = bends[i];
                var p = XYPair.QuadSectSlope(ex.Delta);

                quad[i] = p.quad;
                sect[i] = p.sect;
                slope[i] = p.slope;

                nquad[p.quad] += 1;
                nsect[p.sect] += 1;
            }
            #endregion

            #region Order Edges Based On Bend Point  ==========================
            //	Reorder the connections based on the radial direction to its destination line end
            //	quad[] identifies the radial quadrant (1,2,3,4 clockwise from horz-right)
            //	slope[] is the radial direction within that quadrant

            var allEdges = new List<int>(count);
            for (int i = 0; i < count; i++) { allEdges.Add(i); }
            allEdges.Sort(CompareQuadSlope);


            for (int i = 0; i < count; i++)
            {
                var j = allEdges[i];
                if (j == i) continue;

                var t1 = quad[i]; quad[i] = quad[j]; quad[j] = t1;
                var t2 = sect[i]; sect[i] = sect[j]; sect[j] = t2;
                var t3 = edge[i]; edge[i] = edge[j]; edge[j] = t3;
                var t4 = other[i]; other[i] = other[j]; other[j] = t4;
                var t5 = slope[i]; slope[i] = slope[j]; slope[j] = t5;
            }
            #endregion

            #region ComputeSectEdge  ==========================================
            int h = 0, k = 0, N = 10;
            for (int i = 0; i < count; i++)
            {
                if (k == sect[i]) continue;
                k = sect[i];
                for (; h <= k; h++) { sectEdge[h] = i; }
            }
            for (; h < N; h++) { sectEdge[h] = count; }
            #endregion

            #region InitEdgeRotators  =========================================
            for (int i = 0; i < count; i++)
            {
                conn[i] = new EdgeRotator(edge[i].GetConnect(n1));
            }
            #endregion

            return (count, edge, quad, sect, slope, nquad, nsect, sectEdge, conn);

            #region CompareParallelEdges  =====================================
            int CompareParallelEdges(int i, int j)
            {
                if (other[i].Item.GetHashCode() < other[j].Item.GetHashCode()) return -1;
                if (other[i].Item.GetHashCode() > other[j].Item.GetHashCode()) return 1;
                if (edge[i].GetHashCode() < edge[j].GetHashCode()) return -1;
                if (edge[i].GetHashCode() > edge[j].GetHashCode()) return 1;
                return 0;
            }
            #endregion

            #region CompareQuadSlope  =========================================
            int CompareQuadSlope(int i, int j)
            {
                if (quad[i] < quad[j]) return -1;
                if (quad[i] > quad[j]) return +1;
                if (slope[i] < slope[j]) return -1;
                if (slope[i] > slope[j]) return +1;
                return 0;
            }
            #endregion
        }

        internal static int Displacement(int index, int count)
        {
            //=====================================================================
            // The index is 0 based, the totalCount is 1's based 
            // for examle:    0,  1,  2,  3,  4 with the totalCount = 5
            //    _offset:   -4, -2,  0,  2,  4  
            //              . . . . . | . . . . . 
            //  or examle:  0,  1,  2,  3,  4,  5 with the totalCount = 6
            //    _offset: -5, -3, -1,  1,  3,  5
            //
            // The diference between succesive offset values is always 2
            return 2 * index - (count - 1);
        }
    }
}