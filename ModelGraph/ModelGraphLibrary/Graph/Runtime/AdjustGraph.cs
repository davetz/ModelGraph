using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public partial class Graph
    {
        private void AdjustGraph()
        {
            foreach (var node in Nodes) { AdjustNode(node); }
            foreach (var edge in Edges) { edge.Refresh(); }
            SetExtent();
            MinorDelta += 1;
        }
        public void AdjustGraph(Selector selector)
        {
            var nodes = new HashSet<Node>();
            var edges = new HashSet<Edge>();

            if (selector.HitNode != null) AddNodeEdges(selector.HitNode, nodes, edges);
            if (selector.HitEdge != null) edges.Add(selector.HitEdge);
            foreach (var node in selector.Nodes) { nodes.Add(node); }
            foreach (var edge in selector.Edges) { edges.Add(edge); }
            foreach (var edge in selector.Chops)
            {
                edges.Add(edge);
                AddNodeEdges(edge.Node1, nodes, edges);
                AddNodeEdges(edge.Node2, nodes, edges);
            }

            foreach (var node in nodes) { AdjustNode(node); }
            foreach (var edge in edges) { edge.Refresh(); }

            SetExtent();
            MinorDelta += 1;
        }
        private void AddNodeEdges(Node node, HashSet<Node> nodeHash, HashSet<Edge> edgeHash)
        {
            nodeHash.Add(node);
            List<Edge> edges;
            if (Node_Edges.TryGetValue(node, out edges))
            {
                foreach (var edge in edges) { edgeHash.Add(edge); }
            }
        }
        private void AdjustNode(Node node)
        {
            if (node.Core.IsNode)
            {
                if (node.Core.IsManualSizing || node.Core.IsFixedSizing)
                    AdjustFixedNode(node);
                else
                    AdjustAutoNode(node, GraphX.TerminalSpacing);
            }
            else if (node.Core.IsSymbol)
                AdjustSymbol(node);
        }

    }
}
