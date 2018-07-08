using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        #region Initialize_ModelActions  ======================================
        void Initialize_ModelActions()
        {
            Initialize_ParmDebugList_X();
            Initialize_S_601_X();
            Initialize_S_602_X();
            Initialize_S_603_X();
            Initialize_S_604_X();
            Initialize_S_605_X();
            Initialize_S_606_X();
            Initialize_S_607_X();
            Initialize_S_608_X();
            Initialize_S_609_X();
            Initialize_S_60A_X();
            Initialize_S_60B_X();
            Initialize_S_60C_X();
            Initialize_S_60D_X();
            Initialize_S_60E_X();
            Initialize_S_60F_X();

            Initialize_S_610_X();
            Initialize_RootChef_X();
            Initialize_DataChef_X();
            Initialize_TextColumn_X();
            Initialize_CheckColumn_X();
            Initialize_ComboColumn_X();
            Initialize_TextProperty_X();
            Initialize_CheckProperty_X();
            Initialize_ComboProperty_X();
            Initialize_TextCompute_X();
            Initialize_S_61B_X();
            Initialize_S_61C_X();
            Initialize_S_61D_X();
            Initialize_S_61E_X();
            Initialize_S_61F_X();

            Initialize_ParmRoot_X();
            Initialize_ErrorRoot_X();
            Initialize_ChangeRoot_X();
            Initialize_MetadataRoot_X();
            Initialize_ModelingRoot_X();
            Initialize_MetaRelationList_X();
            Initialize_ErrorType_X();
            Initialize_ErrorText_X();
            Initialize_ChangeSet_X();
            Initialize_ItemChange_X();
            Initialize_S_62A_X();
            Initialize_S_62B_X();
            Initialize_S_62C_X();
            Initialize_S_62D_X();
            Initialize_S_62E_X();
            Initialize_S_62F_X();

            Initialize_ViewXView_X();
            Initialize_ViewXViewM_X();
            Initialize_ViewXQuery_X();
            Initialize_ViewXCommand_X();
            Initialize_ViewXProperty_X();

            Initialize_ViewView_X();
            Initialize_ViewViewM_X();
            Initialize_ViewItem_X();
            Initialize_ViewQuery_X();

            Initialize_EnumXList_X();
            Initialize_TableXList_X();
            Initialize_GraphXList_X();
            Initialize_SymbolXList_X();
            Initialize_TableList_X();
            Initialize_GraphList_X();

            Initialize_PairX_X();
            Initialize_EnumX_X();
            Initialize_TableX_X();
            Initialize_GraphX_X();
            Initialize_SymbolX_X();
            Initialize_ColumnX_X();
            Initialize_ComputeX_X();

            Initialize_ColumnXList_X();
            Initialize_ChildRelationXList_X();
            Initialize_ParentRelationXList_X();
            Initialize_PairXList_X();
            Initialize_EnumColumnList_X();
            Initialize_ComputeXList_X();

            Initialize_ChildRelationX_X();
            Initialize_ParentRelationX_X();
            Initialize_NameColumnRelation_X();
            Initialize_SummaryColumnRelation_X();
            Initialize_NameColumn_X();
            Initialize_SummaryColumn_X();

            Initialize_GraphXColoring_X();
            Initialize_GraphXRootList_X();
            Initialize_GraphXNodeList_X();
            Initialize_GraphXNode_X();
            Initialize_GraphXColorColumn_X();

            Initialize_QueryXRoot_X();
            Initialize_QueryXLink_X();
            Initialize_QueryXPathHead_X();
            Initialize_QueryXPathLink_X();
            Initialize_QueryXGroupHead_X();
            Initialize_QueryXGroupLink_X();
            Initialize_QueryXEgressHead_X();
            Initialize_QueryXEgressLink_X();

            Initialize_GraphXNodeSymbol_X();

            Initialize_ValueHead_X();
            Initialize_ValueLink_X();

            Initialize_Row_X();
            Initialize_View_X();
            Initialize_Table_X();
            Initialize_Graph_X();
            Initialize_RowChildRelation_X();
            Initialize_RowParentRelation_X();
            Initialize_RowRelatedChild_X();
            Initialize_RowRelatedParent_X();
            Initialize_EnumRelatedColumn_X();

            Initialize_RowPropertyList_X();
            Initialize_RowChildRelationList_X();
            Initialize_RowParentRelationList_X();
            Initialize_RowComputeList_X();

            Initialize_QueryRootLink_X();
            Initialize_QueryPathHead_X();
            Initialize_QueryPathLink_X();
            Initialize_QueryGroupHead_X();
            Initialize_QueryGroupLink_X();
            Initialize_QueryEgressHead_X();
            Initialize_QueryEgressLink_X();

            Initialize_QueryRootItem_X();
            Initialize_QueryPathStep_X();
            Initialize_QueryPathTail_X();
            Initialize_QueryGroupStep_X();
            Initialize_QueryGroupTail_X();
            Initialize_QueryEgressStep_X();
            Initialize_QueryEgressTail_X();

            Initialize_GraphXRef_X();
            Initialize_GraphRef_X();
            Initialize_GraphNodeList_X();
            Initialize_GraphEdgeList_X();
            Initialize_GraphRootList_X();
            Initialize_GraphLevelList_X();
            Initialize_GraphLevel_X();
            Initialize_GraphPath_X();
            Initialize_GraphRoot_X();
            Initialize_GraphNode_X();
            Initialize_GraphEdge_X();
            Initialize_GraphOpenList_X();
            Initialize_GraphOpen_X();

            Initialize_PrimeCompute_X();
            Initialize_ComputeStore_X();
            Initialize_InternalStoreList_X();
            Initialize_InternalStore_X();
            Initialize_StoreItem_X();
            Initialize_StoreItemItemList_X();
            Initialize_StoreRelationLinkList_X();
            Initialize_StoreChildRelationList_X();
            Initialize_StoreParentRelationList_X();
            Initialize_StoreItemItem_X();
            Initialize_StoreRelationLink_X();
            Initialize_StoreChildRelation_X();
            Initialize_StoreParentRelation_X();
            Initialize_StoreRelatedItem_X();
        }
        #endregion

        #region AddChildModel  ================================================
        internal bool AddChildModel(List<ItemModel> prev, ItemModel m, Trait trait, Item item, Item aux1, Item aux2, ModelAction get)
        {/*
            I am construction a new list of itemModels but if posible I want to reuse an existing model from the previous itemModel list.
            The existing models are compared with the parameters of the candidate model to see if it matches. A new model will be created if I necessary.
            In lists of 20,000 itemModels it is important to be strategic. The new list will be very much, if not exactly, like the previous one.
            It is not posible to know what changed or why, however I have the previous list and am being feed parameters for candidates one at a time,
         */
            var C = m.ChildModelCount;  // index of next model to be added
            var N = prev.Count;         // length of the previous model list
            var M = N - 1;   // last index of previous list

            if (C > M)
                C = (N / 2);  // keep within the constraints
            else if (TryCopyPrevious(C))
                return false; // lucky dog, got it on the first try.

            for (int i = 0, j = 0, k = C; i < N; i++)
            {/*
                First look at the index then on either side of the index,  
                alternating from left to right in increasing increments.
             */
                k = (i % 2 == 0) ? (k + i) : (k - i); // right (+0, +2, +4,..)  left (-1, -3, -5,..)
                j = (k < 0) ? (k + N) : (k > M) ? (k - N) : k; // wrap arround if necessary

                if (TryCopyPrevious(j)) return false; // I reused the existing model.
            }
            m.ChildModels.Add(new ItemModel(m, trait, item, aux1, aux2, get));
            return true; // I had to create a new model

            // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

            bool TryCopyPrevious(int inx)
            {
                if (IsMatch(prev[inx]))
                {
                    m.ChildModels.Add(prev[inx]);
                    prev[inx] = null;
                    return true;
                }
                return false;

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = 

                bool IsMatch(ItemModel cm)
                {
                    if (cm == null) return false;
                    if (cm.ParentModel != m) return false;
                    if (cm.Trait != trait) return false;
                    if (cm.Item != item) return false;
                    if (cm.Aux1 != aux1) return false;
                    if (cm.Aux2 != aux2) return false;
                    if (cm.Get != get) return false;
                    return true;
                }
            }
        }
        #endregion

        #region AddProperyModel  ==============================================
        private bool AddProperyModels(List<ItemModel> prev, ItemModel model, IEnumerable<ColumnX> cols)
        {
            var anyChange = false;
            var item = model.Item;
            foreach (var col in cols)
            {
                anyChange |= NewPropertyModel(prev, model, item, col);
            }
            return anyChange;
        }
        private bool AddProperyModels(List<ItemModel> prev, ItemModel model, IEnumerable<Property> propList)
        {
            var anyChange = false;
            foreach (var prop in propList)
            {
                anyChange |= AddProperyModel(prev, model, prop);
            }
            return anyChange;
        }
        private bool AddProperyModel(List<ItemModel> prev, ItemModel model, Property prop)
        {
            var item = model.Item;
            if (prop.IsColumnX)
                return NewPropertyModel(prev, model, item, (prop as ColumnX));
            else if (prop.IsComputeX)
                return NewPropertyModel(prev, model, item, (prop as ComputeX));
            else
                return NewPropertyModel(prev, model, item, prop);
        }
        private bool NewPropertyModel(List<ItemModel> prev, ItemModel model, Item item, ColumnX col)
        {
            if (EnumX_ColumnX.TryGetParent(col, out EnumX enu))
                return AddChildModel(prev, model, Trait.ComboProperty_M, item, col, enu, ComboColumn_X);
            else if (col.Value.ValType == ValType.Bool)
                return AddChildModel(prev, model, Trait.CheckProperty_M, item, col, null, CheckColumn_X);
            else
                return AddChildModel(prev, model, Trait.TextProperty_M, item, col, null, TextColumn_X);
        }
        private bool NewPropertyModel(List<ItemModel> prev, List<ItemModel> curr, ItemModel model, Item item, ComputeX cx)
        {
            if (EnumX_ColumnX.TryGetParent(cx, out EnumX enu))
                return AddChildModel(prev, model, Trait.ComboProperty_M, item, cx, enu, ComboProperty_X);
            else if (cx.Value.ValType == ValType.Bool)
                return AddChildModel(prev, model, Trait.CheckProperty_M, item, cx, null, CheckProperty_X);
            else
                return AddChildModel(prev, model, Trait.TextProperty_M, item, cx, null, TextCompute_X);
        }
        private bool NewPropertyModel(List<ItemModel> prev, ItemModel model, Item item, Property prop)
        {
            if (Property_Enum.TryGetValue(prop, out EnumZ enu))
                return AddChildModel(prev, model, Trait.ComboProperty_M, item, prop, enu, ComboProperty_X);
            else if (prop.Value.ValType == ValType.Bool)
                return AddChildModel(prev, model, Trait.CheckProperty_M, item, prop, null, CheckProperty_X);
            else
                return AddChildModel(prev, model, Trait.TextProperty_M, item, prop, null, TextProperty_X);
        }
        #endregion

        #region RefreshViewFlatList  ==========================================
        internal void RefreshViewFlatList(RootModel root, int scroll = 0, ChangeType change = ChangeType.NoChange)
        {
            var select = root.SelectModel;
            var viewList = root.ViewFlatList;
            var capacity = root.ViewCapacity;
            var offset = viewList.IndexOf(select);

            if (capacity > 0)
            {
                var first = ItemModel.FirstValidModel(viewList);
                var start = (first == null);
                var previous = new List<ItemModel>();

                UpdateSelectModel(select, change);

                if (root.ChildModelCount == 0)
                {
                    root.Validate(previous);
                    root.ViewModels = root.ChildModels;
                }

                var modelStack = new TreeModelStack();
                modelStack.PushChildren(root);

                var S = (scroll < 0) ? -scroll : scroll;
                var N = capacity;
                var buffer = new CircularBuffer(N, S);

                #region GoTo<End,Home>  =======================================
                if ((change == ChangeType.GoToEnd || change == ChangeType.GoToHome) && offset >= 0 && first != null)
                {
                    var pm = select.ParentModel;
                    var ix = pm.GetChildlIndex(select);
                    var last = pm.ChildModelCount - 1;

                    if (change == ChangeType.GoToEnd)
                    {
                        if (ix < last)
                        {
                            select = pm.ViewModels[last];
                            if (!viewList.Contains(select)) FindFirst();
                        }
                    }
                    else
                    {
                        if (ix > 0)
                        {
                            select = pm.ViewModels[0];
                            if (!viewList.Contains(select)) FindFirst();
                        }
                    }
                    root.SelectModel = select;

                    void FindFirst()
                    {
                        first = select;
                        var absoluteFirst = root.ViewModels[0];

                        for (; offset > 0; offset--)
                        {
                            if (first == absoluteFirst) break;

                            var p = first.ParentModel;
                            var i = p.GetChildlIndex(first);

                            first = (i > 0) ? p.ViewModels[i - 1] : p;
                        }
                    }
                }
                #endregion

                #region TraverseModelTree   ===================================

                if (scroll < 0) S = 0;

                while (modelStack.IsNotEmpty && (N + S) > 0)
                {
                    var m = modelStack.PopNext();
                    buffer.Add(m);

                    if (!start && m == first) start = buffer.SetFirst();
                    if (start) { if (N > 0) N--; else S--; }

                    ValidateModel(m, previous);
                    modelStack.PushChildren(m);
                }
                #endregion

                #region ScrollViewList  =======================================
                viewList.Clear();

                if (scroll == -1)
                {
                    if (buffer.GetHead(viewList))
                        root.SelectModel = viewList[0];
                }
                else if (scroll == 1)
                {
                    if (buffer.GetTail(viewList))
                        root.SelectModel = viewList[viewList.Count - 1];
                }
                else if (scroll == 0)
                {
                    if (buffer.GetHead(viewList))
                        if (!viewList.Contains(select))
                            root.SelectModel = viewList[0];
                }
                else if (scroll < -1)
                {
                    if (buffer.GetHead(viewList))
                        if (!viewList.Contains(select))
                            root.SelectModel = viewList[viewList.Count - 1];
                }
                else if (scroll > 1)
                {
                    if (buffer.GetTail(viewList))
                        if (!viewList.Contains(select))
                            root.SelectModel = viewList[0];
                }
                #endregion
            }
            else
            {
                root.ViewFlatList.Clear();
                root.SelectModel = null;
            }

            root.UIRequestRefreshModel();
        }

        #region TreeModelStack  ===============================================
        private class TreeModelStack
        {/*
            Keep track of unvisited nodes in a depth first graph traversal
         */
            private List<(List<ItemModel> Models, int Index)> _stack;
            internal TreeModelStack() { _stack = new List<(List<ItemModel> Models, int Index)>(); }
            internal bool IsNotEmpty => (_stack.Count > 0);
            internal void PushChildren(ItemModel m) { if (m.ViewModelCount > 0) _stack.Add((m.ViewModels, 0)); }
            internal ItemModel PopNext()
            {
                var end = _stack.Count - 1;
                var (Models, Index) = _stack[end];
                var model = Models[Index++];

                if (Index < Models.Count)
                    _stack[end] = (Models, Index);
                else
                    _stack.RemoveAt(end);

                return model;
            }
        }
        #endregion

        #region CircularBuffer  ===============================================
        class CircularBuffer
        {/*
            Trace the traversal of an itemModel tree.
            The end result is a flat list of itemModels (of a predefined length).
         */
            ItemModel[] _buffer;
            int _first;
            int _count;
            readonly int _scroll;
            readonly int _length;

            internal CircularBuffer(int length, int scroll)
            {
                _scroll = scroll;
                _length = length;
                _buffer = new ItemModel[length + scroll];
            }

            internal void Add(ItemModel m) => _buffer[Index(_count++)] = m;

            internal bool SetFirst() { _first = (_count - 1); return true; }

            internal bool GetHead(List<ItemModel> list)
            {
                var first = (_first - _scroll);
                if (first < 0) first = 0;
                return CopyBuffer(first, list);
            }

            internal bool GetTail(List<ItemModel> list)
            {
                var first = (_count < _buffer.Length) ? 0 : ((_count - _first) < _length) ? (_count - _length + _scroll) : _first + _scroll;
                return CopyBuffer(first, list);
            }

            #region PrivateMethods  ===========================================
            int Index(int inx) => inx % _buffer.Length;
            bool CopyBuffer(int first, List<ItemModel> list)
            {
                for (int i = 0, j = first; (i < _length && j < _count); i++, j++)
                {
                    list.Add(_buffer[Index(j)]);
                }
                return (_count > 0);
            }
            #endregion
        }
        #endregion

        #region UpdateSelectModel  ============================================
        void UpdateSelectModel(ItemModel m, ChangeType change)
        {
            if (m != null)
            {
                switch (change)
                {
                    case ChangeType.ToggleLeft:
                        m.IsExpandedLeft = !m.IsExpandedLeft;
                        m.ResetDelta();
                        break;

                    case ChangeType.ExpandLeft:
                        m.IsExpandedLeft = true;
                        m.ResetDelta();
                        break;

                    case ChangeType.CollapseLeft:
                        m.IsExpandedLeft = false;
                        m.IsExpandedRight = false;
                        m.IsFilterVisible = false;
                        m.ViewFilter = null;
                        m.ResetDelta();
                        break;

                    case ChangeType.ToggleRight:
                        m.IsExpandedRight = !m.IsExpandedRight;
                        m.ResetDelta();
                        break;

                    case ChangeType.ExpandRight:
                        m.IsExpandedRight = true;
                        m.ResetDelta();
                        break;

                    case ChangeType.CollapseRight:
                        m.IsExpandedRight = false;
                        m.ResetDelta();
                        break;

                    case ChangeType.ToggleFilter:
                        m.IsFilterVisible = !m.IsFilterVisible;
                        if (m.IsFilterVisible)
                            m.ViewFilter = string.Empty;
                        else
                            m.ViewFilter = null;
                        break;

                    case ChangeType.ExpandFilter:
                        m.IsFilterVisible = true;
                        m.ViewFilter = string.Empty;
                        break;

                    case ChangeType.CollapseFilter:
                        m.IsFilterVisible = false;
                        m.ViewFilter = null;
                        break;
                }
            }
        }
        #endregion

        #region ValidateModel  ================================================
        bool ValidateModel(ItemModel m, List<ItemModel> previous)
        {
            if (m.Item.AutoExpandLeft)
            {
                m.IsExpandedLeft = true;
                m.Item.AutoExpandLeft = false;
            }
            if (m.Item.AutoExpandRight)
            {
                m.IsExpandedRight = true;
                m.Item.AutoExpandRight = false;
            }

            if (!m.IsExpanded && !(m.IsFilterVisible && m.HasFilterText)) return WithNoChildren();

            var (hasChildModels, hasChildListChanged) = m.Validate(previous);

            if (!hasChildModels) return WithNoChildren();

            if (!hasChildListChanged && !m.AnyFilterSortChanged) return WithNoChange();

            if (!m.IsSorted && !m.IsFiltered) return WithAllChildren();

            var filterList = new List<(string, ItemModel)>(m.ChildModelCount);

            if (m.IsFiltered)
            {
                if (m.ChangedFilter)
                {
                    var filter = m.RegexViewFilter;

                    foreach (var cm in m.ChildModels)
                    {
                        if (filter != null && !filter.IsMatch(cm.FilterSortName)) continue;
                        if (m.IsUsedFilter && !m.ModelUsed(cm)) continue;
                        if (m.IsNotUsedFilter && m.ModelUsed(cm)) continue;

                        filterList.Add((cm.FilterSortName, cm));
                    }
                }
            }

            if (m.IsSorted)
            {
                if (filterList.Count == 0 && m.ViewModelCount > 0)
                {
                    foreach (var cm in m.ViewModels)
                    {
                        filterList.Add((cm.FilterSortName, cm));
                    }
                }
                filterList.Sort(_alphaSort);
                if (m.IsSortDescending) filterList.Reverse();
            }

            if (filterList.Count > 0)
            {
                m.ViewModels = new List<ItemModel>(filterList.Count);
                foreach (var e in filterList) { m.ViewModels.Add(e.Item2); }
            }
            else
            {
                m.ViewModels = null;
            }
            return true;

            bool WithNoChange()
            {
                return true;
            }
            bool WithNoChildren()
            {
                m.ChildModels = null;
                m.ViewModels = null;
                m.ClearSortUsageMode();
                return false;
            }
            bool WithAllChildren()
            {
                m.ViewModels = new List<ItemModel>(m.ChildModels);
                return true;
            }
        }
        class SortCompare : IComparer<(string, ItemModel)>
        {
            public int Compare((string, ItemModel) x, (string, ItemModel) y) => x.Item1.CompareTo(y.Item1);
        }
        static SortCompare _alphaSort = new SortCompare();
        #endregion
        #endregion

        #region GetAppTabName  ================================================
        internal string GetAppTabName(RootModel root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return GetName(Trait.AppRootModelTab);

                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                case ControlType.GraphDisplay:
                case ControlType.SymbolEditor:
                    return GetRepositoryName();
            }
            return BlankName;
        }
        #endregion

        #region GetAppTabSummary  =============================================
        internal string GetAppTabSummary(RootModel root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return null;

                case ControlType.PrimaryTree:
                    if (Repository == null)
                        return _localize(GetNameKey(Trait.NewModel));

                    var name = Repository.Name;
                    var index = name.LastIndexOf(".");
                    if (index < 0) return name;
                    return name.Substring(0, index);

                case ControlType.GraphDisplay:
                    return null;

                case ControlType.SymbolEditor:
                    return null;

                case ControlType.PartialTree:
                    return null;
            }
            return null;
        }
        #endregion

        #region GetAppTitleName  ==============================================
        internal string GetAppTitleName(RootModel root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return GetName(Trait.AppRootModelTab);

                case ControlType.PrimaryTree:
                    return GetRepositoryName();

                case ControlType.PartialTree:
                    return $"{GetRepositoryName()} - {GetName(root.Trait)}";

                case ControlType.GraphDisplay:
                    var g = root.Item as Graph;
                    var gx = g.GraphX;
                    if (g.SeedItem == null)
                        return $"{gx.Name}";
                    else
                        return $"{gx.Name} - {GetIdentity(g.SeedItem, IdentityStyle.Double)}";

                case ControlType.SymbolEditor:

                    return $"{GetName(Trait.EditSymbol)} : {GetIdentity(root.Item, IdentityStyle.Single)}";
            }
            return BlankName;
        }
        #endregion

        #region GetAppTitleSummary  ===========================================
        internal string GetAppTitleSummary(RootModel root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return null;

                case ControlType.PrimaryTree:
                    if (Repository == null)
                        return _localize(GetNameKey(Trait.NewModel));

                    var name = Repository.Name;
                    var index = name.LastIndexOf(".");
                    if (index < 0) return name;
                    return name.Substring(0, index);

                case ControlType.GraphDisplay:
                    return null;

                case ControlType.SymbolEditor:
                    return null;

                case ControlType.PartialTree:
                    return null;
            }
            return null;
        }
        #endregion


        #region 600 ParmDebugList_X  ==========================================
        internal ModelAction ParmDebugList_X;
        void Initialize_ParmDebugList_X()
        {
            ParmDebugList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _includeItemIdentityIndexProperty);

                    return (true, true);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 601 S_601_X  ==================================================
        internal ModelAction S_601_X;
        void Initialize_S_601_X()
        {
            S_601_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 602 S_602_X  ==================================================
        internal ModelAction S_602_X;
        void Initialize_S_602_X()
        {
            S_602_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 603 S_603_X  ==================================================
        internal ModelAction S_603_X;
        void Initialize_S_603_X()
        {
            S_603_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 604 S_604_X  ==================================================
        internal ModelAction S_604_X;
        void Initialize_S_604_X()
        {
            S_604_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 605 S_605_X  ==================================================
        internal ModelAction S_605_X;
        void Initialize_S_605_X()
        {
            S_605_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 606 S_606_X  ==================================================
        internal ModelAction S_606_X;
        void Initialize_S_606_X()
        {
            S_606_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 607 S_607_X  ==================================================
        internal ModelAction S_607_X;
        void Initialize_S_607_X()
        {
            S_607_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 608 S_608_X  ==================================================
        internal ModelAction S_608_X;
        void Initialize_S_608_X()
        {
            S_608_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 609 S_609_X  ==================================================
        internal ModelAction S_609_X;
        void Initialize_S_609_X()
        {
            S_609_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60A S_60A_X  ==================================================
        internal ModelAction S_60A_X;
        void Initialize_S_60A_X()
        {
            S_60A_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60B S_60B_X  ==================================================
        internal ModelAction S_60B_X;
        void Initialize_S_60B_X()
        {
            S_60B_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60C S_60C_X  ==================================================
        internal ModelAction S_60C_X;
        void Initialize_S_60C_X()
        {
            S_60C_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60D S_60D_X  ==================================================
        internal ModelAction S_60D_X;
        void Initialize_S_60D_X()
        {
            S_60D_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60E S_60E_X  ==================================================
        internal ModelAction S_60E_X;
        void Initialize_S_60E_X()
        {
            S_60E_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 60F S_60F_X  ==================================================
        internal ModelAction S_60F_X;
        void Initialize_S_60F_X()
        {
            S_60F_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion



        #region 610 S_610_X  ==================================================
        internal ModelAction S_610_X;
        void Initialize_S_610_X()
        {
            S_610_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 611 RootChef_M  ===============================================
        internal ModelAction RootChef_X;
        void Initialize_RootChef_X()
        {
            RootChef_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    return (null, null, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    var root = m.GetRootModel();
                    switch (root.ControlType)
                    {
                        case ControlType.AppRootChef:
                            bc.Add(new ModelCommand(this, root, Trait.NewCommand, NewModel));
                            bc.Add(new ModelCommand(this, root, Trait.OpenCommand, OpenModel));
                            break;
                    }
                },
            };


            #region ButtonCommands  ===========================================
            void NewModel(ItemModel model)
            {
                var root = model as RootModel;
                var rootChef = root.Chef;
                var dataChef = new Chef(rootChef, null);

                root.UIRequestCreateView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef.DataChef_X);
                root.UIRequestRefreshModel();
            }
            void OpenModel(ItemModel model, Object parm1)
            {
                var repo = parm1 as IRepository;
                var root = model as RootModel;
                var rootChef = root.Chef;
                var dataChef = new Chef(rootChef, repo);

                root.UIRequestCreateView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef.DataChef_X);
                root.UIRequestRefreshModel();
            }
            #endregion
        }
        #endregion

        #region 613 DataChef_X  ===============================================
        internal ModelAction DataChef_X;
        void Initialize_DataChef_X()
        {
            DataChef_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    var root = m.GetRootModel();
                    switch (root.ControlType)
                    {
                        case ControlType.PrimaryTree:
                            if (root.Chef.Repository == null)
                                bc.Add(new ModelCommand(this, root, Trait.SaveAsCommand, SaveAsModel));
                            else
                                bc.Add(new ModelCommand(this, root, Trait.SaveCommand, SaveModel));

                            bc.Add(new ModelCommand(this, root, Trait.CloseCommand, CloseModel));
                            if (root.Chef.Repository != null)
                                bc.Add(new ModelCommand(this, root, Trait.ReloadCommand, ReloadModel));
                            break;

                        case ControlType.PartialTree:
                            break;

                        case ControlType.GraphDisplay:
                            break;

                        case ControlType.SymbolEditor:
                            bc.Add(new ModelCommand(this, root, Trait.SaveCommand, AppSaveSymbol));
                            bc.Add(new ModelCommand(this, root, Trait.ReloadCommand, AppReloadSymbol));
                            break;
                    }
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    if (m.ChildModelCount == 5) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.ParmRoot_M, _errorStore, null, null, ParmRoot_X);
                    AddChildModel(prev, m, Trait.ErrorRoot_M, _errorStore, null, null, ErrorRoot_X);
                    AddChildModel(prev, m, Trait.ChangeRoot_M, _changeRoot, null, null, ChangeRoot_X);
                    AddChildModel(prev, m, Trait.MetadataRoot_M, m.Item, null, null, MetadataRoot_X);
                    AddChildModel(prev, m, Trait.ModelingRoot_M, m.Item, null, null, ModelingRoot_X);

                    return (true, true);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            #region ButtonCommands  ===========================================
            void SaveAsModel(ItemModel model, Object parm1)
            {
                var repo = parm1 as IRepository;
                var root = model as RootModel;
                var dataChef = root.Chef;
                dataChef.SaveToRepository(repo);
            }
            void SaveModel(ItemModel model)
            {
                var root = model as RootModel;
                var dataChef = root.Chef;
                dataChef.SaveToRepository();
            }
            void CloseModel(ItemModel m) => m.GetRootModel().UIRequestCloseModel();
            void AppSaveSymbol(ItemModel m) => m.GetRootModel().UIRequestSaveModel();
            void AppReloadSymbol(ItemModel m) => m.GetRootModel().UIRequestReloadModel();

            void ReloadModel(ItemModel m)
            {
                var repo = Repository;
                var root = m.GetRootModel();
                if (Owner is Chef rootChef && rootChef.IsRootChef)
                {
                    var dataChef = new Chef(rootChef, repo);

                    root.UIRequestCreateView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef.DataChef_X);

                    root.UIRequestCloseModel();
                }
            }
            #endregion
        }
        #endregion

        #region 614 TextColumn_X  =============================================
        ModelAction TextColumn_X;
        void Initialize_TextColumn_X()
        {
            TextColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetColumnXKindName(m);

                    return (kind, name, 0, ModelType.TextProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetColumnXKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ColumnX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ColumnX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                TextValue = (m) => m.ColumnX.Value.GetString(m.Item),
            };
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        (string, string) GetColumnXKindName(ItemModel m) => (null, m.ColumnX.Name);
        #endregion

        #region 615 CheckColumn_X  ============================================
        ModelAction CheckColumn_X;
        void Initialize_CheckColumn_X()
        {
            CheckColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetColumnXKindName(m);

                    return (kind, name, 0, ModelType.CheckProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetColumnXKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ColumnX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ColumnX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                BoolValue = (m) => m.ColumnX.Value.GetBool(m.Item),
            };
        }
        #endregion

        #region 616 ComboColumn_X  ============================================
        ModelAction ComboColumn_X;
        void Initialize_ComboColumn_X()
        {
            ComboColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetColumnXKindName(m);

                    return (kind, name, 0, ModelType.ComboProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetColumnXKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ColumnX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ColumnX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ListValue = (m) => GetEnumDisplayValues(m.EnumX),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                IndexValue = (m) => GetComboSelectedIndex(m.Item, m.ColumnX, m.EnumX),
            };
        }
        //=====================================================================
        string[] GetEnumDisplayValues(EnumX e)
        {
            string[] values = null;
            if (e != null && e.IsValid)
            {
                var items = e.Items;
                var count = e.Count;
                values = new string[count];

                for (int i = 0; i < count; i++)
                {
                    var p = items[i] as PairX;
                    values[i] = p.DisplayValue;
                }
            }
            return values;
        }
        //=====================================================================
        string[] GetEnumActualValues(EnumX e)
        {
            string[] values = null;
            if (e != null && e.IsValid)
            {
                var items = e.Items;
                var count = e.Count;
                values = new string[count];

                for (int i = 0; i < count; i++)
                {
                    var p = items[i] as PairX;
                    values[i] = p.ActualValue;
                }
            }
            return values;
        }
        //=====================================================================
        int GetComboSelectedIndex(Item itm, Property col, EnumX enu)
        {
            var value = col.Value.GetString(itm);
            var values = GetEnumActualValues(enu);
            var len = (values == null) ? 0 : values.Length;
            for (int i = 0; i < len; i++) { if (value == values[i]) return i; }
            return -1;
        }
        #endregion

        #region 617 TextProperty_X  ===========================================
        ModelAction TextProperty_X;
        void Initialize_TextProperty_X()
        {
            TextProperty_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetPropertyKindName(m);

                    return (kind, name, 0, ModelType.TextProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetPropertyKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Property.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                TextValue = (m) => m.Property.Value.GetString(m.Item),
            };

        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        (string, string) GetPropertyKindName(ItemModel m)
        {
            var name = _localize(m.Property.NameKey);
            return (null, m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {name}" : name);
        }
        #endregion

        #region 618 CheckProperty_X  ==========================================
        ModelAction CheckProperty_X;
        void Initialize_CheckProperty_X()
        {
            CheckProperty_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetPropertyKindName(m);

                    return (kind, name, 0, ModelType.CheckProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetPropertyKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Property.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                BoolValue = (m) => m.Property.Value.GetBool(m.Item),
            };
        }
        #endregion

        #region 619 ComboProperty_X  ==========================================
        ModelAction ComboProperty_X;
        void Initialize_ComboProperty_X()
        {
            ComboProperty_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetPropertyKindName(m);

                    return (kind, name, 0, ModelType.ComboProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetPropertyKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Property.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ListValue = (m) => GetEnumZNames(m.EnumZ),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                IndexValue = (m) => GetEnumZIndex(m.EnumZ, m.Property.Value.GetString(m.Item)),
            };
        }
        #endregion

        #region 61A TextCompute_X  ============================================
        ModelAction TextCompute_X;
        void Initialize_TextCompute_X()
        {
            TextCompute_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.TextProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ComputeX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ComputeX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                TextValue = (m) => m.ComputeX.Value.GetString(m.Item),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ComputeX.Name);
        }
        #endregion

        #region 61B S_61B_X  ==================================================
        internal ModelAction S_61B_X;
        void Initialize_S_61B_X()
        {
            S_61B_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 61C S_61C_X  ==================================================
        internal ModelAction S_61C_X;
        void Initialize_S_61C_X()
        {
            S_61C_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 61D S_61D_X  ==================================================
        internal ModelAction S_61D_X;
        void Initialize_S_61D_X()
        {
            S_61D_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 61E S_61E_X  ==================================================
        internal ModelAction S_61E_X;
        void Initialize_S_61E_X()
        {
            S_61E_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 61F S_61F_X  ==================================================
        internal ModelAction S_61F_X;
        void Initialize_S_61F_X()
        {
            S_61F_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion



        #region 620 ParmRoot  =================================================
        ModelAction ParmRoot_X;
        void Initialize_ParmRoot_X()
        {
            ParmRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.ParmDebugList_M, this, null, null, ParmDebugList_X);

                    return (true, true);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 621 ErrorRoot  ================================================
        ModelAction ErrorRoot_X;
        void Initialize_ErrorRoot_X()
        {
            ErrorRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = _errorStore.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (_errorStore.Count == 0) return (false, false);
                    if (_errorStore.Delta == m.Delta) return (true, false);

                    m.InitChildModels(prev, _errorStore.Count);

                    var anyChange = false;
                    var items = _errorStore.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ErrorType_M, itm, null, null, ErrorType_X);
                    }

                    return (true, anyChange);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 622 ChangeRoot  ===============================================
        ModelAction ChangeRoot_X;
        void Initialize_ChangeRoot_X()
        {
            ChangeRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = _changeRoot.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelInfo = (m) => m.IsExpandedLeft ? null : _changeRootInfoText,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, list) =>
                {
                    if (_changeRoot.Count > 0 && m.IsExpandedLeft == false)
                        list.Add(new ModelCommand(this, m, Trait.ExpandAllCommand, ExpandAllChangeSets));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (_changeRoot.Count == 0) return (false, false);
                    if (_changeRoot.Delta == m.Delta) return (true, false);

                    var anyChange = false;
                    var items = _changeRoot.Items;
                    m.InitChildModels(prev, items.Count);
                    for (int i = (items.Count - 1); i >= 0; i--)
                    {
                        var itm = items[i];
                        anyChange |= AddChildModel(prev, m, Trait.ChangeSet_M, itm, null, null, ChangeSet_X);
                    }

                    return (true, anyChange);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 623 MetadataRoot  =============================================
        ModelAction MetadataRoot_X;
        void Initialize_MetadataRoot_X()
        {
            MetadataRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryMetadataTree));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 5) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.ViewXViewList_M, _viewXStore, null, null, ViewXViewList_X);
                    AddChildModel(prev, m, Trait.EnumXList_M, _enumZStore, null, null, EnumXList_X);
                    AddChildModel(prev, m, Trait.TableXList_M, _tableXStore, null, null, TableXList_X);
                    AddChildModel(prev, m, Trait.GraphXList_M, _graphXStore, null, null, GraphXList_X);
                    AddChildModel(prev, m, Trait.InternalStoreList_M, this, null, null, InternalStoreList_X);

                    return (true, true);
                },                
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondaryMetadataTree(ItemModel m) => m.GetRootModel().UIRequestCreatePage(ControlType.PartialTree, m);
        }
        #endregion

        #region 624 ModelingRoot  =============================================
        ModelAction ModelingRoot_X;
        void Initialize_ModelingRoot_X()
        {
            ModelingRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryModelingTree));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 4) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.ViewViewList_M, m.Item, null, null, ViewXViewList_X);
                    AddChildModel(prev, m, Trait.TableList_M, m.Item, null, null, TableList_X);
                    AddChildModel(prev, m, Trait.GraphList_M, m.Item, null, null, GraphList_X);
                    AddChildModel(prev, m, Trait.PrimeCompute_M, m.Item, null, null, PrimeCompute_X);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondaryModelingTree(ItemModel m) => m.GetRootModel().UIRequestCreatePage(ControlType.PartialTree, m);
        }
        #endregion

        #region 625 MetaRelationList  =========================================
        ModelAction MetaRelationList_X;
        void Initialize_MetaRelationList_X()
        {
            MetaRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 2) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.NameColumnRelation_M, m.Item, TableX_NameProperty, null, NameColumnRelation_X);
                    AddChildModel(prev, m, Trait.SummaryColumnRelation_M, m.Item, TableX_SummaryProperty, null, SummaryColumnRelation_X);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 626 ErrorType  ================================================
        ModelAction ErrorType_X;
        void Initialize_ErrorType_X()
        {
            ErrorType_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Error.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Item.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var er = m.Error;
                    if (er.Count == 0) return (false, false);
                    if (er.Count == m.ChildModelCount) return (true, false);

                    m.InitChildModels(prev, er.Count);
                    for (int i = 0; i < er.Count; i++)
                    {
                        AddChildModel(prev, m, Trait.ErrorText_M, er, null, null, ErrorText_X);
                    }

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.Item.NameKey));
        }
        #endregion

        #region 627 ErrorText  ================================================
        ModelAction ErrorText_X;
        void Initialize_ErrorText_X()
        {
            ErrorText_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetName(m));

            string GetName(ItemModel m)
            {
                var e = m.Error;
                var i = m.ParentModel.GetChildlIndex(m);
                return (i < 0 || e.Count <= i) ? InvalidItem : e.Errors[i];
            }
        }
        #endregion

        #region 628 ChangeSet  ================================================
        ModelAction ChangeSet_X;
        void Initialize_ChangeSet_X()
        {
            ChangeSet_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.ChangeSet.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    var cs = m.ChangeSet;
                    if (cs.CanMerge)
                        bc.Add(new ModelCommand(this, m, Trait.MergeCommand, ModelMerge));
                    if (cs.CanUndo)
                        bc.Add(new ModelCommand(this, m, Trait.UndoCommand, ModelUndo));
                    if (cs.CanRedo)
                        bc.Add(new ModelCommand(this, m, Trait.RedoCommand, ModelRedo));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var cs = m.ChangeSet;
                    if (cs.Count == 0) return (false, false);
                    if (cs.Delta == m.Delta) return (true, false);

                    var anyChange = false;
                    var items = cs.Items;
                    m.InitChildModels(prev, cs.Count);
                    for (int i = (cs.Count - 1); i >= 0; i--) // show in most recent change order
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ItemChange_M, items[i], null, null, ItemChanged_X);
                    }

                    return (true, anyChange);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetName(m));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            string GetName(ItemModel m)
            {
                var cs = m.ChangeSet;
                return cs.IsCongealed ? _localize(cs.NameKey) : cs.Name;
            }

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void ModelMerge(ItemModel model)
            {
                var chg = model.Item as ChangeSet;
                chg.Merge();
            }

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void ModelUndo(ItemModel model)
            {
                var chg = model.Item as ChangeSet;
                Undo(chg);
            }

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void ModelRedo(ItemModel model)
            {
                var chg = model.Item as ChangeSet;
                Redo(chg);
            }
        }
        #endregion

        #region 629 ItemChanged  ==============================================
        ModelAction ItemChanged_X;
        void Initialize_ItemChange_X()
        {
            ItemChanged_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.Item.KindKey), m.ItemChange.Name);
        }
        #endregion

        #region 62A S_62A_X  ==================================================
        internal ModelAction S_62A_X;
        void Initialize_S_62A_X()
        {
            S_62A_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 62B S_62B_X  ==================================================
        internal ModelAction S_62B_X;
        void Initialize_S_62B_X()
        {
            S_62B_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 62C S_62C_X  ==================================================
        internal ModelAction S_62C_X;
        void Initialize_S_62C_X()
        {
            S_62C_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 62D S_62D_X  ==================================================
        internal ModelAction S_62D_X;
        void Initialize_S_62D_X()
        {
            S_62D_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 62E S_62E_X  ==================================================
        internal ModelAction S_62E_X;
        void Initialize_S_62E_X()
        {
            S_62E_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 62F S_62F_X  ==================================================
        internal ModelAction S_62F_X;
        void Initialize_S_62F_X()
        {
            S_62F_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m, prev) =>
                {
                    return (false, false);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion



        #region 631 ViewXViewList_X  ==========================================
        ModelAction ViewXViewList_X;
        void Initialize_ViewXView_X()
        {
            ViewXViewList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var views = _viewXStore.Items;
                    var (kind, name) = GetKindName(m);
                    var count = 0;
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var vxDrop = d.ViewX;
                    if (vxDrop != null && vxDrop.Owner == _viewXStore)
                    {
                        if (doDrop)
                        {
                            if (ViewX_ViewX.TryGetParent(vxDrop, out ViewX vxDropParent))
                                RemoveLink(ViewX_ViewX, vxDropParent, vxDrop);

                            var prevIndex = _viewXStore.IndexOf(vxDrop);
                            ItemMoved(vxDrop, prevIndex, 0);
                        }
                        return DropAction.Move;
                    }
                    return DropAction.None;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (_viewXStore.Count == 0) return (false, false);

                    if (_viewXStore.JointDelta == m.Delta) return (true, false);
                    Delta = _viewXStore.JointDelta;

                    var anyChange = false;
                    var items = _viewXStore.Items;
                    m.InitChildModels(prev, items.Count);
                    foreach (var itm in items)
                    {
                        if ((ViewX_ViewX.HasNoParent(itm)))
                        {
                            anyChange |= AddChildModel(prev, m, Trait.ViewXView_M, itm, null, null, ViewXViewM_X);
                        }
                    }

                    return (true, anyChange);
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                ItemCreated(new ViewX(_viewXStore));
            }
        }
        #endregion

        #region 632 ViewXView_M  ==============================================
        ModelAction ViewXViewM_X;
        void Initialize_ViewXViewM_X()
        {
            ViewXViewM_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var vx = m.ViewX;
                    var (kind, name) = GetKindName(m);
                    var count = (ViewX_ViewX.ChildCount(vx) + ViewX_QueryX.ChildCount(vx) + ViewX_Property.ChildCount(vx));

                    m.CanDrag = true;
                    m.CanSort = count > 1;
                    m.CanFilter = count > 2;
                    m.CanExpandLeft = count > 0;
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ViewX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var view = m.ViewX;
                    if (view != null)
                    {
                        if (d.Item is ViewX vx)
                        {
                            if (vx.Owner == _viewXStore)
                            {
                                if (ViewX_QueryX.HasNoChildren(view) && ViewX_Property.HasNoChildren(view))
                                {
                                    if (doDrop)
                                    {
                                        if (ViewX_ViewX.TryGetParent(vx, out ViewX oldParent))
                                            RemoveLink(ViewX_ViewX, oldParent, vx);
                                        AppendLink(ViewX_ViewX, view, vx);
                                    }
                                    return DropAction.Move;
                                }
                            }
                        }
                        else
                        {
                            if (d.Item is Store st)
                            {
                                if (ViewX_ViewX.HasNoChildren(view) && ViewX_QueryX.HasNoChildren(view) && ViewX_Property.HasNoChildren(view))
                                {
                                    if (doDrop)
                                    {
                                        CreateQueryX(view, st);
                                    }
                                    return DropAction.Link;
                                }
                            }
                            else
                            {
                                if (d.Item is Relation re)
                                {
                                    if (ViewX_ViewX.HasNoChildren(view) && ViewX_Property.HasNoChildren(view))
                                    {
                                        if (doDrop)
                                        {
                                            CreateQueryX(view, re);
                                        }
                                        return DropAction.Link;
                                    }
                                }
                                else if (d.Item is Property pr)
                                {
                                    if (ViewX_ViewX.HasNoChildren(view) && ViewX_QueryX.HasNoChildren(view))
                                    {
                                        if (doDrop)
                                        {
                                            AppendLink(ViewX_Property, view, pr);
                                        }
                                        return DropAction.Link;
                                    }

                                }
                            }
                        }
                    }
                    return DropAction.None;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var vx = m.ViewX;
                    var anyChange = false;
                    m.InitChildModels(prev);

                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _viewXNameProperty);
                        anyChange |= AddProperyModel(prev, m, _viewXSummaryProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (ViewX_Property.TryGetChildren(vx, out IList<Property> pls))
                        {
                            foreach (var pc in pls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXProperty_M, pc, ViewX_Property, vx, ViewXProperty_X);
                            }
                        }

                        if (ViewX_QueryX.TryGetChildren(vx, out IList<QueryX> qls))
                        {
                            foreach (var qc in qls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXQuery_M, qc, ViewX_QueryX, vx, ViewXQuery_X);
                            }
                        }

                        if (ViewX_ViewX.TryGetChildren(vx, out IList<ViewX> vls))
                        {
                            foreach (var vc in vls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXView_M, vc, ViewX_ViewX, vx, ViewXViewM_X);
                            }
                        }
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ViewX.Name);

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var vx = new ViewX(_viewXStore);
                ItemCreated(vx);
                AppendLink(ViewX_ViewX, model.Item, vx);
            }
        }
        internal void ViewXView_M(ItemModel m, RootModel root)
        {
            var view = m.Item as ViewX;

        }
        #endregion

        #region 633 ViewXQuery_M  =============================================
        ModelAction ViewXQuery_X;
        void Initialize_ViewXQuery_X()
        {
            ViewXQuery_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var qx = m.Item as QueryX;
                    var (kind, name) = GetKindName(m);
                    var count = (QueryX_ViewX.ChildCount(qx) + QueryX_QueryX.ChildCount(qx) + QueryX_Property.ChildCount(qx));

                    m.CanSort = (m.IsExpandedLeft && count > 1);
                    m.CanFilter = count > 2;
                    m.CanExpandLeft = count > 0;

                    if (Relation_QueryX.TryGetParent(qx, out Relation _))
                    {
                        return (kind, name, count, ModelType.Default);
                    }
                    else
                    {
                        m.CanDrag = true;
                        m.CanExpandRight = true;

                        return (kind, name, count, ModelType.Default);
                    }
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var qx = m.Item as QueryX;
                    if (d.Item is Relation rel)
                    {
                        if (doDrop)
                        {
                            CreateQueryX(qx, rel, QueryType.View).AutoExpandRight = false;
                        }
                        return DropAction.Link;
                    }
                    else if (d.Item is Property pro)
                    {
                        if (doDrop)
                        {
                            AppendLink(QueryX_Property, qx, pro);
                        }
                        return DropAction.Link;
                    }
                    return DropAction.None;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.QueryX;
                    var anyChange = false;
                    m.InitChildModels(prev);

                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_Property.TryGetChildren(qx, out IList<Property> pls))
                        {
                            foreach (var pc in pls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXProperty_M, pc, QueryX_Property, qx, ViewXProperty_X);
                            }
                        }

                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> qls))
                        {
                            foreach (var qc in qls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXQuery_M, qc, QueryX_QueryX, qx, ViewXQuery_X);
                            }
                        }

                        if (QueryX_ViewX.TryGetChildren(qx, out IList<ViewX> vls))
                        {
                            foreach (var vc in vls)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewXView_M, vc, QueryX_ViewX, qx, ViewXViewM_X);
                            }
                        }
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m)
            {
                var qx = m.Item as QueryX;
                if (Relation_QueryX.TryGetParent(qx, out Relation re))
                {
                    return (_localize(m.KindKey), GetIdentity(qx, IdentityStyle.Single));
                }
                else if (Store_QueryX.TryGetParent(qx, out Store sto))
                {
                    return (GetIdentity(sto, IdentityStyle.Kind), GetIdentity(sto, IdentityStyle.Double));
                }
                return (Chef.BlankName, Chef.BlankName);
            }

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var vx = new ViewX(_viewXStore);
                ItemCreated(vx);
                AppendLink(QueryX_ViewX, model.Item, vx);
            }
        }
        #endregion

        #region 634 ViewXCommand  =============================================
        ModelAction ViewXCommand_X;
        void Initialize_ViewXCommand_X()
        {
            ViewXCommand_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    return (false, false);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 635 ViewXProperty_M  ==========================================
        ModelAction ViewXProperty_X;
        void Initialize_ViewXProperty_X()
        {
            ViewXProperty_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.Double));
        }
        #endregion

        #region 63A ViewView_ZM  ==============================================
        ModelAction ViewView_X;
        void Initialize_ViewView_X()
        {
            ViewView_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var views = _viewXStore.Items;
                    var (kind, name) = GetKindName(m);
                    var count = 0;
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (_viewXStore.Count == 0) return (false, false);

                    if (_viewXStore.JointDelta == m.Delta) return (true, false);
                    m.Delta = _viewXStore.JointDelta;

                    m.InitChildModels(prev);
                    var anyChange = false;
                    var items = _viewXStore.Items;
                    foreach (var itm in items)
                    {
                        if (ViewX_ViewX.HasNoParent(itm)) anyChange |= AddChildModel(prev, m, Trait.ViewView_M, itm, null, null, ViewView_X);
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 63B ViewView_M  ===============================================
        ModelAction ViewViewM_X;
        void Initialize_ViewViewM_X()
        {
            ViewViewM_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var vx = m.ViewX;
                    var key = m.Aux1; // may be null
                    var (kind, name) = GetKindName(m);
                    var count = 0;
                    if (ViewX_QueryX.TryGetChildren(vx, out IList<QueryX> querys))
                    {
                        if (querys.Count == 1 && Store_QueryX.HasParentLink(querys[0]))
                        {
                            if (TryGetQueryItems(querys[0], out List<Item> keys)) count = keys.Count;
                        }
                        else if (key != null)
                            count = querys.Count;
                    }
                    else
                    {
                        count = ViewX_ViewX.ChildCount(vx);
                    }

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ViewX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    m.InitChildModels(prev);

                    var vx = m.ViewX;
                    var key = m.Aux1; // may be null
                    var anyChange = false;

                    var L2 = (ViewX_QueryX.TryGetChildren(vx, out IList<QueryX> queryList)) ? queryList.Count : 0;
                    var L3 = (ViewX_ViewX.TryGetChildren(vx, out IList<ViewX> viewList)) ? viewList.Count : 0;
                    if ((L2 + L3) == 0) return (false, false);


                    if (L2 == 1 && Store_QueryX.HasParentLink(queryList[0]) && TryGetQueryItems(queryList[0], out List<Item> items))
                    {
                        foreach (var itm in items)
                        {
                            anyChange |= AddChildModel(prev, m, Trait.ViewItem_M, itm, queryList[0], null, ViewItem_X);
                        }
                    }
                    else if (key != null && L2 > 0)
                    {
                        foreach (var qx in queryList)
                        {
                            anyChange |= AddChildModel(prev, m, Trait.ViewQuery_M, qx, key, null, ViewQuery_X);
                        }
                    }
                    else if (L3 > 0)
                    {
                        foreach (var v in viewList)
                        {
                            anyChange |= AddChildModel(prev, m, Trait.ViewView_M, v, null, null, ViewView_X);
                        }
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ViewX.Name);
        }
        #endregion

        #region 63C ViewItem_M  ===============================================
        ModelAction ViewItem_X;
        void Initialize_ViewItem_X()
        {
            ViewItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var item = m.Item;
                    var qx = m.Aux1 as QueryX;

                    var (L1, PropertyList, L2, QueryList, L3, ViewList) = GetQueryXChildren(qx);
                    var (kind, name) = GetKindName(m);
                    var count = (L2 + L3);

                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = L1 > 0;
                    m.CanFilterUsage = (m.IsExpandedLeft && count > 1);

                    return (GetIdentity(item.Owner, IdentityStyle.Single), GetIdentity(item, IdentityStyle.Single), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var item = m.Item;
                    var qx = m.Aux1 as QueryX;
                    var (L1, PropertyList, L2, QueryList, L3, ViewList) = GetQueryXChildren(qx);

                    int R = (m.IsExpandedRight) ? L1 : 0;
                    int L = (m.IsExpandedLeft) ? (L2 + L3) : 0;

                    var N = R + L;
                    if (N == 0) return (false, false);

                    var anyChange = false;
                    m.InitChildModels(prev);

                    if (R > 0)
                    {
                        anyChange |= AddProperyModels(prev, m, PropertyList);
                    }

                    if (R > 0)
                    {
                        anyChange |= AddProperyModels(prev, m, PropertyList);
                    }

                    if (L > 0)
                    {
                        if (L2 > 0)
                        {
                            foreach (var q in QueryList)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ViewQuery_M, item, q, null, ViewQuery_X);
                            }
                        }
                        if (L3 > 0)
                        {
                        }
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item.Owner, IdentityStyle.Single), GetIdentity(m.Item, IdentityStyle.Single));
        }
        #endregion

        #region 63D ViewQuery_M  ==============================================
        ModelAction ViewQuery_X;
        void Initialize_ViewQuery_X()
        {
            ViewQuery_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var key = m.Item;
                    var qx = m.QueryX;
                    var (kind, name) = GetKindName(m);
                    var count = TryGetQueryItems(qx, out List<Item> items, key) ? items.Count : 0;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var key = m.Item;
                    var qx = m.QueryX;

                    if (!TryGetQueryItems(qx, out List<Item> items, key))  return (false, false);

                    var anyChange = false;
                    m.InitChildModels(prev);

                    foreach (var itm in items)
                    {
                        anyChange = AddChildModel(prev, m, Trait.ViewItem_M, itm, qx, null, ViewItem_X);
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetIdentity(m.QueryX, IdentityStyle.Single));
        }
        #endregion




        #region 642 EnumXList  ================================================
        ModelAction EnumXList_X;
        void Initialize_EnumXList_X()
        {
            EnumXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = _enumXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var store = _enumXStore;
                    if (store.Count == 0) return (false, false);
                    if (store.Delta == m.Delta) return (true, false);

                    m.Delta = store.Delta;
                    m.InitChildModels(prev, store.Count);

                    var anyChange = false;
                    var items = store.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.EnumX_M, itm, null, null, EnumX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                ItemCreated(new EnumX(_enumXStore));
            }
        }
        #endregion

        #region 643 TableXList  ===============================================
        ModelAction TableXList_X;
        void Initialize_TableXList_X()
        {
            TableXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = _tableXStore.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var store = _tableXStore;
                    if (store.Count == 0) return (false, false);
                    if (store.Delta == m.Delta) return (true, false);

                    m.Delta = store.Delta;
                    m.InitChildModels(prev, store.Count);

                    var anyChange = false;
                    var items = store.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.TableX_M, itm, null, null, TableX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                ItemCreated(new TableX(_tableXStore));
            }
        }
        #endregion

        #region 644 GraphXList  ===============================================
        ModelAction GraphXList_X;
        void Initialize_GraphXList_X()
        {
            GraphXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = _graphXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var store = _graphXStore;
                    if (store.Count == 0) return (false, false);
                    if (store.Delta == m.Delta) return (true, false);

                    m.Delta = store.Delta;
                    m.InitChildModels(prev, store.Count);

                    var anyChange = false;
                    var items = store.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphX_M, itm, null, null, GraphX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                ItemCreated(new GraphX(_graphXStore));
                model.IsExpandedLeft = true;
            }
        }
        #endregion

        #region 645 SymbolXlList  =============================================
        ModelAction SymbolXList_X;
        void Initialize_SymbolXList_X()
        {
            SymbolXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var gx = m.Item as GraphX;
                    var (kind, name) = GetKindName(m);
                    var count = GraphX_SymbolX.ChildCount(gx);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsSymbolX) return DropAction.None;
                    var sx = d.Item as SymbolX;
                    if (doDrop)
                    {
                        var gd = m.Item as GraphX;
                        var sym = new SymbolX(_symbolXStore);
                        ItemCreated(sym);
                        AppendLink(GraphX_SymbolX, gd, sym);
                        m.IsExpandedLeft = true;
                        sym.Data = sx.Data;
                        sym.Name = sx.Name;
                        sym.Summary = sx.Summary;
                    }
                    return DropAction.Copy;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var gx = m.GraphX;
                    if (!GraphX_SymbolX.TryGetChildren(gx, out IList<SymbolX> items)) return (false, false);

                    m.InitChildModels(prev, items.Count);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.SymbolX_M, itm, GraphX_SymbolX, gx, SymbolX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var gd = model.Item as GraphX;
                var sym = new SymbolX(_symbolXStore);
                ItemCreated(sym);
                AppendLink(GraphX_SymbolX, gd, sym);
                model.IsExpandedLeft = true;
            }
        }
        #endregion

        #region 647 TableList  ================================================
        ModelAction TableList_X;
        void Initialize_TableList_X()
        {
            TableList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = _tableXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var store = _tableXStore;
                    if (store.Count == 0) return (false, false);
                    if (store.Delta == m.Delta) return (true, false);

                    m.Delta = store.Delta;
                    m.InitChildModels(prev, store.Count);

                    var anyChange = false;
                    var items = store.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.Table_M, itm, null, null, Table_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 648 GraphList  ================================================
        ModelAction GraphList_X;
        void Initialize_GraphList_X()
        {
            GraphList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = _graphXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var store = _graphXStore;
                    if (store.Count == 0) return (false, false);
                    if (store.Delta == m.Delta) return (true, false);

                    m.Delta = store.Delta;
                    m.InitChildModels(prev, store.Count);

                    var anyChange = false;
                    var items = store.Items;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphXRef_M, itm, null, null, GraphXRef_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion



        #region 652 PairX  ====================================================
        ModelAction PairX_X;
        void Initialize_PairX_X()
        {
            PairX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.PairX.ActualValue,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.IsExpandedRight) return (false, false);
                    if (m.ChildModelCount == 2) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _pairXTextProperty);
                    AddProperyModel(prev, m, _pairXValueProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.PairX.DisplayValue);
        }
        #endregion

        #region 653 EnumX  ====================================================
        ModelAction EnumX_X;
        void Initialize_EnumX_X()
        {
            EnumX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.EnumX.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = count > 0;
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => (m.Item as EnumX).Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var ex = m.EnumX;
                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _enumXNameProperty);
                        anyChange |= AddProperyModel(prev, m, _enumXSummaryProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.EnumValueList_M, ex, null, null, PairXList_X);
                        anyChange |= AddChildModel(prev, m, Trait.EnumColumnList_M, ex, null, null, EnumColumnList_X);
                    }

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.EnumX.Name);

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel m)
            {
                ItemCreated(new PairX(m.EnumX));
            }
        }
        #endregion

        #region 654 TableX  ===================================================
        ModelAction TableX_X;
        void Initialize_TableX_X()
        {
            TableX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.TableX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var R = m.IsExpandedRight ? 2 : 0;
                    var L = m.IsExpandedLeft ? 5 : 0;
                    if (R + L == 0) return (false, false);
                    if (R + L == m.ChildModelCount) return (true, false);

                    m.InitChildModels(prev);

                    if (m.IsExpandedRight)
                    {
                        AddProperyModel(prev, m, _tableXNameProperty);
                        AddProperyModel(prev, m, _tableXSummaryProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        var tx = m.TableX;
                        AddChildModel(prev, m, Trait.ColumnXList_M, tx, null, null, ColumnXList_X);
                        AddChildModel(prev, m, Trait.ComputeXList_M, tx, null, null, ComputeXList_X);
                        AddChildModel(prev, m, Trait.ChildRelationXList_M, tx, null, null, ChildRelationXList_X);
                        AddChildModel(prev, m, Trait.ParentRelatationXList_M, tx, null, null, ParentRelationXList_X);
                        AddChildModel(prev, m, Trait.MetaRelationList_M, tx, null, null, MetaRelationList_X);
                    }
                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.TableX.Name);
        }
        #endregion

        #region 655 GraphX  ===================================================
        ModelAction GraphX_X;
        void Initialize_GraphX_X()
        {
            GraphX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.GraphX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var R = m.IsExpandedRight ? 2 : 0;
                    var L = m.IsExpandedLeft ? 4 : 0;
                    if (R + L == 0) return (false, false);
                    if (R + L == m.ChildModelCount) return (true, false);

                    m.InitChildModels(prev);

                    if (m.IsExpandedRight)
                    {
                        AddProperyModel(prev, m, _graphXNameProperty);
                        AddProperyModel(prev, m, _graphXSummaryProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        var gx = m.GraphX;
                        AddChildModel(prev, m, Trait.GraphXColoring_M, gx, null, null, GraphXColoring_X);
                        AddChildModel(prev, m, Trait.GraphXRootList_M, gx, null, null, GraphXRootList_X);
                        AddChildModel(prev, m, Trait.GraphXNodeList_M, gx, null, null, GraphXNodeList_X);
                        AddChildModel(prev, m, Trait.SymbolXList_M, gx, null, null, SymbolXList_X);
                    }
                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.GraphX.Name);
        }
        #endregion

        #region 656 SymbolX  ==================================================
        ModelAction SymbolX_X;
        void Initialize_SymbolX_X()
        {
            SymbolX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.SymbolX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },
                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.EditCommand, CreateSecondarySymbolEdit));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.IsExpandedRight) return (false, false);
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _symbolXNameProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.SymbolX.Name);

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondarySymbolEdit(ItemModel m) => m.GetRootModel().UIRequestCreatePage(ControlType.SymbolEditor, m);
        }
        #endregion

        #region 657 ColumnX  ==================================================
        ModelAction ColumnX_X;
        void Initialize_ColumnX_X()
        {
            ColumnX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ColumnX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.IsExpandedRight) return (false, false);
                    if (m.ChildModelCount == 5) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _columnXNameProperty);
                    AddProperyModel(prev, m, _columnXSummaryProperty);
                    AddProperyModel(prev, m, _columnXTypeOfProperty);
                    AddProperyModel(prev, m, _columnXIsChoiceProperty);
                    AddProperyModel(prev, m, _columnXInitialProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ColumnX.Name);
        }
        #endregion

        #region 658 ComputeX_M  ===============================================
        ModelAction ComputeX_X;
        void Initialize_ComputeX_X()
        {
            ComputeX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var cx = m.ComputeX;
                    var count = ComputeX_QueryX.TryGetChild(cx, out QueryX qx) ? QueryX_QueryX.ChildCount(qx) : 0;
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ComputeX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Relation rel)) return DropAction.None;

                    var cx = m.ComputeX;
                    if (!ComputeX_QueryX.TryGetChild(cx, out QueryX root)) return DropAction.None;
                    if (!Store_ComputeX.TryGetParent(cx, out Store sto)) return DropAction.None;

                    GetHeadTail(rel, out Store sto1, out Store sto2);
                    if (sto != sto1 && sto != sto2) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(root, rel, QueryType.Value).IsReversed = (sto == sto2);
                        m.IsExpandedLeft = true;
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var cx = m.Item as ComputeX;
                    if (!ComputeX_QueryX.TryGetChild(cx, out QueryX qx)) return (false, false);

                    m.InitChildModels(prev);

                    if (m.IsExpandedRight)
                    {
                        switch (cx.CompuType)
                        {
                            case CompuType.RowValue:
                                if (qx.HasSelect)
                                {
                                    AddProperyModel(prev, m, _computeXNameProperty);
                                    AddProperyModel(prev, m, _computeXSummaryProperty);
                                    AddProperyModel(prev, m, _computeXCompuTypeProperty);
                                    AddProperyModel(prev, m, _computeXSelectProperty);
                                    AddProperyModel(prev, m, _computeXValueTypeProperty);
                                }
                                else
                                {
                                    AddProperyModel(prev, m, _computeXNameProperty);
                                    AddProperyModel(prev, m, _computeXSummaryProperty);
                                    AddProperyModel(prev, m, _computeXCompuTypeProperty);
                                    AddProperyModel(prev, m, _computeXSelectProperty);
                                }
                                break;

                            case CompuType.RelatedValue:
                                AddProperyModel(prev, m, _computeXNameProperty);
                                AddProperyModel(prev, m, _computeXSummaryProperty);
                                AddProperyModel(prev, m, _computeXCompuTypeProperty);
                                AddProperyModel(prev, m, _computeXValueTypeProperty);
                                break;

                            case CompuType.NumericValueSet:
                                AddProperyModel(prev, m, _computeXNameProperty);
                                AddProperyModel(prev, m, _computeXSummaryProperty);
                                AddProperyModel(prev, m, _computeXCompuTypeProperty);
                                AddProperyModel(prev, m, _computeXNumericSetProperty);
                                AddProperyModel(prev, m, _computeXValueTypeProperty);
                                break;

                            case CompuType.CompositeString:
                            case CompuType.CompositeReversed:
                                AddProperyModel(prev, m, _computeXNameProperty);
                                AddProperyModel(prev, m, _computeXSummaryProperty);
                                AddProperyModel(prev, m, _computeXCompuTypeProperty);
                                AddProperyModel(prev, m, _computeXSeparatorProperty);
                                AddProperyModel(prev, m, _computeXSelectProperty);
                                AddProperyModel(prev, m, _computeXValueTypeProperty);
                                break;
                        }
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var itm in list)
                            {
                                AddChildModel(prev, m, Trait.ValueXHead_M, itm, null, null, ValueHead_X);
                            }
                        }
                    }
                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ComputeX.Name);
        }
        #endregion



        #region 661 ColumnXList  ==============================================
        ModelAction ColumnXList_X;
        void Initialize_ColumnXList_X()
        {
            ColumnXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ColumnX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (!TableX_ColumnX.TryGetChildren(tx, out IList<ColumnX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ColumnX_M, itm, TableX_ColumnX, tx, ColumnX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var col = new ColumnX(_columnXStore);
                ItemCreated(col); AppendLink(TableX_ColumnX, model.Item, col);
            }
        }
        #endregion

        #region 662 ChildRelationXList  =======================================
        ModelAction ChildRelationXList_X;
        void Initialize_ChildRelationXList_X()
        {
            ChildRelationXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ChildRelationX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (!TableX_ChildRelationX.TryGetChildren(tx, out IList<RelationX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var rel in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ChildRelationX_M, rel, TableX_ChildRelationX, tx, ChildRelationX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var rel = new RelationX(_relationXStore);
                ItemCreated(rel); AppendLink(TableX_ChildRelationX, model.Item, rel);
            }
        }
        #endregion

        #region 663 ParentRelatationXList  ====================================
        ModelAction ParentRelationXList_X;
        void Initialize_ParentRelationXList_X()
        {
            ParentRelationXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ParentRelationX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (!TableX_ParentRelationX.TryGetChildren(tx, out IList<RelationX> list)) return (false, false);

                    m.InitChildModels(prev);
                    var anyChange = false;
                    foreach (var rel in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ParentRelationX_M, rel, TableX_ParentRelationX, tx, ParentRelationX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var rel = new RelationX(_relationXStore); ItemCreated(rel);
                AppendLink(TableX_ParentRelationX, model.Item, rel);
            }
        }
        #endregion

        #region 664 PairXList  ================================================
        ModelAction PairXList_X;
        void Initialize_PairXList_X()
        {
            PairXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.EnumX.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var ex = m.EnumX;
                    if (ex.Count == 0) return (false, false);
                    if (ex.Delta == m.Delta) return (true, false);

                    m.Delta = ex.Delta;
                    m.InitChildModels(prev);

                    var items = ex.Items;
                    var anyChange = false;
                    foreach (var px in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.PairX_M, px, ex, null, PairX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                ItemCreated(new PairX(model.Item as EnumX));
            }
        }
        #endregion

        #region 665 EnumColumnList  ===========================================
        ModelAction EnumColumnList_X;
        void Initialize_EnumColumnList_X()
        {
            EnumColumnList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = EnumX_ColumnX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsColumnX) return DropAction.None;

                    if (doDrop)
                    {
                        AppendLink(EnumX_ColumnX, m.Item, d.Item);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var ex = m.EnumX;
                    if (!EnumX_ColumnX.TryGetChildren(ex, out IList<ColumnX> list)) return (false, false);

                    m.InitChildModels(prev);
                    var anyChange = false;
                    foreach (var cx in list)
                    {
                        if (TableX_ColumnX.TryGetParent(cx, out TableX tx))
                        {
                            anyChange |= AddChildModel(prev, m, Trait.EnumRelatedColumn_M, cx, tx, ex, EnumRelatedColumn_X);
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 666 ComputeXList  =============================================
        ModelAction ComputeXList_X;
        void Initialize_ComputeXList_X()
        {
            ComputeXList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = Store_ComputeX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var st = m.Store;
                    if (!Store_ComputeX.TryGetChildren(st, out IList<ComputeX> list)) return (false, false);

                    m.InitChildModels(prev);
                    var anyChange = false;
                    foreach (var itm in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.ComputeX_M, itm, Store_ComputeX, st, ComputeX_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var st = model.Item as Store;
                var cx = new ComputeX(_computeXStore);
                ItemCreated(cx);
                AppendLink(Store_ComputeX, st, cx);

                CreateQueryX(cx, st);
            }
        }
        #endregion



        #region 671 ChildRelationX  ===========================================
        ModelAction ChildRelationX_X;
        void Initialize_ChildRelationX_X()
        {
            ChildRelationX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.RelationX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsTableX) return DropAction.None;

                    if (doDrop)
                    {
                        AppendLink(TableX_ParentRelationX, d.Item, m.Item);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.IsExpandedRight) return (false, false);
                    if (m.ChildModelCount == 4) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _relationXNameProperty);
                    AddProperyModel(prev, m, _relationXSummaryProperty);
                    AddProperyModel(prev, m, _relationXPairingProperty);
                    AddProperyModel(prev, m, _relationXIsRequiredProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.RelationX, IdentityStyle.Single));
        }
        #endregion

        #region 672 ParentRelationX  ==========================================
        ModelAction ParentRelationX_X;
        void Initialize_ParentRelationX_X()
        {
            ParentRelationX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.RelationX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsTableX) return DropAction.None;

                    if (doDrop)
                    {
                        AppendLink(TableX_ChildRelationX, d.Item, m.Item);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.IsExpandedRight) return (false, false);
                    if (m.ChildModelCount == 5) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _relationXNameProperty);
                    AddProperyModel(prev, m, _relationXSummaryProperty);
                    AddProperyModel(prev, m, _relationXPairingProperty);
                    AddProperyModel(prev, m, _relationXIsRequiredProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.RelationX, IdentityStyle.Single));
        }
        #endregion

        #region 673 NameColumnRelation  =======================================
        ModelAction NameColumnRelation_X;
        void Initialize_NameColumnRelation_X()
        {
            NameColumnRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_NameProperty.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Property)) return DropAction.None;

                    if (doDrop)
                    {
                        if (m.IsChildModel(d))
                            RemoveLink(TableX_NameProperty, m.Item, d.Item);
                        else
                        {
                            AppendLink(TableX_NameProperty, m.Item, d.Item);
                            m.IsExpandedLeft = true;
                        }
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (!TableX_NameProperty.TryGetChild(tx, out Property pr)) return (false, false);
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.NameColumn_M, pr, TableX_NameProperty, tx, NameColumn_X);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 674 SummaryColumnRelation  ====================================
        ModelAction SummaryColumnRelation_X;
        void Initialize_SummaryColumnRelation_X()
        {
            SummaryColumnRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_SummaryProperty.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Property)) return DropAction.None;

                    if (doDrop)
                    {
                        if (m.IsChildModel(d))
                            RemoveLink(TableX_SummaryProperty, m.Item, d.Item);
                        else
                        {
                            AppendLink(TableX_SummaryProperty, m.Item, d.Item);
                            m.IsExpandedLeft = true;
                        }
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (!TableX_SummaryProperty.TryGetChild(tx, out Property pr)) return (false, false);
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.SummaryColumn_M, pr, TableX_SummaryProperty, tx, SummaryColumn_X);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 675 NameColumn  ===============================================
        ModelAction NameColumn_X;
        void Initialize_NameColumn_X()
        {
            NameColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) =>
                {
                    if (m.Item.IsColumnX) return m.ColumnX.Summary;
                    if (m.Item.IsComputeX) return m.ComputeX.Summary;
                    throw new Exception("Corrupt ItemModelTree");
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedChild));
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m)
            {
                if (m.Item.IsColumnX) return (null, m.ColumnX.Name);
                if (m.Item.IsComputeX) return (null, m.ComputeX.Name);
                throw new Exception("Corrupt ItemModelTree");
            }
        }
        #endregion

        #region 676 SummaryColumn  ============================================
        ModelAction SummaryColumn_X;
        void Initialize_SummaryColumn_X()
        {
            SummaryColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) =>
                {
                    if (m.Item.IsColumnX) return m.ColumnX.Summary;
                    if (m.Item.IsComputeX) return m.ComputeX.Summary;
                    throw new Exception("Corrupt ItemModelTree");
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedChild));
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m)
            {
                if (m.Item.IsColumnX) return (null, m.ColumnX.Name);
                if (m.Item.IsComputeX) return (null, m.ComputeX.Name);
                throw new Exception("Corrupt ItemModelTree");
            }
        }
        #endregion



        #region 681 GraphXColoring  ===========================================
        ModelAction GraphXColoring_X;
        void Initialize_GraphXColoring_X()
        {
            GraphXColoring_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = GraphX_ColorColumnX.ChildCount(m.GraphX);

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var gx = m.GraphX;
                    var col = d.ColumnX;
                    if (col == null) return DropAction.None;
                    if (!gx.IsGraphX) return DropAction.None;

                    if (doDrop)
                    {
                        AppendLink(GraphX_ColorColumnX, gx, col);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!(GraphX_ColorColumnX.TryGetChild(m.Item, out ColumnX cx) && TableX_ColumnX.TryGetParent(cx, out TableX tx))) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = AddChildModel(prev, m, Trait.GraphXColorColumn_M, cx, tx, null, GraphXColorColumn_X);

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 682 GraphXRootList  ===========================================
        ModelAction GraphXRootList_X;
        void Initialize_GraphXRootList_X()
        {
            GraphXRootList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = GraphX_QueryX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is GraphX gx)) return DropAction.None;
                    if (!(d.Item is Store st)) return DropAction.None;
                    if (GraphXAlreadyHasThisRoot(gx, st)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(gx, st);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var gx = m.GraphX;
                    if (!GraphX_QueryX.TryGetChildren(gx, out IList<QueryX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphXRoot_M, itm, gx, null, QueryXRoot_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            bool GraphXAlreadyHasThisRoot(Item gd, Item table)
            {
                if (GraphX_QueryX.TryGetChildren(gd, out IList<QueryX> list))
                {
                    foreach (var qx in list)
                    {
                        if (Store_QueryX.ContainsLink(table, qx)) return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region 683 GraphXNodeList  ===========================================
        ModelAction GraphXNodeList_X;
        void Initialize_GraphXNodeList_X()
        {
            GraphXNodeList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = GetNodeOwners(m.GraphX).Count;

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var gx = m.GraphX;
                    var owners = GetNodeOwners(gx);
                    if (owners.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var sto in owners)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphXNode_M, sto, gx, null, GraphXNode_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 684 GraphXNode  ===============================================
        ModelAction GraphXNode_X;
        void Initialize_GraphXNode_X()
        {
            GraphXNode_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = GetSymbolQueryXCount(m.GraphX, m.Store);

                    m.CanExpandLeft = (count > 0);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is Store st)) return DropAction.None;
                    if (!(m.Aux1 is GraphX gx)) return DropAction.None;
                    if (!(d.Item is SymbolX sx)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(gx, sx, st);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var gx = m.GraphX;
                    var st = m.Store;

                    (var symbols, var querys) = GetSymbolXQueryX(gx, st);
                    if (querys == null) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var qx in querys)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphXNodeSymbol_M, qx, GraphX_SymbolQueryX, gx, GraphXNodeSymbol_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetIdentity(m.Item, IdentityStyle.Single));
        }
        #endregion

        #region 685 GraphXColorColumn  ========================================
        ModelAction GraphXColorColumn_X;
        void Initialize_GraphXColorColumn_X()
        {
            GraphXColorColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), $"{m.TableX.Name} : {m.ColumnX.Name}");
        }
        #endregion



        #region 691 QueryXRoot  ===============================================
        ModelAction QueryXRoot_X;
        void Initialize_QueryXRoot_X()
        {
            QueryXRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!(m.Item is QueryX qx)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Graph);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.QueryX;
                    if (!QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var qc in list)
                    {
                        if (qc.IsPath)
                        {
                            if (qc.IsHead)
                                anyChange |= AddChildModel(prev, m, Trait.GraphXPathHead_M, qc, null, null, QueryXPathHead_X);
                            else
                                anyChange |= AddChildModel(prev, m, Trait.GraphXPathLink_M, qc, null, null, QueryXPathLink_X);
                        }
                        else if (qc.IsGroup)
                        {
                            if (qc.IsHead)
                                anyChange |= AddChildModel(prev, m, Trait.GraphXGroupHead_M, qc, null, null, QueryXGroupHead_X);
                            else
                                anyChange |= AddChildModel(prev, m, Trait.GraphXGroupLink_M, qc, null, null, QueryXGroupLink_X);
                        }
                        else if (qc.IsSegue)
                        {
                            if (qc.IsHead)
                                anyChange |= AddChildModel(prev, m, Trait.GraphXEgressHead_M, qc, null, null, QueryXEgressHead_X);
                            else
                                anyChange |= AddChildModel(prev, m, Trait.GraphXEgressLink_M, qc, null, null, QueryXEgressLink_X);
                        }
                        else
                        {
                            anyChange |= AddChildModel(prev, m, Trait.GraphXLink_M, qc, null, null, QueryXLink_X);
                        }
                    }
                    return(true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m)
            {
                var name = (Store_QueryX.TryGetParent(m.Item, out Store sto)) ? GetIdentity(sto, IdentityStyle.Single) : Chef.BlankName;
                return (_localize(m.NameKey), name);
            }
        }
        #endregion

        #region 692 QueryXLink  ===============================================
        ModelAction QueryXLink_X;
        void Initialize_QueryXLink_X()
        {
            QueryXLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                    mc.Add(new ModelCommand(this, m, Trait.MakePathHeadCommand, MakePathtHead));
                    mc.Add(new ModelCommand(this, m, Trait.MakeGroupHeadCommand, MakeGroupHead));
                    mc.Add(new ModelCommand(this, m, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (d.Item.IsRelationX)
                    {
                        if (doDrop)
                        {
                            var qx = m.Item as QueryX;
                            var re = d.Item as Relation;
                            CreateQueryX(qx, re, QueryType.Graph);
                        }
                        return DropAction.Link;
                    }
                    else if (m.Item.IsQueryGraphLink)
                    {
                        if (doDrop)
                        {
                            RemoveItem(d);
                        }
                        return DropAction.Link;
                    }
                    return DropAction.None;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                if (qc.IsPath)
                                {
                                    if (qc.IsHead)
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXPathHead_M, qc, null, null, QueryXPathHead_X);
                                    else
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXPathLink_M, qc, null, null, QueryXPathLink_X);
                                }
                                else if (qc.IsGroup)
                                {
                                    if (qc.IsHead)
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXGroupHead_M, qc, null, null, QueryXGroupHead_X);
                                    else
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXGroupLink_M, qc, null, null, QueryXGroupLink_X);
                                    break;
                                }
                                else if (qc.IsSegue)
                                {
                                    if (qc.IsHead)
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXEgressHead_M, qc, null, null, QueryXEgressHead_X);
                                    else
                                        anyChange |= AddChildModel(prev, m, Trait.GraphXEgressLink_M, qc, null, null, QueryXEgressLink_X);
                                }
                                else
                                {
                                    anyChange |= AddChildModel(prev, m, Trait.GraphXLink_M, qc, null, null, QueryXLink_X);
                                }
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        #endregion

        #region 693 QueryXPathHead  ===========================================
        ModelAction QueryXPathHead_X;
        void Initialize_QueryXPathHead_X()
        {
            QueryXPathHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                    mc.Add(new ModelCommand(this, m, Trait.MakeRootLinkCommand, MakeRootLink));
                    mc.Add(new ModelCommand(this, m, Trait.MakeGroupHeadCommand, MakeGroupHead));
                    mc.Add(new ModelCommand(this, m, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Path);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsBreakPointProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXConnect1Property);
                        anyChange |= AddProperyModel(prev, m, _queryXConnect2Property);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXPathLink_M, qc, null, null, QueryXPathLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXHeadName(m));
        }
        #endregion

        #region 694 QueryXPathLink  ===========================================
        ModelAction QueryXPathLink_X;
        void Initialize_QueryXPathLink_X()
        {
            QueryXPathLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Path);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsBreakPointProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXPathLink_M, qc, null, null, QueryXPathLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        #endregion

        #region 695 QueryXGroupHead  ==========================================
        ModelAction QueryXGroupHead_X;
        void Initialize_QueryXGroupHead_X()
        {
            QueryXGroupHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                    mc.Add(new ModelCommand(this, m, Trait.MakeRootLinkCommand, MakeRootLink));
                    mc.Add(new ModelCommand(this, m, Trait.MakePathHeadCommand, MakePathtHead));
                    mc.Add(new ModelCommand(this, m, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Group);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXGroupLink_M, qc, null, null, QueryXGroupLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXHeadName(m));
        }
        #endregion

        #region 696 QueryXGroupLink  ==========================================
        ModelAction QueryXGroupLink_X;
        void Initialize_QueryXGroupLink_X()
        {
            QueryXGroupLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Group);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXGroupLink_M, qc, null, null, QueryXGroupLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        #endregion

        #region 697 QueryXEgressHead  =========================================
        ModelAction QueryXEgressHead_X;
        void Initialize_QueryXEgressHead_X()
        {
            QueryXEgressHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                    mc.Add(new ModelCommand(this, m, Trait.MakeRootLinkCommand, MakeRootLink));
                    mc.Add(new ModelCommand(this, m, Trait.MakePathHeadCommand, MakePathtHead));
                    mc.Add(new ModelCommand(this, m, Trait.MakeGroupHeadCommand, MakeGroupHead));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Segue);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXEgressLink_M, qc, null, null, QueryXEgressLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXHeadName(m));
        }
        #endregion

        #region 698 QueryXEgressLink  =========================================
        ModelAction QueryXEgressLink_X;
        void Initialize_QueryXEgressLink_X()
        {
            QueryXEgressLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!CanDropQueryXRelation(qx, d.Item as RelationX)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Segue);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.GraphXEgressLink_M, qc, null, null, QueryXEgressLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        #endregion

        #region 699 GraphXNodeSymbol  =========================================
        ModelAction GraphXNodeSymbol_X;
        void Initialize_GraphXNodeSymbol_X()
        {
            GraphXNodeSymbol_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetQueryXRelationName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 1) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _queryXWhereProperty);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), (SymbolX_QueryX.TryGetParent(m.Item, out SymbolX sym)) ? sym.Name : null);
        }
        #endregion



        #region 69E ValueXHead  ===============================================
        ModelAction ValueHead_X;
        void Initialize_ValueHead_X()
        {
            ValueHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => QueryXComputeName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    GetHeadTail(qx, out Store tb1Head, out Store tb1Tail);
                    GetHeadTail(re, out Store tb2Head, out Store tb2Tail);
                    if ((tb1Head != tb2Head && tb1Head != tb2Tail) && (tb1Tail != tb2Head && tb1Tail != tb2Tail)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Value).IsReversed = (tb1Tail == tb2Tail);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        if (qx.HasSelect)
                        {
                            anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXSelectProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXValueTypeProperty);

                        }
                        else
                        {
                            anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                            anyChange |= AddProperyModel(prev, m, _queryXSelectProperty);
                        }
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ValueXLink_M, qc, null, null, ValueLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        #endregion

        #region 69F ValueXLink  ===============================================
        ModelAction ValueLink_X;
        void Initialize_ValueLink_X()
        {
            ValueLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => QueryXComputeName(m),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!(d.Item is Relation re)) return DropAction.None;
                    if (!(m.Item is QueryX qx)) return DropAction.None;
                    GetHeadTail(qx, out Store tb1Head, out Store tb1Tail);
                    GetHeadTail(re, out Store tb2Head, out Store tb2Tail);
                    if ((tb1Head != tb2Head && tb1Head != tb2Tail) && (tb1Tail != tb2Head && tb1Tail != tb2Tail)) return DropAction.None;

                    if (doDrop)
                    {
                        CreateQueryX(qx, re, QueryType.Value).IsReversed = (tb1Tail == tb2Tail);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var qx = m.Item as QueryX;
                    if (!m.IsExpandedRight && m.IsExpandedLeft && QueryX_QueryX.ChildCount(qx) == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _queryXRelationProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXIsReversedProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXRootWhereProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXSelectProperty);
                        anyChange |= AddProperyModel(prev, m, _queryXValueTypeProperty);
                    }

                    if (m.IsExpandedLeft)
                    {
                        if (QueryX_QueryX.TryGetChildren(qx, out IList<QueryX> list))
                        {
                            foreach (var qc in list)
                            {
                                anyChange |= AddChildModel(prev, m, Trait.ValueXLink_M, qc, null, null, ValueLink_X);
                            }
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXLinkName(m));
        }
        string QueryXComputeName(ItemModel m)
        {
            var sd = m.Item as QueryX;

            GetHeadTail(sd, out Store head1, out Store tail1);
            var sd2 = GetValueeDefTail(sd);
            GetHeadTail(sd2, out Store head2, out Store tail2);

            StringBuilder sb = new StringBuilder(132);
            sb.Append(GetIdentity(head1, IdentityStyle.Single));
            sb.Append(parentNameSuffix);
            sb.Append(GetIdentity(tail2, IdentityStyle.Single));
            return sb.ToString();

            QueryX GetValueeDefTail(QueryX q)
            {
                var q2 = q;
                var q3 = q2;
                while (q3 != null)
                {
                    q2 = q3;
                    QueryX_QueryX.TryGetChild(q3, out q3);
                }
                return q2;
            }
        }
        #endregion



        #region 6A1 RowX  =====================================================
        ModelAction RowX_X;
        void Initialize_Row_X()
        {
            RowX_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);


                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = m.RowX.TableX.HasChoiceColumns;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = RowX_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.Item, IdentityStyle.Single));
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        private (bool, bool) RowX_VX(ItemModel m, List<ItemModel> prev)
        {
            var rx = m.RowX;
            if (!m.IsExpandedLeft && m.IsExpandedRight && !HasChoiceColumns(rx.TableX)) return (false, false);

            m.InitChildModels(prev);

            var anyChange = false;
            if (m.IsExpandedRight && TryGetChoiceColumns(rx.TableX, out IList<ColumnX> columns))
            {
                anyChange |= AddProperyModels(prev, m, columns);
            }

            if (m.IsExpandedLeft)
            {
                anyChange = AddChildModel(prev, m, Trait.RowPropertyList_M, rx, TableX_ColumnX, null, RowPropertyList_X);
                anyChange = AddChildModel(prev, m, Trait.RowComputeList_M, rx, Store_ComputeX, null, RowComputeList_X);
                anyChange = AddChildModel(prev, m, Trait.RowChildRelationList_M, rx, TableX_ChildRelationX, null, RowChildRelationList_X);
                anyChange = AddChildModel(prev, m, Trait.RowParentRelationList_M, rx, TableX_ParentRelationX, null, RowParentRelationList_X);
            }
            return (true, anyChange);
        }

        private DropAction ReorderStoreItem(ItemModel m, ItemModel d, bool doDrop)
        {
            if (!(m.Item.Owner is Store sto)) return DropAction.None;
            if (!m.IsSiblingModel(d)) return DropAction.None;
            
            var item1 = d.Item;
            var item2 = m.Item;
            var index1 = sto.IndexOf(item1);
            var index2 = sto.IndexOf(item2);
            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop)
            {
                ItemMoved(d.Item, index1, index2);
            }
            return DropAction.Move;
        }
        #endregion

        #region 6A3 View  =====================================================
        ModelAction View_X;
        void Initialize_View_X()
        {
            View_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ViewX.Summary,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.ViewX.Name);
        }
        #endregion

        #region 6A4 TableX  ===================================================
        ModelAction Table_X;
        void Initialize_Table_X()
        {
            Table_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.TableX.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.TableX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var tx = m.TableX;
                    if (tx.Count == 0) return (false, false);
                    if (tx.Delta == m.Delta) return (true, false);

                    m.Delta = tx.Delta;
                    m.InitChildModels(prev, tx.Count);

                    var items = tx.Items;
                    var anyChange = (items.Count != prev.Count);
                    if (TableX_NameProperty.TryGetChild(tx, out Property cx))
                    {
                        foreach (var rx in items)
                        {
                            anyChange |= AddChildModel(prev, m, Trait.Row_M, rx, tx, cx, RowX_X);
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.TableX.Name);

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel m) => ItemCreated(new RowX(m.TableX));
        }
        #endregion

        #region 6A5 Graph  ====================================================
        ModelAction Graph_X;
        void Initialize_Graph_X()
        {
            Graph_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.Graph.GraphX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryModelGraph));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 5) return (true, false);

                    var g = m.Graph;
                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.GraphNodeList_M, g, null, null, GraphNodeList_X);
                    AddChildModel(prev, m, Trait.GraphEdgeList_M, g, null, null, GraphEdgeList_X);
                    AddChildModel(prev, m, Trait.GraphOpenList_M, g, null, null, GraphOpenList_X);
                    AddChildModel(prev, m, Trait.GraphRootList_M, g, null, null, GraphRootList_X);
                    AddChildModel(prev, m, Trait.GraphLevelList_M, g, null, null, GraphLevelList_X);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), m.Graph.GraphX.Name);

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondaryModelGraph(ItemModel m) => m.GetRootModel().UIRequestCreatePage(ControlType.GraphDisplay, Trait.GraphRef_M, m.Graph, GraphRef_X);
        }
        #endregion

        #region 6A6 GraphRef  =================================================
        ModelAction GraphRef_X;
        void Initialize_GraphRef_X()
        {
            GraphRef_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.Graph.GraphX.Summary,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), m.Graph.GraphX.Name);
        }
        #endregion

        #region 6A7 RowChildRelation  =========================================
        ModelAction RowChildRelation_X;
        void Initialize_RowChildRelation_X()
        {
            RowChildRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Relation.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.RelationX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsRowX) return DropAction.None;
                    if (!m.Item.IsRowX) return DropAction.None;
                    if (!m.Aux1.IsRelationX) return DropAction.None;
                    if (!TableX_ParentRelationX.TryGetParent(m.Aux1, out TableX expectedOwner)) return DropAction.None;
                    if (d.Item.Owner != expectedOwner) return DropAction.None;

                    if (doDrop)
                    {
                        var rel = m.Aux1 as RelationX;
                        if (m.IsChildModel(d))
                            RemoveLink(rel, m.Item, d.Item);
                        else
                            AppendLink(rel, m.Item, d.Item);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var rx = m.RowX;
                    var re = m.RelationX;
                    if (!re.TryGetChildren(rx, out IList<RowX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var rr in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.RowRelatedChild_M, rr, re, rx, RowRelatedChild_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetRelationName(m.RelationX));
        }
        #endregion

        #region 6A8 RowParentRelation  ========================================
        ModelAction RowParentRelation_X;
        void Initialize_RowParentRelation_X()
        {
            RowParentRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Relation.ParentCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    if (!d.Item.IsRowX) return DropAction.None;
                    if (!m.Item.IsRowX) return DropAction.None;
                    if (!m.Aux1.IsRelationX) return DropAction.None;
                    if (!TableX_ChildRelationX.TryGetParent(m.Aux1, out TableX expectedOwner)) return DropAction.None;
                    if (d.Item.Owner != expectedOwner) return DropAction.None;

                    if (doDrop)
                    {
                        var rel = m.Aux1 as RelationX;
                        if (m.IsChildModel(d))
                            RemoveLink(rel, d.Item, m.Item);
                        else
                            AppendLink(rel, d.Item, m.Item);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var rx = m.RowX;
                    var re = m.RelationX;
                    if (!re.TryGetParents(rx, out IList<RowX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var rr in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.RowRelatedParent_M, rr, re, rx, RowRelatedParent_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetRelationName(m.RelationX));
        }
        #endregion

        #region 6A9 RowRelatedChild  ==========================================
        ModelAction RowRelatedChild_X;
        void Initialize_RowRelatedChild_X()
        {
            RowRelatedChild_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandLeft = true;

                    return (kind, name, m.RelationX.ChildCount(m.RowX), ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => RowSummary(m.RowX),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedChild));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = RowX_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (m.RowX.TableX.Name, GetRowName(m.RowX));
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        void UnlinkRelatedChild(ItemModel m)
        {
            var key = m.Aux2;
            var rel = m.Aux1 as Relation;
            var item = m.Item;
            RemoveLink(rel, key, item);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        DropAction ReorderRelatedChild (ItemModel model, ItemModel drop, bool doDrop)
        {
            if (model.Aux2 == null) return DropAction.None;
            if (model.Aux1 == null || !(model.Aux1 is Relation rel)) return DropAction.None;

            var key = model.Aux2;
            var item1 = drop.Item;
            var item2 = model.Item;
            (int index1, int index2) = rel.GetChildrenIndex(key, item1, item2);

            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop) ItemChildMoved(rel, key, item1, index1, index2);

            return DropAction.Move;
        }
        #endregion

        #region 6AA RowRelatedParent  =========================================
        ModelAction RowRelatedParent_X;
        void Initialize_RowRelatedParent_X()
        {
            RowRelatedParent_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Relation.ParentCount(m.RowX);

                    m.CanDrag = true;
                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => RowSummary(m.RowX),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = (m, d, doDrop) =>
                {
                    if (m.Aux2 == null) return DropAction.None;
                    if (m.Aux1 == null || !(m.Aux1 is Relation rel)) return DropAction.None;

                    var key = m.Aux2;
                    var item1 = d.Item;
                    var item2 = m.Item;
                    (int index1, int index2) = rel.GetParentsIndex(key, item1, item2);

                    if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

                    if (doDrop) ItemParentMoved(rel, key, item1, index1, index2);

                    return DropAction.Move;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedParent));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = RowX_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (m.RowX.TableX.Name, GetRowName(m.RowX));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void UnlinkRelatedParent(ItemModel m)
            {
                var key = m.Item;
                var rel = m.Aux1 as Relation;
                var item = m.Aux2;
                RemoveLink(rel, key, item);
            }
        }
        #endregion

        #region 6AB EnumRelatedColumn  ========================================
        ModelAction EnumRelatedColumn_X;
        void Initialize_EnumRelatedColumn_X()
        {
            EnumRelatedColumn_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedColumn));
                },
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), $"{m.TableX.Name}: {m.ColumnX.Name}");

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void UnlinkRelatedColumn(ItemModel m)
            {
                var col = m.Item;
                var tbl = m.Aux1;
                var enu = m.Aux2;
                RemoveLink(EnumX_ColumnX, enu, col);
            }
        }
        #endregion



        #region 6B1 RowPropertyList  ==========================================
        ModelAction RowPropertyList_X;
        void Initialize_RowPropertyList_X()
        {
            RowPropertyList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ColumnX.ChildCount(m.RowX.TableX);

                    m.CanFilterUsage = count > 0;
                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelUsed = (m,cm) => cm.ColumnX.Value.IsSpecific(cm.RowX),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var rx = m.RowX;
                    if (!TableX_ColumnX.TryGetChildren(rx.TableX, out IList<ColumnX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = AddProperyModels(prev, m, list);

                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B2 RowChildRelationList  =====================================
        ModelAction RowChildRelationList_X;
        void Initialize_RowChildRelationList_X()
        {
            RowChildRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ChildRelationX.ChildCount(m.RowX.TableX);

                    m.CanFilterUsage = count > 0;
                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =
                
                ModelUsed = (m,cm) => cm.RelationX.ChildCount(cm.RowX) > 0,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var rx = m.RowX;
                    if (!TableX_ChildRelationX.TryGetChildren(rx.TableX, out IList<RelationX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var rel in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.RowChildRelation_M, rx, rel, null, RowChildRelation_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B3 RowParentRelationList  ====================================
        ModelAction RowParentRelationList_X;
        void Initialize_RowParentRelationList_X()
        {
            RowParentRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = TableX_ParentRelationX.ChildCount(m.RowX.TableX);

                    m.CanFilterUsage = count > 0;
                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelUsed = (m, cm) => cm.RelationX.ParentCount(cm.RowX) > 0,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var rx = m.RowX;
                    if (!TableX_ParentRelationX.TryGetChildren(rx.TableX, out IList<RelationX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var re in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.RowParentRelation_M, rx, re, null, RowParentRelation_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B7 RowComputeList  ===========================================
        ModelAction RowComputeList_X;
        void Initialize_RowComputeList_X()
        {
            RowComputeList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = Store_ComputeX.ChildCount(m.Item.Owner);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var itm = m.Item;
                    if (!Store_ComputeX.TryGetChildren(itm.Owner, out IList<ComputeX> items)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var cx in items)
                    {
                        anyChange = AddChildModel(prev, m, Trait.TextProperty_M, itm, cx, null, TextCompute_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion



        #region 6C1 QueryRootLink  ============================================
        ModelAction QueryRootLink_X;
        void Initialize_QueryRootLink_X()
        {
            QueryRootLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var q = m.Query;
                    var items = q.Items;
                    if (items == null) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange = AddChildModel(prev, m, Trait.QueryRootItem_M, itm, q, null, QueryRootItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private string QueryLinkName(ItemModel modle)
        {
            var s = modle.Query;
            return QueryXFilterName(s.QueryX);
        }
        #endregion

        #region 6C2 QueryPathHead  ============================================
        ModelAction QueryPathHead_X;
        void Initialize_QueryPathHead_X()
        {
            QueryPathHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryPathLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        #endregion

        #region 6C3 QueryPathLink  ============================================
        ModelAction QueryPathLink_X;
        void Initialize_QueryPathLink_X()
        {
            QueryPathLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryPathLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private (bool, bool) QueryPathLink_VX(ItemModel m, List<ItemModel> prev)
        {
            var q = m.Query;
            var items = q.Items;
            if (items == null) return (false, false);

            m.InitChildModels(prev);

            var anyChange = false;
            if (q.IsTail)
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryPathTail_M, itm, q, null, QueryPathTail_X);
                }
            else
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryPathStep_M, itm, q, null, QueryPathStep_X);
                }
            return (true, anyChange);
        }
        #endregion

        #region 6C4 QueryGroupHead  ===========================================
        ModelAction QueryGroupHead_X;
        void Initialize_QueryGroupHead_X()
        {
            QueryGroupHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryGroupLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        #endregion

        #region 6C5 QueryGroupLink  ===========================================
        ModelAction QueryGroupLink_X;
        void Initialize_QueryGroupLink_X()
        {
            QueryGroupLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryGroupLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private (bool, bool) QueryGroupLink_VX(ItemModel m, List<ItemModel> prev)
        {
            var q = m.Query;
            var items = q.Items;
            if (items == null) return (false, false);

            m.InitChildModels(prev);

            var anyChange = false;
            if (q.IsTail)
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryGroupTail_M, itm, q, null, QueryGroupTail_X);
                }
            else
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryGroupStep_M, itm, q, null, QueryGroupStep_X);
                }
            return (true, anyChange);
        }
        #endregion

        #region 6C6 QueryEgressHead  ==========================================
        ModelAction QueryEgressHead_X;
        void Initialize_QueryEgressHead_X()
        {
            QueryEgressHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryEgressLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        #endregion

        #region 6C7 QueryEgressLink  ==========================================
        ModelAction QueryEgressLink_X;
        void Initialize_QueryEgressLink_X()
        {
            QueryEgressLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = QueryEgressLink_VX,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private (bool, bool) QueryEgressLink_VX(ItemModel m, List<ItemModel> prev)
        {
            var q = m.Query;
            var items = q.Items;
            if (items == null) return (false, false);

            m.InitChildModels(prev);

            var anyChange = false;
            if (q.IsTail)
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryEgressTail_M, itm, q, null, QueryEgressTail_X);
                }
            else
                foreach (var itm in items)
                {
                    anyChange |= AddChildModel(prev, m, Trait.QueryEgressStep_M, itm, q, null, QueryEgressStep_X);
                }
            return (true, anyChange);
        }
        #endregion



        #region 6D1 QueryRootItem  ============================================
        ModelAction QueryRootItem_X;
        void Initialize_QueryRootItem_X()
        {
            QueryRootItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Query.TryGetQuerys(m.Item, out Query[] items)) return (false, false);

                    m.InitChildModels(prev);

                    var itm = m.Item;
                    var anyChange = false;
                    foreach (var q in items)
                    {
                        if (q.IsGraphLink)
                            anyChange |= AddChildModel(prev, m, Trait.QueryRootLink_M, itm, q, null, QueryRootLink_X);
                        else if (q.IsPathHead)
                            anyChange |= AddChildModel(prev, m, Trait.QueryPathHead_M, itm, q, null, QueryPathHead_X);
                        else if (q.IsGroupHead)
                            anyChange |= AddChildModel(prev, m, Trait.QueryGroupHead_M, itm, q, null, QueryGroupHead_X);
                        else if (q.IsSegueHead)
                            anyChange |= AddChildModel(prev, m, Trait.QueryEgressHead_M, itm, q, null, QueryEgressHead_X);
                        else
                            throw new Exception("Invalid Query");
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (m.RowX.TableX.Name, GetRowName(m.RowX));
        }
        #endregion

        #region 6D2 QueryPathStep  ============================================
        ModelAction QueryPathStep_X;
        void Initialize_QueryPathStep_X()
        {
            QueryPathStep_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Query.TryGetQuerys(m.Item, out Query[] items)) return (false, false);

                    m.InitChildModels(prev);

                    var itm = m.Item;
                    var anyChange = false;
                    foreach (var q in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.QueryPathLink_M, itm, q, null, QueryPathLink_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion

        #region 6D3 QueryPathTail  ============================================
        ModelAction QueryPathTail_X;
        void Initialize_QueryPathTail_X()
        {
            QueryPathTail_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion

        #region 6D4 QueryGroupStep  ===========================================
        ModelAction QueryGroupStep_X;
        void Initialize_QueryGroupStep_X()
        {
            QueryGroupStep_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Query.TryGetQuerys(m.Item, out Query[] items)) return (false, false);

                    m.InitChildModels(prev);

                    var itm = m.Item;
                    var anyChange = false;
                    foreach (var q in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.QueryGroupLink_M, itm, q, null, QueryGroupLink_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion

        #region 6D5 QueryGroupTail  ===========================================
        ModelAction QueryGroupTail_X;
        void Initialize_QueryGroupTail_X()
        {
            QueryGroupTail_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion

        #region 6D6 QueryEgressStep  ==========================================
        ModelAction QueryEgressStep_X;
        void Initialize_QueryEgressStep_X()
        {
            QueryEgressStep_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Query.TryGetQuerys(m.Item, out Query[] items)) return (false, false);

                    m.InitChildModels(prev);

                    var itm = m.Item;
                    var anyChange = false;
                    foreach (var q in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.QueryEgressLink_M, itm, q, null, QueryEgressLink_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion

        #region 6D7 QueryEgressTail  ==========================================
        ModelAction QueryEgressTail_X;
        void Initialize_QueryEgressTail_X()
        {
            QueryEgressTail_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX));
        }
        #endregion



        #region 6E1 GraphXRef  ================================================
        ModelAction GraphXRef_X;
        void Initialize_GraphXRef_X()
        {
            GraphXRef_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.GraphX.Count;

                    m.CanExpandLeft = count > 0;

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.GraphX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.CreateCommand, CreateGraph));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var gx = m.GraphX;
                    Store st = null;

                    if (GraphX_QueryX.ChildCount(gx) == 0) return DropAction.None;

                    if (GraphX_QueryX.TryGetChildren(gx, out IList<QueryX> list))
                    {
                        foreach (var item in list)
                        {
                            if (item.IsQueryGraphRoot && Store_QueryX.TryGetParent(item, out st) && d.Item.Owner == st) break;
                        }
                        if (st == null) return DropAction.None;
                    }

                    foreach (var tg in gx.Items)
                    {
                        if (tg.SeedItem == d.Item) return DropAction.None;
                    }

                    if (doDrop)
                    {
                        CreateGraph(gx, out Graph g, d.Item);

                        m.IsExpandedLeft = true;

                        var root = m.GetRootModel();
                        root.UIRequestCreatePage(ControlType.GraphDisplay, Trait.GraphRef_M, GraphRef_X, m);
                    }
                    return DropAction.Copy;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var gx = m.GraphX;
                    if (gx.Count == 0) return (false, false);
                    if (gx.Delta == m.Delta) return (true, false);

                    m.Delta = gx.Delta;
                    m.InitChildModels(prev);

                    var anyChange = false;
                    var items = gx.Items;
                    foreach (var g in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.Graph_M, g, null, null, Graph_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (m.GraphX.Trait.ToString(), m.GraphX.Name);
        }

        void CreateGraph(ItemModel m)
        {
            CreateGraph(m.GraphX, out Graph g);

            m.IsExpandedLeft = true;

            var root = m.GetRootModel();
            root.UIRequestCreatePage(ControlType.GraphDisplay, Trait.GraphRef_M, g, GraphRef_X);
        }
        #endregion

        #region 6E2 GraphNodeList  ============================================
        ModelAction GraphNodeList_X;
        void Initialize_GraphNodeList_X()
        {
            GraphNodeList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Graph.NodeCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var g = m.Graph;
                    var items = g.Nodes;
                    if (items.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphNode_M, itm, null, null, GraphNode_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6E3 GraphEdgeList  ============================================
        ModelAction GraphEdgeList_X;
        void Initialize_GraphEdgeList_X()
        {
            GraphEdgeList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Graph.EdgeCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var g = m.Graph;
                    var items = g.Edges;
                    if (items.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphEdge_M, itm, null, null, GraphEdge_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6E4 GraphRootList  ============================================
        ModelAction GraphRootList_X;
        void Initialize_GraphRootList_X()
        {
            GraphRootList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Graph.QueryCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var g = m.Item as Graph;
                    var items = g.Forest;
                    if (items == null) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var q in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphRoot_M, q.Item, q, null, GraphRoot_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6E5 GraphLevelList  ===========================================
        ModelAction GraphLevelList_X;
        void Initialize_GraphLevelList_X()
        {
            GraphLevelList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Graph.Levels.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var items = m.Graph.Levels;
                    if (items.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var lv in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphLevel_M, lv, null, null, GraphLevel_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6E6 GraphLevel  ===============================================
        ModelAction GraphLevel_X;
        void Initialize_GraphLevel_X()
        {
            GraphLevel_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Level.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var paths = m.Level.Paths;
                    if (paths.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var p in paths)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphPath_M, p, null, null, GraphPath_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), m.Level.Name);
        }
        #endregion

        #region 6E7 GraphPath  ================================================
        ModelAction GraphPath_X;
        void Initialize_GraphPath_X()
        {
            GraphPath_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Path.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.Path.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var items = m.Path.Paths;
                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.GraphPath_M, itm, null, null, GraphPath_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetPathKind(m.Path), GetPathName(m.Path));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            string GetPathName(Path path)
            {
                return GetHeadTailName(path.Head, path.Tail);
            }

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            string GetPathKind(Path path)
            {
                var name = _localize(path.NameKey);
                var kind = path.IsRadial ? _localize(GetKindKey(Trait.RadialPath)) : _localize(GetKindKey(Trait.LinkPath));
                return $"{name}{kind}";
            }
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        string GetHeadTailName(Item head, Item tail)
        {
            var headName = GetIdentity(head, IdentityStyle.Double);
            var tailName = GetIdentity(tail, IdentityStyle.Double);
            return $"{headName} --> {tailName}";
        }
        #endregion

        #region 6E8 GraphRoot  ================================================
        ModelAction GraphRoot_X;
        void Initialize_GraphRoot_X()
        {
            GraphRoot_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var q = m.Query;
                    if (!q.TryGetItems(out Item[] items)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.QueryRootItem_M, itm, q, null, QueryRootItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, m.TableX.Name);
        }
        #endregion

        #region 6E9 GraphNode  ================================================
        ModelAction GraphNode_X;
        void Initialize_GraphNode_X()
        {
            GraphNode_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Node.Graph.Node_Edges.TryGetValue(m.Node, out List<Edge> edges) ? edges.Count : 0;

                    m.CanExpandRight = true;
                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    List<Edge> edges = null;
                    if (!m.IsExpandedRight && (m.IsExpandedLeft && !m.Node.Graph.Node_Edges.TryGetValue(m.Node, out edges))) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (m.IsExpandedRight)
                    {
                        anyChange |= AddProperyModel(prev, m, _nodeCenterXYProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeSizeWHProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeOrientationProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeFlipRotateProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeLabelingProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeResizingProperty);
                        anyChange |= AddProperyModel(prev, m, _nodeBarWidthProperty);
                    }
                    
                    if (m.IsExpandedLeft)
                    {
                        foreach (var e in edges)
                        {
                            anyChange |= AddChildModel(prev, m, Trait.GraphEdge_M, e, null, null, GraphEdge_X);
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetIdentity(m.Node.Item, IdentityStyle.Double));
        }
        #endregion

        #region 6EA GraphEdge  ================================================
        ModelAction GraphEdge_X;
        void Initialize_GraphEdge_X()
        {
            GraphEdge_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandRight = true;
                    
                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 6) return (true, false);

                    m.InitChildModels(prev);

                    AddProperyModel(prev, m, _edgeFace1Property);
                    AddProperyModel(prev, m, _edgeFace2Property);
                    AddProperyModel(prev, m, _edgeGnarl1Property);
                    AddProperyModel(prev, m, _edgeGnarl2Property);
                    AddProperyModel(prev, m, _edgeConnect1Property);
                    AddProperyModel(prev, m, _edgeConnect2Property);

                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), GetHeadTailName(m.Edge.Node1.Item, m.Edge.Node2.Item));
        }
        #endregion

        #region 6EB GraphOpenList  ============================================
        ModelAction GraphOpenList_X;
        void Initialize_GraphOpenList_X()
        {
            GraphOpenList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = m.Graph.OpenQuerys.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var g = m.Graph;
                    if (g.OpenQuerys.Count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var (q1, q2) in g.OpenQuerys)
                    {
                        var h = q1;
                        var t = q2;
                        anyChange |= AddChildModel(prev, m, Trait.GraphOpen_M, g, h, t, GraphOpen_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6EC GraphOpen  ================================================
        ModelAction GraphOpen_X;
        void Initialize_GraphOpen_X()
        {
            GraphOpen_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,
            };
            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string Kind, string Name) GetKindName(ItemModel m)
            {
                GetHeadTail(m.Query.QueryX, out Store head, out Store tail);
                var name = $"{GetIdentity(m.Item, IdentityStyle.Double)}  -->  {GetIdentity(tail, IdentityStyle.Single)}: <?>";

                return (GetIdentity(m.Item, IdentityStyle.Double), name);
            }
        }
        #endregion



        #region 7D0 PrimeCompute  =============================================
        ModelAction PrimeCompute_X;
        void Initialize_PrimeCompute_X()
        {
            PrimeCompute_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var count = 0;
                    foreach (var sto in _primeStores) { if (Store_ComputeX.HasChildLink(sto)) count += 1; }

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(GetSummaryKey(Trait.PrimeCompute_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(GetDescriptionKey(Trait.PrimeCompute_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var st in _primeStores)
                    {
                        if (Store_ComputeX.HasChildLink(st))
                        {
                            anyChange |= AddChildModel(prev, m, Trait.ComputeStore_M, st, null, null, ComputeStore_X);
                        }
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(GetNameKey(Trait.PrimeCompute_M)));
        }
        #endregion

        #region 7D1 ComputeStore  =============================================
        ModelAction ComputeStore_X;
        void Initialize_ComputeStore_X()
        {
            ComputeStore_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var st = m.Store;
                    var (kind, name) = GetKindName(m);
                    var count = Store_ComputeX.ChildCount(st);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Store, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var st = m.Store;
                    if (!Store_ComputeX.TryGetChildren(st, out IList<ComputeX> list)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var cx in list)
                    {
                        anyChange |= AddChildModel(prev, m,  Trait.TextProperty_M, st, cx, null, TextCompute_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.Store, IdentityStyle.Single));
        }
        #endregion



        #region 7F0 InternlStoreList  =========================================
        ModelAction InternalStoreList_X;
        void Initialize_InternalStoreList_X()
        {
            InternalStoreList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = true;

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(GetSummaryKey(Trait.InternalStoreList_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =


                ModelDescription = (m) => _localize(GetDescriptionKey(Trait.InternalStoreList_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (m.ChildModelCount == 11) return (true, false);

                    m.InitChildModels(prev);

                    AddChildModel(prev, m, Trait.InternalStore_M, _viewXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _enumXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m,  Trait.InternalStore_M, _tableXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _graphXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _queryXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _symbolXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m,  Trait.InternalStore_M, _columnXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _relationXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _computeXStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _relationStore, null, null, InternalStore_X);
                    AddChildModel(prev, m, Trait.InternalStore_M, _propertyStore, null, null, InternalStore_X);
                    
                    return (true, true);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(GetNameKey(Trait.InternalStoreList_M)));
        }
        #endregion

        #region 7F1 InternalStore  ============================================
        ModelAction InternalStore_X;
        void Initialize_InternalStore_X()
        {
            InternalStore_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var st = m.Store;
                    var (kind, name) = GetKindName(m);
                    var count = st.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var st = m.Store;
                    if (st.Count == 0) return (false, false);
                    if (st.GetDelta == m.Delta) return (true, false);

                    m.Delta = st.GetDelta;
                    m.InitChildModels(prev);

                    var list = st.GetItems();
                    var anyChange = false;
                    foreach (var item in Items)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.StoreItem_M, item, null, null, StoreItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.Store.NameKey));
        }
        #endregion

        #region 7F2 StoreItem  ================================================
        ModelAction StoreItem_X;
        void Initialize_StoreItem_X()
        {
            StoreItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var item = m.Item;
                    var (hasItems, hasLinks, hasChildRels, hasParentRels, count) = GetItemParms(item);
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => GetIdentity(m.Item, IdentityStyle.Description),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    if (m.Item.IsExternal) mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var item = m.Item;
                    var (hasItems, hasLinks, hasChildRels, hasParentRels, count) = GetItemParms(item);
                    if (count == 0) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    if (hasItems)
                        anyChange |= AddChildModel(prev, m, Trait.StoreItemItemList_M, item, null, null, StoreItemItemList_X);
                    if (hasLinks)
                        anyChange |= AddChildModel(prev, m, Trait.StoreRelationLinkList_M, item, null, null, StoreRelationLinkList_X);
                    if (hasChildRels)
                        anyChange |= AddChildModel(prev, m, Trait.StoreChildRelationList_M, item, null, null, StoreChildRelationList_X);
                    if (hasParentRels)
                        anyChange |= AddChildModel(prev, m, Trait.StoreParentRelationList_M, item, null, null, StoreParentRelationList_X);

                    return (true, anyChange);
                }
            };

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.StoreItem));
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        (bool, bool, bool, bool, int) GetItemParms(Item item)
        {
            var hasItems = (item is Store sto && sto.Count > 0);
            var hasLinks = (item is Relation rel && rel.GetLinksCount() > 0);
            var hasChildRels = false;
            var hasParentRels = false;

            var count = 0;
            if (hasItems) count++;
            if (hasLinks) count++;
            if (hasChildRels) count++;
            if (hasParentRels) count++;

            return (hasItems, hasLinks, hasChildRels, hasParentRels, count);
        }
        #endregion

        #region 7F4 StoreItemItemList  ========================================
        ModelAction StoreItemItemList_X;
        void Initialize_StoreItemItemList_X()
        {
            StoreItemItemList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Store.Count;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var st = m.Store;
                    if (st.Count == 0) return (false, false);

                    m.InitChildModels(prev, st.Count);

                    var list = st.GetItems();

                    var anyChange = false;
                    foreach (var itm in list)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.StoreItemItem_M, itm, null, null, StoreItemItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) =>  (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F5 StoreRelationLinkList  ====================================
        ModelAction StoreRelationLinkList_X;
        void Initialize_StoreRelationLinkList_X()
        {
            StoreRelationLinkList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Relation.GetLinksCount();
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var re = m.Relation;
                    var N = re.GetLinks(out List<Item> parents, out List<Item> children);
                    if (N == 0) return (false, false);

                    m.InitChildModels(prev, N);

                    var anyChange = false;
                    for (int i = 0; i < N; i++)
                    {
                        var parent = parents[i];
                        var child = children[i];
                        anyChange = AddChildModel(prev, m, Trait.StoreRelationLink_M, re, parent, child, StoreRelationLink_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F6 StoreChildRelationList  ===================================
        ModelAction StoreChildRelationList_X;
        void Initialize_StoreChildRelationList_X()
        {
            StoreChildRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = 0;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    return (false, false);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F7 StoreParentRelationList  ==================================
        ModelAction StoreParentRelationList_X;
        void Initialize_StoreParentRelationList_X()
        {
            StoreParentRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = 0;
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    return (false, false);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F8 StoreItemItem  ============================================
        ModelAction StoreItemItem_X;
        void Initialize_StoreItemItem_X()
        {
            StoreItemItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.Item.KindKey), GetIdentity(m.Item, IdentityStyle.Double));
        }
        #endregion

        #region 7F9 StoreRelationLink  ========================================
        ModelAction StoreRelationLink_X;
        void Initialize_StoreRelationLink_X()
        {
            StoreRelationLink_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);

                    return (kind, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), $"({GetIdentity(m.Aux1, IdentityStyle.Double)}) --> ({GetIdentity(m.Aux2, IdentityStyle.Double)})");
        }
        #endregion

        #region 7FA StoreChildRelation  =======================================
        ModelAction StoreChildRelation_X;
        void Initialize_StoreChildRelation_X()
        {
            StoreChildRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Relation.ChildCount(m.Aux1);
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Relation.TryGetChildren(m.Aux1, out List<Item> items)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange = AddChildModel(prev, m, Trait.StoreRelatedItem_M, itm, null, null, StoreRelatedItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) =>  (GetKind(m.Relation.Trait), GetIdentity(m.Relation, IdentityStyle.Single));

        }
        #endregion

        #region 7FA StoreParentRelation  ======================================
        ModelAction StoreParentRelation_X;
        void Initialize_StoreParentRelation_X()
        {
            StoreParentRelation_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Relation.ParentCount(m.Aux1);
                    var (kind, name) = GetKindName(m);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    if (!m.Relation.TryGetParents(m.Aux1, out List<Item> items)) return (false, false);

                    m.InitChildModels(prev);

                    var anyChange = false;
                    foreach (var itm in items)
                    {
                        anyChange = AddChildModel(prev, m, Trait.StoreRelatedItem_M, itm, null, null, StoreRelatedItem_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetKind(m.Relation.Trait), GetIdentity(m.Relation, IdentityStyle.Single));
        }
        #endregion

        #region 7FC StoreRelatedItem  =========================================
        ModelAction StoreRelatedItem_X;
        void Initialize_StoreRelatedItem_X()
        {
            StoreRelatedItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    var (hasChildRels, hasParentRels, count) = GetItemParms(m.Item);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => GetIdentity(m.Item, IdentityStyle.Description),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    if (m.Item.IsExternal) mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m,prev) =>
                {
                    var itm = m.Item;
                    var (hasChildRels, hasParentRels, N) = GetItemParms(itm);
                    if (N == 0) return (false, false);

                    m.InitChildModels(prev);


                    var anyChange = false;
                    if (hasChildRels)
                    {
                        anyChange |= AddChildModel(prev, m,  Trait.StoreChildRelationList_M, itm, null, null, StoreChildRelationList_X);
                    }
                    if (hasParentRels)
                    {
                        anyChange |= AddChildModel(prev, m, Trait.StoreParentRelationList_M, itm, null, null, StoreParentRelationList_X);
                    }
                    return (true, anyChange);
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.StoreItem));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (bool, bool, int) GetItemParms(Item item)
            {
                var hasChildRels = false;
                var hasParentRels = false;
                var count = 0;
                if (hasChildRels) count++;
                if (hasParentRels) count++;

                return (hasChildRels, hasParentRels, count);
            }
        }
        #endregion
    }
}
