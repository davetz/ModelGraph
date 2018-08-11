using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        private void AdjustGraph()
        {
            foreach (var node in Nodes) { AdjustNode(node); }
            foreach (var edge in Edges) { edge.Refresh(); }
            SetExtent();
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
            var ndList = new List<Node>();
            foreach (var nd in nodes)
            {
                if (Node_Edges.TryGetValue(nd, out List<Edge> egList))
                {
                    foreach (var eg in egList)
                    {
                        edges.Add(eg);
                        ndList.Add(eg.Node1);
                        ndList.Add(eg.Node2);
                    }
                }
            }
            foreach (var nd in ndList)
            {
                nodes.Add(nd);
            }

            foreach (var node in nodes) { AdjustNode(node); }
            foreach (var edge in edges) { edge.Refresh(); }

            SetExtent();
        }
        private void AddNodeEdges(Node node, HashSet<Node> nodeHash, HashSet<Edge> edgeHash)
        {
            nodeHash.Add(node);
            if (Node_Edges.TryGetValue(node, out List<Edge> edges))
            {
                foreach (var edge in edges) { edgeHash.Add(edge); }
            }
        }
        private void AdjustNode(Node node)
        {
            if (node.IsNode)
            {
                if (node.IsManualSizing || node.IsFixedSizing)
                    AdjustFixedNode(node);
                else
                    AdjustAutoNode(node, GraphX.TerminalSpacing);
            }
            else if (node.IsSymbol)
                AdjustSymbol(node);
        }

    }
}
