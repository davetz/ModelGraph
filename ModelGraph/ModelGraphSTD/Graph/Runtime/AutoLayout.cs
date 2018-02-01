using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        internal void CheckLayout()
        {
            var cp = GraphParm.CenterOffset;

            foreach (var node in Nodes)
            {
                if (node.Core.TryInitialize(cp)) cp += 8;
            }
            AdjustGraph();
        }
    }
}
