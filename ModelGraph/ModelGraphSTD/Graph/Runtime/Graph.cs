using System.Collections.Generic;
using System.Diagnostics;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph : Item
    {
        public Item SeedItem;    // seed of query forest 
        public Query[] Forest;   // roots of the query forest
        public SymbolX[][] Symbols; // referenced by [Node.Symbol][Node.FlipRotate]

        public List<Node> Nodes = new List<Node>();
        public List<Edge> Edges = new List<Edge>();
        public List<Path> Paths = new List<Path>();
        public List<Level> Levels = new List<Level>();
        internal List<(Query, Query)> PathQuerys = new List<(Query, Query)>();  // completed path query sequences
        internal List<(Query, Query)> OpenQuerys = new List<(Query, Query)>();  // incomplete path query sequences
        internal List<(Query, Query)> GroupQuerys = new List<(Query, Query)>(); // completed group query sequences
        internal List<(Query, Query)> SegueQuerys = new List<(Query, Query)>(); // completed segue query sequences

        public HashSet<Item> NodeItems = new HashSet<Item>(); // hash of Node.Item for all nodes
        public Dictionary<Node, List<Edge>> Node_Edges = new Dictionary<Node, List<Edge>>(); // list of edges for each node
        public Dictionary<Item, Node> Item_Node = new Dictionary<Item, Node>();              // look up item -> node

        public List<(byte A, byte R, byte G, byte B)> ARGBList => GraphX.ARGBList;

        public Extent Extent;  // current x,y extent of this graph

        #region Constructor  ==================================================
        internal Graph(GraphX owner, Item seedItem = null)
        {
            Owner = owner;
            SeedItem = seedItem;
            Trait = Trait.Graph;

            owner.Add(this);
        }
        public GraphX GraphX => Owner as GraphX;
        #endregion

        #region Properties/Methods  ===========================================
        internal (int count, Edge[] edges) ConnectedEdges(Node n) => Node_Edges.TryGetValue(n, out List<Edge> list) ? (list.Count, list.ToArray()) : (0, null);

        public void Add(Path path) { Paths.Add(path); }
        public void Add(Level level) { Levels.Add(level); }

        public int QueryCount => (Forest == null) ? 0 : Forest.Length;
        public int OpenPathCount(int index)
        {
            if (index < 0 || index > OpenQuerys.Count) return 0;
            var head = OpenQuerys[index].Item1.Item;
            var count = 1;
            for (var i = index + 1; i < OpenQuerys.Count; i++)
            {
                if (OpenQuerys[index].Item1.Item != head) return count;
                count += 1;
            }
            return count;
        }
        public int Count => Levels.Count;
        private int Last => Count - 1;

        public bool TryGetTopLevel(out Level lvl) { lvl = null; if (Count == 0) return false; lvl = Levels[Last] as Level; return true; }

        public string Name => GraphX.Name;

        public int NodeCount { get { return (Nodes == null) ? 0 : Nodes.Count; } }
        public int EdgeCount { get { return (Edges == null) ? 0 : Edges.Count; } }
        public int SymbolCount { get { return (Symbols == null) ? 0 : Symbols.Length; } }

        internal Attach GetAttach(Node node)
        {
            var n = node.Symbol - 2;
            if (n < 0 || n > Symbols.Length) return Attach.Normal;

            return Symbols[n].Attach;
        }

        public void Reset()
        {
            Forest = null;
            Symbols = null;

            Nodes.Clear();
            Edges.Clear();
            Paths.Clear();
            Levels.Clear();
            NodeItems.Clear();
            PathQuerys.Clear();
            OpenQuerys.Clear();
            GroupQuerys.Clear();
            SegueQuerys.Clear();

            NodeItems.Clear();
            Item_Node.Clear();
            Node_Edges.Clear();
        }
        #endregion

        #region (GraphRef -> RootModel) Interface  ============================
        private void SetExtent()
        {
            Extent = new Extent();
            Extent = Extent.SetExtent(Nodes, 16);
        }
        #endregion
    }
}