using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        #region Initialize_ModelActions  ======================================
        void Initialize_ModelActions()
        {
            Initialize_RootChef_X();
            Initialize_DataChef_X();
            Initialize_TextColumn_X();
            Initialize_CheckColumn_X();
            Initialize_ComboColumn_X();
            Initialize_TextProperty_X();
            Initialize_CheckProperty_X();
            Initialize_ComboProperty_X();
            Initialize_TextCompute_X();

            Initialize_ErrorRoot_X();
            Initialize_ChangeRoot_X();
            Initialize_MetadataRoot_X();
            Initialize_ModelingRoot_X();
            Initialize_MetaRelationList_X();
            Initialize_ErrorType_X();
            Initialize_ErrorText_X();
            Initialize_ChangeSet_X();
            Initialize_ItemChange_X();

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
            Initialize_RowDefaultPropertyList_X();
            Initialize_RowUnusedChildRelationList_X();
            Initialize_RowUnusedParentRelationList_X();
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
            Initialize_InternalStoreZ_X();
            Initialize_InternalStore_X();
            Initialize_StoreItem_X();
            Initialize_StoreItemItemZ_X();
            Initialize_StoreRelationLinkZ_X();
            Initialize_StoreChildRelationZ_X();
            Initialize_StoreParentRelationZ_X();
            Initialize_StoreItemItem_X();
            Initialize_StoreRelationLink_X();
            Initialize_StoreChildRelation_X();
            Initialize_StoreParentRelation_X();
            Initialize_StoreRelatedItem_X();
        }
        #endregion

        #region TryGetPrevModel  ==============================================
        /// <summary>
        /// Try to find and reuse an existing model matching the callers parameters
        /// </summary>
        private static bool TryGetPrevModel(ItemModel m, Trait trait, int index, Item item = null, Item aux1 = null, Item aux2 = null)
        {
            if (m.PrevModels == null || m.PrevModels.Length == 0) return false;

            var N = m.PrevModels.Length;
            var i = (index < N) ? index : N / 2;
            for (int n = 0; n < N; n++, i = (i + 1) % N)
            {
                var pm = m.PrevModels[i];
                if (pm == null) continue;
                if (trait != Trait.Empty && trait != pm.Trait) continue;
                if (item != null && item != pm.Item) continue;
                if (aux1 != null && aux1 != pm.Aux1) continue;
                if (aux2 != null && aux2 != pm.Aux2) continue;
                m.PrevModels[i] = null;
                m.ChildModels[index] = pm;
                return true;
            }
            return false;
        }
        #endregion

        #region RefreshViewList  ==============================================
        internal void RefreshViewList(RootModel root, int scroll = 0, ChangeType change = ChangeType.NoChange)
        {
            var select = root.SelectModel;
            var viewList = root.ViewModels;
            var capacity = root.ViewCapacity;
            var offset = viewList.IndexOf(select);

            if (capacity > 0)
            {
                var first = ItemModel.FirstValidModel(viewList);
                var start = (first == null);

                UpdateSelectModel(select, change);

                if (root.ChildModelCount == 0) ValidateModel(root);

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
                            select = pm.ChildModels[last];
                            if (!viewList.Contains(select)) FindFirst();
                        }
                    }
                    else
                    {
                        if (ix > 0)
                        {
                            select = pm.ChildModels[0];
                            if (!viewList.Contains(select)) FindFirst();
                        }
                    }
                    root.SelectModel = select;

                    void FindFirst()
                    {
                        first = select;
                        var absoluteFirst = root.ChildModels[0];

                        for (; offset > 0; offset--)
                        {
                            if (first == absoluteFirst) break;

                            var p = first.ParentModel;
                            var i = p.GetChildlIndex(first);

                            first = (i > 0) ? p.ChildModels[i - 1] : p;
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

                    ValidateModel(m);
                    modelStack.PushChildren(m);
                }
                #endregion

                #region ScrollViewList  =======================================
                viewList.Clear();

                if (scroll == -1)
                {
                    buffer.GetHead(viewList);
                    root.SelectModel = viewList[0];
                }
                else if (scroll == 1)
                {
                    buffer.GetTail(viewList);
                    root.SelectModel = viewList[viewList.Count - 1];
                }
                else if (scroll == 0)
                {
                    buffer.GetHead(viewList);
                    if (!viewList.Contains(select))
                        root.SelectModel = viewList[0];
                }
                else if (scroll < -1)
                {
                    buffer.GetHead(viewList);
                    if (!viewList.Contains(select))
                        root.SelectModel = viewList[viewList.Count - 1];
                }
                else if (scroll > 1)
                {
                    buffer.GetTail(viewList);
                    if (!viewList.Contains(select))
                        root.SelectModel = viewList[0];
                }
                #endregion
            }
            else
            {
                root.ViewModels.Clear();
                root.SelectModel = null;
            }

            root.UIRequestRefreshModel();
        }

        #region TreeModelStack  ===============================================
        private class TreeModelStack
        {/*
            Keep track of unvisited nodes in a depth first graph traversal
         */
            private List<(ItemModel[] Models, int Index)> _stack;
            internal TreeModelStack() { _stack = new List<(ItemModel[] Models, int Index)>(); }
            internal bool IsNotEmpty => (_stack.Count > 0);
            internal void PushChildren(ItemModel m) { if (m.ChildModelCount > 0) _stack.Add((m.ChildModels, 0)); }
            internal ItemModel PopNext()
            {
                var end = _stack.Count - 1;
                var (Models, Index) = _stack[end];
                var model = Models[Index++];

                if (Index < Models.Length)
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

            internal void GetHead(List<ItemModel> list)
            {
                var first = (_first - _scroll);
                if (first < 0) first = 0;
                CopyBuffer(first, list);
            }

            internal void GetTail(List<ItemModel> list)
            {
                var first = (_count < _buffer.Length) ? 0 : ((_count - _first) < _length) ? (_count - _length + _scroll) : _first + _scroll;
                CopyBuffer(first, list);
            }

            #region PrivateMethods  ===========================================
            int Index(int inx) => inx % _buffer.Length;
            void CopyBuffer(int first, List<ItemModel> list)
            {
                for (int i = 0, j = first; (i < _length && j < _count); i++, j++)
                {
                    list.Add(_buffer[Index(j)]);
                }
            }
            #endregion
        }
        #endregion

        #region UpdateSelectModel  ============================================
        void UpdateSelectModel(ItemModel m, ChangeType change)
        {
            if (m != null)
            {
               if (m.Item.AutoExpandLeft)
                {
                    m.Item.AutoExpandLeft = false;
                    m.IsExpandedLeft = true;
                }
                if (m.Item.AutoExpandRight)
                {
                    m.Item.AutoExpandRight = false;
                    m.IsExpandedRight = true;
                }

                switch (change)
                {
                    case ChangeType.ToggleLeft:
                        m.IsExpandedLeft = !m.IsExpandedLeft;
                        break;

                    case ChangeType.ExpandLeft:
                        m.IsExpandedLeft = true;
                        break;

                    case ChangeType.CollapseLeft:
                        m.IsExpandedLeft = false;
                        m.IsExpandedRight = false;
                        m.IsExpandedFilter = false;
                        m.ViewFilter = null;
                        break;

                    case ChangeType.ToggleRight:
                        m.IsExpandedRight = !m.IsExpandedRight;
                        break;

                    case ChangeType.ExpandRight:
                        m.IsExpandedRight = true;
                        break;

                    case ChangeType.CollapseRight:
                        m.IsExpandedRight = false;
                        break;

                    case ChangeType.ToggleFilter:
                        m.IsExpandedFilter = !m.IsExpandedFilter;
                        if (m.IsExpandedFilter)
                            m.ViewFilter = string.Empty;
                        else
                            m.ViewFilter = null;
                        break;

                    case ChangeType.ExpandFilter:
                        m.IsExpandedFilter = true;
                        m.ViewFilter = string.Empty;
                        break;

                    case ChangeType.CollapseFilter:
                        m.IsExpandedFilter = false;
                        m.ViewFilter = null;
                        break;
                }
            }
        }
        #endregion

        #region ValidateModel  ================================================
        void ValidateModel(ItemModel m)
        {
            if (m.Item.AutoExpandRight)
            {
                m.IsExpandedRight = true;
                m.Item.AutoExpandRight = false;
            }

            if ((m.IsExpanded || m.IsExpandedFilter) && m.Validate())
            {
                var N = m.ChildModelCount;

                if (N > 0)
                {
                    var filter = m.RegexViewFilter;
                    if (N > 3 && filter != null)
                    {
                        if (m.PrevModels == null)
                        {
                            m.PrevModels = new ItemModel[N];
                            m.ChildModels.CopyTo(m.PrevModels, 0);
                        }

                        if (m.IsAlreadyFiltered == false)
                        {
                            m.IsAlreadyFiltered = true;

                            var filterList = new List<ItemModel>(N);

                            foreach (var cm in m.PrevModels)
                            {
                                if (!filter.IsMatch(cm.FilterSortName)) continue;
                                filterList.Add(cm);
                            }

                            m.ChildModels = (filterList.Count == 0) ? null : filterList.ToArray();
                            N = m.ChildModelCount;
                        }
                    }

                    if (N > 2 && m.IsSorted)
                    {
                        if (m.PrevModels == null)
                        {
                            m.PrevModels = new ItemModel[N];
                            m.ChildModels.CopyTo(m.PrevModels, 0);
                        }

                        if (m.IsAlreadySorted == false)
                        {
                            m.IsAlreadySorted = true;

                            var sortList = new List<(string Name, ItemModel Model)>(N);
                            foreach (var cm in m.ChildModels)
                            {
                                sortList.Add((cm.FilterSortName, cm));
                            }

                            sortList.Sort(_alphaSort);
                            if (m.IsSortDescending) sortList.Reverse();

                            for (int i = 0; i < N; i++)
                            {
                                m.ChildModels[i] = sortList[i].Model;
                            }
                        }
                    }
                }
                else
                {
                    m.ClearChildren();
                }
            }
            else
            {
                m.ClearChildren();
            }
        }
        class SortCompare : IComparer<(string, ItemModel)>
        {
            public int Compare((string, ItemModel) x, (string, ItemModel) y) => x.Item1.CompareTo(y.Item1);
        }
        static SortCompare _alphaSort = new SortCompare();
        #endregion
        #endregion

        #region AddProperyModels  =============================================
        private void AddProperyModels(ItemModel model, ColumnX[] cols)
        {
            var item = model.Item;
            var N = cols.Length;
            for (int i = 0; i < N; i++)
            {
                var col = cols[i];
                if (!TryGetPrevModel(model, Trait.Empty, i, item, col))
                {
                    model.ChildModels[i] = NewPropertyModel(model, model.Depth, item, col);
                }
            }
        }
        private void AddProperyModels(ItemModel model, Property[] props)
        {
            var item = model.Item;
            var N = props.Length;
            for (int i = 0; i < N; i++)
            {
                var prop = props[i];
                if (!TryGetPrevModel(model, Trait.Empty, i, item, prop))
                {
                    if (prop.IsColumnX)
                        model.ChildModels[i] = NewPropertyModel(model, model.Depth, item, (prop as ColumnX));
                    else if (prop.IsComputeX)
                        model.ChildModels[i] = NewPropertyModel(model, model.Depth, item, (prop as ComputeX));
                    else
                        model.ChildModels[i] = NewPropertyModel(model, model.Depth, item, prop);

                    if (prop.IsReadOnly) model.ChildModels[i].IsReadOnly = true;
                    if (prop.CanMultiline) model.ChildModels[i].CanMultiline = true;
                }
            }
        }
        private ItemModel NewPropertyModel(ItemModel model, byte depth, Item item, ColumnX col)
        {
            if (EnumX_ColumnX.TryGetParent(col, out EnumX enu))
                return new ItemModel(model, Trait.ComboProperty_M, depth, item, col, enu, ComboColumn_X);
            else if (col.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, depth, item, col, null, CheckColumn_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, depth, item, col, null, TextColumn_X);
        }
        private ItemModel NewPropertyModel(ItemModel model, byte depth, Item item, ComputeX cx)
        {
            if (EnumX_ColumnX.TryGetParent(cx, out EnumX enu))
                return new ItemModel(model, Trait.ComboProperty_M, depth, item, cx, enu, ComboProperty_X);
            else if (cx.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, depth, item, cx, null, CheckProperty_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, depth, item, cx, null, TextCompute_X);
        }
        private ItemModel NewPropertyModel(ItemModel model, byte depth, Item item, Property prop)
        {
            if (Property_Enum.TryGetValue(prop, out EnumZ enu))
                return new ItemModel(model, Trait.ComboProperty_M, depth, item, prop, enu, ComboProperty_X);
            else if (prop.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, depth, item, prop, null, CheckProperty_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, depth, item, prop, null, TextProperty_X);
        }
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
                    if (g.RootItem == null)
                        return $"{gx.Name}";
                    else
                        return $"{gx.Name} - {GetIdentity(g.RootItem, IdentityStyle.Double)}";

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



        #region 611 RootChef_M  ===============================================
        internal ModelAction RootChef_X;
        void Initialize_RootChef_X()
        {
            RootChef_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    m.CanExpandLeft = true;
                    m.IsExpandedLeft = true;

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

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var chef = m.Item as Chef;

                    var N = chef.Count;
                    if (N == 0) return false;

                    var items = chef.Items;
                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var child = items[i] as Chef;
                        if (!TryGetPrevModel(m, Trait.MockChef_M, i, child))
                            m.ChildModels[i] = new ItemModel(m, Trait.DataChef_M, 0, child, null, null, child.DataChef_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
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

        #region 612 DataChef_X  ===============================================
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

                ValidateChildModels = (m) =>
                {
                    var N = 4;

                    if (m.ChildModelCount != N) // avoid unnecessary validation, once initialized, it won't change
                    {
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetPrevModel(m, Trait.ErrorRoot_M, i))
                            m.ChildModels[i] = new ItemModel(m, Trait.ErrorRoot_M, depth, _errorStore, null, null, ErrorRoot_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.ChangeRoot_M, i))
                            m.ChildModels[i] = new ItemModel(m, Trait.ChangeRoot_M, depth, _changeRoot, null, null, ChangeRoot_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.MetadataRoot_M, i))
                            m.ChildModels[i] = new ItemModel(m, Trait.MetadataRoot_M, depth, item, null, null, MetadataRoot_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.ModelingRoot_M, i))
                            m.ChildModels[i] = new ItemModel(m, Trait.ModelingRoot_M, depth, item, null, null, ModelingRoot_X);

                        m.PrevModels = null;
                    }
                    return true;
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

                MajorDelta += 1;
                dataChef.SaveToRepository(repo);
            }
            void SaveModel(ItemModel model)
            {
                var root = model as RootModel;
                var dataChef = root.Chef;

                MajorDelta += 1;
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

                ValidateChildModels = (m) =>
                {
                    var N = _errorStore.Count;
                    if (N == 0) return false;

                    var items = _errorStore.Items;
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Error;
                        if (!TryGetPrevModel(m, Trait.ErrorType_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ErrorType_M, depth, itm, null, null, ErrorType_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ModelInfo = (m) => m.IsExpandedLeft ? null : _changeRootInfoText,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, list) =>
                {
                    if (_changeRoot.Count > 0 && m.IsExpandedLeft == false)
                        list.Add(new ModelCommand(this, m, Trait.ExpandAllCommand, ExpandAllChangeSets));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var N = _changeRoot.Count;
                    if (N == 0) return false;

                    if (_changeRoot.Delta != m.Delta)
                    {
                        m.Delta = _changeRoot.Delta;

                        var items = _changeRoot.Items;
                        items.Reverse();
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetPrevModel(m, Trait.ChangeSet_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ChangeSet_M, depth, itm, null, null, ChangeSet_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = 5;
                    if (m.ChildModelCount != N)
                    {
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetPrevModel(m, Trait.ViewXView_ZM, i, _viewXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_ZM, depth, _viewXStore, null, null, ViewXView_ZX);

                        i++;
                        if (!TryGetPrevModel(m, Trait.EnumX_ZM, i, _enumZStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.EnumX_ZM, depth, _enumZStore, null, null, EnumXList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.TableX_ZM, i, _tableXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.TableX_ZM, depth, _tableXStore, null, null, TableXList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.GraphX_ZM, i, _graphXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphX_ZM, depth, _graphXStore, null, null, GraphXList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.InternalStore_ZM, i, this))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_ZM, depth, this, null, null, InternalStoreZ_X);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N =  4;
                    if (m.ChildModelCount != N)
                    {
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetPrevModel(m, Trait.ViewView_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewView_ZM, depth, item, null, null, ViewXView_ZX);

                        i++;
                        if (!TryGetPrevModel(m, Trait.Table_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.Table_ZM, depth, item, null, null, TableList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.Graph_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.Graph_ZM, depth, item, null, null, GraphList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.PrimeCompute_M, i, this))
                            m.ChildModels[i] = new ItemModel(m, Trait.PrimeCompute_M, depth, this, null, null, PrimeCompute_X);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    int N = 2;
                    if (m.ChildModelCount != N)
                    {
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetPrevModel(m, Trait.NameColumnRelation_M, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.NameColumnRelation_M, depth, item, TableX_NameProperty, null, NameColumnRelation_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.SummaryColumnRelation_M, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.SummaryColumnRelation_M, depth, item, TableX_SummaryProperty, null, SummaryColumnRelation_X);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var er = m.Error;

                    var N = er.Count;
                    if (N == 0) return false;

                    if (N != m.ChildModelCount)
                    {
                        var depth = (byte)(m.Depth + 1);
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            m.ChildModels[i] = new ItemModel(m, Trait.ErrorText_M, depth, er, null, null, ErrorText_X);
                        }

                        m.PrevModels = null;
                    }

                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var cs = m.ChangeSet;

                    var N = cs.Count;
                    if (N == 0) return false;

                    if (m.Delta != cs.Delta)
                    {
                        var items = (cs.IsReversed) ? cs.Items : cs.ItemsReversed;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as ItemChange;
                            if (!TryGetPrevModel(m, Trait.ItemChange_M, i, itm))
                            {
                                m.ChildModels[i] = new ItemModel(m, Trait.ItemChange_M, depth, itm, null, null, ItemChanged_X);
                            }
                        }

                        m.PrevModels = null;
                    }

                    return true;
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
                MajorDelta += 1;
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



        #region 631 ViewXView_ZX  =============================================
        ModelAction ViewXView_ZX;
        void Initialize_ViewXView_X()
        {
            ViewXView_ZX = new ModelAction
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
                            var vxDropParent = ViewX_ViewX.GetParent(vxDrop);
                            if (vxDropParent != null) RemoveLink(ViewX_ViewX, vxDropParent, vxDrop);

                            var prevIndex = _viewXStore.IndexOf(vxDrop);
                            ItemMoved(vxDrop, prevIndex, 0);
                        }
                        return DropAction.Move;
                    }
                    return DropAction.None;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    int N = 0;
                    var views = _viewXStore.Items;
                    var roots = new List<ViewX>();
                    foreach (var view in views) { if (ViewX_ViewX.HasNoParent(view)) { roots.Add(view); N++; } }

                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = roots[i];
                        if (!TryGetPrevModel(m, Trait.ViewXView_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, itm, null, null, ViewXViewM_X);
                    }

                    m.PrevModels = null;
                    return true;
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
                                        var oldParent = ViewX_ViewX.GetParent(vx);
                                        if (oldParent != null) RemoveLink(ViewX_ViewX, oldParent, vx);
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

                ValidateChildModels = (m) =>
                {
                    var view = m.ViewX;

                    int N = 0;
                    int R = 0;
                    Property[] props = null;
                    if (m.IsExpandedRight)
                    {
                        props = new Property[] { _viewXNameProperty, _viewXSummaryProperty };
                        R = props.Length;
                    }

                    int L1 = 0, L2 = 0, L3 = 0;
                    Property[] propertyList = null;
                    QueryX[] queryList = null;
                    ViewX[] viewList = null;
                    if (m.IsExpandedLeft)
                    {
                        propertyList = ViewX_Property.GetChildren(view);
                        queryList = ViewX_QueryX.GetChildren(view);
                        viewList = ViewX_ViewX.GetChildren(view);

                        L1 = (propertyList == null) ? 0 : propertyList.Length;
                        L2 = (queryList == null) ? 0 : queryList.Length;
                        L3 = (viewList == null) ? 0 : viewList.Length;
                    }

                    N = R + L1 + L2 + L3;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, props);
                    }
                    if (L1 > 0)
                    {
                        for (int i = R, j = 0; j < L1; i++, j++)
                        {
                            var px = propertyList[j];

                            if (!TryGetPrevModel(m, Trait.ViewXProperty_M, i, px))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXProperty_M, depth, px, ViewX_Property, view, ViewXProperty_X);
                        }
                    }
                    if (L2 > 0)
                    {
                        for (int i = (R + L1), j = 0; j < L2; i++, j++)
                        {
                            var qx = queryList[j];
                            if (!TryGetPrevModel(m, Trait.ViewXQuery_M, i, qx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXQuery_M, depth, qx, ViewX_QueryX, view, ViewXQuery_X);
                        }
                    }
                    if (L3 > 0)
                    {
                        for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                        {
                            var vx = viewList[j];
                            if (!TryGetPrevModel(m, Trait.ViewXView_M, i, vx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, vx, ViewX_ViewX, view, ViewXViewM_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                    if (Relation_QueryX.GetParent(qx) != null)
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

                ValidateChildModels = (m) =>
                {
                    var qx = m.QueryX;
                    int N = 0;
                    int R = 0;

                    Property[] props = null;
                    if (m.IsExpandedRight)
                    {
                        props = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                        R = props.Length;
                    }

                    int L1 = 0, L2 = 0, L3 = 0;
                    Property[] propertyList = null;
                    QueryX[] queryList = null;
                    ViewX[] viewList = null;
                    if (m.IsExpandedLeft)
                    {
                        propertyList = QueryX_Property.GetChildren(qx);
                        queryList = QueryX_QueryX.GetChildren(qx);
                        viewList = QueryX_ViewX.GetChildren(qx);

                        L1 = (propertyList == null) ? 0 : propertyList.Length;
                        L2 = (queryList == null) ? 0 : queryList.Length;
                        L3 = (viewList == null) ? 0 : viewList.Length;
                    }

                    N = R + L1 + L2 + L3;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, props);
                    }
                    if (L1 > 0)
                    {
                        for (int i = R, j = 0; j < L1; i++, j++)
                        {
                            var px = propertyList[j];

                            if (!TryGetPrevModel(m, Trait.ViewXProperty_M, i, px))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXProperty_M, depth, px, QueryX_Property, qx, ViewXProperty_X);
                        }
                    }
                    if (L2 > 0)
                    {
                        for (int i = (R + L1), j = 0; j < L2; i++, j++)
                        {
                            var qr = queryList[j];
                            if (!TryGetPrevModel(m, Trait.ViewXQuery_M, i, qr))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXQuery_M, depth, qr, QueryX_QueryX, qx, ViewXQuery_X);
                        }
                    }
                    if (L3 > 0)
                    {
                        for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                        {
                            var vx = viewList[j];
                            if (!TryGetPrevModel(m, Trait.ViewXView_M, i, vx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, vx, QueryX_ViewX, qx, ViewXViewM_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m)
            {
                var qx = m.Item as QueryX;
                var rel = Relation_QueryX.GetParent(qx);
                if (rel != null)
                {
                    return (_localize(m.KindKey), GetIdentity(qx, IdentityStyle.Single));
                }
                else
                {
                    var sto = Store_QueryX.GetParent(qx);
                    return (GetIdentity(sto, IdentityStyle.Kind), GetIdentity(sto, IdentityStyle.Double));
                }
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

                ValidateChildModels = (m) =>
                {
                    return false;
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

                ValidateChildModels = (m) =>
                {
                    int N = 0;
                    var views = _viewXStore.Items;
                    var roots = new List<ViewX>();
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) { roots.Add(vx); N++; } }
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = roots[i];
                        if (!TryGetPrevModel(m, Trait.ViewView_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewView_M, depth, itm, null, null, ViewView_X);
                    }

                    m.PrevModels = null;
                    return true;
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
                    var querys = ViewX_QueryX.GetChildren(vx);
                    if (querys != null)
                    {
                        if (querys.Length == 1 && Store_QueryX.HasParentLink(querys[0]))
                        {
                            if (TryGetQueryItems(querys[0], out Item[] keys)) count = keys.Length;
                        }
                        else if (key != null)
                            count = querys.Length;
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

                ValidateChildModels = (m) =>
                {
                    var vx = m.ViewX;
                    var key = m.Aux1; // may be null

                    var propertyList = ViewX_Property.GetChildren(vx);
                    var queryList = ViewX_QueryX.GetChildren(vx);
                    var viewList = ViewX_ViewX.GetChildren(vx);

                    var L1 = (propertyList == null) ? 0 : propertyList.Length;
                    var L2 = (queryList == null) ? 0 : queryList.Length;
                    var L3 = (viewList == null) ? 0 : viewList.Length;

                    var N = L1 + L2 + L3;
                    if (N == 0) return false;

                    if (L2 == 1 && Store_QueryX.HasParentLink(queryList[0]) && TryGetQueryItems(queryList[0], out Item[] items))
                    {
                        N = items.Length;
                        var depth = (byte)(m.Depth + 1);
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];

                            if (!TryGetPrevModel(m, Trait.ViewItem_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewItem_M, depth, itm, queryList[0], null, ViewItem_X);
                        }

                        m.PrevModels = null;
                    }
                    else if (key != null && L2 > 0)
                    {
                        N = L2;
                        var depth = (byte)(m.Depth + 1);
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var qx = queryList[i];

                            if (!TryGetPrevModel(m, Trait.ViewQuery_M, i, qx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewQuery_M, depth, qx, key, null, ViewQuery_X);
                        }

                        m.PrevModels = null;
                    }
                    else if (L3 > 0)
                    {
                        N = L3;
                        var depth = (byte)(m.Depth + 1);
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var v = viewList[i];

                            if (!TryGetPrevModel(m, Trait.ViewView_M, i, v))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewView_M, depth, v, null, null, ViewView_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var item = m.Item;
                    var qx = m.Aux1 as QueryX;
                    var (L1, PropertyList, L2, QueryList, L3, ViewList) = GetQueryXChildren(qx);

                    int R = (m.IsExpandedRight) ? L1 : 0;
                    int L = (m.IsExpandedLeft) ? (L2 + L3) : 0;

                    var N = R + L;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, PropertyList);
                    }

                    if (L > 0)
                    {
                        int i = R;
                        for (int j = 0; j < L2; i++, j++)
                        {
                            var q = QueryList[j];
                            if (!TryGetPrevModel(m, Trait.ViewQuery_M, i, item, q))
                                m.ChildModels[i] = new ItemModel(m, Trait.ViewQuery_M, depth, item, q, null, ViewQuery_X);
                        }
                        for (int j = 0; j < L3; i++, j++)
                        {

                        }
                    }

                    m.PrevModels = null;
                    return true;
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
                    var count = TryGetQueryItems(qx, out Item[] items, key) ? items.Length : 0;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var key = m.Item;
                    var qx = m.QueryX;

                    var N = TryGetQueryItems(qx, out Item[] items, key) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.ViewItem_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewItem_M, depth, itm, qx, null, ViewItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = _enumXStore.Count;
                    if (N == 0) return false;

                    if (m.Delta != _enumXStore.Delta)
                    {
                        m.Delta = _enumXStore.Delta;

                        var items = _enumXStore.Items;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var ex = items[i];
                            if (!TryGetPrevModel(m, Trait.EnumX_M, i, ex))
                                m.ChildModels[i] = new ItemModel(m, Trait.EnumX_M, depth, ex, null, null, EnumX_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = _tableXStore.Count;
                    if (N == 0) return false;

                    if (m.Delta != _tableXStore.Delta)
                    {
                        m.Delta = _tableXStore.Delta;

                        var items = _tableXStore.Items;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetPrevModel(m, Trait.TableX_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.TableX_M, depth, itm, null, null, TableX_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = _graphXStore.Count;
                    if (N == 0) return false;

                    if (m.Delta != _graphXStore.Delta)
                    {
                        m.Delta = _graphXStore.Delta;

                        var items = _graphXStore.Items;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as GraphX;
                            if (!TryGetPrevModel(m, Trait.GraphX_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphX_M, depth, itm, null, null, GraphX_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var gx = m.GraphX;
                    var N = GraphX_SymbolX.ChildCount(gx);
                    if (N == 0) return false;

                    var syms = GraphX_SymbolX.GetChildren(gx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var sym = syms[i];
                        if (!TryGetPrevModel(m, Trait.SymbolX_M, i, sym))
                            m.ChildModels[i] = new ItemModel(m, Trait.SymbolX_M, depth, sym, GraphX_SymbolX, gx, SymbolX_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = _tableXStore.Count;
                    if (N == 0) return false;

                    if (m.Delta != _tableXStore.Delta)
                    {
                        m.Delta = _tableXStore.Delta;

                        var depth = (byte)(m.Depth + 1);
                        var items = _tableXStore.Items;

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetPrevModel(m, Trait.Table_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.Table_M, depth, itm, null, null, Table_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = _graphXStore.Count;
                    if (N == 0) return false;

                    if (m.Delta != _graphXStore.Delta)
                    {
                        m.Delta = _graphXStore.Delta;

                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);
                        var items = _graphXStore.Items;

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var gx = items[i];
                            if (!TryGetPrevModel(m, Trait.GraphXRef_M, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXRef_M, depth, gx, null, null, GraphXRef_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    if (!m.IsExpandedRight) return false;

                    var sp = new Property[] { _pairXTextProperty, _pairXValueProperty };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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
                    var (kind, name) = GetKindName(m);

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (kind, name, 0, ModelType.Default);
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

                ValidateChildModels = (m) =>
                {
                    var ex = m.EnumX;
                    var sp = new Property[] { _enumXNameProperty, _enumXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 2 : 0;

                    var N = R + L;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetPrevModel(m, Trait.EnumValue_ZM, i, ex))
                            m.ChildModels[i] = new ItemModel(m, Trait.EnumValue_ZM, depth, ex, null, null, PairXList_X);

                        i++;
                        if (!TryGetPrevModel(m, Trait.EnumColumn_ZM, i, ex))
                            m.ChildModels[i] = new ItemModel(m, Trait.EnumColumn_ZM, depth, ex, null, null, EnumColumnList_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _tableXNameProperty, _tableXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 5 : 0;

                    var N = R + L;
                    if (N == 0) return false;

                    if (m.ChildModelCount != N)
                    {
                        var tx = m.TableX;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, sp);
                        }
                        if (L > 0)
                        {
                            var i = R;
                            if (!TryGetPrevModel(m, Trait.ColumnX_ZM, i, tx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ColumnX_ZM, depth, tx, null, null, ColumnXList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.ComputeX_ZM, i, tx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ComputeX_ZM, depth, tx, null, null, ComputeXList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.ChildRelationX_ZM, i, tx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ChildRelationX_ZM, depth, tx, null, null, ChildRelationXList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.ParentRelatationX_ZM, i, tx))
                                m.ChildModels[i] = new ItemModel(m, Trait.ParentRelatationX_ZM, depth, tx, null, null, ParentRelationXList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.MetaRelation_ZM, i, tx))
                                m.ChildModels[i] = new ItemModel(m, Trait.MetaRelation_ZM, depth, tx, null, null, MetaRelationList_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _graphXNameProperty, _graphXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 4 : 0;

                    var N = R + L;
                    if (N == 0) return false;

                    if (m.ChildModelCount != N)
                    {
                        var gx = m.GraphX;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, sp);
                        }
                        if (L > 0)
                        {
                            var i = R;
                            if (!TryGetPrevModel(m, Trait.GraphXColoring_M, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXColoring_M, depth, gx, null, null, GraphXColoring_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphXRoot_ZM, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXRoot_ZM, depth, gx, null, null, GraphXRootList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphXNode_ZM, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXNode_ZM, depth, gx, null, null, GraphXNodeList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.SymbolX_ZM, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.SymbolX_ZM, depth, gx, null, null, SymbolXList_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    if (!m.IsExpandedRight) return false;

                    var sp = new Property[] { _symbolXNameProperty };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    if (!m.IsExpandedRight) return false;

                    var sp = new Property[] { _columnXNameProperty, _columnXSummaryProperty, _columnXTypeOfProperty, _columnXIsChoiceProperty, _columnXInitialProperty };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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
                    var qx = ComputeX_QueryX.GetChild(cx);
                    var (kind, name) = GetKindName(m);
                    var count = (qx == null) ? 0 : QueryX_QueryX.ChildCount(qx);

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

                    var cd = m.Item as ComputeX;
                    var root = ComputeX_QueryX.GetChild(cd);
                    if (root == null) return DropAction.None;


                    var sto = Store_ComputeX.GetParent(cd);
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

                ValidateChildModels = (m) =>
                {
                    var cx = m.Item as ComputeX;
                    var qx = ComputeX_QueryX.GetChild(cx);

                    int R = 0;
                    Property[] sp = null;
                    if (m.IsExpandedRight)
                    {
                        switch (cx.CompuType)
                        {
                            case CompuType.RowValue:
                                sp = qx.HasSelect ? new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXSelectProperty, _computeXValueTypeProperty } :
                                                    new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXSelectProperty };
                                break;

                            case CompuType.RelatedValue:
                                sp = new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXValueTypeProperty };
                                break;

                            case CompuType.NumericValueSet:
                                sp = new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXNumericSetProperty, _computeXValueTypeProperty };
                                break;

                            case CompuType.CompositeString:
                            case CompuType.CompositeReversed:
                                sp = new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXSeparatorProperty, _computeXSelectProperty, _computeXValueTypeProperty };
                                break;
                        }
                        R = sp.Length;
                    }
                    var L = (m.IsExpandedLeft && qx != null) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(qx);
                        var depth = (byte)(m.Depth + 1);
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j] as QueryX;
                            if (!TryGetPrevModel(m, Trait.ValueXHead_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ValueXHead_M, depth, itm, null, null, ValueHead_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    var N = TableX_ColumnX.ChildCount(tx);
                    if (N == 0) return false;

                    var items = TableX_ColumnX.GetChildren(tx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.ColumnX_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ColumnX_M, depth, itm, TableX_ColumnX, tx, ColumnX_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    var N = TableX_ChildRelationX.ChildCount(tx);
                    if (N == 0) return false;

                    var items = TableX_ChildRelationX.GetChildren(tx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetPrevModel(m, Trait.ChildRelationX_M, i, rel))
                            m.ChildModels[i] = new ItemModel(m, Trait.ChildRelationX_M, depth, rel, TableX_ChildRelationX, tx, ChildRelationX_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    var N = TableX_ParentRelationX.ChildCount(tx);
                    if (N == 0) return false;

                    var items = TableX_ParentRelationX.GetChildren(tx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetPrevModel(m, Trait.ParentRelationX_M, i, rel))
                            m.ChildModels[i] = new ItemModel(m, Trait.ParentRelationX_M, depth, rel, TableX_ParentRelationX, tx, ParentRelationX_X);
                    }

                    m.PrevModels = null;
                    return true;
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
                    var ex = m.EnumX;
                    var (kind, name) = GetKindName(m);
                    var count = ex.Count;

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

                ValidateChildModels = (m) =>
                {
                    var ex = m.EnumX;
                    var N = ex.Count;
                    if (N == 0) return false;

                    if (m.Delta != ex.Delta)
                    {
                        m.Delta = ex.Delta;

                        var items = ex.Items;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var px = items[i];
                            if (!TryGetPrevModel(m, Trait.PairX_M, i, px))
                                m.ChildModels[i] = new ItemModel(m, Trait.PairX_M, depth, px, ex, null, PairX_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var ex = m.EnumX;
                    var N = (m.IsExpandedLeft) ? EnumX_ColumnX.ChildCount(ex) : 0;
                    if (N == 0) return false;

                    var items = EnumX_ColumnX.GetChildren(ex);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var cx = items[i];
                        var tx = TableX_ColumnX.GetParent(cx);
                        if (tx != null)
                        {
                            if (!TryGetPrevModel(m, Trait.EnumRelatedColumn_M, i, cx))
                                m.ChildModels[i] = new ItemModel(m, Trait.EnumRelatedColumn_M, depth, cx, tx, ex, EnumRelatedColumn_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var st = m.Store;
                    var N = Store_ComputeX.ChildCount(st);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);
                    var items = Store_ComputeX.GetChildren(st);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.ComputeX_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.ComputeX_M, depth, itm, Store_ComputeX, st, ComputeX_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    if (!m.IsExpandedRight) return false;

                    var sp = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    if (!m.IsExpandedRight) return false;

                    var sp = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty, _relationXIsLimitedProperty };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    if (!TableX_NameProperty.TryGetChild(tx, out Property pr)) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[1];

                    if (!TryGetPrevModel(m, Trait.NameColumn_M, 0, pr))
                        m.ChildModels[0] = new ItemModel(m, Trait.NameColumn_M, depth, pr, TableX_NameProperty, tx, NameColumn_X);

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    if (!TableX_SummaryProperty.TryGetChild(tx, out Property pr)) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[1];

                    if (!TryGetPrevModel(m, Trait.SummaryColumn_M, 0, pr))
                        m.ChildModels[0] = new ItemModel(m, Trait.SummaryColumn_M, depth, pr, TableX_SummaryProperty, tx, SummaryColumn_X);

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    if (!(GraphX_ColorColumnX.TryGetChild(m.Item, out ColumnX cx) && TableX_ColumnX.TryGetParent(cx, out TableX tx))) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[1];

                    if (!TryGetPrevModel(m, Trait.GraphXColorColumn_M, 0, cx, tx))
                        m.ChildModels[0] = new ItemModel(m, Trait.GraphXColorColumn_M, depth, cx, tx, null, GraphXColorColumn_X);

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var gx = m.GraphX;
                    var N = GraphX_QueryX.ChildCount(gx);
                    if (N == 0) return false;

                    var items = GraphX_QueryX.GetChildren(gx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.GraphXRoot_M, i, itm, gx))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXRoot_M, depth, itm, gx, null, QueryXRoot_X);
                    }

                    m.PrevModels = null;
                    return false;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            bool GraphXAlreadyHasThisRoot(Item gd, Item table)
            {
                if (GraphX_QueryX.ChildCount(gd) > 0)
                {
                    var items = GraphX_QueryX.GetChildren(gd);
                    foreach (var sd in items)
                    {
                        if (Store_QueryX.ContainsLink(table, sd)) return true;
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

                ValidateChildModels = (m) =>
                {
                    var gx = m.GraphX;
                    var owners = GetNodeOwners(gx).ToArray();

                    var N = owners.Length;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var sto = owners[i];
                        if (!TryGetPrevModel(m, Trait.GraphXNode_M, i, sto, gx))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXNode_M, depth, sto, gx, null, GraphXNode_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var gx = m.GraphX;
                    var st = m.Store;

                    var N = GetSymbolQueryXCount(gx, st);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    (var symbols, var querys) = GetSymbolXQueryX(gx, st);
                    for (int i = 0; i < N; i++)
                    {
                        var qx = querys[i];
                        if (!TryGetPrevModel(m, Trait.GraphXNodeSymbol_M, i, qx))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXNodeSymbol_M, depth, qx, GraphX_SymbolQueryX, gx, GraphXNodeSymbol_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { }; // will be used in the future
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var qx = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var children = QueryX_QueryX.GetChildren(qx);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var child = children[j];
                            if (child.IsPath)
                            {
                                if (child.IsHead)
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXPathHead_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathHead_M, depth, child, null, null, QueryXPathHead_X);
                                }
                                else
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXPathLink_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, child, null, null, QueryXPathLink_X);
                                }
                            }
                            else if (child.IsGroup)
                            {
                                if (child.IsHead)
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXGroupHead_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupHead_M, depth, child, null, null, QueryXGroupHead_X);
                                }
                                else
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXGroupLink_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, child, null, null, QueryXGroupLink_X);
                                }
                            }
                            else if (child.IsSegue)
                            {
                                if (child.IsHead)
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXEgressHead_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressHead_M, depth, child, null, null, QueryXEgressHead_X);
                                }
                                else
                                {
                                    if (!TryGetPrevModel(m, Trait.GraphXEgressLink_M, i, child))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, child, null, null, QueryXEgressLink_X);
                                }
                            }
                            else if (child.IsRoot)
                            {
                                if (!TryGetPrevModel(m, Trait.GraphXLink_M, i, child))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, child, null, null, QueryXLink_X);
                            }
                            else
                            {
                                if (!TryGetPrevModel(m, Trait.GraphXLink_M, i, child))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, child, null, null, QueryXLink_X);
                            }
                        }
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.NameKey), GetIdentity(Store_QueryX.GetParent(m.Item), IdentityStyle.Single));
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var qx = items[j];
                            switch (qx.QueryKind)
                            {
                                case QueryType.Path:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXPathHead_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathHead_M, depth, qx, null, null, QueryXPathHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXPathLink_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, qx, null, null, QueryXPathLink_X);
                                    }
                                    break;

                                case QueryType.Group:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXGroupHead_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupHead_M, depth, qx, null, null, QueryXGroupHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXGroupLink_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, qx, null, null, QueryXGroupLink_X);
                                    }
                                    break;

                                case QueryType.Segue:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXEgressHead_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressHead_M, depth, qx, null, null, QueryXEgressHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetPrevModel(m, Trait.GraphXEgressLink_M, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, qx, null, null, QueryXEgressLink_X);
                                    }
                                    break;

                                case QueryType.Graph:
                                    if (!TryGetPrevModel(m, Trait.GraphXLink_M, i, qx))
                                        m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, qx, null, null, QueryXLink_X);
                                    break;

                                default:
                                    throw new Exception("Invalid item trait");
                            }
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXConnect1Property, _queryXConnect2Property, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var qx = m.QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(qx);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXPathLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, itm, null, null, QueryXPathLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXPathLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, itm, null, null, QueryXPathLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXGroupLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, itm, null, null, QueryXGroupLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXGroupLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, itm, null, null, QueryXGroupLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXEgressLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, itm, null, null, QueryXEgressLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var depth = (byte)(m.Depth + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetPrevModel(m, Trait.GraphXEgressLink_M, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, itm, null, null, QueryXEgressLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXWhereProperty };
                    var N = m.IsExpandedRight ? sp.Length : 0;

                    if (N == 0) return false;

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    AddProperyModels(m, sp);

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var qx = m.Item as QueryX;
                    var sp1 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty };
                    var sp2 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };

                    var sp = qx.HasSelect ? sp2 : sp1;
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    var items = QueryX_QueryX.GetChildren(qx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var cq = items[j];
                            if (!TryGetPrevModel(m, Trait.ValueXLink_M, i, cq))
                                m.ChildModels[i] = new ItemModel(m, Trait.ValueXLink_M, depth, cq, null, null, ValueLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var vd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(vd) : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    var items = QueryX_QueryX.GetChildren(vd);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var cq = items[j];
                            if (!TryGetPrevModel(m, Trait.ValueXLink_M, i, cq))
                                m.ChildModels[i] = new ItemModel(m, Trait.ValueXLink_M, depth, cq, null, null, ValueLink_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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
                    q3 = QueryX_QueryX.GetChild(q3);
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

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) => RowX_VX(m)
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.Item, IdentityStyle.Single));
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        private bool RowX_VX(ItemModel m)
        {
            var rx = m.RowX;
            ColumnX[] cx = null;
            var R = (m.IsExpandedRight && TryGetChoiceColumns(rx.Owner, out cx)) ? ((ColumnX[])null).Length : 0;
            var L = (m.IsExpandedLeft) ? 7 : 0;

            var N = R + L;
            if (N == 0) return false;

            var depth = (byte)(m.Depth + 1);

            m.PrevModels = m.ChildModels;
            m.ChildModels = new ItemModel[N];

            if (R > 0)
            {
                AddProperyModels(m, null);
            }
            if (L > 0)
            {
                GetColumnCount(rx, out int usedColumnCount, out int unusedColumnCount);
                GetChildRelationCount(rx, out int usedChidRelationCount, out int unusedChildRelationCount);
                GetParentRelationCount(rx, out int usedParentRelationCount, out int unusedParentRelationCount);

                int i = R;
                if (!TryGetPrevModel(m, Trait.RowProperty_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowProperty_ZM, depth, rx, TableX_ColumnX, null, RowPropertyList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowCompute_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowCompute_ZM, depth, rx, Store_ComputeX, null, RowComputeList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowChildRelation_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_ZM, depth, rx, TableX_ChildRelationX, null, RowChildRelationList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowParentRelation_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_ZM, depth, rx, TableX_ParentRelationX, null, RowParentRelationList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowDefaultProperty_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowDefaultProperty_ZM, depth, rx, TableX_ColumnX, null, RowDefaultPropertyList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowUnusedChildRelation_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowUnusedChildRelation_ZM, depth, rx, TableX_ChildRelationX, null, RowUnusedChildRelationList_X);

                i++;
                if (!TryGetPrevModel(m, Trait.RowUnusedParentRelation_ZM, i))
                    m.ChildModels[i] = new ItemModel(m, Trait.RowUnusedParentRelation_ZM, depth, rx, TableX_ParentRelationX, null, RowUnusedParentRelationList_X);
            }

            m.PrevModels = null;
            return true;
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

                ValidateChildModels = (m) =>
                {
                    var tx = m.TableX;
                    var N = tx.Count;
                    if (N == 0) return false;

                    if (m.Delta != tx.Delta)
                    {
                        m.Delta = tx.Delta;

                        var cx = TableX_NameProperty.GetChild(tx);
                        var items = tx.Items;
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var row = items[i];
                            if (!TryGetPrevModel(m, Trait.Row_M, i, row, tx, cx))
                                m.ChildModels[i] = new ItemModel(m, Trait.Row_M, depth, row, tx, cx, RowX_X);
                        }

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = 5;
                    if (m.ChildModelCount != N)
                    {
                            var g = m.Graph;
                            var depth = (byte)(m.Depth + 1);

                            m.PrevModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            var i = 0;
                            if (!TryGetPrevModel(m, Trait.GraphNodeList_M, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphNodeList_M, depth, g, null, null, GraphNodeList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphEdgeList_M, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphEdgeList_M, depth, g, null, null, GraphEdgeList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphOpenList_M, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphOpenList_M, depth, g, null, null, GraphOpenList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphRootList_M, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphRootList_M, depth, g, null, null, GraphRootList_X);

                            i++;
                            if (!TryGetPrevModel(m, Trait.GraphLevelList_M, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphLevelList_M, depth, g, null, null, GraphLevelList_X);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;
                    var re = m.RelationX;

                    var N = re.ChildCount(rx);
                    if (N == 0) return false;

                    var items = re.GetChildren(rx);
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rr = items[i];
                        if (!TryGetPrevModel(m, Trait.RowRelatedChild_M, i, rr))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowRelatedChild_M, depth, rr, re, rx, RowRelatedChild_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;
                    var re = m.RelationX;

                    var N = re.TryGetParents(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rr = items[i];
                        if (!TryGetPrevModel(m, Trait.RowRelatedParent_M, i, rr))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowRelatedParent_M, depth, rr, re, rx, RowRelatedParent_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) => RowX_VX(m),
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

                ValidateChildModels = (m) => RowX_VX(m),
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
                    GetColumnCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;

                    var N = TryGetUsedColumns(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var cx = items[i] as ColumnX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, cx))
                            m.ChildModels[i] = NewPropertyModel(m, depth, rx, cx);
                    }

                    m.PrevModels = null;
                    return true;
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
                    GetChildRelationCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;

                    var N = TryGetUsedChildRelations(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, rel))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_M, depth, rx, rel, null, RowChildRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
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
                    GetParentRelationCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;

                    var N = TryGetUsedParentRelations(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var re = items[i] as RelationX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, re))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_M, depth, rx, re, null, RowParentRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B4 RowDefaultPropertyList  ===================================
        ModelAction RowDefaultPropertyList_X;
        void Initialize_RowDefaultPropertyList_X()
        {
            RowDefaultPropertyList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    GetColumnCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;

                    var N = TryGetUnusedColumns(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var prop = items[i] as ColumnX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, prop))
                            m.ChildModels[i] = NewPropertyModel(m, depth, rx, prop);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B5 RowUnusedChildRelationList  ===============================
        ModelAction RowUnusedChildRelationList_X;
        void Initialize_RowUnusedChildRelationList_X()
        {
            RowUnusedChildRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    GetChildRelationCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.RowX;

                    var N = TryGetUnusedChildRelations(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var re = items[i] as RelationX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, re))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_M, depth, rx, re, null, RowChildRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 6B6 RowUnusedParentRelationList  ==============================
        ModelAction RowUnusedParentRelationList_X;
        void Initialize_RowUnusedParentRelationList_X()
        {
            RowUnusedParentRelationList_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var (kind, name) = GetKindName(m);
                    GetParentRelationCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (kind, name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelKindName = GetKindName,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var rx = m.Item as RowX;

                    var N = TryGetUnusedParentRelations(rx, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var re = items[i] as RelationX;

                        if (!TryGetPrevModel(m, Trait.Empty, i, rx, re))
                            m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_M, depth, rx, re, null, RowParentRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var ro = m.Item;
                    var st = ro.Owner;

                    var N = Store_ComputeX.ChildCount(st);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    var list = Store_ComputeX.GetChildren(st);
                    for (int i = 0; i < N; i++)
                    {
                        var cx = list[i];
                        if (!TryGetPrevModel(m, Trait.TextProperty_M, i, ro, cx))
                            m.ChildModels[i] = new ItemModel(m, Trait.TextProperty_M, depth, ro, cx, null, TextCompute_X);
                    }

                    m.PrevModels = null;
                    return false;
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

                ValidateChildModels = (m) =>
                {
                    var q = m.Query;

                    var N = q.TryGetItems(out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.QueryRootItem_M, i, itm, q))
                            m.ChildModels[i] = new ItemModel(m, Trait.QueryRootItem_M, depth, itm, q, null, QueryRootItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) => QueryPathLink_VX(m),
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

                ValidateChildModels = (m) => QueryPathLink_VX(m),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private bool QueryPathLink_VX(ItemModel m)
        {
            var q = m.Query;
            if (!q.TryGetItems(out Item[] items)) return false;

            var N = items.Length;
            var depth = (byte)(m.Depth + 1);

            m.PrevModels = m.ChildModels;
            m.ChildModels = new ItemModel[N];

            for (int i = 0; i < N; i++)
            {
                var itm = items[i];
                if (q.IsTail)
                {
                    if (!TryGetPrevModel(m, Trait.QueryPathTail_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryPathTail_M, depth, itm, q, null, QueryPathTail_X);
                }
                else
                {
                    if (!TryGetPrevModel(m, Trait.QueryPathStep_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryPathStep_M, depth, itm, q, null, QueryPathStep_X);
                }
            }

            m.PrevModels = null;
            return true;
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

                ValidateChildModels = (m) => QueryGroupLink_VX(m),
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

                ValidateChildModels = (m) => QueryGroupLink_VX(m),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private bool QueryGroupLink_VX(ItemModel m)
        {
            var q = m.Query;
            var N = q.TryGetItems(out Item[] items) ? items.Length : 0;
            if (N == 0) return false;

            var depth = (byte)(m.Depth + 1);

            m.PrevModels = m.ChildModels;
            m.ChildModels = new ItemModel[N];

            for (int i = 0; i < N; i++)
            {
                var itm = items[i];
                if (q.IsTail)
                {
                    if (!TryGetPrevModel(m, Trait.QueryGroupTail_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupTail_M, depth, itm, q, null, QueryGroupTail_X);
                }
                else
                {
                    if (!TryGetPrevModel(m, Trait.QueryGroupStep_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupStep_M, depth, itm, q, null, QueryGroupStep_X);
                }
            }

            m.PrevModels = null;
            return true;
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

                ValidateChildModels = (m) => QueryEgressLink_VX(m),
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

                ValidateChildModels = (m) => QueryEgressLink_VX(m),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX));
        }
        private bool QueryEgressLink_VX(ItemModel m)
        {
            var q = m.Query;
            var N = q.TryGetItems(out Item[] items) ? items.Length : 0;
            if (N == 0) return false;

            var depth = (byte)(m.Depth + 1);

            m.PrevModels = m.ChildModels;
            m.ChildModels = new ItemModel[N];

            for (int i = 0; i < N; i++)
            {
                var itm = items[i];
                if (q.IsTail)
                {
                    if (!TryGetPrevModel(m, Trait.QueryEgressTail_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressTail_M, depth, itm, q, null, QueryEgressTail_X);
                }
                else
                {
                    if (!TryGetPrevModel(m, Trait.QueryEgressStep_M, i, itm, q))
                        m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressStep_M, depth, itm, q, null, QueryEgressStep_X);
                }
            }

            m.PrevModels = null;
            return true;
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var q = m.Query;

                    var N = q.TryGetQuerys(itm, out Query[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        q = items[i];
                        if (q.IsGraphLink)
                        {
                            if (!TryGetPrevModel(m, Trait.QueryRootLink_M, i, itm, q))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryRootLink_M, depth, itm, q, null, QueryRootLink_X);
                        }
                        else if (q.IsPathHead)
                        {
                            if (!TryGetPrevModel(m, Trait.QueryPathHead_M, i, itm, q))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryPathHead_M, depth, itm, q, null, QueryPathHead_X);
                        }
                        else if (q.IsGroupHead)
                        {
                            if (!TryGetPrevModel(m, Trait.QueryGroupHead_M, i, itm, q))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupHead_M, depth, itm, q, null, QueryGroupHead_X);
                        }
                        else if (q.IsSegueHead)
                        {
                            if (!TryGetPrevModel(m, Trait.QueryEgressHead_M, i, itm, q))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressHead_M, depth, itm, q, null, QueryEgressHead_X);
                        }
                        else
                            throw new Exception("Invalid Query");
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var q = m.Query;

                    var N = q.TryGetQuerys(itm, out Query[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        q = items[i];
                        if (!TryGetPrevModel(m, Trait.QueryPathLink_M, i, itm, q))
                            m.ChildModels[i] = new ItemModel(m, Trait.QueryPathLink_M, depth, itm, q, null, QueryPathLink_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var q = m.Query;

                    var N = q.TryGetQuerys(itm, out Query[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        q = items[i];
                        if (!TryGetPrevModel(m, Trait.QueryGroupLink_M, i, itm, q))
                            m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupLink_M, depth, itm, q, null, QueryGroupLink_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var q = m.Query;

                    var N = q.TryGetQuerys(itm, out Query[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        q = items[i];
                        if (!TryGetPrevModel(m, Trait.QueryEgressLink_M, i, itm, q))
                            m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressLink_M, depth, itm, q, null, QueryEgressLink_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                    var items = GraphX_QueryX.GetChildren(gx);
                    foreach (var item in items)
                    {
                        if (item.IsQueryGraphRoot && Store_QueryX.TryGetParent(item, out st) && d.Item.Owner == st) break;
                    }
                    if (st == null) return DropAction.None;

                    foreach (var tg in gx.Items)
                    {
                        if (tg.RootItem == d.Item) return DropAction.None;
                    }

                    if (doDrop)
                    {
                        CreateGraph(gx, out Graph g, d.Item);

                        m.IsExpandedLeft = true;
                        MajorDelta += 1;

                        var root = m.GetRootModel();
                        root.UIRequestCreatePage(ControlType.GraphDisplay, Trait.GraphRef_M, GraphRef_X, m);
                    }
                    return DropAction.Copy;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var gx = m.GraphX;
                    var N = gx.Count;
                    if (N == 0) return false;

                    var items = gx.Items;
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var g = items[i] as Graph;
                        if (!TryGetPrevModel(m, Trait.Graph_M, i, g))
                            m.ChildModels[i] = new ItemModel(m, Trait.Graph_M, depth, g, null, null, Graph_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (m.GraphX.Trait.ToString(), m.GraphX.Name);
        }

        void CreateGraph(ItemModel m)
        {
            CreateGraph(m.GraphX, out Graph g);

            m.IsExpandedLeft = true;
            MajorDelta += 1;

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

                ValidateChildModels = (m) =>
                {
                    var g = m.Item as Graph;
                    var items = g.Nodes;

                    var N = items.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.GraphNode_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphNode_M, depth, itm, null, null, GraphNode_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var g = m.Graph;
                    var items = g.Edges;

                    var N = items.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.GraphEdge_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphEdge_M, depth, itm, null, null, GraphEdge_X);
                    }

                    m.PrevModels = null;
                    return false;
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

                ValidateChildModels = (m) =>
                {
                    var g = m.Item as Graph;
                    Query[] items = g.Forest;

                    var N = g.QueryCount;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var seg = items[i];
                        var tbl = seg.Item;
                        if (!TryGetPrevModel(m, Trait.GraphRoot_M, i, tbl, seg))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphRoot_M, depth, tbl, seg, null, GraphRoot_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var levels = m.Graph.Levels;
                    var N =levels.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var lv = levels[i];
                        if (!TryGetPrevModel(m, Trait.GraphLevel_M, i, lv))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphLevel_M, depth, lv, null, null, GraphLevel_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var paths = m.Level.Paths;
                    var N = paths.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var p = paths[i];
                        if (!TryGetPrevModel(m, Trait.GraphPath_M, i, p))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphPath_M, depth, p, null, null, GraphPath_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var p = m.Path;
                    var N = p.Count;
                    if (N == 0) return false;

                    var items = p.Items;
                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Path;
                        if (!TryGetPrevModel(m, Trait.GraphPath_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphPath_M, depth, itm, null, null, GraphPath_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var q = m.Query;
                    var N = q.TryGetItems(out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.QueryRootItem_M, i, itm, q))
                            m.ChildModels[i] = new ItemModel(m, Trait.QueryRootItem_M, depth, itm, q, null, QueryRootItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    List<Edge> edges = null;
                    var sp = new Property[] { _nodeCenterXYProperty, _nodeSizeWHProperty, _nodeOrientationProperty, _nodeFlipRotateProperty, _nodeLabelingProperty, _nodeResizingProperty, _nodeBarWidthProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var n = m.Node;
                    var g = n.Graph;
                    var L = (m.IsExpandedLeft && g.Node_Edges.TryGetValue(n, out edges)) ? edges.Count : 0;

                    var N = L + R;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(m, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var e = edges[j];
                            if (!TryGetPrevModel(m, Trait.GraphEdge_M, i, e))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphEdge_M, depth, e, null, null, GraphEdge_X);
                        }
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var sp = new Property[] { _edgeFace1Property, _edgeFace2Property, _edgeGnarl1Property, _edgeGnarl2Property, _edgeConnect1Property, _edgeConnect2Property };
                    var N = sp.Length;

                    if (m.ChildModelCount != N)
                    {
                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, sp);

                        m.PrevModels = null;
                    }
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var g = m.Graph;
                    var N = g.OpenQuerys.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var h = g.OpenQuerys[i].Query1;
                        var t = g.OpenQuerys[i].Query2;
                        if (!TryGetPrevModel(m, Trait.GraphOpen_M, i, g, h, t))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphOpen_M, depth, g, h, t, GraphOpen_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var childList = new List<Store>();
                    foreach (var st in _primeStores) { if (Store_ComputeX.HasChildLink(st)) childList.Add(st); }

                    var N = childList.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    var list = new List<Store>();
                    foreach (var sto in _primeStores) { if (Store_ComputeX.HasChildLink(sto)) list.Add(sto); }

                    for (int i = 0; i < N; i++)
                    {
                        var sto = childList[i];
                        if (!TryGetPrevModel(m, Trait.ComputeStore_M, i, sto))
                            m.ChildModels[i] = new ItemModel(m, Trait.ComputeStore_M, depth, sto, null, null, ComputeStore_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var st = m.Store;
                    var N = Store_ComputeX.ChildCount(st);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    var list = Store_ComputeX.GetChildren(st);
                    for (int i = 0; i < N; i++)
                    {
                        var itm = list[i];
                        if (!TryGetPrevModel(m, Trait.TextProperty_M, i, st, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.TextProperty_M, depth, st, itm, null, TextCompute_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, GetIdentity(m.Store, IdentityStyle.Single));
        }
        #endregion


        #region 7F0 InternlStoreZ  ============================================
        ModelAction InternalStoreZ_X;
        void Initialize_InternalStoreZ_X()
        {
            InternalStoreZ_X = new ModelAction
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

                ModelSummary = (m) => _localize(GetSummaryKey(Trait.InternalStore_ZM)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =


                ModelDescription = (m) => _localize(GetDescriptionKey(Trait.InternalStore_ZM)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ValidateChildModels = (m) =>
                {
                    var N = 11;
                    if (m.ChildModelCount != N)
                    {
                        var depth = (byte)(m.Depth + 1);

                        m.PrevModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        int i = 0;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _viewXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _viewXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _enumXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _enumXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _tableXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _tableXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _graphXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _graphXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _queryXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _queryXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _symbolXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _symbolXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _columnXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _columnXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _relationXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _relationXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _computeXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _computeXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _relationStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _relationStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetPrevModel(m, Trait.InternalStore_M, i, _propertyStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _propertyStore, null, null, InternalStore_X);

                        m.PrevModels = null;
                    }
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(GetNameKey(Trait.InternalStore_ZM)));
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

                ValidateChildModels = (m) =>
                {
                    var st = m.Store;
                    var N = st.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    var list = st.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetPrevModel(m, Trait.StoreItem_M, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreItem_M, depth, item, null, null, StoreItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var item = m.Item;
                    var (hasItems, hasLinks, hasChildRels, hasParentRels, count) = GetItemParms(item);
                    if (count == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[count];

                    int i = -1;
                    if (hasItems)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreItemItem_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreItemItem_ZM, depth, item, null, null, StoreItemItemZ_X);
                    }
                    if (hasLinks)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreRelationLink_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreRelationLink_ZM, depth, item, null, null, StoreRelationLinkZ_X);
                    }
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreChildRelation_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_ZM, depth, item, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreParentRelation_ZM, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_ZM, depth, item, null, null, StoreParentRelationZ_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.StoreItem));
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

        (bool, bool, bool, bool, int) GetItemParms(Item item)
        {
            var hasItems = (item is Store sto && sto.Count > 0);
            var hasLinks = (item is Relation rel && rel.GetLinksCount() > 0);
            var hasChildRels = (GetChildRelationCount(item, SubsetType.Used) > 0);
            var hasParentRels = (GetParentRelationCount(item, SubsetType.Used) > 0);

            var count = 0;
            if (hasItems) count++;
            if (hasLinks) count++;
            if (hasChildRels) count++;
            if (hasParentRels) count++;

            return (hasItems, hasLinks, hasChildRels, hasParentRels, count);
        }
        #endregion

        #region 7F4 StoreItemItemZ  ===========================================
        ModelAction StoreItemItemZ_X;
        void Initialize_StoreItemItemZ_X()
        {
            StoreItemItemZ_X = new ModelAction
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

                ValidateChildModels = (m) =>
                {
                    var st = m.Store;
                    var N = st.Count;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    var list = st.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetPrevModel(m, Trait.StoreItemItem_M, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreItemItem_M, depth, item, null, null, StoreItemItem_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) =>  (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F5 StoreRelationLinkZ  =======================================
        ModelAction StoreRelationLinkZ_X;
        void Initialize_StoreRelationLinkZ_X()
        {
            StoreRelationLinkZ_X = new ModelAction
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

                ValidateChildModels = (m) =>
                {
                    var re = m.Relation;
                    var N = re.GetLinks(out Item[] parents, out Item[] children);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var parent = parents[i];
                        var child = children[i];
                        if (!TryGetPrevModel(m, Trait.StoreRelationLink_M, i, re, parent, child))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreRelationLink_M, depth, re, parent, child, StoreRelationLink_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F6 StoreChildRelationZ  ======================================
        ModelAction StoreChildRelationZ_X;
        void Initialize_StoreChildRelationZ_X()
        {
            StoreChildRelationZ_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = GetChildRelationCount(m.Item, SubsetType.Used);
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var N = (TryGetChildRelations(itm, out Relation[] relations, SubsetType.Used)) ? relations.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetPrevModel(m, Trait.StoreChildRelation_M, i, rel, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_M, depth, rel, itm, null, StoreChildRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (null, _localize(m.NameKey));
        }
        #endregion

        #region 7F7 StoreParentRelationZ  =====================================
        ModelAction StoreParentRelationZ_X;
        void Initialize_StoreParentRelationZ_X()
        {
            StoreParentRelationZ_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = GetParentRelationCount(m.Item, SubsetType.Used);
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var N = (TryGetParentRelations(itm, out Relation[] relations, SubsetType.Used)) ? relations.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetPrevModel(m, Trait.StoreParentRelation_M, i, rel, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_M, depth, rel, itm, null, StoreParentRelation_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = m.Relation.TryGetChildren(m.Aux1, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.StoreRelatedItem_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreRelatedItem_M, depth, itm, null, null, StoreRelatedItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var N = m.Relation.TryGetParents(m.Aux1, out Item[] items) ? items.Length : 0;
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetPrevModel(m, Trait.StoreRelatedItem_M, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreRelatedItem_M, depth, itm, null, null, StoreRelatedItem_X);
                    }

                    m.PrevModels = null;
                    return true;
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

                ValidateChildModels = (m) =>
                {
                    var itm = m.Item;
                    var (hasChildRels, hasParentRels, N) = GetItemParms(itm);
                    if (N == 0) return false;

                    var depth = (byte)(m.Depth + 1);

                    m.PrevModels = m.ChildModels;
                    m.ChildModels = new ItemModel[N];

                    int i = -1;
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreChildRelation_ZM, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_ZM, depth, itm, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetPrevModel(m, Trait.StoreParentRelation_ZM, i, itm))
                            m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_ZM, depth, itm, null, null, StoreParentRelationZ_X);
                    }

                    m.PrevModels = null;
                    return true;
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (string, string) GetKindName(ItemModel m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.StoreItem));

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            (bool, bool, int) GetItemParms(Item item)
            {
                var hasChildRels = (GetChildRelationCount(item, SubsetType.Used) > 0);
                var hasParentRels = (GetParentRelationCount(item, SubsetType.Used) > 0);
                var count = 0;
                if (hasChildRels) count++;
                if (hasParentRels) count++;

                return (hasChildRels, hasParentRels, count);
            }
        }
        #endregion
    }
}
