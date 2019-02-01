using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
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
                foreach (var qx in QueryXStore.Items)
                {
                    var sto = GetQueryXTarget(qx);
                    if (qx.Select != null && qx.Select.TryValidate(sto)) anyChange |= qx.Select.TryResolve();
                    if (qx.Where != null && qx.Where.TryValidate(sto)) anyChange |= qx.Where.TryResolve();
                    qx.IsTail = (QueryX_QueryX.HasNoChildren(qx));
                }
                foreach (var cx in ComputeXStore.Items)
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
                foreach (var cx in ComputeXStore.Items)
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

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        bool TrySetWhereProperty(QueryX qx, string val)
        {
            qx.WhereString = val;
            ValidateDependants(qx);
            return qx.IsValid;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        bool TrySetSelectProperty(QueryX qx, string val)
        {
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

        QueryX GetQueryXTail(QueryX qx)
        {
            while (QueryX_QueryX.TryGetChild(qx, out QueryX qx2)) { qx = qx2; }
            return qx;
        }
    }
}