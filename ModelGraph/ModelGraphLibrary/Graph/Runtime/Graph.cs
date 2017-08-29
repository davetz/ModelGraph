using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public partial class Graph : Item
    {
        internal Item RootItem;    // specified root of graph (if any)
        internal Query[] Forest;   // roots of the query forest
        internal SymbolX[] Symbols; // referenced by (byte)Node.NodeCore.Symbol

        internal List<Node> Nodes = new List<Node>();
        internal List<Edge> Edges = new List<Edge>();
        internal List<Path> Paths = new List<Path>();
        internal List<Level> Levels = new List<Level>();
        internal List<QueryPair> PathQuerys = new List<QueryPair>();  // completed path query sequences
        internal List<QueryPair> OpenQuerys = new List<QueryPair>();  // incomplete path query sequences
        internal List<QueryPair> GroupQuerys = new List<QueryPair>(); // completed group query sequences
        internal List<QueryPair> SegueQuerys = new List<QueryPair>(); // completed segue query sequences

        internal HashSet<Item> NodeItems = new HashSet<Item>(); // hash of Node.Item for all nodes
        internal Dictionary<Node, List<Edge>> Node_Edges = new Dictionary<Node, List<Edge>>(); // list of edges for each node
        internal Dictionary<Item, Node> Item_Node = new Dictionary<Item, Node>();              // look up item -> node

        internal GroupColor[] GroupColors;
        internal Dictionary<Item, int> Item_ColorIndex;
        internal Dictionary<Item, int> Group_ColorIndex;

        internal int MinorDelta = 1; // incremented each time am item property changes
        internal int MajorDelta = 1; // incremented each time an item collection changes

        internal Extent Extent;  // current x,y extent of this graph

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
        internal void Add(Path path) { Paths.Add(path); }
        internal void Add(Level level) { Levels.Add(level); }

        internal GraphX GraphX { get { return Owner as GraphX; } }
        internal int QueryCount { get { return (Forest == null) ? 0 : Forest.Length; } }
        internal int OpenPathCount(int index)
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
        internal int Count => Levels.Count;

        internal bool TryGetTopLevel(out Level lvl) { lvl = null; if (Count == 0) return false; lvl = Levels[Count - 1] as Level; return true; }

        internal string Name => GraphX.Name;

        internal int NodeCount { get { return (Nodes == null) ? 0 : Nodes.Count; } }
        internal int EdgeCount { get { return (Edges == null) ? 0 : Edges.Count; } }
        internal int SymbolCount { get { return (Symbols == null) ? 0 : Symbols.Length; } }

        internal void Reset()
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
        internal void RefreshGraphPoints()
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
    internal struct GroupColor
    {
        internal byte A;
        internal byte R;
        internal byte G;
        internal byte B;
        internal GroupColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
    internal struct QueryPair
    {
        internal Query Query1;
        internal Query Query2;
        internal QueryPair(Query query1, Query query2)
        {
            Query1 = query1;
            Query2 = query2;
        }
    }
    #endregion
}
