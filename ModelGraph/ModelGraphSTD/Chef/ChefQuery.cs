﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
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

        #region ValidateQueryXStore  ==========================================
        /// <summary>
        /// Validate all uses cases of queryX
        /// </summary>
        private void ValidateQueryXStore()
        {/*
            QueryX is used to traverse conditional relational paths, filtering table rows,
            selecting graphic symbols, and compute values. These various used cases can 
            overlap and have interdependancies. 
            
            This method will preform a comprehensive validation all cases. Because of 
            posible interdependicies, it's necessary to persist until all unresolved
            queryX's have been delt with.
            
            Circular dependanies and invalid where/select clauses will be identified.
         */
            ResetAllComputeValues();

            var anyChange = true;
            while (anyChange)
            {
                anyChange = false;
                foreach (var qx in _queryXStore.ToArray)
                {
                    var sto = GetQueryXTarget(qx);
                    if (qx.Select != null && qx.Select.TryValidate(sto)) anyChange |= qx.Select.TryResolve();
                    if (qx.Where != null && qx.Where.TryValidate(sto)) anyChange |= qx.Where.TryResolve();
                    qx.IsTail = (QueryX_QueryX.HasNoChildren(qx));
                }
                foreach (var cx in _computeXStore.ToArray)
                {
                    if (ComputeX_QueryX.TryGetChild(cx, out QueryX qx))
                    {
                        anyChange |= ValidateComputeX(cx, qx);
                    }
                    else
                        cx.Value = ValuesInvalid;
                }
            }

            bool ValidateComputeX(ComputeX cx, QueryX qx)
            {
                var valType = cx.Value.ValType;
                if (valType == ValType.IsUnknown)
                    AllocateValueCache(cx);
                return valType != cx.Value.ValType; // anyChange
            }

            void ResetAllComputeValues()
            {
                foreach (var cx in _computeXStore.ToArray)
                {
                    cx.Value.Clear();
                    cx.Value = ValuesUnknown;
                }
            }
        }
        #endregion

        #region ValidateDependants  ===========================================
        void ValidateDependants(QueryX qx)
        {
            while (qx != null)
            {
                if (ValidateQueryX())
                {
                    if (ComputeX_QueryX.TryGetParent(qx, out ComputeX cx))
                    {
                        cx.Value.Clear();
                        cx.Value = ValuesUnknown;
                        AllocateValueCache(cx);
                    }
                }
                else
                {
                    if (ComputeX_QueryX.TryGetParent(qx, out ComputeX cx))
                    {
                        cx.Value.Clear();
                        cx.Value = ValuesUnknown;                        
                    }
                }
                if (!QueryX_QueryX.TryGetParent(qx, out qx)) return;
            }

            bool ValidateQueryX()
            {
                qx.IsTail = (QueryX_QueryX.HasNoChildren(qx));

                var sto = GetQueryXTarget(qx);
                if (qx.Select != null)
                {
                    if (!qx.Select.TryValidate(sto)) return false;
                    qx.Select.TryResolve();
                }
                if (qx.Where != null)
                {
                    if (!qx.Where.TryValidate(sto)) return false;
                    qx.Where.TryResolve();
                }
                return true;
            }
        }
        #endregion

        #region ConvertQueryType  =============================================
        private void MakeRootLink(ItemModel model)
        {
            var qx = model.Item as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Graph; qxi.IsHead = false; qxi.IsTail = false; }
            }
            MajorDelta += 1;
        }
        private void MakePathtHead(ItemModel model)
        {
            var qx = model.Item as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Path; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
            MajorDelta += 1;
        }
        private void MakeGroupHead(ItemModel model)
        {
            var qx = model.Item as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) {qxi.QueryKind = QueryType.Group; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
            MajorDelta += 1;
        }
        private void MakeBridgeHead(ItemModel model)
        {
            var qx = model.Item as QueryX;
            if (TryGetQueryXList(qx, out List<QueryX> list))
            {
                foreach (var qxi in list) { qxi.QueryKind = QueryType.Segue; }
                var tail = list.Count - 1;
                list[0].IsHead = true;
                list[tail].IsTail = true;
            }
            MajorDelta += 1;
        }

        internal bool CanConvertQueryType(ItemModel model)
        {
            var qx = model.Item as QueryX;
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
            list = new List<QueryX>() { qx };
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

        #region GetQueryXTarget  ==============================================
        internal Store  GetQueryXTarget(QueryX qx)
        {
            Store target = null;
            var re = Relation_QueryX.GetParent(qx);
            if (re != null)
            {
                GetHeadTail(re, out Store head, out target);
                if (qx.IsReversed) { target = head; }
            }
            else
            {
                target = Store_QueryX.GetParent(qx);
            }
            return target;
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


        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        bool TrySetWhereProperty(QueryX qx, string val)
        {
            MajorDelta += 1;
            qx.WhereString = val;
            ValidateDependants(qx);
            return qx.IsValid;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        bool TrySetSelectProperty(QueryX qx, string val)
        {
            MajorDelta += 1;
            qx.SelectString = val;
            ValidateDependants(qx);
            return qx.IsValid;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        private string GetQueryXRelationName(ItemModel m)
        {
            if (Relation_QueryX.TryGetParent(m.Item, out Relation parent))
            {
                var rel = parent as RelationX;
                return GetRelationName(rel);
            }
            return null;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        string QueryXLinkName(ItemModel model)
        {
            return QueryXFilterName(model.Item as QueryX);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        string QueryXFilterName(QueryX sd)
        {
            GetHeadTail(sd, out Store head, out Store tail);
            if (head == null || tail == null) return InvalidItem;

            var headName = GetIdentity(head, IdentityStyle.Single);
            var tailName = GetIdentity(tail, IdentityStyle.Single);
            {
                if (sd.HasWhere)
                    return $"{headName}{parentNameSuffix}{tailName}  ({sd.WhereString})";
                else
                    return $"{headName}{parentNameSuffix}{tailName}";
            }
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        string QueryXHeadName(ItemModel m)
        {
            var sd = m.Item as QueryX;
            GetHeadTail(sd, out Store head1, out Store tail1);
            var sd2 = GetQueryXTail(sd);
            GetHeadTail(sd2, out Store head2, out Store tail2);

            StringBuilder sb = new StringBuilder(132);
            sb.Append(GetIdentity(head1, IdentityStyle.Single));
            sb.Append(parentNameSuffix);
            sb.Append(GetIdentity(tail2, IdentityStyle.Single));
            return sb.ToString();
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        QueryX GetQueryXTail(QueryX sd)
        {
            var sd2 = sd;
            var sd3 = sd2;
            while (sd3 != null)
            {
                sd2 = sd3;
                sd3 = QueryX_QueryX.GetChild(sd3);
            }
            return sd2;
        }
    }
}