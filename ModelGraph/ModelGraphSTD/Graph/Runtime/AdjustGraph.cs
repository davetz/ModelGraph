using System.Collections.Generic;
using System.Linq;

namespace ModelGraphSTD
{
    public partial class Graph
    {
        public void AdjustGraph()
        {
            AdjustNodes(Nodes);
        }
        public void AdjustGraph(Selector selector)
        {
            var nodes = new HashSet<Node>();

            if (selector.HitNode != null) nodes.Add(selector.HitNode);
            if (selector.HitEdge != null) { nodes.Add(selector.HitEdge.Node1); nodes.Add(selector.HitEdge.Node2); }
            foreach (var node in selector.Nodes) { nodes.Add(node); }
            foreach (var edge in selector.Edges) { nodes.Add(edge.Node1); nodes.Add(edge.Node2); }
            foreach (var e in selector.Points)
            {
                var edge = e.Key;
                nodes.Add(edge.Node1);
                nodes.Add(edge.Node2);
            }

            ExpandNeighborhood();
            AdjustNodes(nodes);

            #region ExpandNeighborhood  =======================================
            void ExpandNeighborhood()
            {
                for (int i = 0; i < 2; i++)
                {
                    var ndList = nodes.ToArray();
                    foreach (var nd in ndList)
                    {
                        if (Node_Edges.TryGetValue(nd, out List<Edge> egList))
                        {
                            foreach (var eg in egList)
                            {
                                nodes.Add(eg.Node1);
                                nodes.Add(eg.Node2);
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void AdjustNodes(IEnumerable<Node> nodes)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var node in nodes) { if (node.Aspect != Aspect.Point) AdjustNode(node); }
                foreach (var node in nodes) { if (node.Aspect == Aspect.Point) AdjustNode(node); }
            }
            SetExtent();

            void AdjustNode(Node node)
            {
                if (node.IsGraphNode)
                {
                    if (node.Aspect == Aspect.Point)
                        AdjustAutoNode(node);
                    else if (node.IsManualSizing || node.IsFixedSizing)
                        AdjustManualNode(node);
                    else
                        AdjustAutoNode(node);
                }
                else if (node.IsGraphSymbol)
                    AdjustSymbol(node);
            }
        }
    }
}
