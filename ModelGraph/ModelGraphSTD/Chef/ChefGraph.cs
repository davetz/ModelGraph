using System;
using System.Linq;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private Dictionary<GraphX, Dictionary<Item, Dictionary<Item, List<Item>>>> _graphParms =
            new Dictionary<GraphX, Dictionary<Item, Dictionary<Item, List<Item>>>>();
        private void InitializeGraphParams() { _graphParms.Clear(); }

        internal Dictionary<GraphX, Dictionary<Item, Dictionary<Item, List<Item>>>> T_GraphParms
        {
            get { return _graphParms; }
            set { _graphParms = value; }
        }
        
        #region ValidateGraphParms  ===========================================
        // Ensure all edges and nodes have parameters.
        bool ValidateGraphParms(Graph g)
        {
            var gx = g.GraphX;
            var rt = (g.RootItem == null) ? _dummy : (Item)g.RootItem;
            var anyChange = false;

            #region Build validPathPairs dictionary  ==========================

            List<ItemPair> validItemPair = null;
            var validPathPairs = new Dictionary<Item, List<ItemPair>>();

            foreach (var item in g.PathQuerys)
            {
                var sd = item.Query1.QueryX;
                if (!validPathPairs.TryGetValue(sd, out validItemPair))
                {
                    validItemPair = new List<ItemPair>(g.PathQuerys.Count);
                    validPathPairs.Add(sd, validItemPair);
                }

                g.NodeItems.Add(item.Query1.Item);

                for (int i = 0; i < item.Query2.ItemCount; i++)
                {
                    g.NodeItems.Add(item.Query2.Items[i]);

                    validItemPair.Add(new ItemPair(item.Query1.Item, item.Query2.Items[i]));
                }
            }
            #endregion

            if (g.NodeItems.Count > 0)
            {

                if (!_graphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<Item, List<Item>>> rtSdParams))
                {
                    rtSdParams = new Dictionary<Item, Dictionary<Item, List<Item>>>();
                    _graphParms.Add(gx, rtSdParams);
                }
                if (!rtSdParams.TryGetValue(rt, out Dictionary<Item, List<Item>> sdParams))
                {
                    sdParams = new Dictionary<Item, List<Item>>();
                    rtSdParams.Add(rt, sdParams);
                }

                #region Remove invalid SdParams  ==============================

                var invalidSd = new HashSet<Item>();
                var invalidSdParam = new Dictionary<Item, List<Item>>();

                foreach (var e1 in sdParams)
                {
                    List<Item> invalidParams = null;
                    if (e1.Key == _dummy)
                    {

                        foreach (var pm in e1.Value)
                        {
                            var nd = pm as Node;
                            if (!g.NodeItems.Contains(nd.Item))
                            {
                                if (invalidParams == null)
                                {
                                    invalidParams = new List<Item>();
                                    invalidSdParam.Add(e1.Key, invalidParams);
                                }
                                invalidParams.Add(pm);
                            }
                        }
                    }
                    else
                    {
                        foreach (var pm in e1.Value)
                        {
                            var eg = pm as Edge;
                            //var id = $"({GetIdentity(eg.Node1.Item, IdentityStyle.Double)})  -->  ({GetIdentity(eg.Node2.Item, IdentityStyle.Double)})";

                            if (!g.NodeItems.Contains(eg.Node1.Item) || !g.NodeItems.Contains(eg.Node2.Item))
                            {
                                if (invalidParams == null)
                                {
                                    invalidParams = new List<Item>();
                                    invalidSdParam.Add(e1.Key, invalidParams);
                                }
                                invalidParams.Add(pm);
                            }
                            else
                            {
                                validItemPair = null;
                                if (validPathPairs.TryGetValue(eg.QueryX, out validItemPair))
                                {
                                    bool found = false;
                                    foreach (var tp in validItemPair)
                                    {
                                        if (tp.Item1 != eg.Node1.Item || tp.Item2 != eg.Node2.Item) continue;
                                        found = true;
                                        break;
                                    }
                                    if (!found)
                                    {
                                        if (invalidParams == null)
                                        {
                                            invalidParams = new List<Item>();
                                            invalidSdParam.Add(e1.Key, invalidParams);
                                        }
                                        invalidParams.Add(pm);
                                    }
                                }
                                else
                                {
                                    invalidSd.Add(e1.Key);
                                }
                            }
                        }
                    }
                }

                // remove the invalid graphic params 
                foreach (var sd in invalidSd) { sdParams.Remove(sd); }
                foreach (var e1 in invalidSdParam)
                {
                    foreach (var pm in e1.Value)
                    {
                        sdParams[e1.Key].Remove(pm);
                    }
                    if (sdParams[e1.Key].Count == 0) sdParams.Remove(e1.Key);
                }

                #endregion
                #region Add new SdParams  =====================================
                #endregion

                #region Add new SdParams  =====================================

                if (!sdParams.TryGetValue(_dummy, out List<Item> parmList))
                {
                    // there weren't any existing node parms,
                    // so create all new ones
                    anyChange = true;
                    parmList = new List<Item>(g.NodeItems.Count);
                    sdParams.Add(_dummy, parmList);
                    foreach (var item in g.NodeItems)
                    {
                        var node = new Node();
                        node.Item = item;
                        node.Owner = g;
                        parmList.Add(node);
                        g.Nodes.Add(node);
                        g.Item_Node.Add(item, node);
                    }
                }
                else
                {
                    // validate the existing nodes
                    foreach (var pm in parmList)
                    {
                        var node = pm as Node;
                        node.Owner = g;
                        g.Nodes.Add(node);
                        g.Item_Node.Add(node.Item, node);
                    }
                    // add new nodes that where missing
                    foreach (var item in g.NodeItems)
                    {
                        if (!g.Item_Node.ContainsKey(item))
                        {
                            anyChange = true;
                            var node = new Node();
                            node.Item = item;
                            node.Owner = g;
                            parmList.Add(node);
                            g.Nodes.Add(node);
                            g.Item_Node.Add(item, node);
                        }
                    }
                }

                foreach (var e1 in validPathPairs)
                {
                    // skip over the nodes, they are already done
                    if (e1.Key == _dummy) continue;

                    if (!sdParams.TryGetValue(e1.Key, out List<Item> paramList))
                    {
                        // there weren't any existing edge parms,
                        // so create all new ones
                        paramList = new List<Item>(e1.Value.Count);
                        sdParams.Add(e1.Key, paramList);
                        anyChange = true;

                        foreach (var pair in e1.Value)
                        {
                            var eg = new Edge(e1.Key);
                            eg.Owner = g;
                            g.Edges.Add(eg);
                            paramList.Add(eg);
                            eg.Node1 = g.Item_Node[pair.Item1];
                            eg.Node2 = g.Item_Node[pair.Item2];
                        }
                    }
                    else
                    {
                        // validate the existing edges
                        List<Item> items;
                        var item_items = new Dictionary<Item, List<Item>>();
                        foreach (var pm in paramList)
                        {
                            var eg = pm as Edge;
                            eg.Owner = g;
                            g.Edges.Add(eg);

                            if (!item_items.TryGetValue(eg.Node1.Item, out items))
                            {
                                items = new List<Item>(4);
                                item_items.Add(eg.Node1.Item, items);
                            }
                            items.Add(eg.Node2.Item);
                        }
                        // add new edges that where missing
                        foreach (var pair in e1.Value)
                        {
                            if (item_items.TryGetValue(pair.Item1, out items) && items.Contains(pair.Item2)) continue;

                            anyChange = true;
                            var eg = new Edge(e1.Key);
                            eg.Owner = g;
                            g.Edges.Add(eg);
                            paramList.Add(eg);
                            eg.Node1 = g.Item_Node[pair.Item1];
                            eg.Node2 = g.Item_Node[pair.Item2];
                        }
                    }
                }

                #endregion
                #region populate g.Node_Edges  ================================
                #endregion

                #region populate g.Node_Edges  ================================

                foreach (var edge in g.Edges)
                {
                    if (!g.Node_Edges.TryGetValue(edge.Node1, out List<Edge> edgeList))
                    {
                        edgeList = new List<Edge>(2);
                        g.Node_Edges.Add(edge.Node1, edgeList);
                    }
                    edgeList.Add(edge);

                    if (!g.Node_Edges.TryGetValue(edge.Node2, out edgeList))
                    {
                        edgeList = new List<Edge>(2);
                        g.Node_Edges.Add(edge.Node2, edgeList);
                    }
                    edgeList.Add(edge);
                }
                #endregion
            }
            else
            {
                if (rt == _dummy) _graphParms.Remove(gx);
                else if (_graphParms.ContainsKey(gx)) _graphParms[gx].Remove(rt);
            }

            #region OpenPathIndex  ============================================
            var N = g.OpenQuerys.Count;
            for (int i = 0; i < N;)
            {
                var item = g.OpenQuerys[i].Query1.Item;
                var j = i + 1;
                for (; j < N; j++) { if (g.OpenQuerys[j].Query1.Item != item) break; }
                if (g.Item_Node.TryGetValue(item, out Node node)) { node.OpenPathIndex = i; }
                i = j;
            }
            #endregion

            #region AssignSymbolIndex  ========================================
            var symbols = g.Symbols = GraphX_SymbolX.GetChildren(gx);
            if (symbols == null)
            {
                g.Symbols = new SymbolX[0];
            }
            else
            {
                var storeNonSymbols = new HashSet<Store>();
                var storeSymbolXQueryX = new Dictionary<Store, (SymbolX[], QueryX[])>();
                foreach (var node in g.Nodes)
                {
                    var sto = node.Item.Store;
                    if (storeNonSymbols.Contains(sto)) continue;
                    if (storeSymbolXQueryX.ContainsKey(sto)) continue;

                    var symseg = GetSymbolXQueryX(gx, sto);
                    if (symseg.symbols == null)
                        storeNonSymbols.Add(sto);
                    else
                        storeSymbolXQueryX.Add(sto, symseg);
                }

                foreach (var node in g.Nodes)
                {
                    node.Core.Symbol = 0;
                    var row = node.Item;
                    var sto = row.Store;
                    if (storeNonSymbols.Contains(sto)) continue;

                    (var syms, var segs) = storeSymbolXQueryX[sto];

                    int i, j;
                    for (j = 0; j < syms.Length; j++)
                    {
                        var filter = segs[j].Where;
                        if (filter == null || filter.Matches(row)) break;
                    }
                    if (j == syms.Length) continue;

                    var sym = syms[j];
                    for (i = 0; i < symbols.Length; i++)
                    {
                        if (symbols[i] == sym) break;
                    }
                    if (i == symbols.Length) continue;

                    node.Core.Symbol = (byte)(i + 2);
                    node.Core.Orientation = Orientation.Central;
                    if ((node.Core.FlipRotate & FlipRotate.RotateClockWise) == 0)
                    {
                        node.Core.DX = (byte)(sym.Width / 2);
                        node.Core.DY = (byte)(sym.Height / 2);
                    }
                    else
                    {
                        node.Core.DY = (byte)(sym.Width / 2);
                        node.Core.DX = (byte)(sym.Height / 2);
                    }
                }
            }
            #endregion

            return anyChange;
        }
        #endregion

        #region CreateGraph  ==================================================
        private bool CreateGraph(GraphX gd, out Graph graph, Item root = null)
        {
            if (!gd.TryGetGraph(root, out graph))
                graph = new Graph(gd, null, root);

            RefreshGraph(graph);

            return true;
        }
        #endregion

        #region RefreshGraph  =================================================

        private void RefreshAllGraphs()
        {
            foreach (var gx in _graphXStore.ToArray)
            {
                if (gx.Count > 0) { foreach (var g in gx.ToArray) { RefreshGraph(g); } }
            }
        }

        private void RefreshGraph(Graph g)
        {
            var gx = g.GraphX;
            var rt = g.RootItem;
            var nodeOwners = GetNodeOwners(gx);

            g.Reset();
            g.Forest = GetForest(g, rt, nodeOwners);
            var anyChange = ValidateGraphParms(g);

            TryGetColorCriteria(g);
            TryCreateQueryPaths(g);
            while (TryPathReduction(g)) { }

            if (anyChange) g.CheckLayout();
            g.RefreshGraphPoints();
        }
        #endregion

        #region TryGetColorCriteria  ==========================================
        private bool TryGetColorCriteria(Graph g)
        {

            if (GraphX_ColorColumnX.TryGetChild(g.GraphX, out ColumnX col) && TableX_ColumnX.TryGetParent(col, out TableX tbl))
            {
                var N = tbl.Count;
                if (N > 0)
                {
                    var items = tbl.ToArray;
                    var colors = new List<GroupColor>(N);
                    g.Group_ColorIndex = new Dictionary<Item, int>(N);

                    for (int i = 0; i < N; i++)
                    {
                        var key = items[i];
                        var argb = GetGroupColor(col.Value.GetString(key));
                        int j = 0;
                        for (; j < colors.Count; j++)
                        {
                            if (colors[j].A != argb.A) continue;
                            if (colors[j].R != argb.R) continue;
                            if (colors[j].G != argb.G) continue;
                            if (colors[j].B != argb.B) continue;
                            break;
                        }
                        if (j == colors.Count) colors.Add(argb);
                        g.Group_ColorIndex.Add(key, j);
                    }
                    g.GroupColors = colors.ToArray();
                    g.Item_ColorIndex = new Dictionary<Item, int>(g.NodeItems.Count);
                    foreach (var pair in g.GroupQuerys)
                    {
                        var grp = pair.Query2.Items[0];
                        if (g.Group_ColorIndex.TryGetValue(grp, out int inx))
                        {
                            var itm = pair.Query1.Item;
                            g.Item_ColorIndex.Add(itm, inx);
                        }
                    }
                    return true;
                }
            }
            g.Group_ColorIndex = null;
            g.Item_ColorIndex = null;
            g.GroupColors = null;
            return false;
        }
        private void SetDefaultGroupColor(Graph g, int i)
        {
            g.GroupColors[i].A = 0xFF;
            g.GroupColors[i].R = 0x80;
            g.GroupColors[i].G = 0x80;
            g.GroupColors[i].B = 0x70;
        }
        private static string _hexValues = "0123456789abcdef";
        private GroupColor GetGroupColor(string color)
        {
            const int N = 9;
            var argb = new GroupColor(0xFF, 0x80, 0x80, 0x70);
            if (!string.IsNullOrWhiteSpace(color) && color.Length == N)
            {
                var ca = color.ToLower().ToCharArray();
                if (ca[0] == '#')
                {
                    int[] va = new int[N];
                    for (int j = 1; j < N; j++)
                    {
                        va[j] = _hexValues.IndexOf(ca[j]);
                        if (va[j] < 0) return argb;
                    }
                    argb.A = (byte)((va[1] << 4) | va[2]);
                    argb.R = (byte)((va[3] << 4) | va[4]);
                    argb.G = (byte)((va[5] << 4) | va[6]);
                    argb.B = (byte)((va[7] << 4) | va[8]);
                }
            }
            return argb;
        }
        #endregion

        #region GetNodeOwners  ================================================
        private HashSet<Store> GetNodeOwners(GraphX gx)
        {
            var nodeOwners = new HashSet<Store>();

            var sxChildren = GraphX_QueryX.GetChildren(gx);
            if (sxChildren != null)
            {
                var workQueue = new Queue<QueryX>(sxChildren);
                while (workQueue.Count > 0)
                {
                    var qx = workQueue.Dequeue();
                    if (qx.IsHead && qx.QueryKind == QueryType.Path)
                    {
                        GetHeadTail(qx, out Store head, out Store tail);
                        {
                            nodeOwners.Add(head);

                            var cx = qx;
                            var tx = cx;
                            while (cx != null) { tx = cx; cx = QueryX_QueryX.GetChild(cx); }
                            if (tx != qx) GetHeadTail(tx, out head, out tail);

                            nodeOwners.Add(tail);
                        }
                    }
                    else if ((sxChildren = QueryX_QueryX.GetChildren(qx)) != null)
                    {
                        foreach (var cx in sxChildren) 
                        {
                            switch (cx.QueryKind)
                            {
                                case QueryType.Path:
                                case QueryType.Graph:
                                    workQueue.Enqueue(cx);
                                    break;
                            }
                        }
                    }
                }
            }
            return nodeOwners;
        }
        #endregion
    }
}
