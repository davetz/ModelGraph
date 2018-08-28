using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        internal static (int count, Edge[] edges) FarNodeParms(Graph g, Node n)
        {
            var (count, edges) = g.ConnectedEdges(n);
            if (count == 0) return (count, null);

            var other = new Node[count];
            var sect = new int[count];
            var quad = new int[count];
            var slope = new float[count];

            var bends = new (int X, int Y)[count];
            var ex = new Extent(n.Center);

            var nquad = new int[5];
            var nsect = new int[9];
            var sectEdge = new int[10];

            var mult = new Dictionary<Node, List<Edge>>();
            for (int i = 0; i < count; i++)
            {
               var p = edges[i].OtherBend(n);
                other[i] = p.other;
                bends[i] = p.bend;

                if (mult.TryGetValue(p.other, out List<Edge> list))
                    list.Add(edges[i]);
                else
                    mult[p.other] = new List<Edge>() { edges[i] };
            }




            return (count, edges);


        }
    }
}
