using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public partial class Graph : Item
    {
        public Item RootItem;    // specified root of graph (if any)
        public Query[] Forest;   // roots of the query forest
        public SymbolX[] Symbols; // referenced by (byte)Node.NodeCore.Symbol

        public List<Node> Nodes = new List<Node>();
        public List<Edge> Edges = new List<Edge>();
        public List<Path> Paths = new List<Path>();
        public List<Level> Levels = new List<Level>();
        internal List<QueryPair> PathQuerys = new List<QueryPair>();  // completed path query sequences
        internal List<QueryPair> OpenQuerys = new List<QueryPair>();  // incomplete path query sequences
        internal List<QueryPair> GroupQuerys = new List<QueryPair>(); // completed group query sequences
        internal List<QueryPair> SegueQuerys = new List<QueryPair>(); // completed segue query sequences

        public HashSet<Item> NodeItems = new HashSet<Item>(); // hash of Node.Item for all nodes
        public Dictionary<Node, List<Edge>> Node_Edges = new Dictionary<Node, List<Edge>>(); // list of edges for each node
        public Dictionary<Item, Node> Item_Node = new Dictionary<Item, Node>();              // look up item -> node

        public GroupColor[] GroupColors;
        public Dictionary<Item, int> Item_ColorIndex;
        public Dictionary<Item, int> Group_ColorIndex;

        public int MinorDelta = 1; // incremented each time am item property changes
        public int MajorDelta = 1; // incremented each time an item collection changes

        public Extent Extent;  // current x,y extent of this graph

        #region Constructor  ==================================================
        internal Graph(GraphX owner, Query[] querys, Item rootItem = null)
        {
            Owner = owner;
            Trait = Trait.Graph;
            RootItem = rootItem;
            Forest = querys;

            owner.Append(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        public void Add(Path path) { Paths.Add(path); }
        public void Add(Level level) { Levels.Add(level); }

        public GraphX GraphX { get { return Owner as GraphX; } }
        public int QueryCount { get { return (Forest == null) ? 0 : Forest.Length; } }
        public int OpenPathCount(int index)
        {
            if (index < 0 || index > OpenQuerys.Count) return 0;
            var head = OpenQuerys[index].Query1.Item;
            var count = 1;
            for (var i = index + 1; i < OpenQuerys.Count; i++)
            {
                if (OpenQuerys[index].Query1.Item != head) return count;
                count += 1;
            }
            return count;
        }
        public int Count => Levels.Count;

        public bool TryGetTopLevel(out Level lvl) { lvl = null; if (Count == 0) return false; lvl = Levels[Count - 1] as Level; return true; }

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
