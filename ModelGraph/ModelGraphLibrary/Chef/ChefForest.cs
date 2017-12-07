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
        private Query[] GetForest(Graph g, Item seed, HashSet<Store> nodeOwners)
        {
            var gd = g.GraphX;
            QueryX[] roots = GraphX_QueryX.GetChildren(gd); 
            if (roots == null) return null;

            List<Query> segList = new List<Query>();
            var forest = GetRoots(roots, seed, segList);
            if (forest == null) return null;

            var keyPairs = new Dictionary<byte, List<ItemPair>>();
            var workQueue = new Queue<Query>(forest);
            while (workQueue.Count > 0)
            {
                var seg = workQueue.Dequeue();

                if (nodeOwners.Contains(seg.Item.Store)) g.NodeItems.Add(seg.Item);
                if (seg.Items == null) continue;
                foreach (var itm in seg.Items) { if (nodeOwners.Contains(itm.Store)) g.NodeItems.Add(itm); }

                QueryX[] sdChildren = QueryX_QueryX.GetChildren(seg.Owner);
                if (sdChildren == null) continue;

                var N = seg.Items.Length;
                var M = sdChildren.Length;

                seg.Children = new Query[N][];
                for (int i = 0; i < N; i++)
                {
                    var item = seg.Items[i];
                    segList.Clear();

                    foreach (var sd in sdChildren)
                    {
                        var child = GetChildQuery(g, sd, seg, item, keyPairs);
                        if (child == null) continue;

                        segList.Add(child);
                        workQueue.Enqueue(child);
                    }
                    if (segList.Count > 0) seg.Children[i] = segList.ToArray();
                }
            }
            return forest;
        }
        #endregion

        #region GetForest  ====================================================
        /// <summary>
        /// Return a ComputeX forest of query trees
        /// </summary>
        private Query[] GetForest(ComputeX cx, Item seed, List<Query> tailSegs)
        {
            QueryX[] roots = ComputeX_QueryX.GetChildren(cx);
            if (roots == null) return null;

            List<Query> qList = new List<Query>();
            var forest = GetRoots(roots, seed, qList);
            if (forest == null) return null;

            var workQueue = new Queue<Query>(forest);
            while (workQueue.Count > 0)
            {
                var q = workQueue.Dequeue();
                if (q.Items == null) continue;

                var qxChildren = QueryX_QueryX.GetChildren(q.Owner);
                if (qxChildren != null)
                {
                    var N = q.Items.Length;
                    var M = qxChildren.Length;

                    q.Children = new Query[N][];
                    for (int i = 0; i < N; i++)
                    {
                        var item = q.Items[i];
                        qList.Clear();

                        foreach (var qx in qxChildren)
                        {
                            var child = GetChildQuery(qx, q, item);
                            if (child == null) continue;
                            if (child.IsTail) tailSegs.Add(child);

                            qList.Add(child);
                            workQueue.Enqueue(child);
                        }
                        if (qList.Count > 0) q.Children[i] = qList.ToArray();
                    }
                }
            }
            return forest;
        }
        #endregion

        #region GetRoots  =====================================================
        Query[] GetRoots(QueryX[] roots, Item seed, List<Query> segList)
        {
            if (roots != null && roots.Length > 0)
            {
                foreach (var root in roots)
                {
                    var st = Store_QueryX.GetParent(root);
                    if (seed == null || seed.Owner != st && st.Count > 0)
                    {
                        var items = st.GetItems();
                        if (root.HasWhere) items = ApplyFilter(root, items);

                        if (items != null) segList.Add(new Query(root, null, st, items));
                    }
                    else if (seed != null && seed.Owner == st)
                    {
                        segList.Add(new Query(root, null, st, new Item[] { seed }));
                    }
                }
            }

            return (segList.Count > 0) ? segList.ToArray() : null;
        }
        #endregion

        #region GetChildQuery  ================================================
        Query GetChildQuery(Graph g, QueryX qx, Query q, Item item, Dictionary<byte, List<ItemPair>> keyPairs)
        {
            var r = Relation_QueryX.GetParent(qx);
            if (r == null) return null;

            Item[] items = null;
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

            var s2 = new Query(qx, q, item, items);
            if (qx.IsTail)
            {
                var s1 = s2.GetHeadQuery();
                switch (qx.QueryKind)
                {
                    case QueryType.Path:
                        g.PathQuerys.Add(new QueryPair(s1, s2));
                        break;
                    case QueryType.Group:
                        g.GroupQuerys.Add(new QueryPair(s1, s2));
                        break;
                    case QueryType.Segue:
                        g.SegueQuerys.Add(new QueryPair(s1, s2));
                        break;
                }
            }

            return s2;
        }
        private void AddOpenQueryPair(Graph g, Query  q2)
        {
            var q1 = q2.GetHeadQuery();
            var N = g.OpenQuerys.Count;
            for (int i = 0; i < N; i++)
            {
                if (g.OpenQuerys[i].Query1.Item != q1.Item) continue;
                g.OpenQuerys.Insert(i, new QueryPair(q1, q2));
                return;
            }
            g.OpenQuerys.Add(new QueryPair(q1, q2));
        }
        #endregion

        #region GetChildQuery  ================================================
        Query GetChildQuery(QueryX qx, Query q, Item item)
        {
            var r = Relation_QueryX.GetParent(qx);
            if (r == null) return null;

            Item[] items = null;
            if (qx.IsReversed)
                r.TryGetParents(item, out items);
            else
                r.TryGetChildren(item, out items);

            if (qx.HasWhere) items = ApplyFilter(qx, items);
            if (items == null) return null;

            return new Query(qx, q, item, items);
        }
        #endregion

        #region ApplyFilter  ==================================================
        Item[] ApplyFilter(QueryX sd, Item[] input)
        {
            if (input == null) return null;

            var output = input;
            var M = input.Length;
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
        Item[] RemoveDuplicates(QueryX sd, Item item, Item[] input, Dictionary<byte, List<ItemPair>> keyPairs)
        {
            var output = input;
            var M = input.Length;
            var N = M;

            List<ItemPair> itemPairs;
            if (!keyPairs.TryGetValue(sd.ExclusiveKey, out itemPairs))
            {
                itemPairs = new List<ItemPair>(M);
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
                if (item2 != null) itemPairs.Add(new ItemPair(item, item2));
            }
            return RemoveNulls(input, M, N);
        }
        #endregion

        #region RemoveNulls  ==================================================
        private Item[] RemoveNulls(Item[] input, int M, int N)
        {
            if (N == 0) return null;
            if (N == M) return input;

            var output = new Item[N];
            for (int i = 0, j = 0; i < M; i++)
            {
                var item = input[i];
                if (item != null) output[j++] = item;
            }
            return output;
        }
        private QueryX[] RemoveNulls(QueryX[] input, int M, int N)
        {
            if (N == 0) return null;
            if (N == M) return input;

            var output = new QueryX[N];
            for (int i = 0, j = 0; i < M; i++)
            {
                var item = input[i];
                if (item != null) output[j++] = item;
            }
            return output;
        }
        #endregion
    }
}
