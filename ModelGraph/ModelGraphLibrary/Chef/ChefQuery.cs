using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        #region ItemPair  =====================================================
        internal struct ItemPair
        {
            internal Item Item1;
            internal Item Item2;
            internal ItemPair(Item item1, Item item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }
        #endregion

        #region ValidateSelectors  ============================================
        private void ValidateAllSelectors()
        {
            var anyChange = true;
            while (anyChange)
            {
                anyChange = false;
                foreach (var qx in _queryXStore.Items) { anyChange |= ValidateQueryX(qx); }
            }
        }
        private bool ValidateQueryX(QueryX sx)
        {
            if (sx.Where == null) return false;
            if (sx.Where.IsValid) return false;

            GetHeadTail(sx, out Store head, out Store tail);
            if (tail == null)
            {
                sx.Where = null;
                return false;
            }

            var prevNativeType = sx.Where.ValueType;
            sx.Where.Validate(tail);

            return (prevNativeType != sx.Where.ValueType);
        }
        private bool ValidateValueX(QueryX vx)
        {
            if ((vx.Where == null || vx.Where.IsValid) && (vx.Select == null || vx.Select.IsValid)) return false;

            GetHeadTail(vx, out Store head, out Store tail);
            if (tail == null)
            {
                vx.Where = null;
                vx.Select = null;
                return false;
            }

            var anyChange = false;
            if (vx.Where != null)
            {
                var prevNativeType = vx.Where.ValueType;
                vx.Where.Validate(tail);
                anyChange = (prevNativeType != vx.Where.ValueType);
            }
            if (vx.Select != null)
            {
                var prevNativeType = vx.Select.ValueType;
                vx.Select.Validate(tail);
                anyChange |= (prevNativeType != vx.Select.ValueType);
            }
            return anyChange;
        }
        #endregion

        #region ConvertQueryType  =============================================
        private void MakeRootLink(ItemModel model)
        {
            var qx = model.Item1 as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Graph; qxi.IsHead = false; qxi.IsTail = false; }
            }
        }
        private void MakePathtHead(ItemModel model)
        {
            var qx = model.Item1 as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Path; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
        }
        private void MakeGroupHead(ItemModel model)
        {
            var qx = model.Item1 as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) {qxi.QueryKind = QueryType.Group; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
        }
        private void MakeBridgeHead(ItemModel model)
        {
            var qx = model.Item1 as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Segue; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
        }

        internal bool CanConvertQueryType(ItemModel model)
        {
            var qx = model.Item1 as QueryX;
            var prev = QueryX_QueryX.GetParent(qx);
            if (prev == null) return false;
            if (!prev.IsQueryGraphLink && !prev.IsQueryGraphRoot) return false;

            QueryX[] items = null;
            while (QueryX_QueryX.TryGetChildren(qx, out items))
            {
                if (items.Length > 1) return false;
                qx = items[0];
            }
            return true;
        }

        private bool TryGetQueryXList(QueryX qx, out List<QueryX> list)
        {
            list = new List<QueryX>();
            list.Add(qx);
            QueryX[] children;
            while ((children = QueryX_QueryX.GetChildren(qx)) != null)
            {
                if (children.Length != 1) return false;
                qx = children[0];
                list.Add(qx);
            }
            return true;
        }
        #endregion

        #region CanDropQueryXRelation  ========================================
        private bool CanDropQueryXRelation(QueryX sx, Relation re)
        {
            GetHeadTail(sx, out Store p1, out Store c1);
            GetHeadTail(re, out Store p2, out Store c2);

            return (p1 == p2 || c1 == c2 || p1 == c2 || c1 == p2);
        }
        #endregion

        #region GetHeadTail  ==================================================
        internal void GetHeadTail(QueryX sx, out Store head, out Store tail)
        {
            var re = Relation_QueryX.GetParent(sx);
            if (re != null)
            {
                GetHeadTail(re, out head, out tail);
                if (sx.IsReversed) { var temp = head; head = tail; tail = temp; }
            }
            else
            {
                head = tail = Store_QueryX.GetParent(sx);
            }
        }
        string GetSelectName(QueryX vx)
        {
            GetHeadTail(vx, out Store head, out Store tail);
            return GetIdentity(tail, IdentityStyle.Single);
        }
        string GetWhereName(QueryX sx)
        {
            GetHeadTail(sx, out Store head, out Store tail);
            return GetIdentity(tail, IdentityStyle.Single);
        }
        #endregion

        #region GetSymbolXQueryX  =============================================
        int GetSymbolQueryXCount(GraphX gx, Store nodeOwner)
        {
            var qxList = GraphX_SymbolQueryX.GetChildren(gx);
            if (qxList == null) return 0;

            var N = 0;
            foreach (var qx in qxList) { if (nodeOwner == Store_QueryX.GetParent(qx)) N += 1; }
            return N;
        }
        (SymbolX[] symbols, QueryX[] querys) GetSymbolXQueryX(GraphX gx, Store nodeOwner)
        {
            var sqxList = GraphX_SymbolQueryX.GetChildren(gx);
            if (sqxList != null)
            {
                var sxList = new List<SymbolX>(sqxList.Length);
                var qxList = new List<QueryX>(sqxList.Length);
                foreach (var qx in sqxList)
                {
                    if (nodeOwner == Store_QueryX.GetParent(qx))
                    {
                        var sx = SymbolX_QueryX.GetParent(qx);
                        sxList.Add(sx);
                        qxList.Add(qx);
                    }
                }
                if (sxList.Count > 0) return (sxList.ToArray(), qxList.ToArray());
            }
            return (null, null);
        }
        #endregion

        #region ValidateQueryXStore  ==========================================
        private void ValidateQueryXStore()
        {
            var qxStore = _queryXStore.Items;
            foreach (var qx in qxStore)
            {
                qx.IsTail = QueryX_QueryX.HasNoChildren(qx);
                qx.IsRoot = Store_QueryX.HasParentLink(qx);
            }
        }
        #endregion

        #region CreateQueryX  =================================================
        private QueryX CreateQueryX(ViewX vx, Store st)
        {
            var qxNew = new QueryX(_queryXStore, QueryType.View, true);
            ItemCreated(qxNew);
            AppendLink(ViewX_QueryX, vx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(GraphX gx, Store st)
        {
            var qxNew = new QueryX(_queryXStore, QueryType.Graph, true);
            ItemCreated(qxNew);
            AppendLink(GraphX_QueryX, gx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(ComputeX cx, Store st)
        {
            var qxNew = new QueryX(_queryXStore, QueryType.Value, true);
            ItemCreated(qxNew);
            AppendLink(ComputeX_QueryX, cx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }

        private QueryX CreateQueryX(GraphX gx, SymbolX sx, Store st)
        {
            var qxNew = new QueryX(_queryXStore, QueryType.Symbol, true);
            ItemCreated(qxNew);
            AppendLink(GraphX_SymbolQueryX, gx, qxNew);
            AppendLink(SymbolX_QueryX, sx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }

        private QueryX CreateQueryX(ViewX vx, Relation re)
        {
            var qxNew = new QueryX(_queryXStore, QueryType.View);
            ItemCreated(qxNew);
            AppendLink(ViewX_QueryX, vx, qxNew);
            AppendLink(Relation_QueryX, re, qxNew);
            ClearParentTailFlags(qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(QueryX qx, Relation re, QueryType kind)
        {
            qx.IsTail = false;
            var qxNew = new QueryX(_queryXStore, kind);
            ItemCreated(qx);
            AppendLink(QueryX_QueryX, qx, qxNew);
            AppendLink(Relation_QueryX, re, qxNew);
            ClearParentTailFlags(qxNew);
            return qxNew;
        }
        private void ClearParentTailFlags(QueryX qx)
        {
            var parents = QueryX_QueryX.GetParents(qx);
            if (parents == null) return;

            foreach (var qp in parents) { qp.IsTail = false; }
        }
        #endregion
    }
}