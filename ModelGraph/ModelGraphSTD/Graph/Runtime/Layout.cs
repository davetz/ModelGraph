using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        #region ConnectedEdges  ===============================================
        internal static (Edge edge, Target targ, Node other, (float x, float y) bend, int tuple, int order, Attach atch, TupleSort tsort)[] ConnectedEdges(Node n)
        {
            var (N, edges) = n.Graph.ConnectedEdges(n);
            if (N == 0) return null;

            var node_tuple = new Dictionary<Node,int>(N);
            var output = new (Edge edge, Target targ, Node other, (float x, float y) bend, int tuple, int order, Attach atch, TupleSort tsort)[N];

            bool haveTuples = false;
            for (int i = 0; i < N; i++)
            {
                var (targ, other, bend, isBend, atch, tsort) = edges[i].TargetOtherBendAttachTSort(n);
                if (!isBend && node_tuple.TryGetValue(other, out int tup))
                {
                    if (tup < N)
                    {
                        node_tuple[other] = tup + N;
                        haveTuples = true;
                    }
                }
                else
                    node_tuple[other] = i;

                output[i] = (edges[i], targ, other, bend, 0, 0, atch, tsort); 
            }
            if (haveTuples)
            {
                for (int i = 0; i < N; i++)
                {
                    var tup = node_tuple[output[i].other];
                    if (tup < N) continue;
                    output[i].tuple = tup;
                    output[i].order = output[i].edge.GetHashCode();
                }
            }
            return output;
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