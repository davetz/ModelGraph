using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class HitMap
    {/*
        Build node and edge hit test maps for the purpose of providing a hash
        of nodes and edges that are in the vincinity of the hit test point.
        
        There are two parameters that control the speed and acuracy of the hit test:

        Scale:  Specified by the caller in Initialize(.. nodeScale .. edgeScale).
                It is used by the shift right operator x >> scale, y >> scale. 
                It has the effect of squeezing the target points closer together.
                If scale is too small the hit test radius has to be made larger to compensate,
                if it's too large the target points are squeezed so much that they
                overlap and overwrite each other in the dictionaries.
                
        Radius: Specified by the caller in NearByNodes(.. radius), NearByEdges(.. radius)
                When specifing radius, use the actual expected radius, ignore scale because
                actually, the callers radius will be scaled before it is used.
                If radius is too small the hit test may miss valid targes.
                It it's too large the hit test will take an excessive amount of time.
     */
        private int _nodeScale;
        private int _edgeScale;

        private Dictionary<(int, int), Node> Point_Node = new Dictionary<(int, int), Node>();
        private Dictionary<(int, int), Edge> Point_Edge = new Dictionary<(int, int), Edge>();

        #region Initialize  ===================================================
        internal void Initialize(int nodeScale = 3, int edgeScale = 3)
        {
            _nodeScale = nodeScale;
            _edgeScale = edgeScale;
            Point_Node.Clear();
            Point_Edge.Clear();
        }
        private (int x, int y) NodeScale((int x, int y) p) => (p.x >> _nodeScale, p.y >> _nodeScale);
        private (int x, int y) EdgeScale((int x, int y) p) => (p.x >> _edgeScale, p.y >> _edgeScale);
        private int Scale(int radius, int scale) => (radius >> scale) + 1;
        #endregion

        #region Add<Node,Edge>  ===============================================
        internal void AddNode(Node node)
        {
            Point_Node[NodeScale(node.Center)] = node;
        }

        internal void AddEdge(Edge edge)
        {
            var DS = (1 << _nodeScale);
            var N = edge.Points.Length;

            var p2 = EdgeScale(edge.Points[0]);
            Point_Edge[p2] = edge;

            for (int i = 1; i < N; i++)
            {
                var p1 = p2;
                p2 = EdgeScale(edge.Points[i]);
                var ds = XYPair.Diagonal(p1, p2);

                if (ds < DS) continue;
                Point_Edge[p2] = edge;

                var ln = Math.Sqrt(ds);
                var n = 2 * (ln / DS);
                var x = p2.x;
                var y = p2.y;
                var dx = (int)((p1.x - x) / n);
                var dy = (int)((p1.y - y) / n);

                for (int j = 0; j < n; j++)
                {
                    x += dx;
                    y += dy;
                    Point_Edge[(x, y)] = edge;
                }
            }
        }
        #endregion

        #region NearBy<Nodes,Edges>  ==========================================
        internal HashSet<Node> NearByNodes(HashSet<Node> selectNodes, int radius = 3)
        {
            const int N = 4;
            var cornerPoint = new (int x, int y)[N]; 
            HashSet<Node> nodeHash = null;

            foreach (var node in selectNodes)
            {
                var (x, y, w, h) = node.Values();
                cornerPoint[0] = (x - w, y - h);
                cornerPoint[1] = (x - w, y + h);
                cornerPoint[2] = (x + w, y - h);
                cornerPoint[3] = (x + w, y + h);

                for (int i = 0; i < N; i++)
                {
                    var testHash = NearByNodes(cornerPoint[i], radius);
                    if (testHash is null) continue;

                    foreach (var tnode in testHash)
                    {
                        if (selectNodes.Contains(tnode)) continue;

                        if (nodeHash is null) nodeHash = new HashSet<Node>();
                        nodeHash.Add(tnode);
                    }
                }
            }
            return nodeHash;
        }
        internal HashSet<Node> NearByNodes((int x, int y) testPoint, int radius = 3)
        {
            HashSet<Node> nodeHash = null;
            var (x, y) = NodeScale(testPoint);

            HitTest(x, y, Scale(radius, _nodeScale), Test);
            return nodeHash;

            void Test(int u, int v)
            {
                if (!Point_Node.TryGetValue((u, v), out Node node)) return;

                if (nodeHash is null) nodeHash = new HashSet<Node>();
                nodeHash.Add(node);
            }
        }
        internal HashSet<Edge> NearByEdges((int x, int y) testPoint, int radius = 3)
        {
            HashSet<Edge> edgeHash = null;
            var (x, y) = EdgeScale(testPoint);

            HitTest(x, y, Scale(radius, _edgeScale), Test);
            return edgeHash;

            void Test(int u, int v)
            {
                if (!Point_Edge.TryGetValue((u, v), out Edge edge)) return;

                if (edgeHash is null) edgeHash = new HashSet<Edge>();
                edgeHash.Add(edge);
            }
        }
        private void HitTest(int x, int y, int radius, Action<int, int> Test)
        {

            var M = 2;  //the number of test to preform on each face of the tier

            Test(x, y); //test the reference point
            for (int i = 0; i < radius; i++, M += 2)
            {
                //spiral arround with an increasing radius
                Test(++x, ++y); //south-east corner of next rectangular tier
                for (int j = 0; j < M; j++) { Test(--x, y); } //south face of tier heading west
                for (int j = 0; j < M; j++) { Test(x, --y); } // west face of tier heading north
                for (int j = 0; j < M; j++) { Test(++x, y); } //north face of tier heading east
                for (int j = 0; j < M; j++) { Test(x, ++y); } // east face of tier heading south
            }
        }
        #endregion
    }
}
