using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph
    {
        public void AdjustGraph()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var node in Nodes) { if (node.Aspect != Aspect.Point) AdjustNode(node); }
                foreach (var node in Nodes) { if (node.Aspect == Aspect.Point) AdjustNode(node); }
            }
            SetExtent();
        }
        public void AdjustGraph(Selector selector)
        {
            var nodes = new HashSet<Node>();
            var edges = new HashSet<Edge>();

            if (selector.HitNode != null) AddNodeEdges(selector.HitNode);
            if (selector.HitEdge != null) edges.Add(selector.HitEdge);
            foreach (var node in selector.Nodes) { nodes.Add(node); }
            foreach (var edge in selector.Edges) { edges.Add(edge); }
            foreach (var edge in selector.Chops)
            {
                edges.Add(edge);
                AddNodeEdges(edge.Node1);
                AddNodeEdges(edge.Node2);
            }

            for (int i = 0; i < 2; i++) { ExpandNeighborhood(); }
            for (int i = 0; i < 2; i++)
            {
                foreach (var node in nodes) { if (node.Aspect != Aspect.Point) AdjustNode(node); }
                foreach (var node in nodes) { if (node.Aspect == Aspect.Point) AdjustNode(node); }
            }

            SetExtent();

            #region AddNodeEdges  =============================================
            void AddNodeEdges(Node node)
            {
                nodes.Add(node);
                if (Node_Edges.TryGetValue(node, out List<Edge> list))
                {
                    foreach (var edge in list) { edges.Add(edge); }
                }
            }
            #endregion

            #region ExpandNeighborhood  =======================================
            void ExpandNeighborhood()
            {
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
            }
            #endregion
        }
        private void AdjustNode(Node node)
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
