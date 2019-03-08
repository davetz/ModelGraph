using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        #region ConnectedEdges  ===============================================
        static internal (Edge edge, Target targ, Node other, (float x, float y) bend, int tuple, int order, Attach atch, bool horz, bool revr)[] ConnectedEdges(Node n)
        {
            var (N, edges) = n.Graph.ConnectedEdges(n);
            if (N == 0) return null;

            var node_tuple = new Dictionary<Node,int>(N);
            var output = new (Edge edge, Target targ, Node other, (float x, float y) bend, int tuple, int order, Attach atch, bool horz, bool revr)[N];

            bool haveTuples = false;
            for (int i = 0; i < N; i++)
            {
                var (targ, other, bend, atch, horz, revr) = edges[i].TargetOtherBendAttachHorzRevr(n);
                if (node_tuple.TryGetValue(other, out int tuple))
                {
                    if (tuple < N)
                    {
                        node_tuple[other] = tuple + N;
                        haveTuples = true;
                    }
                }
                else
                {
                    node_tuple[other] = i;
                }
                output[i] = (edges[i], targ, other, bend, 0, 0, atch, horz, revr); 
            }
            if (haveTuples)
            {
                for (int i = 0; i < N; i++)
                {
                    var (edge, targ, other, bend, tuple, order, atch, horz, revr) = output[i];
                    var tup = node_tuple[other];
                    if (tup < N) continue;
                    output[i] = (edge, targ, other, bend, tup, edge.GetHashCode(), atch, horz, revr);
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