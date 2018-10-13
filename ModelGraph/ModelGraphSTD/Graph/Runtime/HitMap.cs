using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    internal class HitMap
    {
        private int _z; // scale decreases accuracy and gives more false positive hits
        private int _n; // radius of the area arround test point (must be big enough to avoid false misses) 

        private Dictionary<(int, int), Node> Point_Node = new Dictionary<(int, int), Node>();
        private Dictionary<(int, int), Edge> Point_Edge = new Dictionary<(int, int), Edge>();

        internal void Initialize(int z = 1)
        {
            _z = z;
            Point_Node.Clear();
            Point_Edge.Clear();
        }
        private (int x, int y) Scale((int x, int y) p) => (p.x >> _z, p.y >> _z);

        internal void Add(Node node)
        {
            Point_Node[Scale(node.Center)] = node;
        }

        internal void Add(Edge edge)
        {
            var DS = (1 << _z);
            var N = edge.Points.Length;

            var p2 = Scale(edge.Points[0]);
            Point_Edge[p2] = edge;

            for (int i = 1; i < N; i++)
            {
                var p1 = p2;
                p2 = Scale(edge.Points[i]);
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

        internal HashSet<Node> NearByNodes((int x, int y) p, int n = 5)
        {
            var (x, y) = Scale(p);
            return NearByNodes(x, y, n);
        }
        private HashSet<Node> NearByNodes(int x, int y, int n)
        {
            HashSet<Node> list = null;

            var M = 2;  //the number of test to preform on each face of the tier

            Test(x, y); //test the reference point
            for (int i = 0; i < n; i++)
            {
               //spiral arround with an increasing radius
               Test(++x, ++y); //south-east corner of next tier
                for (int j = 0; j < M; j++) { Test(--x, y); } //south face of tier heading west
                for (int j = 0; j < M; j++) { Test(x, --y); } // west face of tier heading north
                for (int j = 0; j < M; j++) { Test(++x, y); } //north face of tier heading east
                for (int j = 0; j < M; j++) { Test(x, ++y); } // east face of tier heading south
                M += 2;
            }
            return list;

            void Test(int u, int v)
            {
                if (!Point_Node.TryGetValue((u, v), out Node node)) return;

                if (list is null) list = new HashSet<Node>();
                list.Add(node);
            }
        }
        internal HashSet<Edge> NearByEdges((int x, int y) p, int n = 5)
        {
            var (x, y) = Scale(p);
            return NearByEdges(x, y, n);
        }
        private HashSet<Edge> NearByEdges(int x, int y, int n)
        {
            HashSet<Edge> list = null;

            var M = 2;  //the number of test to preform on each face of the tier

            Test(x, y); //test the reference point
            for (int i = 0; i < n; i++)
            {
                //spiral arround with an increasing radius
                Test(++x, ++y); //south-east corner of next tier
                for (int j = 0; j < M; j++) { Test(--x, y); } //south face of tier heading west
                for (int j = 0; j < M; j++) { Test(x, --y); } // west face of tier heading north
                for (int j = 0; j < M; j++) { Test(++x, y); } //north face of tier heading east
                for (int j = 0; j < M; j++) { Test(x, ++y); } // east face of tier heading south
                M += 2;
            }
            return list;

            void Test(int u, int v)
            {
                if (!Point_Edge.TryGetValue((u, v), out Edge edge)) return;

                if (list is null) list = new HashSet<Edge>();
                list.Add(edge);
            }
        }
    }
}
