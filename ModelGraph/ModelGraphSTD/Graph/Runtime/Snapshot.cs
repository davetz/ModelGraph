using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class Snapshot
    {
        private readonly List<(Edge edge, ((float x, float y)[], Facet, Facet) snapshot)> _edges;
        private readonly List<(Node node, (float, float, byte, byte, byte, byte, Labeling, Sizing, BarWidth, FlipRotate, Aspect) snapshot)> _nodes;

        internal Snapshot(Selector selector)
        {/*
            Create a graphic snapshot for the nodes and edges specified in the callers selector
         */
            if (selector.IsRegionHit)
            {
                if (selector.Nodes.Count > 0)
                {
                    _nodes = new List<(Node node, (float, float, byte, byte, byte, byte, Labeling, Sizing, BarWidth, FlipRotate, Aspect))>(selector.Nodes.Count);
                    foreach (var node in selector.Nodes)
                    {
                        _nodes.Add((node, node.Snapshot));
                    }
                }
                if (selector.Edges.Count > 0 || selector.Points.Count > 0)
                {
                    _edges = new List<(Edge edge, ((float x, float y)[], Facet, Facet))>(selector.Edges.Count + selector.Points.Count);
                    foreach (var edge in selector.Edges)
                    {
                        _edges.Add((edge, edge.Snapshot));
                    }
                    foreach (var p in selector.Points)
                    {
                        _edges.Add((p.Key, p.Key.Snapshot));
                    }
                }
            }
            else if (selector.IsNodeHit)
            {
                _nodes = new List<(Node node, (float, float, byte, byte, byte, byte, Labeling, Sizing, BarWidth, FlipRotate, Aspect))>(1);
                _nodes.Add((selector.HitNode, selector.HitNode.Snapshot));
            }
            else if (selector.IsEdgeHit)
            {
                _edges = new List<(Edge edge, ((float x, float y)[], Facet, Facet))>(1);
                _edges.Add((selector.HitEdge, selector.HitEdge.Snapshot));
            }
        }



        internal Snapshot(Snapshot snap)
        {/*
            Create a graphic snapshot for the nodes and edges specified by a previous snapshot.

            We have to preserve the current values before they are changed by resoring the previous snapshot
         */
            if (snap._nodes != null)
            {
                _nodes = new List<(Node node, (float, float, byte, byte, byte, byte, Labeling, Sizing, BarWidth, FlipRotate, Aspect))>(snap._nodes.Count);
                foreach (var n in snap._nodes)
                {
                    _nodes.Add((n.node, n.node.Snapshot));
                }

            }
            if (snap._edges != null)
            {
                _edges = new List<(Edge edge, ((float x, float y)[], Facet, Facet))>(snap._edges.Count);
                foreach (var e in snap._edges)
                {
                    _edges.Add((e.edge, e.edge.Snapshot));
                }
            }
        }



        internal void Restore()
        {
            if (_nodes != null) { foreach (var (node, snapshot) in _nodes) { node.Snapshot = snapshot; } }
            if (_edges != null) { foreach (var (edge, snapshot) in _edges) { edge.Snapshot = snapshot; } }
        }
    }
}
