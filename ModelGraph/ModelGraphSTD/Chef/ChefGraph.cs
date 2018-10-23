using System.Collections.Generic;
using System.Linq;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private void InitializeGraphParams() { GraphParms.Clear(); }

        internal Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>> GraphParms { get; set; } = new Dictionary<GraphX, Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>>();

        #region ValidateGraphParms  ===========================================
        // Ensure all edges and nodes have parameters.
        bool ValidateGraphParms(Graph g)
        {
            var gx = g.GraphX;
            var rt = (g.SeedItem == null) ? Dummy : g.SeedItem;
            var anyChange = false;

            #region Build validPathPairs dictionary  ==========================

            List<(Item, Item)> validItemPair = null;
            var validPathPairs = new Dictionary<QueryX, List<(Item, Item)>>();

            foreach (var (q1, q2) in g.PathQuerys)
            {
                var qx = q1.QueryX;
                if (!validPathPairs.TryGetValue(qx, out validItemPair))
                {
                    validItemPair = new List<(Item, Item)>(g.PathQuerys.Count);
                    validPathPairs.Add(qx, validItemPair);
                }

                g.NodeItems.Add(q1.Item);

                for (int i = 0; i < q2.ItemCount; i++)
                {
                    g.NodeItems.Add(q2.Items[i]);

                    validItemPair.Add((q1.Item, q2.Items[i]));
                }
            }
            #endregion

            if (g.NodeItems.Count > 0)
            {

                if (!GraphParms.TryGetValue(gx, out Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>> rtQxParams))
                {
                    rtQxParams = new Dictionary<Item, Dictionary<QueryX, List<NodeEdge>>>();
                    GraphParms.Add(gx, rtQxParams);
                }
                if (!rtQxParams.TryGetValue(rt, out Dictionary<QueryX, List<NodeEdge>> qxParams))
                {
                    qxParams = new Dictionary<QueryX, List<NodeEdge>>();
                    rtQxParams.Add(rt, qxParams);
                }

                #region Remove invalid QxParams  ==============================

                var invalidQx = new HashSet<QueryX>();
                var invalidQxParam = new Dictionary<QueryX, List<NodeEdge>>();

                foreach (var e1 in qxParams)
                {
                    List<NodeEdge> invalidParams = null;
                    if (e1.Key == QueryXNode)
                    {

                        foreach (var pm in e1.Value)
                        {
                            var nd = pm as Node;
                            if (!g.NodeItems.Contains(nd.Item))
                            {
                                if (invalidParams == null)
                                {
                                    invalidParams = new List<NodeEdge>();
                                    invalidQxParam.Add(e1.Key, invalidParams);
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
                                    invalidParams = new List<NodeEdge>();
                                    invalidQxParam.Add(e1.Key, invalidParams);
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
                                            invalidParams = new List<NodeEdge>();
                                            invalidQxParam.Add(e1.Key, invalidParams);
                                        }
                                        invalidParams.Add(pm);
                                    }
                                }
                                else
                                {
                                    invalidQx.Add(e1.Key);
                                }
                            }
                        }
                    }
                }

                // remove the invalid graphic params 
                foreach (var qx in invalidQx) { qxParams.Remove(qx); }
                foreach (var e1 in invalidQxParam)
                {
                    foreach (var pm in e1.Value)
                    {
                        if (qxParams.ContainsKey(e1.Key)) qxParams[e1.Key].Remove(pm);
                    }
                    if (qxParams[e1.Key].Count == 0) qxParams.Remove(e1.Key);
                }
                #endregion

                #region Add new QxParams  =====================================

                if (!qxParams.TryGetValue(QueryXNode, out List<NodeEdge> parmList))
                {
                    // there weren't any existing node parms,
                    // so create all new ones
                    anyChange = true;
                    parmList = new List<NodeEdge>(g.NodeItems.Count);
                    qxParams.Add(QueryXNode, parmList);
                    foreach (var item in g.NodeItems)
                    {
                        Node node1 = new Node
                        {
                            Item = item,
                            Owner = g
                        };
                        var node = node1;
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
                            var node = new Node
                            {
                                Item = item,
                                Owner = g
                            };
                            parmList.Add(node);
                            g.Nodes.Add(node);
                            g.Item_Node.Add(item, node);
                        }
                    }
                }

                foreach (var e1 in validPathPairs)
                {
                    // skip over the nodes, they are already done
                    if (e1.Key == QueryXNode) continue;

                    if (!qxParams.TryGetValue(e1.Key, out List<NodeEdge> paramList))
                    {
                        // there weren't any existing edge parms,
                        // so create all new ones
                        paramList = new List<NodeEdge>(e1.Value.Count);
                        qxParams.Add(e1.Key, paramList);
                        anyChange = true;

                        foreach (var pair in e1.Value)
                        {
                            var eg = new Edge(e1.Key)
                            {
                                Owner = g,
                            };
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
                            var eg = new Edge(e1.Key)
                            {
                                Owner = g,
                            };
                            g.Edges.Add(eg);
                            paramList.Add(eg);
                            eg.Node1 = g.Item_Node[pair.Item1];
                            eg.Node2 = g.Item_Node[pair.Item2];
                        }
                    }
                }

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
                if (rt == Dummy) GraphParms.Remove(gx);
                else if (GraphParms.ContainsKey(gx)) GraphParms[gx].Remove(rt);
            }

            #region OpenPathIndex  ============================================
            var N = g.OpenQuerys.Count;
            for (int i = 0; i < N;)
            {
                var item = g.OpenQuerys[i].Item1.Item;
                var j = i + 1;
                for (; j < N; j++) { if (g.OpenQuerys[j].Item1.Item != item) break; }
                if (g.Item_Node.TryGetValue(item, out Node node)) { node.OpenPathIndex = i; }
                i = j;
            }
            #endregion

            #region AssignSymbolIndex  ========================================
            
            if (GraphX_SymbolX.TryGetChildren(gx, out IList<SymbolX> symbols))
            {
                g.Symbols = symbols.ToArray();

                var storeNonSymbols = new HashSet<Store>();
                var storeSymbolXQueryX = new Dictionary<Store, (List<SymbolX>, List<QueryX>)>();
                foreach (var node in g.Nodes)
                {
                    var sto = node.Item.Store;
                    if (storeNonSymbols.Contains(sto)) continue;
                    if (storeSymbolXQueryX.ContainsKey(sto)) continue;

                    var sxqx = GetSymbolXQueryX(gx, sto);
                    if (sxqx.symbols == null)
                        storeNonSymbols.Add(sto);
                    else
                        storeSymbolXQueryX.Add(sto, sxqx);
                }

                foreach (var node in g.Nodes)
                {
                    node.Symbol = 0;
                    var row = node.Item;
                    var sto = row.Store;
                    if (storeNonSymbols.Contains(sto)) continue;

                    (var sxList, var qxList) = storeSymbolXQueryX[sto];

                    int i, j;
                    for (j = 0; j < sxList.Count; j++)
                    {
                        var filter = qxList[j].Where;
                        if (filter == null || filter.Matches(row)) break;
                    }
                    if (j == sxList.Count) continue;

                    var sx = sxList[j];
                    for (i = 0; i < symbols.Count; i++)
                    {
                        if (symbols[i] == sx) break;
                    }
                    if (i == symbols.Count) continue;

                    node.Symbol = (byte)(i + 2);
                    node.Aspect = Aspect.Central;
                    if ((node.FlipRotate & FlipRotate.RotateClockWise) == 0)
                    {
                        node.DX = (byte)(sx.Width / 2);
                        node.DY = (byte)(sx.Height / 2);
                    }
                    else
                    {
                        node.DY = (byte)(sx.Width / 2);
                        node.DX = (byte)(sx.Height / 2);
                    }
                }
            }
            #endregion

            return anyChange;
        }
        #endregion

        #region CreateGraph  ==================================================
        private bool CreateGraph(GraphX gd, out Graph graph, Item seed = null)
        {
            if (!gd.TryGetGraph(seed, out graph))
                graph = new Graph(gd, seed);

            RefreshGraph(graph);

            return true;
        }
        #endregion

        #region RefreshGraph  =================================================

        private void RefreshAllGraphs()
        {
            foreach (var gx in GraphXStore.Items)
            {
                if (gx.Count > 0)
                {
                    foreach (var g in gx.Items) { RefreshGraph(g); }
                }
            }
        }

        private bool RefreshGraphX(QueryX qx)
        {
            var qr = qx;
            while (QueryX_QueryX.TryGetParent(qr, out qx)) { qr = qx; }

            if (GraphX_QueryX.TryGetParent(qr, out GraphX gx))
                RefreshGraphX(gx);

            return true;
        }
        private void RefreshGraphX(GraphX gx)
        {
            #region Rebuild ARGBList, NodeOwners  =========================
            gx.Color.Reset();
            gx.NodeOwners.Clear();

            if (GraphX_ColorColumnX.TryGetChild(gx, out ColumnX cx) && TableX_ColumnX.TryGetParent(cx, out TableX tx) && tx.Count > 0)
            {
                foreach (var item in tx.Items)
                {
                    gx.Color.BuildARGBList(cx.Value.GetString(item));
                }
            }

            if (GraphX_QueryX.TryGetChildren(gx, out IList<QueryX> qxList))
            {
                var workQueue = new Queue<QueryX>(qxList);
                while (workQueue.Count > 0)
                {
                    var qx = workQueue.Dequeue();
                    if (qx.PathParm != null)
                    {
                        gx.Color.BuildARGBList(qx.PathParm.LineColor);
                    }
                    if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> qcList))
                    {
                        foreach (var qc in qcList)
                        {
                            workQueue.Enqueue(qc);
                        }
                    }
                    if (qx.QueryKind == QueryType.Path && qx.IsHead)
                    {
                        GetHeadTail(qx, out Store head, out Store t);
                        gx.NodeOwners.Add(head);

                        var qt = qx;
                        while (QueryX_QueryX.TryGetChild(qt, out QueryX qn)) { qt = qn; }

                        GetHeadTail(qx, out Store h, out Store tail);
                        gx.NodeOwners.Add(tail);
                    }

                }
                #endregion

                foreach (var g in gx.Items)
                {
                    RefreshGraph(g);
                }
            }
        }

        private void RefreshGraph(Graph g)
        {
            var gx = g.GraphX;
            var rt = g.SeedItem;

            g.Reset();
            TryGetForest(g, rt, gx.NodeOwners);
            var anyChange = ValidateGraphParms(g);

            TryCreateQueryPaths(g);
            while (TryPathReduction(g)) { }

            AssignNodeColor(g);
            AssignEdgeColor(g);

            if (anyChange) g.CheckLayout();
            g.AdjustGraph();
        }
        #endregion

        #region AssignNodeColor  ==============================================
        private void AssignNodeColor(Graph g)
        {
            var group_Color = new Dictionary<Item, byte>();
            var item_group = new Dictionary<Item, Item>();

            if (GraphX_ColorColumnX.TryGetChild(g.GraphX, out ColumnX cx) && TableX_ColumnX.TryGetParent(cx, out TableX tx) && tx.Count > 0)
            {
                foreach (var gp in tx.Items)
                {
                    group_Color[gp] = g.GraphX.Color.ColorIndex(cx.Value.GetString(gp));
                }

                foreach (var (q1, q2) in g.GroupQuerys)
                {
                    item_group[q1.Item] = q2.Items[0];
                }

                foreach (var nd in g.Nodes)
                {
                    nd.Color = (item_group.TryGetValue(nd.Item, out Item gp) && group_Color.TryGetValue(gp, out byte index)) ? index : (byte)0;
                }
            }
            else
            {
                foreach (var node in g.Nodes) { node.Color = 0; }
            }
        }
        #endregion

        #region AssignEdgeColor  ==============================================
        private void AssignEdgeColor(Graph g)
        {
            foreach (var eg in g.Edges)
            {
                eg.LineColor = (eg.QueryX.PathParm != null) ? g.GraphX.Color.ColorIndex(eg.QueryX.PathParm.LineColor) : (byte)0;
            }
        }
        #endregion
    }
}
