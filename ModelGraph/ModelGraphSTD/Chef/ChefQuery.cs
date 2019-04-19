using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        #region ValidateQueryXStore  ==========================================
        private void ValidateQueryXStore()
        {
            foreach (var cx in ComputeXStore.Items)
            {
                ValidateComputeQuery(cx);
            }
            RevalidateUnresolved();
            foreach (var gx in GraphXStore.Items)
            {
                ValidateGraphQuery(gx);
            }
        }

        private void ValidateQueryDependants(QueryX qx)
        {
            while (QueryX_QueryX.TryGetParent(qx, out QueryX qp)) { qx = qp; }

            if (ComputeX_QueryX.TryGetParent(qx, out ComputeX cx))
                ValidateComputeQuery(cx, true);
            else if (GraphX_QueryX.TryGetParent(qx, out GraphX gx))
            {
                ValidateGraphQuery(gx, true);
            }
            else if (SymbolX_QueryX.TryGetParent(qx, out SymbolX sx))
            {
                ValidateWhere(qx, qx, _queryXWhereProperty, true);
            }
        }

        private void ValidateComputeQuery(ComputeX cx, bool clearError = false)
        {
            cx.Value.Clear();
            cx.Value = ValuesUnknown;
            if (clearError)
            {
                ClearError(cx);
                ClearError(cx, _computeXSelectProperty);
            }
            if (!ComputeX_QueryX.TryGetChild(cx, out QueryX qx))
            {
                cx.Value = ValuesInvalid;
                TryAddErrorNone(cx, Trait.ComputeMissingRootQueryError);
            }
            else
            {
                if (qx.Select == null)
                {
                    if (cx.CompuType == CompuType.RowValue)
                    {
                        TryAddErrorNone(cx, _computeXSelectProperty, Trait.ComputeMissingSelectError);
                    }
                }
                else
                {
                    ValidateSelect(qx, cx, _computeXSelectProperty, clearError);
                }
            }
            if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> children))
                ValidateQueryHierarchy(children, clearError);
            AllocateValueCache(cx);
        }
        private void ValidateGraphQuery(GraphX gx, bool clearError = false)
        {
            if (clearError) ClearError(gx);

            if (GraphX_QueryX.TryGetChildren(gx, out IList<QueryX> pathQuery))
                ValidateQueryHierarchy(pathQuery, clearError);

            if (GraphX_SymbolQueryX.TryGetChildren(gx, out IList<QueryX> list))
            {
                ValidateQueryHierarchy(list, clearError);
            }
        }
        private void RevalidateUnresolved()
        {
            for (int i = 0; i < 10; i++)
            {
                var anyUnresolved = false;

                foreach (var cx in ComputeXStore.Items)
                {
                    if (cx.Value.ValType == ValType.IsUnresolved)
                    {
                        anyUnresolved = true;
                        ValidateComputeQuery(cx);
                    }
                }
                if (!anyUnresolved) break;
            }
        }

        private void ValidateQueryHierarchy(IList<QueryX> rootChildren, bool clearError = false)
        {
            var queryQueue = new Queue<QueryX>();
            foreach (var qc in rootChildren) { queryQueue.Enqueue(qc); }
                
            while (queryQueue.Count > 0)
            {
                var qx = queryQueue.Dequeue();
                if (clearError)
                {
                    ClearError(qx, _queryXWhereProperty);
                    ClearError(qx, _queryXSelectProperty);
                }

                if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> children))
                {
                    qx.IsTail = false;
                    foreach (var qc in children) { queryQueue.Enqueue(qc); }
                }
                else
                    qx.IsTail = true;

                ValidateWhereSelect(qx);
            }

            void ValidateWhereSelect(QueryX qx)
            {
                var sto = GetQueryXTarget(qx);
                if (qx.Select != null)
                {
                    if (qx.Select.TryValidate(sto))
                    {
                        qx.Select.TryResolve();
                        if (qx.Select.AnyUnresolved)
                        {
                            var error = TryAddErrorMany(qx, _queryXSelectProperty, Trait.QueryUnresolvedSelectError);
                            if (error != null) qx.Select.GetTree(error.List);
                        }
                    }
                    else
                    {
                        var error = TryAddErrorMany(qx, _queryXSelectProperty, Trait.QueryInvalidSelectError);
                        if (error != null) qx.Select.GetTree(error.List);
                    }
                }
                if (qx.Where != null)
                {
                    if (qx.Where.TryValidate(sto))
                    {
                        qx.Where.TryResolve();
                        if (qx.Where.AnyUnresolved)
                        {
                            var error = TryAddErrorMany(qx, _queryXWhereProperty, Trait.QueryUnresolvedWhereError);
                            if (error != null) qx.Where.GetTree(error.List);
                        }
                    }
                    else
                    {
                        var error = TryAddErrorMany(qx, _queryXWhereProperty, Trait.QueryInvalidWhereError);
                        if (error != null) qx.Where.GetTree(error.List);
                    }
                }
            }
        }
        bool ValidateWhere(QueryX qx, Item item, Property prop, bool clearError)
        {
            if (clearError) ClearError(item, prop);

            var sto = GetQueryXTarget(qx);
            if (qx.Where != null)
            {
                if (qx.Where.TryValidate(sto))
                {
                    qx.Where.TryResolve();
                    if (qx.Where.AnyUnresolved)
                    {
                        var error = TryAddErrorMany(item, prop, Trait.QueryUnresolvedWhereError);
                        if (error != null) qx.Where.GetTree(error.List);
                        return false;
                    }
                }
                else
                {
                    var error = TryAddErrorMany(item, prop, Trait.QueryInvalidWhereError);
                    if (error != null) qx.Where.GetTree(error.List);
                    return false;
                }
            }
            return true;
        }
        bool ValidateSelect(QueryX qx, Item item, Property prop, bool clearError)
        {
            if (clearError) ClearError(item, prop);

            var sto = GetQueryXTarget(qx);
            if (qx.Select != null)
            {
                if (qx.Select.TryValidate(sto))
                {
                    qx.Select.TryResolve();
                    if (qx.Select.AnyUnresolved)
                    {
                        var error = TryAddErrorMany(item, prop, Trait.QueryUnresolvedSelectError);
                        if (error != null) qx.Select.GetTree(error.List);
                        return false;
                    }
                }
                else
                {
                    var error = TryAddErrorMany(item, prop, Trait.QueryInvalidSelectError);
                    if (error != null) qx.Select.GetTree(error.List);
                    return false;
                }
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
            if (Relation_QueryX.TryGetParent(qx, out Relation re))
            {
                GetHeadTail(re, out Store head, out target);
                if (qx.IsReversed) { target = head; }
            }
            else
                Store_QueryX.TryGetParent(qx, out target);

            return target;
        }
        #endregion

        #region GetHeadTail  ==================================================
        internal void GetHeadTail(QueryX sx, out Store head, out Store tail)
        {
            if (Relation_QueryX.TryGetParent(sx, out Relation re))
            {
                GetHeadTail(re, out head, out tail);
                if (sx.IsReversed) { var temp = head; head = tail; tail = temp; }
            }
            else
            {
                Store_QueryX.TryGetParent(sx, out head);
                tail = head;
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
            var N = 0;
            if (GraphX_SymbolQueryX.TryGetChildren(gx, out IList<QueryX> qxList))
            {
                foreach (var qx in qxList) { if (Store_QueryX.TryGetParent(qx, out Store parent) && nodeOwner == parent) N++; }
            }
            return N;
        }
        (List<SymbolX> symbols, List<QueryX> querys) GetSymbolXQueryX(GraphX gx, Store nodeOwner)
        {
            if (GraphX_SymbolQueryX.TryGetChildren(gx, out IList<QueryX> sqxList))
            {
                var sxList = new List<SymbolX>(sqxList.Count);
                var qxList = new List<QueryX>(sqxList.Count);

                foreach (var qx in sqxList)
                {
                    if (Store_QueryX.TryGetParent(qx, out Store store) && store == nodeOwner)
                    {
                        if (SymbolX_QueryX.TryGetParent(qx, out SymbolX sx))
                        {
                            sxList.Add(sx);
                            qxList.Add(qx);
                        }
                    }
                }
                if (sxList.Count > 0) return (sxList, qxList);
            }
            return (null, null);
        }
        #endregion

        #region CreateQueryX  =================================================
        private QueryX CreateQueryX(ViewX vx, Store st)
        {
            var qxNew = new QueryX(QueryXStore, QueryType.View, true);
            ItemCreated(qxNew);
            AppendLink(ViewX_QueryX, vx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(GraphX gx, Store st)
        {
            var qxNew = new QueryX(QueryXStore, QueryType.Graph, true);
            ItemCreated(qxNew);
            AppendLink(GraphX_QueryX, gx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(ComputeX cx, Store st)
        {
            var qxNew = new QueryX(QueryXStore, QueryType.Value, true);
            ItemCreated(qxNew);
            AppendLink(ComputeX_QueryX, cx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }

        private QueryX CreateQueryX(GraphX gx, SymbolX sx, Store st)
        {
            var qxNew = new QueryX(QueryXStore, QueryType.Symbol, true);
            ItemCreated(qxNew);
            AppendLink(GraphX_SymbolQueryX, gx, qxNew);
            AppendLink(SymbolX_QueryX, sx, qxNew);
            AppendLink(Store_QueryX, st, qxNew);
            return qxNew;
        }

        private QueryX CreateQueryX(ViewX vx, Relation re)
        {
            var qxNew = new QueryX(QueryXStore, QueryType.View);
            ItemCreated(qxNew);
            AppendLink(ViewX_QueryX, vx, qxNew);
            AppendLink(Relation_QueryX, re, qxNew);
            ClearParentTailFlags(qxNew);
            return qxNew;
        }
        private QueryX CreateQueryX(QueryX qx, Relation re, QueryType kind)
        {
            qx.IsTail = false;
            var qxNew = new QueryX(QueryXStore, kind);
            ItemCreated(qx);
            AppendLink(QueryX_QueryX, qx, qxNew);
            AppendLink(Relation_QueryX, re, qxNew);
            ClearParentTailFlags(qxNew);
            return qxNew;
        }
        private void ClearParentTailFlags(QueryX qx)
        {
            if (QueryX_QueryX.TryGetParents(qx, out IList<QueryX> list))
            {
                foreach (var qp in list) { qp.IsTail = false; }
            }
        }
        #endregion

        #region GeTarget<String, Value>  ======================================
        private string GetTargetString(Target targ)
        {
            if (targ == Target.Any) return "any";
            if (targ == Target.None) return string.Empty;

            var sb = new StringBuilder();

            if ((targ & Target.N) != 0) Add("N");
            if ((targ & Target.NW) != 0) Add("NW");
            if ((targ & Target.NE) != 0) Add("NE");

            if ((targ & Target.S) != 0) Add("S");
            if ((targ & Target.SW) != 0) Add("SW");
            if ((targ & Target.SE) != 0) Add("SE");

            if ((targ & Target.E) != 0) Add("E");
            if ((targ & Target.EN) != 0) Add("EN");
            if ((targ & Target.ES) != 0) Add("ES");

            if ((targ & Target.W) != 0) Add("W");
            if ((targ & Target.WN) != 0) Add("WN");
            if ((targ & Target.WS) != 0) Add("WS");

            if ((targ & Target.NWC) != 0) Add("NWC");
            if ((targ & Target.NEC) != 0) Add("NEC");
            if ((targ & Target.SWC) != 0) Add("SWC");
            if ((targ & Target.SEC) != 0) Add("SEC");

            return sb.ToString();

            void Add(string v)
            {
                if (sb.Length > 0) sb.Append(" - ");
                sb.Append(v);
            }
        }
        private Target GetTargetValue(string val)
        {
            var targ = Target.None;
            var v = " " + val.ToUpper().Replace(",", " ").Replace("-", " ").Replace("_", " ") + " ";

            if (v.Contains(" ANY ")) targ = Target.Any;
            if (v.Contains(" N ")) targ |= Target.N;
            if (v.Contains(" NW ")) targ |= Target.NW;
            if (v.Contains(" NE ")) targ |= Target.NE;

            if (v.Contains(" W ")) targ |= Target.W;
            if (v.Contains(" WN ")) targ |= Target.WN;
            if (v.Contains(" WS ")) targ |= Target.WS;

            if (v.Contains(" E ")) targ |= Target.E;
            if (v.Contains(" EN ")) targ |= Target.EN;
            if (v.Contains(" ES ")) targ |= Target.ES;

            if (v.Contains(" S ")) targ |= Target.S;
            if (v.Contains(" SW ")) targ |= Target.SW;
            if (v.Contains(" SE ")) targ |= Target.SE;

            if (v.Contains(" NWC ")) targ |= Target.NWC;
            if (v.Contains(" NEC ")) targ |= Target.NEC;
            if (v.Contains(" SWC ")) targ |= Target.SWC;
            if (v.Contains(" SEC ")) targ |= Target.SEC;

            return targ;
        }
        #endregion

        #region Legacy  =======================================================
        bool TrySetWhereProperty(QueryX qx, string val)
        {
            qx.WhereString = val;
            ValidateQueryDependants(qx);
            return qx.IsValid;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        bool TrySetSelectProperty(QueryX qx, string val)
        {
            qx.SelectString = val;
            ValidateQueryDependants(qx);
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

        QueryX GetQueryXTail(QueryX qx)
        {
            while (QueryX_QueryX.TryGetChild(qx, out QueryX qx2)) { qx = qx2; }
            return qx;
        }
        #endregion
    }
}