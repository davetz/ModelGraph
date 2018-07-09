using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Graph : Item
    {
        public Item SeedItem;    // seed of graph forest 
        public Query[] Forest;   // roots of the query forest
        public SymbolX[] Symbols; // referenced by (byte)Node.NodeCore.Symbol

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

        public List<GroupColor> GroupColors;
        public Dictionary<Item, int> Item_ColorIndex;
        public Dictionary<Item, int> Group_ColorIndex;

        public Extent Extent;  // current x,y extent of this graph

        public ushort MinorDelta;      // increments whenever node/edge property changes
        public ushort MajorDelta;      // increments whenever the graph changes

        #region Constructor  ==================================================
        internal Graph(GraphX owner)
        {
            Owner = owner;
            Trait = Trait.Graph;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        public void Add(Path path) { Paths.Add(path); }
        public void Add(Level level) { Levels.Add(level); }

        public GraphX GraphX => Owner as GraphX;
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

            GroupColors = null;
            Item_ColorIndex = null;
            Group_ColorIndex = null;
        }

        #endregion

        #region (GraphRef -> RootModel) Interface  ============================
        private HashSet<RootModel> _rootModels = new HashSet<RootModel>();

        internal void AddRootModel(RootModel root)
        {
            _rootModels.Add(root);
        }
        internal void RemoveRootModel(RootModel root)
        {
            _rootModels.Remove(root);
            if (_rootModels.Count == 0)
            {
                GraphX.Remove(this);
            }
        }
        public void RefreshGraphPoints()
        {
            SetExtent();
            foreach (var edge in Edges) { edge.Refresh(); }
        }
        private void SetExtent()
        {
            Extent = new Extent(GraphParm.CenterOffset);
            Extent = Extent.SetExtent(Nodes, 16);
        }
        #endregion
    }

    #region RelatedStructures  ================================================
    public struct GroupColor
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;
        public GroupColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
    public struct QueryPair
    {
        public Query Query1;
        public Query Query2;
        public QueryPair(Query query1, Query query2)
        {
            Query1 = query1;
            Query2 = query2;
        }
    }
    #endregion
}
