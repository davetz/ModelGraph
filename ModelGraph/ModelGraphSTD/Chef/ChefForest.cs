using System.Collections.Generic;

namespace ModelGraphSTD
{/*
 */
    public partial class Chef
    {
        #region GetForest  ====================================================
        /// <summary>
        /// Return a GraphX forest of query trees
        /// </summary>
        private bool TryGetForest(Graph g, Item seed, HashSet<Store> nodeOwners)
        {
            g.Forest = null;
            var gx = g.GraphX;
            if (GraphX_QueryX.TryGetChildren(gx, out IList<QueryX> roots))
            {
                var workList = new List<Query>();
                if (TryGetForestRoots(roots, seed, workList, out Query[] forest))
                {
                    g.Forest = forest;

                    var keyPairs = new Dictionary<byte, List<(Item, Item)>>();
                    var workQueue = new Queue<Query>(forest);
                    while (workQueue.Count > 0)
                    {
                        var query = workQueue.Dequeue();

                        if (nodeOwners.Contains(query.Item.Store)) g.NodeItems.Add(query.Item);
                        if (query.Items == null) continue;
                        foreach (var itm in query.Items) { if (nodeOwners.Contains(itm.Store)) g.NodeItems.Add(itm); }

                        if (QueryX_QueryX.TryGetChildren(query.Owner, out IList<QueryX> qxChildren))
                        {
                            var N = query.Items.Length;
                            query.Children = new Query[N][];

                            for (int i = 0; i < N; i++)
                            {
                                var item = query.Items[i];
                                workList.Clear();

                                foreach (var qc in qxChildren)
                                {
                                    var child = GetChildQuery(g, qc, query, item, keyPairs);
                                    if (child == null) continue;

                                    workList.Add(child);
                                    workQueue.Enqueue(child);
                                }
                                if (workList.Count > 0) query.Children[i] = workList.ToArray();
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetForest  ====================================================
        /// <summary>
        /// Return a query forest for the callers computeX.
        /// Also return a list of query's who's parent queryX has a valid select clause. 
        /// </summary>
        private bool TryGetForest(ComputeX cx, Item seed, List<Query> selectors, out Query[] forest)
        {
            forest = null;
            if (ComputeX_QueryX.TryGetChildren(cx, out IList<QueryX> qxRoots))
            {
                var workList = new List<Query>();
                if (TryGetForestRoots(qxRoots, seed, workList, out forest))
                {
                    var workQueue = new Queue<Query>(forest);
                    while (workQueue.Count > 0)
                    {
                        var q = workQueue.Dequeue();
                        if (q.Items != null)
                        {
                            if (QueryX_QueryX.TryGetChildren(q.Owner, out IList<QueryX> qxChildren))
                            {
                                var N = q.Items.Length;
                                q.Children = new Query[N][];

                                for (int i = 0; i < N; i++)
                                {
                                    var item = q.Items[i];

                                    workList.Clear();
                                    foreach (var qx in qxChildren)
                                    {
                                        var qc = GetChildQuery(qx, q, item);
                                        if (qc == null) continue;
                                        if (qc.IsTail && qx.HasValidSelect)
                                        {
                                            var p = qc.Parent;
                                            while (p.Parent != null) { p = p.Parent; }
                                            var qp = p.QueryX;
                                            if (qp.HasValidSelect)
                                                selectors.Add(p);
                                            selectors.Add(qc);
                                        }
                                        workList.Add(qc);
                                        workQueue.Enqueue(qc);
                                    }
                                    if (workList.Count > 0) q.Children[i] = workList.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            return (forest != null);
        }
        #endregion

        #region GetRoots  =====================================================
        bool TryGetForestRoots(IList<QueryX> qxRoots, Item seed, List<Query> workList, out Query[] forest)
        {/*
            Create the roots of a query forest.
         */
            workList.Clear();
            if (qxRoots != null && qxRoots.Count > 0)
            {
                foreach (var qx in qxRoots)
                {
                    var sto = Store_QueryX.GetParent(qx);
                    if (seed == null || seed.Owner != sto && sto.Count > 0)
                    {
                        var items = sto.GetItems();
                        if (qx.HasWhere) items = ApplyFilter(qx, items);

                        if (items != null) workList.Add(new Query(qx, null, sto, items.ToArray()));
                    }
                    else if (seed != null && seed.Owner == sto)
                    {
                        workList.Add(new Query(qx, null, sto, new Item[] { seed }));
                    }
                }
            }
            forest = (workList.Count > 0) ? workList.ToArray() : null;
            return (forest != null);
        }
        #endregion

        #region GetChildQuery  ================================================
        Query GetChildQuery(Graph g, QueryX qx, Query q, Item item, Dictionary<byte, List<(Item, Item)>> keyPairs)
        {
            var r = Relation_QueryX.GetParent(qx);
            if (r == null) return null;

            List<Item> items = null;
            if (qx.IsReversed)
                r.TryGetParents(item, out items);
            else
                r.TryGetChildren(item, out items);

            if (qx.HasWhere && items != null)
            {
                items = ApplyFilter(qx, items);
                if (items == null) return null;
            }

            if (items == null)
            {
                if (qx.QueryKind == QueryType.Path)
                {
                    AddOpenQueryPair(g, new Query(qx, q, item, null));
                }
                return null;
            }

            if (qx.IsExclusive) items = RemoveDuplicates(qx, item, items, keyPairs);
            if (items == null) return null;

            if (QueryX_QueryX.HasNoChildren(qx)) { qx.IsTail = true; }

            var q2 = new Query(qx, q, item, items.ToArray());
            if (qx.IsTail)
            {
                var q1 = q2.GetHeadQuery();
                switch (qx.QueryKind)
                {
                    case QueryType.Path:
                        g.PathQuerys.Add((q1, q2));
                        break;
                    case QueryType.Group:
                        g.GroupQuerys.Add((q1, q2));
                        break;
                    case QueryType.Segue:
                        g.SegueQuerys.Add((q1, q2));
                        break;
                }
            }

            return q2;
        }
        private void AddOpenQueryPair(Graph g, Query  q2)
        {
            var q1 = q2.GetHeadQuery();
            var N = g.OpenQuerys.Count;
            for (int i = 0; i < N; i++)
            {
                if (g.OpenQuerys[i].Item1.Item != q1.Item) continue;
                g.OpenQuerys.Insert(i, (q1, q2));
                return;
            }
            g.OpenQuerys.Add((q1, q2));
        }
        #endregion

        #region GetChildQuery  ================================================
        Query GetChildQuery(QueryX qx, Query q, Item item)
        {
            var r = Relation_QueryX.GetParent(qx);
            if (r == null) return null;

            List<Item> items = null;
            if (qx.IsReversed)
                r.TryGetParents(item, out items);
            else
                r.TryGetChildren(item, out items);

            if (qx.HasWhere) items = ApplyFilter(qx, items);
            if (items == null) return null;

            return new Query(qx, q, item, items.ToArray());
        }
        #endregion

        #region ApplyFilter  ==================================================
        List<Item> ApplyFilter(QueryX sd, List<Item> input)
        {
            if (input == null) return null;

            var output = input;
            var M = input.Count;
            var N = M;
            var filter = sd.Where;
            for (int i = 0; i < M; i++)
            {
                if (filter.Matches(input[i])) continue;
                input[i] = null; N--;
            }
            return RemoveNulls(input, M, N);
        }
        #endregion

        #region RemoveDuplicates  =============================================
        // do not cross the same edge (item to input[n]) twice
        List<Item> RemoveDuplicates(QueryX sd, Item item, List<Item> input, Dictionary<byte, List<(Item, Item)>> keyPairs)
        {
            var output = input;
            var M = input.Count;
            var N = M;

            if (!keyPairs.TryGetValue(sd.ExclusiveKey, out List<(Item, Item)> itemPairs))
            {
                itemPairs = new List<(Item, Item)>(M);
                keyPairs.Add(sd.ExclusiveKey, itemPairs);
            }

            for (var i = 0; i < M; i++)
            {
                var item2 = input[i];
                if (item2 == null) continue;

                for (var j = 0; j < itemPairs.Count; j++)
                {
                    if (itemPairs[j].Item1 != item) continue;
                    if (itemPairs[j].Item2 != item2) continue;
                    item2 = input[i] = null; N--;
                    break;
                }
                if (item2 != null) itemPairs.Add((item, item2));
            }
            return RemoveNulls(input, M, N);
        }
        #endregion

        #region RemoveNulls  ==================================================
        private List<Item> RemoveNulls(List<Item> input, int M, int N)
        {
            if (N == 0) return null;
            if (N == M) return input;

            var output = new List<Item>(N);
            foreach (var item in input)
            {
                if (item == null) continue;
                output.Add(item);
            }
            return output;
        }
        private List<QueryX> RemoveNulls(List<QueryX> input, int M, int N)
        {
            if (N == 0) return null;
            if (N == M) return input;

            var output = new List<QueryX>(N);
            foreach (var item in input)
            {
                if (item == null) continue;
                output.Add(item);
            }
            return output;
        }
        #endregion
    }
}
