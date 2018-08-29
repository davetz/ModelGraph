using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        internal static (int count, Edge[] edges, int[] quad, int[] sect, Side[] side, float[] slope, int[] nquad, int[] nsect, int[] sectEdge, EdgeRotator[] conn) 
            FarNodeParms(Node n1)
        {/*
            Construct an optimumly ordered line list for the given node.
            Consider the given node is at the center of a circle and the
            line connections are represented as a sequence of radial vectors. 
            Order the lines so that the radial vectors progress arround the circle
            in a clockwise direction. The circle has 8 sectors and 4 qaudrants as
            shown below. Keep track of the number of lines in each quadrant and
            sector.

                sect         quad       side
               =======       ====       ======
               5\6|7/8        3|4         N
               ~~~+~~~        ~+~       W + E
               4/3|2\1        2|1         S
        */
            var (count, edges) = n1.Graph.ConnectedEdges(n1);
            if (count == 0) return (count, null, null, null, null, null, null, null, null, null);

            var other = new Node[count];
            var sect = new int[count];
            var quad = new int[count];
            var slope = new float[count];
            var side = new Side[count];

            var bends = new (int X, int Y)[count];
            var ex = new Extent(n1.Center);

            var nquad = new int[5];
            var nsect = new int[9];

            var sectEdge = new int[10];
            var conn = new EdgeRotator[count];

            var indexOfParallelEdge = new List<int>(count);
            var nodeHash = new HashSet<Node>();
            for (int i = 0; i < count; i++)
            {
                var p = edges[i].OtherBend(n1);
                other[i] = p.other;
                bends[i] = p.bend;

                if (nodeHash.Contains(p.other))
                    indexOfParallelEdge.Add(i); // there are parallel edges between two nodes
                else
                    nodeHash.Add(p.other);
            }

            if (indexOfParallelEdge.Count > 0)
            {
                indexOfParallelEdge.Sort(CompareParallelEdges);

                var i = 0;
                var k = indexOfParallelEdge.Count;
                while (i < k)
                {
                    var n2 = other[indexOfParallelEdge[i]];
                    var (w, z) = ex.Point2 = n2.GetCenter();
                    var (x, y) = GetVector();

                    var n = InParallelCount(i);
                    var l = indexOfParallelEdge[i];
                    for (int j = 0; j < n; i++, j++)
                    {
                        if (n2.Sizing == Sizing.Auto && n2.Orient != Orient.Point)
                        {
                            var d = (2 * j - (n - 1)) * 2;
                            bends[l] = (w + x * d, z + y * d);
                        }
                    }

                    int InParallelCount(int t)
                    {
                        while (t < k && n2 == other[indexOfParallelEdge[t]]) { t++; }
                        return t - i;
                    }

                    (int x, int y) GetVector()
                    {
                        switch (XYPair.Quad(ex.Delta))
                        {
                            case 1: return (-1, +1);
                            case 2: return (-1, -1);
                            case 3: return (+1, -1);
                            case 4: return (+1, +1);
                            default: return (0, 0);
                        }
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                ex.Point2 = bends[i];
                var p = XYPair.QuadSectSideSlope(ex.Delta);

                quad[i] = p.quad;
                sect[i] = p.sect;
                side[i] = p.side;

                nquad[p.quad] += 1;
                nsect[p.sect] += 1;

                slope[i] = p.slope;
            }

            //	Reorder the connections based on the radial direction to its destination line end
            //	quad[] identifies the radial quadrant (1,2,3,4 clockwise from horz-right)
            //	slope[] is the radial direction within that quadrant
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    switch (quad[i])
                    {
                        case 1:
                            if ((quad[j] == 1) && (slope[j] < slope[i])) Swap();
                            break;
                        case 2:
                            if (quad[j] < 2) Swap();
                            else if ((quad[j] == 2) && (slope[j] < slope[i])) Swap();
                            break;
                        case 3:
                            if (quad[j] < 3) Swap();
                            else if ((quad[j] == 3) && (slope[j] < slope[i])) Swap();
                            break;
                        case 4:
                            if (quad[j] < 4) Swap();
                            else if ((quad[j] == 4) && (slope[j] < slope[i])) Swap();
                            break;
                    }

                    void Swap()
                    {
                        var t1 = quad[i]; quad[i] = quad[j]; quad[j] = t1;
                        var t2 = sect[i]; sect[i] = sect[j]; sect[j] = t2;
                        var t3 = edges[i]; edges[i] = edges[j]; edges[j] = t3;
                        var t4 = other[i]; other[i] = other[j]; other[j] = t4;
                        var t5 = slope[i]; slope[i] = slope[j]; slope[j] = t5;
                    }
                }
            }

            ComputeSectEdge();
            InitEdgeRotators();

            return (count, edges, quad, sect, side, slope, nquad, nsect, sectEdge, conn);


            void ComputeSectEdge()
            {
                int h = 0, k = 0, N = 10;
                for (int i = 0; i < count; i++)
                {
                    if (k == sect[i]) continue;
                    k = sect[i];
                    for (; h <= k; h++) { sectEdge[h] = i; }
                }
                for (; h < N; h++) { sectEdge[h] = count; }
            }

            void InitEdgeRotators()
            {
                for (int i = 0; i < count; i++)
                {
                    conn[i] = new EdgeRotator(edges[i].GetConnect(n1));
                }
            }

            int CompareParallelEdges(int i, int j)
            {
                if (other[i].GetHashCode() < other[j].GetHashCode()) return -1;
                if (other[i].GetHashCode() > other[j].GetHashCode()) return 1;
                if (edges[i].GetHashCode() < edges[j].GetHashCode()) return -1;
                if (edges[i].GetHashCode() > edges[j].GetHashCode()) return 1;
                return 0;
            }
        }
    }
}