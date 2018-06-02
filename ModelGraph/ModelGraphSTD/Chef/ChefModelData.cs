using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region Initialize_ModelActions  ======================================
        void Initialize_ModelActions()
        {
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

        #region TryGetOldModel  ===============================================
        /// <summary>
        /// Try to find and reuse an existing model matching the callers parameters
        /// </summary>
        private static bool TryGetOldModel(ItemModel model, Trait trait, ItemModel[] oldModels, int index, Item itm1 = null, Item itm2 = null, Item itm3 = null)
        {
            if (oldModels == null || oldModels.Length == 0) return false;

            var N = oldModels.Length;
            var i = (index < N) ? index : N / 2;
            for (int n = 0; n < N; n++, i = (i + 1) % N)
            {
                var mod = oldModels[i];
                if (mod == null) continue;
                if (trait != Trait.Empty && trait != mod.Trait) continue;
                if (itm1 != null && itm1 != mod.Item) continue;
                if (itm2 != null && itm2 != mod.Aux1) continue;
                if (itm3 != null && itm3 != mod.Aux2) continue;
                oldModels[i] = null;
                model.ChildModels[index] = mod;
                return true;
            }
            return false;
        }
        #endregion

        #region TreeModelStack  ===============================================
        /// <summary>
        /// Keep track of unvisited nodes in a depth first graph traversal
        /// </summary>
        private class TreeModelStack
        {
            private int _count;
            private (ItemModel[] Models, int Index)[] _stack;
            internal bool IsNotEmpty => (_count > 0);

            #region Constructor  ==============================================
            private const int minLength = 25;
            internal TreeModelStack(int capacity = minLength)
            {
                var length = (capacity < minLength) ? minLength : capacity;
                _stack = new(ItemModel[] Models, int Index)[length];
            }
            #endregion

            #region PushChildren  =============================================
            /// <summary>
            /// Push the ChildModels (if any)
            /// </summary>
            internal int PushChildren(ItemModel model)
            {
                var models = model.ChildModels;
                var length = (models == null) ? 0 : models.Length;

                if (length > 0)
                {
                    _stack[_count] = (models, 0);
                    _count += 1;
                    if (_count == _stack.Length)
                    {
                        var oldStack = _stack;
                        _stack = new(ItemModel[] Models, int Index)[_count  * 2];
                        Array.Copy(oldStack, _stack, _count);
                    }
                }
                return length;
            }
            #endregion

            #region PopNext  ==================================================
            /// <summary>
            /// Get the next unvisited model in the TreeModelTree
            /// </summary>
            /// <returns></returns>
            internal ItemModel PopNext()
            {
                var end = _count - 1;
                var entry = _stack[end];

                var index = entry.Index;
                var model = entry.Models[index];

                entry.Index = index + 1;
                if (entry.Index < entry.Models.Length)
                    _stack[end] = entry;
                else
                    _count = end; //don't worry about the trash at _stack[_count], it will be overwritten

                return model;
            }
            #endregion
        }
        #endregion

        #region ValidateModelTree =============================================
        public bool ValidateModelTree(RootModel root, ChangeType change = ChangeType.NoChange)
        {
            var oldModels = root.ViewModels;
            var oldLen = (oldModels == null) ? 0 : oldModels.Length;

            var rebuildTree = (root.MajorDelta != MajorDelta) || (oldModels == null) || (change != ChangeType.NoChange);
            var anyChange = (root.MinorDelta != MinorDelta) || rebuildTree;
            if (!anyChange) return false;

            root.MajorDelta = MajorDelta;
            root.MinorDelta = MinorDelta;

            UpdateSelectModel();

            var modelStack = new TreeModelStack();
            var validFilter = new HashSet<object>();
            if (rebuildTree)
            {
                //=============================================================
                // rebuild the itemModel tree
                //=============================================================
                root.Validate();
                var newLen = modelStack.PushChildren(root); // count the number of child models
                while (modelStack.IsNotEmpty)
                {
                    var model = modelStack.PopNext();
                    if (root.ViewFilter.ContainsKey(model)) validFilter.Add(model);

                    ValidateModel(model);
                    newLen += modelStack.PushChildren(model); // count the numer of child models
                }
                ValidateViewFilters();
                //=============================================================
                // build a flat array representation of the itemModel tree
                //=============================================================
                var n = 0;
                var newModels = root.ViewModels = new ItemModel[newLen]; // create array of the correct size

                modelStack.PushChildren(root);
                while (modelStack.IsNotEmpty)
                {
                    var model = newModels[n++] = modelStack.PopNext();
                    modelStack.PushChildren(model);                    
                }
                //=============================================================
                // try to scroll to the previously visible location
                //=============================================================
                if (newLen > 0)
                {
                    if (oldLen > 0)
                    {
                        var index1 = root.ViewIndex1;
                        var index2 = root.ViewIndex2;
                        var select = root.ViewSelectModel;
                        var index = IndexOf(select, index1, index2);
                        var delta = OldDelta(select, index1, index2);

                        if (IndexOf(oldModels[index1], index1, index2) < 0)
                        {
                            if (index < 0)
                            {
                                index = IndexOf(oldModels[index2 - 1], index1, index2);
                                if (index < 0)
                                {
                                    root.ViewSelectModel = FindSelectParent();
                                    if (index < 0)
                                        SetScroll(0);
                                    else
                                       SetScroll(index);
                                }
                                else // did not find newModels[index1] or select, but did find newModels[index2 - 1]
                                {
                                    if (index < 0 && delta > 0) // old select was deleted ?
                                        root.ViewSelectModel = root.ViewSelectModel = oldModels[delta - 1];
                                    SetScroll(index2 - root.ViewCapacity);
                                }
                            }
                            else // did not find newModels[index1], but did find select
                            {
                                SetScroll(index - delta);
                            }
                        }
                        else // found newModels[index1]
                        {
                            if (index < 0 && delta > 0) // old select was deleted ?
                            {
                                var model = oldModels[delta - 1];
                                if (IndexOf(model, index1, index2) < 0)
                                    model = FindSelectParent();
                                root.ViewSelectModel = model;
                            }
                            SetScroll(index1);
                        }

                        #region FindSelectParent  =============================
                        ItemModel FindSelectParent()
                        {
                            var mod = select;
                            while (mod != null && mod.ParentModel != null)
                            {
                                mod = mod.ParentModel;
                                index = IndexOf(mod, 0, newLen);
                                if (index >= 0) break;
                            }
                            return mod;
                        }
                        #endregion
                    }
                    else
                    {
                        SetScroll(0);
                    }

                    #region IndexOf  ==========================================
                    int IndexOf(ItemModel model, int index1, int index2)
                    {
                        if (index2 > newLen) index2 = newLen;

                        for (int i = index1; i < index2; i++)
                        {
                            if (model == newModels[i]) return i;
                        }
                        return -1;
                    }
                    #endregion

                    #region OldDelta  =========================================
                    int OldDelta(ItemModel model, int index1, int index2)
                    {
                        if (index2 > oldLen) index2 = oldLen;
                        for (int i = index1; i < index2; i++)
                        {
                            if (model == oldModels[i]) return (i - index1);
                        }
                        return -1;
                    }
                    #endregion

                    #region SetScroll  ========================================
                    void SetScroll(int index1)
                    {
                        if (index1 < 0) index1 = 0;

                        if (root.ViewCapacity < newLen)
                        {
                            if ((index1 + root.ViewCapacity) < newLen)
                            {
                                root.ViewIndex1 = index1;
                                root.ViewIndex2 = index1 + root.ViewCapacity;
                            }
                            else
                            {
                                root.ViewIndex1 = newLen - root.ViewCapacity;
                                root.ViewIndex2 = newLen;
                            }
                        }
                        else
                        {
                            root.ViewIndex1 = 0;
                            root.ViewIndex2 = newLen;
                        }
                        if (IndexOf(root.ViewSelectModel, root.ViewIndex1, root.ViewIndex2) < 0)
                            root.ViewSelectModel = root.ViewModels[root.ViewIndex1];
                    }
                    #endregion
                }
                else
                {
                    root.ViewIndex1 = root.ViewIndex2 = 0;
                    root.ViewSelectModel = null;
                }
            }

            return anyChange;

            #region UpdateSelectModel  ========================================
            void UpdateSelectModel()
            {
                if (root.ViewSelectModel != null)
                {
                    switch (change)
                    {
                        case ChangeType.ToggleLeft:
                            root.ViewSelectModel.IsExpandedLeft = !root.ViewSelectModel.IsExpandedLeft;
                            break;

                        case ChangeType.ExpandLeft:
                            root.ViewSelectModel.IsExpandedLeft = true;
                            break;

                        case ChangeType.CollapseLeft:
                            root.ViewSelectModel.IsExpandedLeft = false;
                            root.ViewSelectModel.IsExpandedRight = false;
                            root.ViewSelectModel.IsExpandedFilter = false;
                            root.ViewFilter.Remove(root.ViewSelectModel);
                            break;

                        case ChangeType.ToggleRight:
                            root.ViewSelectModel.IsExpandedRight = !root.ViewSelectModel.IsExpandedRight;
                            break;

                        case ChangeType.ExpandRight:
                            root.ViewSelectModel.IsExpandedRight = true;
                            break;

                        case ChangeType.CollapseRight:
                            root.ViewSelectModel.IsExpandedRight = false;
                            break;

                        case ChangeType.ToggleFilter:
                            root.ViewSelectModel.IsExpandedFilter = !root.ViewSelectModel.IsExpandedFilter;
                            if (root.ViewSelectModel.IsExpandedFilter)
                                root.ViewFilter[root.ViewSelectModel] = string.Empty;
                            else
                                root.ViewFilter.Remove(root.ViewSelectModel);
                            break;

                        case ChangeType.ExpandFilter:
                            root.ViewSelectModel.IsExpandedFilter = true;
                            root.ViewFilter[root.ViewSelectModel] = string.Empty;
                            break;

                        case ChangeType.CollapseFilter:
                            root.ViewSelectModel.IsExpandedFilter = false;
                            root.ViewFilter.Remove(root.ViewSelectModel);
                            break;
                    }
                }
            }
            #endregion

            #region ValidateModel  ============================================
            void ValidateModel(ItemModel model)
            {
                if (model.Item.AutoExpandRight)
                {
                    model.IsExpandedRight = true;
                    model.Item.AutoExpandRight = false;
                }

                if (rebuildTree || model.IsChanged)
                {
                    model.IsChanged = false;

                    if (model.IsExpandedLeft || model.IsExpandedRight || model.ChildModels != null)
                    {
                        model.Validate();
                        CheckFilterSort(model);
                    }
                    else
                    {
                        model.ChildModels = null;
                    }
                }
            }
            #endregion

            #region ValidateViewFilters  ======================================
            void ValidateViewFilters()
            {
                var invalidFilter = new HashSet<object>();
                foreach (var e in root.ViewFilter)
                {
                    if (validFilter.Contains(e.Key)) continue;
                    invalidFilter.Add(e.Key);
                }
                foreach (var key in invalidFilter)
                {
                    root.ViewFilter.Remove(key);
                }
            }
            #endregion
        }
        #endregion


        #region CheckFilterSort  ==============================================
        private void CheckFilterSort(ItemModel model)
        {
            var children = model.ChildModels;
            var count = (children == null) ? 0 : children.Length;
            if (count > 2 && (model.IsSorted || model.IsExpandedFilter))
            {
                // get the strings needed for the filter sort operations
                var items = new List<FilterSortItem>(count);

                if (TryGetFilter(model, out Regex filter))
                {
                    foreach (var child in children)
                    {
                        var name = GetFilterSortName(child);
                        if (!filter.IsMatch(name)) continue;
                        items.Add(new FilterSortItem(child, name));
                    }
                }
                else
                {
                    foreach (var child in children)
                    {
                        items.Add(new FilterSortItem(child, GetFilterSortName(child)));
                    }
                }
                if (model.IsSorted) items.Sort(alphabeticOrder);
                if (model.IsSortDescending) items.Reverse();

                count = items.Count;
                children = new ItemModel[count];
                for (int i = 0; i < count; i++)
                {
                    children[i] = items[i].Model;
                }
                model.ChildModels = children;
            }
        }
        private string GetFilterSortName(ItemModel model)
        {
            var refRoot = PrimaryRootModel;
            refRoot.GetModelItemData(model);
            var kind = string.IsNullOrEmpty(refRoot.ModelKind) ? " " : refRoot.ModelKind;
            var name = string.IsNullOrEmpty(refRoot.ModelName) ? " " : refRoot.ModelName;
            return $"{kind} {name}";
        }
        private bool TryGetFilter(ItemModel model, out Regex filter)
        {
            filter = null;
            if (model.IsExpandedFilter)
            {
                var root = model.GetRootModel();
                if (root.ViewFilter.TryGetValue(model, out string str))
                    filter = new Regex(".*" + str + ".*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            return (filter != null);
        }
        private class FilterSortItem
        {
            internal string Name;
            internal ItemModel Model;

            internal FilterSortItem(ItemModel model, string name)
            {
                Name = name;
                Model = model;
            }
        }
        private class AlphabeticOrder : IComparer<FilterSortItem>
        {
            int IComparer<FilterSortItem>.Compare(FilterSortItem x, FilterSortItem y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        private static AlphabeticOrder alphabeticOrder = new AlphabeticOrder();
        #endregion


        #region AddProperyModels  =============================================
        private void AddProperyModels(ItemModel model, ItemModel[] oldModels, ColumnX[] cols)
        {
            var item = model.Item;
            var N = cols.Length;
            for (int i = 0; i < N; i++)
            {
                var col = cols[i];
                if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, col))
                {
                    model.ChildModels[i] = NewPropertyModel(model, model.Level, item, col);
                }
            }
        }
        private void AddProperyModels(ItemModel model, ItemModel[] oldModels, Property[] props)
        {
            var item = model.Item;
            var N = props.Length;
            for (int i = 0; i < N; i++)
            {
                var prop = props[i];
                if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, prop))
                {
                    if (prop.IsColumnX)
                        model.ChildModels[i] = NewPropertyModel(model, model.Level, item, (prop as ColumnX));
                    else if (prop.IsComputeX)
                        model.ChildModels[i] = NewPropertyModel(model, model.Level, item, (prop as ComputeX));
                    else
                        model.ChildModels[i] = NewPropertyModel(model, model.Level, item, prop);

                    if (prop.IsReadOnly) model.ChildModels[i].IsReadOnly = true;
                    if (prop.CanMultiline) model.ChildModels[i].CanMultiline = true;
                }
            }
        }
        private ItemModel NewPropertyModel(ItemModel model, byte level, Item item, ColumnX col)
        {
            if (EnumX_ColumnX.TryGetParent(col, out EnumX enu))
                return new ItemModel(model, Trait.ComboProperty_M, level, item, col, enu, ComboColomn_X);
            else if (col.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, level, item, col, null, CheckColumn_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, level, item, col, null, TextColumn_M);
        }
        private ItemModel NewPropertyModel(ItemModel model, byte level, Item item, ComputeX cx)
        {
            if (EnumX_ColumnX.TryGetParent(cx, out EnumX enu))
                return new ItemModel(model, Trait.ComboProperty_M, level, item, cx, enu, ComboProperty_X);
            else if (cx.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, level, item, cx, null, CheckProperty_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, level, item, cx, null, TextCompute_X);
        }
        private ItemModel NewPropertyModel(ItemModel model, byte level, Item item, Property prop)
        {
            if (Property_Enum.TryGetValue(prop, out EnumZ enu))
                return new ItemModel(model, Trait.ComboProperty_M, level, item, prop, enu, ComboProperty_X);
            else if (prop.Value.ValType == ValType.Bool)
                return new ItemModel(model, Trait.CheckProperty_M, level, item, prop, null, CheckProperty_X);
            else
                return new ItemModel(model, Trait.TextProperty_M, level, item, prop, null, TextProperty_X);
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

        #region GetAppCommands  ===============================================
        internal void GetAppCommands(RootModel root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.NewCommand, AppRootNewModel));
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.OpenCommand, AppRootOpenModel));
                    break;

                case ControlType.PrimaryTree:
                    if (root.Chef.Repository == null)
                        root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.SaveAsCommand, AppRootSaveAsModel));
                    else
                        root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.SaveCommand, AppRootSaveModel));

                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.CloseCommand, AppRootCloseModel));
                    if (root.Chef.Repository != null)
                        root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.ReloadCommand, AppRootReloadModel));
                    break;

                case ControlType.PartialTree:
                    break;

                case ControlType.GraphDisplay:
                    break;

                case ControlType.SymbolEditor:
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.SaveCommand, AppSaveSymbol));
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.ReloadCommand, AppReloadSymbol));
                    break;
            }
        }
        private void AppRootNewModel(ItemModel model)
        {
            var root = model as RootModel;
            var rootChef = root.Chef;
            var dataChef = new Chef(rootChef, null);

            root.UIRequest = UIRequest.CreateNewView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_M);
        }
        public void AppRootOpenModel(ItemModel model, Object parm1)
        {
            var repo = parm1 as IRepository;
            var root = model as RootModel;
            var rootChef = root.Chef;
            var dataChef = new Chef(rootChef, repo);

            root.UIRequest = UIRequest.CreateNewView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_M);
        }
        internal void AppRootSaveAsModel(ItemModel model, Object parm1)
        {
            var repo = parm1 as IRepository;
            var root = model as RootModel;
            var dataChef = root.Chef;

            MajorDelta += 1;
            dataChef.SaveToRepository(repo);
        }
        private void AppRootSaveModel(ItemModel model)
        {
            var root = model as RootModel;
            var dataChef = root.Chef;

            MajorDelta += 1;
            dataChef.SaveToRepository();
        }
        private void AppRootCloseModel(ItemModel model)
        {
            var root = model as RootModel;
            root.UIRequest = UIRequest.CloseModel(root);
        }
        private void AppRootReloadModel(ItemModel model)
        {
            var root = model as RootModel;
            root.UIRequest = UIRequest.ReloadModel(root);
        }
        private void AppSaveSymbol(ItemModel model)
        {
            var root = model as RootModel;
            root.UIRequest = UIRequest.SaveModel(root);
        }
        private void AppReloadSymbol(ItemModel model)
        {
            var root = model as RootModel;
            root.UIRequest = UIRequest.ReloadModel(root);
        }
        #endregion


        #region 612 DataChef_X  ===============================================
        ModelAction DataChef_X;
        void Initialize_DataChef_X()
        {
            DataChef_X = new ModelAction
            {
                ChildCount = (m) => 1, // allow expand left, but don't display the count
                ModelName = (m) => _localize(m.NameKey),

                Validate = Validate_DataChef_X,
            };
        }
        void Validate_DataChef_X(ItemModel model)
        {
            var item = model.Item;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            var N = 4;

            if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

            var i = 0;
            if (!TryGetOldModel(model, Trait.ErrorRoot_M, oldModels, i))
                model.ChildModels[i] = new ItemModel(model, Trait.ErrorRoot_M, level, _errorStore, null, null, ErrorRoot_X);

            i++;
            if (!TryGetOldModel(model, Trait.ChangeRoot_M, oldModels, i))
                model.ChildModels[i] = new ItemModel(model, Trait.ChangeRoot_M, level, _changeRoot, null, null, ChangeRoot_X);

            i++;
            if (!TryGetOldModel(model, Trait.MetadataRoot_M, oldModels, i))
                model.ChildModels[i] = new ItemModel(model, Trait.MetadataRoot_M, level, item, null, null, MetadataRoot_X);

            i++;
            if (!TryGetOldModel(model, Trait.ModelingRoot_M, oldModels, i))
                model.ChildModels[i] = new ItemModel(model, Trait.ModelingRoot_M, level, item, null, null, ModelingRoot_X);
        }
        #endregion

        #region 614 TextColumn_X  =============================================
        ModelAction TextColumn_X;
        void Initialize_TextColumn_X()
        {
            TextColumn_X = new ModelAction
            {
                ModelName = (m) => m.ColumnX.Name,
                ModelSummary = (m) => m.ColumnX.Summary,
                ModelDescription = (m) => m.ColumnX.Description,

                TextValue = (m) => m.ColumnX.Value.GetString(m.Item),
            };
        }
        #endregion

        #region 615 CheckColumn_X  ============================================
        ModelAction CheckColumn_X;
        void Initialize_CheckColumn_X()
        {
            CheckColumn_X = new ModelAction
            {
                ModelName = (m) => m.ColumnX.Name,
                ModelSummary = (m) => m.ColumnX.Summary,
                ModelDescription = (m) => m.ColumnX.Description,

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
                ModelName = (m) => m.ColumnX.Name,
                ModelSummary = (m) => m.ColumnX.Summary,
                ModelDescription = (m) => m.ColumnX.Description,

                ListValue = (m) => GetEnumDisplayValues(m.EnumX),
                IndexValue = (m) => GetComboSelectedIndex(m.Item, m.ColumnX, m.EnumX),
            };
        }
        //=====================================================================
        string[] GetEnumDisplayValues(EnumX e)
        {
            string[] values = null;
            if (e != null && e.IsValid)
            {
                var items = e.ToArray;
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
                var items = e.ToArray;
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
                ModelName = (m) => m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {_localize(m.Property.NameKey)}" : _localize(m.Property.NameKey),
                ModelSummary = (m) => _localize(m.Property.SummaryKey),
                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                TextValue = (m) => m.Property.Value.GetString(m.Item),
            };
        }
        #endregion

        #region 618 CheckProperty_X  ==========================================
        ModelAction CheckProperty_X;
        void Initialize_CheckProperty_X()
        {
            CheckProperty_X = new ModelAction
            {
                ModelName = (m) => m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {_localize(m.Property.NameKey)}" : _localize(m.Property.NameKey),
                ModelSummary = (m) => _localize(m.Property.SummaryKey),
                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

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
                ModelName = (m) => m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {_localize(m.Property.NameKey)}" : _localize(m.Property.NameKey),
                ModelSummary = (m) => _localize(m.Property.SummaryKey),
                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                ListValue = (m) => GetEnumZNames(m.EnumZ),
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
                ModelName = (m) => m.ComputeX.Name,
                ModelSummary = (m) => m.ComputeX.Summary,
                ModelDescription = (m) => m.ComputeX.Description,

                TextValue = (m) => m.ComputeX.Value.GetString(m.Item),
            };
        }
        #endregion



        #region 621 ErrorRoot  ================================================
        ModelAction ErrorRoot_X;
        void Initialize_ErrorRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ErrorRoot_X,
                Refresh = Refresh_ErrorRoot_X,
                Validate = Validate_ErrorRoot_X
            };
        }
        void Refresh_ErrorRoot_X(RootModel root, ItemModel model)
        {
        }
        void Refresh_ErrorRoot_X(RootModel root, ItemModel model)
        {
            var store = model.Item as StoreOf<Error>;

            root.ModelName = _localize(model.NameKey);
            root.ModelCount = store.Count;

            model.CanExpandLeft = (root.ChildCount > 0);
        }
        void Validate_ErrorRoot_X(ItemModel model)
        {
            var N = model.IsExpandedLeft ? _errorStore.Count : 0;

            if (N > 0)
            {
                var items = _errorStore.ToArray;
                var item = model.Item;
                var level = (byte)(model.Level + 1);

                var oldModels = model.ChildModels;
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i] as Error;
                    if (!TryGetOldModel(model, Trait.ErrorType_M, oldModels, i, itm))
                        model.ChildModels[i] = new ItemModel(model, Trait.ErrorType_M, level, itm, null, null, ErrorType_X);
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion

        #region 622 ChangeRoot  ===============================================
        ModelAction ChangeRoot_X;
        void Initialize_ChangeRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ChangeRoot_X,
                Refresh = Refresh_ChangeRoot_X,
                Select = Select_ChangeRoot_X,
                Validate = Validate_ChangeRoot_X
            };
        }
        void Refresh_ChangeRoot_X(RootModel root, ItemModel model)
        {
        }
        void Refresh_ChangeRoot_X(RootModel root, ItemModel model)
        {
            var chg = model.Item as ChangeRoot;
            root.ModelName = _localize(model.NameKey);
            if (model.IsExpandedLeft)
            {
                _changeRootInfoItem = null;
                _changeRootInfoText = string.Empty;
            }
            root.ModelInfo = _changeRootInfoText;
            root.ModelCount = chg.Count;

            model.CanExpandLeft = (root.ChildCount > 0);
        }
        void Select_ChangeRoot_X(RootModel root, ItemModel model)
        {
            if (_changeRoot.Count > 0 && model.IsExpandedLeft == false)
                root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ExpandAllCommand, ExpandAllChangeSets));
        }
        void Validate_ChangeRoot_X(ItemModel model)
        {
            var oldModels = model.ChildModels;
            model.ChildModels = null;

            if (model.IsExpandedLeft)
            {
                var N = _changeRoot.Count;

                if (N > 0)
                {
                    var items = new List<ChangeSet>(_changeRoot.ToArray);
                    items.Reverse();
                    var level = (byte)(model.Level + 1);
                    model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as ChangeSet;
                        if (!TryGetOldModel(model, Trait.ChangeSet_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.ChangeSet_M, level, itm, null, null, ChangeSet_X);
                    }
                }
            }
        }
        #endregion

        #region 623 MetadataRoot  =============================================
        ModelAction MetadataRoot_X;
        void Initialize_MetadataRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_MetadataRoot_X,
                Refresh = Refresh_MetadataRoot_X,
                Select = Select_MetadataRoot_X,
                Validate = Validate_MetadataRoot_X
            };
        }
        void Refresh_MetadataRoot_X(RootModel root, ItemModel model)
        {
        }
        void Refresh_MetadataRoot_X(RootModel root, ItemModel model)
        {
            root.ModelName = _localize(model.NameKey);

            model.CanExpandLeft = true;
        }
        void Select_MetadataRoot_X(RootModel root, ItemModel model)
        {
            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreateSecondaryMetadataTree));
        }
        void Validate_MetadataRoot_X(ItemModel model)
        {
            var N = (model.IsExpandedLeft) ? 5 : 0;

            if (N > 0)
            {
                var level = (byte)(model.Level + 1);

                var oldModels = model.ChildModels;
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                var i = 0;
                if (!TryGetOldModel(model, Trait.ViewXView_ZM, oldModels, i, _viewXStore))
                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXView_ZM, level, _viewXStore, null, null, ViewXView_ZM);

                i++;
                if (!TryGetOldModel(model, Trait.EnumX_ZM, oldModels, i, _enumZStore))
                    model.ChildModels[i] = new ItemModel(model, Trait.EnumX_ZM, level, _enumZStore, null, null, EnumXList_X);

                i++;
                if (!TryGetOldModel(model, Trait.TableX_ZM, oldModels, i, _tableXStore))
                    model.ChildModels[i] = new ItemModel(model, Trait.TableX_ZM, level, _tableXStore, null, null, TableXList_X);

                i++;
                if (!TryGetOldModel(model, Trait.GraphX_ZM, oldModels, i, _graphXStore))
                    model.ChildModels[i] = new ItemModel(model, Trait.GraphX_ZM, level, _graphXStore, null, null, GraphXList_X);

                i++;
                if (!TryGetOldModel(model, Trait.InternalStore_ZM, oldModels, i, this))
                    model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_ZM, level, this, null, null, InternalStoreZ_X);
            }
            else
            {
                model.ChildModels = null;
            }
        }
        private void CreateSecondaryMetadataTree(ItemModel model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.PartialTree);
        }
        #endregion

        #region 624 ModelingRoot  =============================================
        ModelAction ModelingRoot_X;
        void Initialize_ModelingRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ModelingRoot_X,
                Refresh = Refresh_ModelingRoot_X,
                Select = Select_ModelingRoot_X,
                Validate = Validate_ModelingRoot_X
            };
        }
        void Refresh_ModelingRoot_X(RootModel root, ItemModel model)
        {
        }
        void Refresh_ModelingRoot_X(RootModel root, ItemModel model)
        {
            root.ModelName = _localize(model.NameKey);

            model.CanExpandLeft = true;
        }
        void Select_ModelingRoot_X(RootModel root, ItemModel model)
        {
            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreateSecondaryModelingTree));
        }
        void Validate_ModelingRoot_X(ItemModel model)
        {
            var N = (model.IsExpandedLeft) ? 4 : 0;

            if (N > 0)
            {
                var item = model.Item;
                var level = (byte)(model.Level + 1);

                var oldModels = model.ChildModels;
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                var i = 0;
                if (!TryGetOldModel(model, Trait.ViewView_ZM, oldModels, i, item))
                    model.ChildModels[i] = new ItemModel(model, Trait.ViewView_ZM, level, item, null, null, ViewView_ZM);

                i++;
                if (!TryGetOldModel(model, Trait.Table_ZM, oldModels, i, item))
                    model.ChildModels[i] = new ItemModel(model, Trait.Table_ZM, level, item, null, null, TableList_X);

                i++;
                if (!TryGetOldModel(model, Trait.Graph_ZM, oldModels, i, item))
                    model.ChildModels[i] = new ItemModel(model, Trait.Graph_ZM, level, item, null, null, GraphList_X);

                i++;
                if (!TryGetOldModel(model, Trait.PrimeCompute_M, oldModels, i, this))
                    model.ChildModels[i] = new ItemModel(model, Trait.PrimeCompute_M, level, this, null, null, PrimeCompute_X);
            }
            else
            {
                model.ChildModels = null;
            }
        }
        private void CreateSecondaryModelingTree(ItemModel model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.PartialTree);
        }
        #endregion

        #region 625 MetaRelationList  =========================================
        ModelAction MetaRelationList_X;
        void Initialize_MetaRelationList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_MetaRelationList_X,
                Validate = Validate_MetaRelationList_X
            };
        }
        void Refresh_MetaRelationList_X(RootModel root, ItemModel model)
        {
            root.ModelName = _localize(model.NameKey);

            model.CanExpandLeft = true;
        }
        void Validate_MetaRelationList_X(ItemModel model)
        {
            var item = model.Item;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            int N = (model.IsExpandedLeft) ? 2 : 0;

            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                var i = 0;
                if (!TryGetOldModel(model, Trait.NameColumnRelation_M, oldModels, i, item))
                    model.ChildModels[i] = new ItemModel(model, Trait.NameColumnRelation_M, level, item, TableX_NameProperty, null, NameColumnRelation_X);

                i++;
                if (!TryGetOldModel(model, Trait.SummaryColumnRelation_M, oldModels, i, item))
                    model.ChildModels[i] = new ItemModel(model, Trait.SummaryColumnRelation_M, level, item, TableX_SummaryProperty, null, SummaryColumnRelation_X);
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion

        #region 626 ErrorType  ================================================
        ModelAction ErrorType_X;
        void Initialize_ErrorType_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ErrorType_X,
            };
        }
        void Refresh_ErrorType_X(RootModel root, ItemModel model)
        {
        }
        internal void ErrorType_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as Error;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(item.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(item.NameKey);
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:
                        break;
                }
            }
            else  // validate the list of child models
            {
                var error = model.Item as Error;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = (model.IsExpandedLeft) ? error.Count : 0;
                if (N > 0)
                {
                    if (oldModels == null || oldModels.Length != N)
                    {
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            model.ChildModels[i] = new ItemModel(model, Trait.ErrorText_M, level, error);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 627 ErrorText  ================================================
        ModelAction ErrorText_X;
        void Initialize_ErrorText_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ErrorText_X,
            };
        }
        void Refresh_ErrorText_X(RootModel root, ItemModel model)
        {
        }
        internal void ErrorText_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:
                        break;

                    case ModelActionX.PointerOver:
                        break;

                    case ModelActionX.ModelRefresh:

                        var err = model.Item as Error;
                        var inx = model.ParentModel.GetChildlIndex(model);
                        if (inx < 0 || err.Count <= inx)
                            root.ModelName = InvalidItem;
                        else
                            root.ModelName = err.Errors[inx];
                        break;

                    case ModelActionX.ModelSelect:
                        break;
                }
            }
        }
        #endregion

        #region 628 ChangeSet  ================================================
        ModelAction ChangeSet_X;
        void Initialize_ChangeSet_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ChangeSet_X,
            };
        }
        void Refresh_ChangeSet_X(RootModel root, ItemModel model)
        {
        }
        internal void ChangeSet_X(ItemModel model, RootModel root)
        {
            var chg = model.Item as ChangeSet;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = GetChangeSetName(chg);
                        root.ModelCount = chg.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        if (chg.CanMerge)
                            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.MergeCommand, ModelMerge));
                        if (chg.CanUndo)
                            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.UndoCommand, ModelUndo));
                        if (chg.CanRedo)
                            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.RedoCommand, ModelRedo));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var cs = model.Item as ChangeSet;
                var N = model.IsExpandedLeft ? cs.Count : 0;

                if (N > 0)
                {
                    var items = (cs.IsReversed) ? cs.ToArray : cs.ItemsReversed;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as ItemChange;
                        if (!TryGetOldModel(model, Trait.ItemChange_M, oldModels, i, itm))
                        {
                            model.ChildModels[i] = new ItemModel(model, Trait.ItemChange_M, level, itm, null, null, ItemChanged_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private string GetChangeSetName(ChangeSet chg)
        {
            if (chg.IsCongealed)
                return _localize(chg.NameKey);
            else
                return chg.Name;
        }
        private void ModelMerge(ItemModel model)
        {
            var chg = model.Item as ChangeSet;
            chg.Merge();
            MajorDelta += 1;
        }
        private void ModelUndo(ItemModel model)
        {
            var chg = model.Item as ChangeSet;
            Undo(chg);
        }
        private void ModelRedo(ItemModel model)
        {
            var chg = model.Item as ChangeSet;
            Redo(chg);
        }
        #endregion

        #region 629 ItemChanged  ==============================================
        ModelAction ItemChange_X;
        void Initialize_ItemChange_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ItemChange_X,
            };
        }
        void Refresh_ItemChange_X(RootModel root, ItemModel model)
        {
        }
        internal void ItemChanged_X(ItemModel model, RootModel root)
        {
            var chg = model.Item as ItemChange;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(chg.KindKey);
                        root.ModelName = chg.Name;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 631 ViewXView_ZM  =============================================
        ModelAction ViewXView_X;
        void Initialize_ViewXView_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewXView_X,
            };
        }
        void Refresh_ViewXView_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewXView_ZM(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ViewXView_ZM_Drop;
                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);

                        var views = _viewXStore.ToArray;
                        var count = 0;
                        foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ViewXView_ZM_Insert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    var views = _viewXStore.ToArray;
                    var roots = new List<ViewX>();
                    foreach (var view in views) { if (ViewX_ViewX.HasNoParent(view)) { roots.Add(view); N++; } } 
                    
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = roots[i];
                            if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewXView_M, level, itm, null, null, ViewXView_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXView_ZM_Insert(ItemModel model)
        {
            ItemCreated(new ViewX(_viewXStore));
        }
        private DropAction ViewXView_ZM_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            var vxDrop = drop.Item as ViewX;
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
        }
        #endregion

        #region 632 ViewXView_M  ==============================================
        ModelAction ViewXViewM_X;
        void Initialize_ViewXViewM_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewXViewM_X,
            };
        }
        void Refresh_ViewXViewM_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewXView_M(ItemModel model, RootModel root)
        {
            var view = model.Item as ViewX;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ViewXView_M_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = view.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = view.Name;
                        var count = (ViewX_ViewX.ChildCount(view) + ViewX_QueryX.ChildCount(view) + ViewX_Property.ChildCount(view));
                        root.ModelCount = count;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ViewXView_M_Insert));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft || model.IsExpandedRight)
                {
                    int R = 0;
                    Property[] props = null;
                    if (model.IsExpandedRight)
                    {
                        props = new Property[] { _viewXNameProperty, _viewXSummaryProperty };
                        R = props.Length;
                    }

                    int L1 = 0, L2 = 0, L3 = 0;
                    Property[] propertyList = null;
                    QueryX[] queryList = null;
                    ViewX[] viewList = null;
                    if (model.IsExpandedLeft)
                    {
                        propertyList = ViewX_Property.GetChildren(view);
                        queryList = ViewX_QueryX.GetChildren(view);
                        viewList = ViewX_ViewX.GetChildren(view);

                        L1 = (propertyList == null) ? 0 : propertyList.Length;
                        L2 = (queryList == null) ? 0 : queryList.Length;
                        L3 = (viewList == null) ? 0 : viewList.Length;
                    }

                    N = R + L1 + L2 + L3;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(model, oldModels, props);
                        }
                        if (L1 > 0)
                        {
                            for (int i = R, j = 0; j < L1; i++, j++)
                            {
                                var px = propertyList[j];

                                if (!TryGetOldModel(model, Trait.ViewXProperty_M, oldModels, i, px))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXProperty_M, level, px, ViewX_Property, view, ViewXProperty_M);
                            }
                        }
                        if (L2 > 0)
                        {
                            for (int i = (R + L1), j = 0; j < L2; i++, j++)
                            {
                                var qx = queryList[j];
                                if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qx))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXQuery_M, level, qx, ViewX_QueryX, view, ViewXQuery_M);
                            }
                        }
                        if (L3 > 0)
                        {
                            for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                            {
                                var vx = viewList[j];
                                if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXView_M, level, vx, ViewX_ViewX, view, ViewXView_M);
                            }
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXView_M_Insert(ItemModel model)
        {
            var vx = new ViewX(_viewXStore);
            ItemCreated(vx);
            AppendLink(ViewX_ViewX, model.Item, vx);
        }
        private DropAction ViewXView_M_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            var view = model.Item as ViewX;
            if (view != null)
            {
                var vx = drop.Item as ViewX;
                if (vx != null)
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
                    var st = drop.Item as Store;
                    if (st != null)
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
                        var re = drop.Item as Relation;
                        if (re != null)
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
                        else
                        {
                            var pr = drop.Item as Property;
                            if (pr != null)
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
            }
            return DropAction.None;
        }
        #endregion

        #region 633 ViewXQuery_M  =============================================
        ModelAction ViewXQuery_X;
        void Initialize_ViewXQuery_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewXQuery_X,
            };
        }
        void Refresh_ViewXQuery_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewXQuery_M(ItemModel model, RootModel root)
        {
            var qx = model.Item as QueryX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ViewXQuery_M_Drop;
                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:


                        var rel = Relation_QueryX.GetParent(qx);
                        if (rel != null)
                        {
                            root.ModelKind = _localize(qx.KindKey);
                            root.ModelName = GetIdentity(qx, IdentityStyle.Single);
                        }
                        else
                        {
                            var sto = Store_QueryX.GetParent(qx);
                            root.ModelKind = GetIdentity(sto, IdentityStyle.Kind);
                            root.ModelName = GetIdentity(sto, IdentityStyle.Double);

                            model.CanDrag = true;
                            model.CanExpandRight = true;
                        }

                        var count = (QueryX_ViewX.ChildCount(qx) + QueryX_QueryX.ChildCount(qx) + QueryX_Property.ChildCount(qx));
                        root.ModelCount = count;

                        model.CanExpandRight = true;
                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(model.DescriptionKey);
                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ViewXQuery_M_Insert));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft || model.IsExpandedRight)
                {
                    int R = 0;

                    Property[] props = null;
                    if (model.IsExpandedRight)
                    {
                            props = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                        R = props.Length;
                    }

                    int L1 = 0, L2 = 0, L3 = 0;
                    Property[] propertyList = null;
                    QueryX[] queryList = null;
                    ViewX[] viewList = null;
                    if (model.IsExpandedLeft)
                    {
                        propertyList = QueryX_Property.GetChildren(qx);
                        queryList = QueryX_QueryX.GetChildren(qx);
                        viewList = QueryX_ViewX.GetChildren(qx);

                        L1 = (propertyList == null) ? 0 : propertyList.Length;
                        L2 = (queryList == null) ? 0 : queryList.Length;
                        L3 = (viewList == null) ? 0 : viewList.Length;
                    }

                    N = R + L1 + L2 + L3;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(model, oldModels, props);
                        }
                        if (L1 > 0)
                        {
                            for (int i = R, j = 0; j < L1; i++, j++)
                            {
                                var px = propertyList[j];

                                if (!TryGetOldModel(model, Trait.ViewXProperty_M, oldModels, i, px))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXProperty_M, level, px, QueryX_Property, qx, ViewXProperty_M);
                            }
                        }
                        if (L2 > 0)
                        {
                            for (int i = (R + L1), j = 0; j < L2; i++, j++)
                            {
                                var qr = queryList[j];
                                if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qr))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXQuery_M, level, qr, QueryX_QueryX, qx, ViewXQuery_M);
                            }
                        }
                        if (L3 > 0)
                        {
                            for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                            {
                                var vx = viewList[j];
                                if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewXView_M, level, vx, QueryX_ViewX, qx, ViewXView_M);
                            }
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXQuery_M_Insert(ItemModel model)
        {
            var vx = new ViewX(_viewXStore);
            ItemCreated(vx);
            AppendLink(QueryX_ViewX, model.Item, vx);
        }

        private DropAction ViewXQuery_M_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            var query = model.Item as QueryX;
            if (drop.Item is Relation rel)
            {
                if (doDrop)
                {
                    CreateQueryX(query, rel, QueryType.View).AutoExpandRight = false;
                }
                return DropAction.Link;
            }
            else if (drop.Item is Property prop)
            {
                if (doDrop)
                {
                    AppendLink(QueryX_Property, query, prop);
                }
                return DropAction.Link;
            }
            return DropAction.None;
        }
        #endregion

        #region 634 ViewXCommand  =============================================
        ModelAction ViewXCommand_X;
        void Initialize_ViewXCommand_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewXCommand_X,
            };
        }
        void Refresh_ViewXCommand_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewXCommand_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion

        #region 635 ViewXProperty_M  ==========================================
        ModelAction ViewXProperty_X;
        void Initialize_ViewXProperty_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewXProperty_X,
            };
        }
        void Refresh_ViewXProperty_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewXProperty_M(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as Property;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.Double);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion


        #region 63A ViewView_ZM  ==============================================
        ModelAction ViewView_X;
        void Initialize_ViewView_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewView_X,
            };
        }
        void Refresh_ViewView_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewView_ZM(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        var views = _viewXStore.ToArray;
                        var count = 0;
                        foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    var views = _viewXStore.ToArray;
                    var roots = new List<ViewX>();
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) { roots.Add(vx); N++; } }

                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = roots[i];
                            if (!TryGetOldModel(model, Trait.ViewView_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewView_M, level, itm, null, null, ViewView_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion

        #region 63B ViewView_M  ===============================================
        ModelAction ViewViewM_X;
        void Initialize_ViewViewM_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewViewM_X,
            };
        }
        void Refresh_ViewViewM_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewView_M(ItemModel model, RootModel root)
        {
            var view = model.Item as ViewX;
            var key = model.Aux1; // may be null

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = view.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        var count = 0;
                        var querys = ViewX_QueryX.GetChildren(view);
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
                            count = ViewX_ViewX.ChildCount(view);
                        }
                       
                        root.ModelName = view.Name;
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    var propertyList = ViewX_Property.GetChildren(view);
                    var queryList = ViewX_QueryX.GetChildren(view);
                    var viewList = ViewX_ViewX.GetChildren(view);

                    var L1 = (propertyList == null) ? 0 : propertyList.Length;
                    var L2 = (queryList == null) ? 0 : queryList.Length;
                    var L3 = (viewList == null) ? 0 : viewList.Length;

                    if (L2 == 1 && Store_QueryX.HasParentLink(queryList[0]) && TryGetQueryItems(queryList[0], out Item[] items))
                    {
                        N = items.Length;
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];

                            if (!TryGetOldModel(model, Trait.ViewItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewItem_M, level, itm, queryList[0], null, ViewItem_M);
                        }
                    }
                    else if (key != null && L2 > 0)
                    {
                        N = L2;
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var qx = queryList[i];

                            if (!TryGetOldModel(model, Trait.ViewQuery_M, oldModels, i, qx))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewQuery_M, level, qx, key, null, ViewQuery_M);
                        }
                    }
                    else if (L3 > 0)
                    {
                        N = L3;
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var vx = viewList[i];

                            if (!TryGetOldModel(model, Trait.ViewView_M, oldModels, i, vx))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewView_M, level, vx, null, null, ViewView_M);
                        }
                    }
                    //N = L1 + L2 + L3;
                    //if (N > 0)
                    //{
                    //    var level = (byte)(model.Level + 1);
                    //    var oldModels = model.ChildModels;
                    //    model.ChildModels = new TreeModel[N];

                    //    if (L1 > 0)
                    //    {
                    //        for (int i = 0, j = 0; j < L1; i++, j++)
                    //        {
                    //            var px = propertyList[j];

                    //            if (!TryGetOldModel(model, Trait.ViewXProperty_M, oldModels, i, px))
                    //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXProperty_M, level, px, ViewX_P, view, ViewXProperty_M);
                    //        }
                    //    }
                    //    if (L2 > 0)
                    //    {
                    //        for (int i = (L1), j = 0; j < L2; i++, j++)
                    //        {
                    //            var qx = queryList[j];
                    //            if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qx))
                    //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXQuery_M, level, qx, ViewX_QueryX, view, ViewQuery_M);
                    //        }
                    //    }
                    //    if (L3 > 0)
                    //    {
                    //        for (int i = (L1 + L2), j = 0; j < L3; i++, j++)
                    //        {
                    //            var vx = viewList[j];
                    //            if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                    //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXView_M, level, vx, ViewX_ViewX, view, ViewView_M);
                    //        }
                    //    }
                    //}
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion

        #region 63C ViewItem_M  ===============================================
        ModelAction ViewItem_X;
        void Initialize_ViewItem_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewItem_X,
            };
        }
        void Refresh_ViewItem_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewItem_M(ItemModel model, RootModel root)
        {
            var item = model.Item;
            var query = model.Aux1 as QueryX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = GetIdentity(item.Owner, IdentityStyle.Single);
                        root.ModelName = GetIdentity(item, IdentityStyle.Single);

                        var qc = GetQueryXChildren(query);
                        var count = (qc.L2 + qc.L3);
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanExpandRight = qc.L1 > 0;
                        model.CanFilterUsage = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft || model.IsExpandedRight)
                {
                    var qxc = GetQueryXChildren(query);

                    int R = (model.IsExpandedRight) ? qxc.L1 : 0;
                    int L = (model.IsExpandedLeft) ? (qxc.L2 + qxc.L3) : 0;

                    N = R + L;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(model, oldModels, qxc.PropertyList);
                        }

                        if (L > 0)
                        {
                            int i = R;
                            for (int j = 0; j < qxc.L2; i++, j++)
                            {
                                var qx = qxc.QueryList[j];
                                if (!TryGetOldModel(model, Trait.ViewQuery_M, oldModels, i, item, qx))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ViewQuery_M, level, item, qx, null, ViewQuery_M);
                            }
                            for (int j = 0; j < qxc.L3; i++, j++)
                            {

                            }
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion

        #region 63D ViewQuery_M  ==============================================
        ModelAction ViewQuery_X;
        void Initialize_ViewQuery_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ViewQuery_X,
            };
        }
        void Refresh_ViewQuery_X(RootModel root, ItemModel model)
        {
        }
        internal void ViewQuery_M(ItemModel model, RootModel root)
        {
            var key = model.Item;
            var query = model.Aux1 as QueryX;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:
                        var count = TryGetQueryItems(query, out Item[] items, key) ? items.Length : 0;

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = GetIdentity(query, IdentityStyle.Single);
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    N = TryGetQueryItems(query, out Item[] items, key) ? items.Length : 0;

                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.ViewItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ViewItem_M, level, itm, query, null, ViewItem_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion




        #region 642 EnumXList  ================================================
        ModelAction EnumXList_X;
        void Initialize_EnumXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_EnumXList_X,
            };
        }
        void Refresh_EnumXList_X(RootModel root, ItemModel model)
        {
        }
        internal void EnumXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = _enumXStore.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, EnumDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _enumXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _enumXStore.ToArray;
                    var item = model.Item;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as EnumX;
                        if (!TryGetOldModel(model, Trait.EnumX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.EnumX_M, level, itm, null, null, EnumX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void EnumDefListInsert(ItemModel model)
        {
            ItemCreated(new EnumX(_enumXStore));
        }
        #endregion

        #region 643 TableXList  ===============================================
        ModelAction TableXList_X;
        void Initialize_TableXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_TableXList_X,
            };
        }
        void Refresh_TableXList_X(RootModel root, ItemModel model)
        {
        }
        internal void TableXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = _tableXStore.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, TableDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _tableXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _tableXStore.ToArray;
                    var item = model.Item;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as TableX;
                        if (!TryGetOldModel(model, Trait.TableX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.TableX_M, level, itm, null, null, TableX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private void TableDefListInsert(ItemModel model)
        {
            ItemCreated(new TableX(_tableXStore));
        }
        #endregion

        #region 644 GraphXList  ===============================================
        ModelAction GraphXList_X;
        void Initialize_GraphXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXList_X,
            };
        }
        void Refresh_GraphXList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = _graphXStore.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, GraphXRootInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _graphXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _graphXStore.ToArray;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as GraphX;
                        if (!TryGetOldModel(model, Trait.GraphX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphX_M, level, itm, null, null, GraphX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private void GraphXRootInsert(ItemModel model)
        {
            ItemCreated(new GraphX(_graphXStore));
            model.IsExpandedLeft = true;
        }
        #endregion

        #region 645 SymbolXlList  =============================================
        ModelAction SymbolXList_X;
        void Initialize_SymbolXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_SymbolXList_X,
            };
        }
        void Refresh_SymbolXList_X(RootModel root, ItemModel model)
        {
        }
        internal void SymbolXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item as GraphX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = SymbolDef_Drop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GraphX_SymbolX.ChildCount(gd);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, SymbolListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gd = model.Item as GraphX;
                var N = GraphX_SymbolX.ChildCount(gd);

                if (model.IsExpandedLeft && N > 0)
                {
                    var syms = GraphX_SymbolX.GetChildren(gd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var sym = syms[i];
                        if (!TryGetOldModel(model, Trait.SymbolX_M, oldModels, i, sym))
                            model.ChildModels[i] = new ItemModel(model, Trait.SymbolX_M, level, sym, GraphX_SymbolX, gd, SymbolX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void SymbolListInsert(ItemModel model)
        {
            var gd = model.Item as GraphX;
            var sym = new SymbolX(_symbolXStore);
            ItemCreated(sym);
            AppendLink(GraphX_SymbolX, gd, sym);
            model.IsExpandedLeft = true;
        }
        private DropAction SymbolDef_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!drop.Item.IsSymbolX) return DropAction.None;
            var src = drop.Item as SymbolX;
            if (doDrop)
            {
                var gd = model.Item as GraphX;
                var sym = new SymbolX(_symbolXStore);
                ItemCreated(sym);
                AppendLink(GraphX_SymbolX, gd, sym);
                model.IsExpandedLeft = true;
                sym.Data = src.Data;
                sym.Name = src.Name;
                sym.Summary = src.Summary;
            }
            return DropAction.Copy;
        }
        #endregion


        #region 647 TableList  ================================================
        ModelAction TableList_X;
        void Initialize_TableList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_TableList_X,
            };
        }
        void Refresh_TableList_X(RootModel root, ItemModel model)
        {
        }
        internal void TableList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = _tableXStore.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = _tableXStore.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _tableXStore.ToArray;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.Table_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.Table_M, level, itm, null, null, Table_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 648 GraphList  ================================================
        ModelAction GraphList_X;
        void Initialize_GraphList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphList_X,
            };
        }
        void Refresh_GraphList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = _graphXStore.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = _graphXStore.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _graphXStore.ToArray;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var gx = items[i];
                        if (!TryGetOldModel(model, Trait.GraphXRef_M, oldModels, i, gx))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXRef_M, level, gx, null, null, GraphXRef_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion




        #region 652 PairX  ====================================================
        ModelAction PairX_X;
        void Initialize_PairX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_PairX_X,
            };
        }
        void Refresh_PairX_X(RootModel root, ItemModel model)
        {
        }
        internal void PairX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as PairX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = item.ActualValue;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = item.DisplayValue;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedRight)
                {
                    var sp = new Property[] { _pairXTextProperty, _pairXValueProperty };
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 653 EnumX  ====================================================
        ModelAction EnumX_X;
        void Initialize_EnumX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_EnumX_X,
            };
        }
        void Refresh_EnumX_X(RootModel root, ItemModel model)
        {
        }
        internal void EnumX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as EnumX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = item.Name;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _enumXNameProperty, _enumXSummaryProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;
                var L = model.IsExpandedLeft ? 2 : 0;
                var N = R + L;

                if (N > 0)
                {
                    var item = model.Item as EnumX;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.EnumValue_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.EnumValue_ZM, level, item, null, null, PairXList_Dx);

                        i++;
                        if (!TryGetOldModel(model, Trait.EnumColumn_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.EnumColumn_ZM, level, item, null, null, EnumColumnList_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 654 TableX  ===================================================
        ModelAction TableX_X;
        void Initialize_TableX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_TableX_X,
            };
        }
        void Refresh_TableX_X(RootModel root, ItemModel model)
        {
        }
        internal void TableX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item as TableX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = tbl.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = tbl.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _tableXNameProperty, _tableXSummaryProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;
                var L = model.IsExpandedLeft ? 5 : 0;
                var N = R + L;

                if (N > 0)
                {
                    var item = model.Item as TableX;
                    var level = (byte)(model.Level + 1);
                    var oldModels = model.ChildModels;

                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.ColumnX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.ColumnX_ZM, level, item, null, null, ColumnXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ComputeX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.ComputeX_ZM, level, item, null, null, ComputeXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ChildRelationX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.ChildRelationX_ZM, level, item, null, null, ChildRelationXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ParentRelatationX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.ParentRelatationX_ZM, level, item, null, null, ParentRelatationXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.MetaRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.MetaRelation_ZM, level, item, null, null, MetaRelationList_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 655 GraphX  ===================================================
        ModelAction GraphX_X;
        void Initialize_GraphX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphX_X,
            };
        }
        void Refresh_GraphX_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item as GraphX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = gd.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = gd.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _graphXNameProperty, _graphXSummaryProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;
                var L = model.IsExpandedLeft ? 4 : 0;
                var N = R + L;

                if (N > 0)
                {
                    var item = model.Item as GraphX;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.GraphXColoring_M, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXColoring_M, level, item, null, null, GraphXColoring_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.GraphXRoot_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXRoot_ZM, level, item, null, null, GraphXRootList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.GraphXNode_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXNode_ZM, level, item, null, null, GraphXNodeList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.SymbolX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.SymbolX_ZM, level, item, null, null, SymbolXList_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 656 SymbolX  ==================================================
        ModelAction SymbolX_X;
        void Initialize_SymbolX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_SymbolX_X,
            };
        }
        void Refresh_SymbolX_X(RootModel root, ItemModel model)
        {
        }
        internal void SymbolX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as SymbolX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = item.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.EditCommand, CreateSecondarySymbolEdit));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedRight)
                {
                    var sp = new Property[] { _symbolXNameProperty };
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateSecondarySymbolEdit(ItemModel model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.SymbolEditor);
        }
        #endregion

        #region 657 ColumnX  ==================================================
        ModelAction ColumnX_X;
        void Initialize_ColumnX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ColumnX_X,
            };
        }
        void Refresh_ColumnX_X(RootModel root, ItemModel model)
        {
        }
        internal void ColumnX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item as ColumnX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = col.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = col.Name;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedRight)
                {
                    var sp = new Property[] { _columnXNameProperty, _columnXSummaryProperty, _columnXTypeOfProperty, _columnXIsChoiceProperty, _columnXInitialProperty };
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 658 ComputeX_M  ===============================================
        ModelAction ComputeX_X;
        void Initialize_ComputeX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ComputeX_X,
            };
        }
        void Refresh_ComputeX_X(RootModel root, ItemModel model)
        {
        }
        internal void ComputeX_M(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var cpd = model.Item as ComputeX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ComputedX_M_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = cpd.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = cpd.Name;
                        var qx = ComputeX_QueryX.GetChild(cpd);
                        if (qx != null) root.ModelCount = QueryX_QueryX.ChildCount(qx);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpanded)
                {
                    var cx = model.Item as ComputeX;
                    var qx = ComputeX_QueryX.GetChild(cx);

                    int R = 0;
                    Property[] sp = null;
                    if (model.IsExpandedRight)
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
                    var L = (model.IsExpandedLeft && qx != null) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = model.ChildModels;
                        if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(model, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(qx);
                            var level = (byte)(model.Level + 1);
                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j] as QueryX;
                                if (!TryGetOldModel(model, Trait.ValueXHead_M, oldModels, i, itm))
                                    model.ChildModels[i] = new ItemModel(model, Trait.ValueXHead_M, level, itm, null, null, ValueXHead_X);
                            }
                        }
                    }
                    else
                    {
                        model.ChildModels = null;
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }

        private DropAction ComputedX_M_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(drop.Item is Relation rel)) return DropAction.None;

            var cd = model.Item as ComputeX;
            var root = ComputeX_QueryX.GetChild(cd);
            if (root == null) return DropAction.None;

            Store sto1, sto2;

            var sto = Store_ComputeX.GetParent(cd);
            GetHeadTail(rel, out sto1, out sto2);
            if (sto != sto1 && sto != sto2) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(root, rel, QueryType.Value).IsReversed = (sto == sto2);
                model.IsExpandedLeft = true;
            }
            return DropAction.Link;
        }
        #endregion



        #region 661 ColumnXList  ==============================================
        ModelAction ColumnXList_X;
        void Initialize_ColumnXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ColumnXList_X,
            };
        }
        void Refresh_ColumnXList_X(RootModel root, ItemModel model)
        {
        }
        internal void ColumnXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = TableX_ColumnX.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ColumnDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ColumnX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ColumnX.GetChildren(tbl);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.ColumnX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.ColumnX_M, level, itm, TableX_ColumnX, tbl, ColumnX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ColumnDefListInsert(ItemModel model)
        {
            var col = new ColumnX(_columnXStore);
            ItemCreated(col); AppendLink(TableX_ColumnX, model.Item, col);
        }
        #endregion

        #region 662 ChildRelationXList  =======================================
        ModelAction ChildRelationXList_X;
        void Initialize_ChildRelationXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ChildRelationXList_X,
            };
        }
        void Refresh_ChildRelationXList_X(RootModel root, ItemModel model)
        {
        }
        internal void ChildRelationXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = TableX_ChildRelationX.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ChildRelationDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ChildRelationX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ChildRelationX.GetChildren(tbl);

                    var level = (byte)(model.Level + 1);
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetOldModel(model, Trait.ChildRelationX_M, oldModels, i, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.ChildRelationX_M, level, rel, TableX_ChildRelationX, tbl, ChildRelationX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ChildRelationDefListInsert(ItemModel model)
        {
            var rel = new RelationX(_relationXStore);
            ItemCreated(rel); AppendLink(TableX_ChildRelationX, model.Item, rel);
        }
        #endregion

        #region 663 ParentRelatationXList  ====================================
        ModelAction ParentRelationXList_X;
        void Initialize_ParentRelationXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ParentRelationXList_X,
            };
        }
        void Refresh_ParentRelationXList_X(RootModel root, ItemModel model)
        {
        }
        internal void ParentRelatationXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = TableX_ParentRelationX.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ParentRelationDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ParentRelationX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ParentRelationX.GetChildren(tbl);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetOldModel(model, Trait.ParentRelationX_M, oldModels, i, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.ParentRelationX_M, level, rel, TableX_ParentRelationX, tbl, ParentRelationX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ParentRelationDefListInsert(ItemModel model)
        {
            var rel = new RelationX(_relationXStore); ItemCreated(rel);
            AppendLink(TableX_ParentRelationX, model.Item, rel);
        }
        #endregion

        #region 664 PairXList  ================================================
        ModelAction PairXList_X;
        void Initialize_PairXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_PairXList_X,
            };
        }
        void Refresh_PairXList_X(RootModel root, ItemModel model)
        {
        }
        internal void PairXList_Dx(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as EnumX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, EnumValueListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var enu = model.Item as EnumX;
                var N = enu.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = enu.ToArray;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as PairX;
                        if (!TryGetOldModel(model, Trait.PairX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.PairX_M, level, itm, enu, null, PairX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void EnumValueListInsert(ItemModel model)
        {
            ItemCreated(new PairX(model.Item as EnumX));
        }
        #endregion

        #region 665 EnumColumnList  ===========================================
        ModelAction EnumColumnList_X;
        void Initialize_EnumColumnList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_EnumColumnList_X,
            };
        }
        void Refresh_EnumColumnList_X(RootModel root, ItemModel model)
        {
        }
        internal void EnumColumnList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as EnumX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = EnumColumn_Drop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = EnumX_ColumnX.ChildCount(item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var enu = model.Item as EnumX;

                var N = (model.IsExpandedLeft) ? EnumX_ColumnX.ChildCount(enu): 0;
                if (N > 0)
                {
                    var items = EnumX_ColumnX.GetChildren(enu);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var col = items[i];
                        var tbl = TableX_ColumnX.GetParent(col);
                        if (tbl != null)
                        {
                            if (!TryGetOldModel(model, Trait.EnumRelatedColumn_M, oldModels, i, col))
                                model.ChildModels[i] = new ItemModel(model, Trait.EnumRelatedColumn_M, level, col, tbl, enu, EnumRelatedColumn_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction EnumColumn_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!drop.Item.IsColumnX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(EnumX_ColumnX, model.Item, drop.Item);
            }
            return DropAction.Link;
        }
        #endregion

        #region 666 ComputeXList  =============================================
        ModelAction ComputeXList_X;
        void Initialize_ComputeXList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ComputeXList_X,
            };
        }
        void Refresh_ComputeXList_X(RootModel root, ItemModel model)
        {
        }
        internal void ComputeXList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var sto = model.Item as Store;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ComputedDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sto = model.Item as Store;

                var N = (model.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;
                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);
                    var items = Store_ComputeX.GetChildren(sto);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.ComputeX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.ComputeX_M, level, itm, Store_ComputeX, sto, ComputeX_M);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ComputedDefListInsert(ItemModel model)
        {
            var st = model.Item as Store;
            var cx = new ComputeX(_computeXStore);
            ItemCreated(cx);
            AppendLink(Store_ComputeX, st, cx);

            CreateQueryX(cx, st);
        }
        #endregion



        #region 671 ChildRelationX  ===========================================
        ModelAction ChildRelationX_X;
        void Initialize_ChildRelationX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ChildRelationX_X,
            };
        }
        void Refresh_ChildRelationX_X(RootModel root, ItemModel model)
        {
        }
        internal void ChildRelationX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ChildRelationDef_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = GetIdentity(item, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedRight)
                {
                    var sp1 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty};
                    var sp2 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty};

                    var item = model.Item as RelationX;
                    var sp = item.IsLimited ? sp2 : sp1;
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private DropAction ChildRelationDef_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!drop.Item.IsTableX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(TableX_ParentRelationX, drop.Item, model.Item);
            }
            return DropAction.Link;
        }
        #endregion

        #region 672 ParentRelationX  ==========================================
        ModelAction ParentRelationX_X;
        void Initialize_ParentRelationX_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ParentRelationX_X,
            };
        }
        void Refresh_ParentRelationX_X(RootModel root, ItemModel model)
        {
        }
        internal void ParentRelationX_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ParentRelationDef_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedRight)
                {
                    var sp1 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty, _relationXIsLimitedProperty };
                    var sp2 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty, _relationXIsLimitedProperty, _relationXMinOccuranceProperty, _relationXMaxOccuranceProperty };

                    var item = model.Item as RelationX;
                    var sp = item.IsLimited ? sp2 : sp1;
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private DropAction ParentRelationDef_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!drop.Item.IsTableX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(TableX_ChildRelationX, drop.Item, model.Item);
            }
            return DropAction.Link;
        }
        #endregion

        #region 673 NameColumnRelation  =======================================
        ModelAction NameColumnRelation_X;
        void Initialize_NameColumnRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_NameColumnRelation_X,
            };
        }
        void Refresh_NameColumnRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void NameColumnRelation_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = NameColumnRelation_Drop;
                        break;

                    case ModelActionX.PointerOver:
                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = TableX_NameProperty.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;
                Property prop;

                if (model.IsExpandedLeft && TableX_NameProperty.TryGetChild(tbl, out prop))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ItemModel[1];

                    if (!TryGetOldModel(model, Trait.NameColumn_M, oldModels, 0, prop))
                        model.ChildModels[0] = new ItemModel(model, Trait.NameColumn_M, level, prop, tbl, null, NameColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction NameColumnRelation_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(drop.Item is Property)) return DropAction.None;

            if (doDrop)
            {
                if (model.IsChildModel(drop))
                    RemoveLink(TableX_NameProperty, model.Item, drop.Item);
                else
                {
                    AppendLink(TableX_NameProperty, model.Item, drop.Item);
                    model.IsExpandedLeft = true;
                }
            }
            return DropAction.Link;
        }
        #endregion

        #region 674 SummaryColumnRelation  ====================================
        ModelAction SummaryColumnRelation_X;
        void Initialize_SummaryColumnRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_SummaryColumnRelation_X,
            };
        }
        void Refresh_SummaryColumnRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void SummaryColumnRelation_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = SummaryColRelation_Drop;
                        break;

                    case ModelActionX.PointerOver:
                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = TableX_SummaryProperty.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;
                Property prop = null;

                if (model.IsExpandedLeft && TableX_SummaryProperty.TryGetChild(tbl, out prop))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ItemModel[1];

                    if (!TryGetOldModel(model, Trait.SummaryColumn_M, oldModels, 0, prop))
                        model.ChildModels[0] = new ItemModel(model, Trait.SummaryColumn_M, level, prop, tbl, null, SummaryColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction SummaryColRelation_Drop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(drop.Item is Property)) return DropAction.None;

            if (doDrop)
            {
                if (model.IsChildModel(drop))
                    RemoveLink(TableX_SummaryProperty, model.Item, drop.Item);
                else
                {
                    AppendLink(TableX_SummaryProperty, model.Item, drop.Item);
                    model.IsExpandedLeft = true;
                }
            }
            return DropAction.Link;
        }
        #endregion

        #region 675 NameColumn  ===============================================
        ModelAction NameColumn_X;
        void Initialize_NameColumn_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_NameColumn_X,
            };
        }
        void Refresh_NameColumn_X(RootModel root, ItemModel model)
        {
        }
        internal void NameColumn_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        if (item.IsColumnX)
                            root.ModelSummary = (item as ColumnX).Summary;
                        else if (item.IsComputeX)
                            root.ModelSummary = (item as ComputeX).Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        if (item.IsColumnX)
                            root.ModelName = (item as ColumnX).Name;
                        else if (item.IsComputeX)
                            root.ModelName = (item as ComputeX).Name;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 676 SummaryColumn  ============================================
        ModelAction SummaryColumn_X;
        void Initialize_SummaryColumn_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_SummaryColumn_X,
            };
        }
        void Refresh_SummaryColumn_X(RootModel root, ItemModel model)
        {
        }
        internal void SummaryColumn_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        if (item.IsColumnX)
                            root.ModelSummary = (item as ColumnX).Summary;
                        else if (item.IsComputeX)
                            root.ModelSummary = (item as ComputeX).Summary;
                        break;

                    case ModelActionX.ModelRefresh:


                        if (item.IsColumnX)
                            root.ModelName = (item as ColumnX).Name;
                        else if (item.IsComputeX)
                            root.ModelName = (item as ComputeX).Name;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 681 GraphXColoring  ===========================================
        ModelAction GraphXColoring_X;
        void Initialize_GraphXColoring_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXColoring_X,
            };
        }
        void Refresh_GraphXColoring_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXColoring_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item as GraphX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = GraphXColoringDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GraphX_ColorColumnX.ChildCount(gd);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(model.DescriptionKey);
                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item;
                ColumnX col = null;
                TableX tbl = null;
                if (model.IsExpandedLeft && GraphX_ColorColumnX.TryGetChild(item, out col) && TableX_ColumnX.TryGetParent(col, out tbl))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ItemModel[1];

                    if (!TryGetOldModel(model, Trait.GraphXColorColumn_M, oldModels, 0, col, tbl))
                        model.ChildModels[0] = new ItemModel(model, Trait.GraphXColorColumn_M, level, col, tbl, null, GraphXColorColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction GraphXColoringDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            var gd = model.Item;
            var col = drop.Item;
            if (!col.IsColumnX) return DropAction.None;
            if (!gd.IsGraphX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(GraphX_ColorColumnX, gd, col);
            }
            return DropAction.Link;
        }
        #endregion

        #region 682 GraphXRootList  ===========================================
        ModelAction GraphXRootList_X;
        void Initialize_GraphXRootList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXRootList_X,
            };
        }
        void Refresh_GraphXRootList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXRootList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = GraphXRootListDrop;
                        break;

                    case ModelActionX.PointerOver:
                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GraphX_QueryX.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(model.DescriptionKey);
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gd = model.Item as GraphX;

                var N = (model.IsExpandedLeft) ? GraphX_QueryX.ChildCount(gd): 0;
                if (N > 0)
                {
                    var items = GraphX_QueryX.GetChildren(gd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphXRoot_M, oldModels, i, itm, gd))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXRoot_M, level, itm, gd, null, QueryXRoot_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction GraphXRootListDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item is GraphX gx)) return DropAction.None;
            if (!(drop.Item is Store st)) return DropAction.None;
            if (GraphXAlreadyHasThisRoot(gx, st)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(gx, st);
            }
            return DropAction.Link;
        }
        private bool GraphXAlreadyHasThisRoot(Item gd, Item table)
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
        #endregion

        #region 683 GraphXNodeList  ===========================================
        ModelAction GraphXNodeList_X;
        void Initialize_GraphXNodeList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXNodeList_X,
            };
        }
        void Refresh_GraphXNodeList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXNodeList_X(ItemModel model, RootModel root)
        {
            var gx = model.Item as GraphX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GetNodeOwners(gx).Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedLeft)
                {
                    var owners = GetNodeOwners(gx).ToArray();
                    var N = owners.Length;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);

                        var oldModels = model.ChildModels;
                        if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var sto = owners[i];
                            if (!TryGetOldModel(model, Trait.GraphXNode_M, oldModels, i, sto, gx))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXNode_M, level, sto, gx, null, GraphXNode_X);
                        }
                    }
                    else
                    {
                        model.ChildModels = null;
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 684 GraphXNode  ===============================================
        ModelAction GraphXNode_X;
        void Initialize_GraphXNode_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXNode_X,
            };
        }
        void Refresh_GraphXNode_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXNode_X(ItemModel model, RootModel root)
        {
            var sto = model.Item as Store;
            var gx = model.Aux1 as GraphX;

            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as TableX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = SymbolNodeOwnerDrop;
                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = item.Name;
                        root.ModelCount = GetSymbolQueryXCount(gx, sto);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? GetSymbolQueryXCount(gx, sto) : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    (var symbols, var querys) = GetSymbolXQueryX(gx, sto);
                    for (int i = 0; i < N; i++)
                    {
                        var seg = querys[i];
                        if (!TryGetOldModel(model, Trait.GraphXNodeSymbol_M, oldModels, i, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXNodeSymbol_M, level, seg, GraphX_SymbolQueryX, gx, GraphXNodeSymbol_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction SymbolNodeOwnerDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item is Store st)) return DropAction.None;
            if (!(model.Aux1 is GraphX gx)) return DropAction.None;
            if (!(drop.Item is SymbolX sx)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(gx, sx, st);
            }
            return DropAction.Link;
        }
        #endregion

        #region 685 GraphXColorColumn  ========================================
        ModelAction GraphXColorColumn_X;
        void Initialize_GraphXColorColumn_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXColorColumn_X,
            };
        }
        void Refresh_GraphXColorColumn_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXColorColumn_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item as ColumnX;
                var tbl = model.Aux1 as TableX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = $"{tbl.Name} : {col.Name}";
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(model.DescriptionKey);
                        break;
                }
            }
        }
        #endregion



        #region 691 QueryXRoot  ===============================================
        ModelAction QueryXRoot_X;
        void Initialize_QueryXRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXRoot_X,
            };
        }
        void Refresh_QueryXRoot_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXRoot_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXRootHeadDrop;
                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.NameKey);
                        root.ModelName = QueryXRootName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(model.DescriptionKey);
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft || model.IsExpandedRight)
                {
                    var qx = model.Item as QueryX;
                    var sp = new Property[] { };
                    var R = model.IsExpandedRight ? sp.Length : 0;

                    var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    N = L + R;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(model, oldModels, sp);
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
                                        if (!TryGetOldModel(model, Trait.GraphXPathHead_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathHead_M, level, child, null, null, QueryXPathHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathLink_M, level, child, null, null, QueryXPathLink_X);
                                    }
                                }
                                else if (child.IsGroup)
                                {
                                    if (child.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupHead_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupHead_M, level, child, null, null, QueryXGroupHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupLink_M, level, child, null, null, QueryXGroupLink_X);
                                    }
                                }
                                else if (child.IsSegue)
                                {
                                    if (child.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressHead_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressHead_M, level, child, null, null, QueryXEgressHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressLink_M, level, child, null, null, QueryXEgressLink_X);
                                    }
                                }
                                else if (child.IsRoot)
                                {
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, child))
                                        model.ChildModels[i] = new ItemModel(model, Trait.GraphXLink_M, level, child, null, null, QueryXLink_X);
                                }
                                else
                                {
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, child))
                                        model.ChildModels[i] = new ItemModel(model, Trait.GraphXLink_M, level, child, null, null, QueryXLink_X);
                                }
                            }
                        }
                    }
                }
                if (N == 0)    model.ChildModels = null;
            }
        }
        private DropAction QueryXRootHeadDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(drop.Item is Relation re)) return DropAction.None;
            if (!(model.Item is QueryX qx)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Graph);
            }
            return DropAction.Link;
        }
        private string QueryXRootName(ItemModel modle)
        {
            var sd = modle.Item;
            var tb = Store_QueryX.GetParent(sd);
            return GetIdentity(tb, IdentityStyle.Single);
        }
        #endregion

        #region 692 QueryXLink  ===============================================
        ModelAction QueryXLink_X;
        void Initialize_QueryXLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXLink_X,
            };
        }
        void Refresh_QueryXLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXRootLinkDrop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakePathHeadCommand, MakePathtHead));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeGroupHeadCommand, MakeGroupHead));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var qx = items[j];
                            switch (qx.QueryKind)
                            {
                                case QueryType.Path:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXPathHead_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathHead_M, level, qx, null, null, QueryXPathHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathLink_M, level, qx, null, null, QueryXPathLink_X);
                                    }
                                    break;

                                case QueryType.Group:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupHead_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupHead_M, level, qx, null, null, QueryXGroupHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupLink_M, level, qx, null, null, QueryXGroupLink_X);
                                    }
                                    break;

                                case QueryType.Segue:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressHead_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressHead_M, level, qx, null, null, QueryXEgressHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressLink_M, level, qx, null, null, QueryXEgressLink_X);
                                    }
                                    break;

                                case QueryType.Graph:
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, qx))
                                        model.ChildModels[i] = new ItemModel(model, Trait.GraphXLink_M, level, qx, null, null, QueryXLink_X);
                                    break;

                                default:
                                    throw new Exception("Invalid item trait");
                            }
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }

        private string GetQueryXRelationName(ItemModel model)
        {
            Relation parent;
            if (Relation_QueryX.TryGetParent(model.Item, out parent))
            {
                var rel = parent as RelationX;
                return GetRelationName(rel);
            }
            return null;
        }
        private DropAction QueryXRootLinkDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (drop.Item.IsRelationX)
            {
                if (doDrop)
                {
                    var qx = model.Item as QueryX;
                    var re = drop.Item as Relation;
                    CreateQueryX(qx, re, QueryType.Graph);
                }
                return DropAction.Link;
            }
            else if (model.Item.IsQueryGraphLink)
            {
                if (doDrop)
                {
                    RemoveItem(drop);
                }
                return DropAction.Link;
            }
            return DropAction.None;
        }
        string QueryXLinkName(ItemModel model)
        {
            return QueryXFilterName(model.Item as QueryX);
        }
        string QueryXFilterName(QueryX sd)
        {
            Store head, tail;
            GetHeadTail(sd, out head, out tail);
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
        string QueryXSelectorName(QueryX sd)
        {
            Store head, tail;
            GetHeadTail(sd, out head, out tail);


            if (head == null || tail == null) return InvalidItem;
            var headName = GetIdentity(head, IdentityStyle.Single);
            var tailName = GetIdentity(tail, IdentityStyle.Single);
            return $"{headName}{parentNameSuffix}{tailName}";
        }
        #endregion

        #region 693 QueryXPathHead  ===========================================
        ModelAction QueryXPathHead_X;
        void Initialize_QueryXPathHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXPathHead_X,
            };
        }
        void Refresh_QueryXPathHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXPathHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXPathDrop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeRootLinkCommand, MakeRootLink));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeGroupHeadCommand, MakeGroupHead));
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXConnect1Property, _queryXConnect2Property, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathLink_M, level, itm, null, null, QueryXPathLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        string QueryXHeadName(ItemModel model)
        {
            var sd = model.Item as QueryX;

            Store head1, tail1, head2, tail2;
            GetHeadTail(sd, out head1, out tail1);
            var sd2 = GetQueryXTail(sd);
            GetHeadTail(sd2, out head2, out tail2);

            StringBuilder sb = new StringBuilder(132);
            sb.Append(GetIdentity(head1, IdentityStyle.Single));
            sb.Append(parentNameSuffix);
            sb.Append(GetIdentity(tail2, IdentityStyle.Single));
            return sb.ToString();       
        }
        private QueryX GetQueryXTail(QueryX sd)
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
        #endregion

        #region 694 QueryXPathLink  ===========================================
        ModelAction QueryXPathLink_X;
        void Initialize_QueryXPathLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXPathLink_X,
            };
        }
        void Refresh_QueryXPathLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXPathLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXPathDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXPathLink_M, level, itm, null, null, QueryXPathLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXPathDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item is QueryX qx)) return DropAction.None;
            if (!(drop.Item is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Path);
            }
            return DropAction.Link;
        }
        #endregion

        #region 695 QueryXGroupHead  ==========================================
        ModelAction QueryXGroupHead_X;
        void Initialize_QueryXGroupHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXGroupHead_X,
            };
        }
        void Refresh_QueryXGroupHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXGroupHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXGroupDrop;
                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        if (CanConvertQueryType(model))
                        {
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeRootLinkCommand, MakeRootLink));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakePathHeadCommand, MakePathtHead));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeEgressHeadCommand, MakeBridgeHead));
                        }
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupLink_M, level, itm, null, null, QueryXGroupLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 696 QueryXGroupLink  ==========================================
        ModelAction QueryXGroupLink_X;
        void Initialize_QueryXGroupLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXGroupLink_X,
            };
        }
        void Refresh_QueryXGroupLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXGroupLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXGroupDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXGroupLink_M, level, itm, null, null, QueryXGroupLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXGroupDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item is QueryX qx)) return DropAction.None;
            if (!(drop.Item is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Group);
            }
            return DropAction.Link;
        }
        #endregion

        #region 697 QueryXEgressHead  =========================================
        ModelAction QueryXEgressHead_X;
        void Initialize_QueryXEgressHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXEgressHead_X,
            };
        }
        void Refresh_QueryXEgressHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXEgressHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXBridgeDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        if (CanConvertQueryType(model))
                        {
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeRootLinkCommand, MakeRootLink));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakePathHeadCommand, MakePathtHead));
                            root.MenuCommands.Add(new ModelCommand(this, model, Trait.MakeGroupHeadCommand, MakeGroupHead));
                        };
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressLink_M, level, itm, null, null, QueryXEgressLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 698 QueryXEgressLink  =========================================
        ModelAction QueryXEgressLink_X;
        void Initialize_QueryXEgressLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryXEgressLink_X,
            };
        }
        void Refresh_QueryXEgressLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryXEgressLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = QueryXBridgeDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(sd);
                        var level = (byte)(model.Level + 1);

                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphXEgressLink_M, level, itm, null, null, QueryXEgressLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXBridgeDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item is QueryX qx)) return DropAction.None;
            if (!(drop.Item is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Segue);
            }
            return DropAction.Link;
        }
        #endregion

        #region 699 GraphXNodeSymbol  =========================================
        ModelAction GraphXNodeSymbol_X;
        void Initialize_GraphXNodeSymbol_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXNodeSymbol_X,
            };
        }
        void Refresh_GraphXNodeSymbol_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXNodeSymbol_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXNodeSymbolName(model);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXWhereProperty };
                var N = model.IsExpandedRight ? sp.Length : 0;

                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private string QueryXNodeSymbolName(ItemModel model)
        {
            var sd = model.Item;
            SymbolX sym;
            return (SymbolX_QueryX.TryGetParent(sd, out sym)) ? sym.Name : null;
        }
        #endregion


        #region 69E ValueXHead  ===============================================
        ModelAction ValueHead_X;
        void Initialize_ValueHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ValueHead_X,
            };
        }
        void Refresh_ValueHead_X(RootModel root, ItemModel model)
        {
        }
        internal void ValueXHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ValueXLinkDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = QueryXComputeName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var qx = model.Item as QueryX;
                var sp1 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty};
                var sp2 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                var sp = qx.HasSelect ? sp2 : sp1;
                var R = model.IsExpandedRight ? sp.Length : 0;

                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var items = QueryX_QueryX.GetChildren(qx);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.ValueXLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ValueXLink_M, level, itm, null, null, ValueXLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 69F ValueXLink  ===============================================
        ModelAction ValueLink_X;
        void Initialize_ValueLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ValueLink_X,
            };
        }
        void Refresh_ValueLink_X(RootModel root, ItemModel model)
        {
        }
        internal void ValueXLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = ValueXLinkDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = QueryXComputeName(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var vd = model.Item as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(vd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var items = QueryX_QueryX.GetChildren(vd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var itm = items[j];
                            if (!TryGetOldModel(model, Trait.ValueXLink_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.ValueXLink_M, level, itm, null, null, ValueXLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction ValueXLinkDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(drop.Item is Relation re)) return DropAction.None;
            if (!(model.Item is QueryX qx)) return DropAction.None;

            Store tb1Head, tb1Tail, tb2Head, tb2Tail;
            GetHeadTail(qx,out tb1Head, out tb1Tail);
            GetHeadTail(re, out tb2Head, out tb2Tail);
            if ((tb1Head != tb2Head && tb1Head != tb2Tail) && (tb1Tail != tb2Head && tb1Tail != tb2Tail)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Value).IsReversed = (tb1Tail == tb2Tail);
            }
            return DropAction.Link;
        }
        string QueryXComputeName(ItemModel model)
        {
            var sd = model.Item as QueryX;

            Store head1, tail1, head2, tail2;
            GetHeadTail(sd, out head1, out tail1);
            var sd2 = GetValueeDefTail(sd);
            GetHeadTail(sd2, out head2, out tail2);

            StringBuilder sb = new StringBuilder(132);
            sb.Append(GetIdentity(head1, IdentityStyle.Single));
            sb.Append(parentNameSuffix);
            sb.Append(GetIdentity(tail2, IdentityStyle.Single));
            return sb.ToString();
        }
        private QueryX GetValueeDefTail(QueryX sd)
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
        #endregion



        #region 6A1 RowX  =====================================================
        ModelAction Row_X;
        void Initialize_Row_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_Row_X,
            };
        }
        void Refresh_Row_X(RootModel root, ItemModel model)
        {
        }
        internal void RowX_X(ItemModel model, RootModel root)
        {
            var row = model.Item as RowX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(model.Item, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = GetIdentity(model.Item, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = row.TableX.HasChoiceColumns;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void RowX_VX(ItemModel model)
        {
            var row = model.Item as RowX;
            ColumnX[] cols = null;
            var R = (model.IsExpandedRight && TryGetChoiceColumns(row.Owner, out cols)) ? cols.Length : 0;
            var L = (model.IsExpandedLeft) ? 7 : 0;
            var N = R + L;

            if (N > 0)
            {
                var level = (byte)(model.Level + 1);

                var oldModels = model.ChildModels;
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];


                if (R > 0)
                {
                    AddProperyModels(model, oldModels, cols);
                }
                if (L > 0)
                {
                    int usedColumnCount, unusedColumnCount, usedChidRelationCount, unusedChildRelationCount, usedParentRelationCount, unusedParentRelationCount;
                    GetColumnCount(row, out usedColumnCount, out unusedColumnCount);
                    GetChildRelationCount(row, out usedChidRelationCount, out unusedChildRelationCount);
                    GetParentRelationCount(row, out usedParentRelationCount, out unusedParentRelationCount);

                    int i = R;
                    if (!TryGetOldModel(model, Trait.RowProperty_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowProperty_ZM, level, row, TableX_ColumnX, null, RowPropertyList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowCompute_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowCompute_ZM, level, row, Store_ComputeX, null, RowComputeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowChildRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowChildRelation_ZM, level, row, TableX_ChildRelationX, null, RowChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowParentRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowParentRelation_ZM, level, row, TableX_ParentRelationX, null, RowParentRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowDefaultProperty_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowDefaultProperty_ZM, level, row, TableX_ColumnX, null, RowDefaultPropertyList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowUnusedChildRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowUnusedChildRelation_ZM, level, row, TableX_ChildRelationX, null, RowUnusedChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowUnusedParentRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ItemModel(model, Trait.RowUnusedParentRelation_ZM, level, row, TableX_ParentRelationX, null, RowUnusedParentRelationList_X);
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        private DropAction ReorderStoreItem(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (!(model.Item.Owner is Store sto)) return DropAction.None;
            if (!model.IsSiblingModel(drop)) return DropAction.None;
            
            var item1 = drop.Item;
            var item2 = model.Item;
            var index1 = sto.IndexOf(item1);
            var index2 = sto.IndexOf(item2);
            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop)
            {
                ItemMoved(drop.Item, index1, index2);
            }
            return DropAction.Move;
        }
        #endregion

        #region 6A3 View  =====================================================
        ModelAction View_X;
        void Initialize_View_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_View_X,
            };
        }
        void Refresh_View_X(RootModel root, ItemModel model)
        {
        }
        internal void View_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as ViewX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = item.Name;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6A4 TableX  ===================================================
        ModelAction Table_X;
        void Initialize_Table_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_Table_X,
            };
        }
        void Refresh_Table_X(RootModel root, ItemModel model)
        {
        }
        internal void Table_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item as TableX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = tbl.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = tbl.Name;
                        root.ModelCount = tbl.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, TableInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item as TableX;
                var col = TableX_NameProperty.GetChild(tbl);
                var N = model.IsExpandedLeft ? tbl.Count : 0;

                if (N > 0)
                {
                    var items = tbl.ToArray;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var row = items[i];
                        if (!TryGetOldModel(model, Trait.Row_M, oldModels, i, row, tbl, col))
                            model.ChildModels[i] = new ItemModel(model, Trait.Row_M, level, row, tbl, col, RowX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void TableInsert(ItemModel model)
        {
            var tbl = model.Item as TableX;
            ItemCreated(new RowX(tbl));
        }
        #endregion

        #region 6A5 Graph  ====================================================
        ModelAction Graph_X;
        void Initialize_Graph_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_Graph_X,
            };
        }
        void Refresh_Graph_X(RootModel root, ItemModel model)
        {
        }
        internal void Graph_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var g = model.Item as Graph;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = g.GraphX.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = g.Name;

                        model.CanExpandLeft = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreateSecondaryModelGraph));
                        //root.ButtonCommands.Add(new ModelCommand(this, model, Trait.RefreshCommand, RefreshGraph));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? 5 : 0;
                if (N > 0)
                {
                    var item = model.Item as Graph;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var i = 0;
                    if (!TryGetOldModel(model, Trait.GraphNode_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ItemModel(model, Trait.GraphNode_ZM, level, item, null, null, GraphNodeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphEdge_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ItemModel(model, Trait.GraphEdge_ZM, level, item, null, null, GraphEdgeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphOpen_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ItemModel(model, Trait.GraphOpen_ZM, level, item, null, null, GraphOpenList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphRoot_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ItemModel(model, Trait.GraphRoot_ZM, level, item, null, null, GraphRootList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphLevel_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ItemModel(model, Trait.GraphLevel_ZM, level, item, null, null, GraphLevelList_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateSecondaryModelGraph(ItemModel model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.GraphDisplay, Trait.GraphRef_M, GraphRef_X);
        }
        #endregion

        #region 6A6 GraphRef  =================================================
        ModelAction GraphRef_X;
        void Initialize_GraphRef_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphRef_X,
            };
        }
        void Refresh_GraphRef_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphRef_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var graph = model.Item as Graph;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = graph.GraphX.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = graph.Name;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else
            {
                var graph = model.Item as Graph;
                //graph.GraphRefValidate(model as RootModel);
            }
        }
        #endregion

        #region 6A7 RowChildRelation  =========================================
        ModelAction RowChildRelation_X;
        void Initialize_RowChildRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowChildRelation_X,
            };
        }
        void Refresh_RowChildRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void RowChildRelation_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Aux1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = RowChildRelationDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = GetRelationName(rel);
                        root.ModelCount = model.Relation.ChildCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var row1 = model.Item as RowX;
                var rel = model.Aux1 as RelationX;

                var N = (model.IsExpandedLeft) ? rel.ChildCount(row1) : 0;

                if (N > 0)
                {
                    var items = rel.GetChildren(row1);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var row2 = items[i];
                        if (!TryGetOldModel(model, Trait.RowRelatedChild_M, oldModels, i, row2))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowRelatedChild_M, level, row2, rel, row1, RowRelatedChild_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction RowChildRelationDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            TableX expectedOwner;
            if (!drop.Item.IsRowX) return DropAction.None;
            if (!model.Item.IsRowX) return DropAction.None;
            if (!model.Aux1.IsRelationX) return DropAction.None;
            if (!TableX_ParentRelationX.TryGetParent(model.Aux1, out expectedOwner)) return DropAction.None;
            if (drop.Item.Owner != expectedOwner) return DropAction.None;

            if (doDrop)
            {
                var rel = model.Aux1 as RelationX;
                if (model.IsChildModel(drop))
                    RemoveLink(rel, model.Item, drop.Item);
                else
                    AppendLink(rel, model.Item, drop.Item);
            }
            return DropAction.Link;
        }
        #endregion

        #region 6A8 RowParentRelation  ========================================
        ModelAction RowParentRelation_X;
        void Initialize_RowParentRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowParentRelation_X,
            };
        }
        void Refresh_RowParentRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void RowParentRelation_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Aux1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = RowParentRelationDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = GetRelationName(rel);
                        root.ModelCount = model.Relation.ParentCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var rel = model.Aux1 as RelationX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && rel.TryGetParents(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.RowRelatedParent_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowRelatedParent_M, level, itm, rel, item, RowRelatedParent_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction RowParentRelationDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            TableX expectedOwner;
            if (!drop.Item.IsRowX) return DropAction.None;
            if (!model.Item.IsRowX) return DropAction.None;
            if (!model.Aux1.IsRelationX) return DropAction.None;
            if (!TableX_ChildRelationX.TryGetParent(model.Aux1, out expectedOwner)) return DropAction.None;
            if (drop.Item.Owner != expectedOwner) return DropAction.None;

            if (doDrop)
            {
                var rel = model.Aux1 as RelationX;
                if (model.IsChildModel(drop))
                    RemoveLink(rel, drop.Item, model.Item);
                else
                    AppendLink(rel, drop.Item, model.Item);
            }
            return DropAction.Link;
        }
        #endregion

        #region 6A9 RowRelatedChild  ============================================
        ModelAction RowRelatedChild_X;
        void Initialize_RowRelatedChild_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowRelatedChild_X,
            };
        }
        void Refresh_RowRelatedChild_X(RootModel root, ItemModel model)
        {
        }
        internal void RowRelatedChild_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var rel = model.Aux1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = RowSummary(model);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = row.TableX.Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = rel.ChildCount(row);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedChild));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void UnlinkRelatedChild(ItemModel model)
        {
            var key = model.Aux2;
            var rel = model.Aux1 as Relation;
            var item = model.Item;
            RemoveLink(rel, key, item);
        }
        private void UnlinkRelatedRow(ItemModel model)
        {
            var parent = model.ParentModel;
            if (parent.IsRowChildRelationModel)
            {
                var row2 = model.Item;
                var row1 = parent.Item;
                var rel = parent.Aux1 as Relation;
                RemoveLink(rel, row1, row2);
            }
            else if (parent.IsRowParentRelationModel)
            {
                var row1 = model.Item;
                var row2 = parent.Item;
                var rel = parent.Aux1 as Relation;
                RemoveLink(rel, row1, row2);
            }
        }
        private DropAction ReorderRelatedChild (ItemModel model, ItemModel drop, bool doDrop)
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

        #region 6AA RowRelatedParent  ============================================
        ModelAction RowRelatedParent_X;
        void Initialize_RowRelatedParent_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowRelatedParent_X,
            };
        }
        void Refresh_RowRelatedParent_X(RootModel root, ItemModel model)
        {
        }
        internal void RowRelatedParent_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var rel = model.Aux1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ReorderItems = ReorderRelatedParent;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = RowSummary(model);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = row.TableX.Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = rel.ParentCount(row);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedParent));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void UnlinkRelatedParent(ItemModel model)
        {
            var key = model.Item;
            var rel = model.Aux1 as Relation;
            var item = model.Aux2;
            RemoveLink(rel, key, item);
        }
        private DropAction ReorderRelatedParent(ItemModel model, ItemModel drop, bool doDrop)
        {
            if (model.Aux2 == null) return DropAction.None;
            if (model.Aux1 == null || !(model.Aux1 is Relation rel)) return DropAction.None;

            var key = model.Aux2;
            var item1 = drop.Item;
            var item2 = model.Item;
            (int index1, int index2) = rel.GetParentsIndex(key, item1, item2);

            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop) ItemParentMoved(rel, key, item1, index1, index2);
            
            return DropAction.Move;
        }
        #endregion

        #region 6AB EnumRelatedColumn  ========================================
        ModelAction EnumRelatedColumn_X;
        void Initialize_EnumRelatedColumn_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_EnumRelatedColumn_X,
            };
        }
        void Refresh_EnumRelatedColumn_X(RootModel root, ItemModel model)
        {
        }
        internal void EnumRelatedColumn_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item as ColumnX;
                var tbl = model.Aux1 as TableX;

                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = $"{tbl.Name}: {col.Name}";
                        break;

                    case ModelActionX.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedColumn));
                        break;
                }
            }
        }
        private void UnlinkRelatedColumn(ItemModel model)
        {
            var col = model.Item;
            var tbl = model.Aux1;
            var enu = model.Aux2;
            RemoveLink(EnumX_ColumnX, enu, col);
        }
        #endregion



        #region 6B1 RowPropertyList  ==========================================
        ModelAction RowPropertyList_X;
        void Initialize_RowPropertyList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowPropertyList_X,
            };
        }
        void Refresh_RowPropertyList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowPropertyList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetColumnCount(model.Item, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var row = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedColumns(row, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var col = items[i] as ColumnX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, row, col))
                            model.ChildModels[i] = NewPropertyModel(model, level, row, col);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B2 RowChildRelationList  =====================================
        ModelAction RowChildRelationList_X;
        void Initialize_RowChildRelationList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowChildRelationList_X,
            };
        }
        void Refresh_RowChildRelationList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowChildRelationList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetChildRelationCount(model.Item, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedChildRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowChildRelation_M, level, item, rel, null, RowChildRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B3 RowParentRelationList  ====================================
        ModelAction RowParentRelationList_X;
        void Initialize_RowParentRelationList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowParentRelationList_X,
            };
        }
        void Refresh_RowParentRelationList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowParentRelationList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetParentRelationCount(model.Item, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedParentRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowParentRelation_M, level, item, rel, null, RowParentRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B4 RowDefaultPropertyList  ===================================
        ModelAction RowDefaultPropertyList_X;
        void Initialize_RowDefaultPropertyList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowDefaultPropertyList_X,
            };
        }
        void Refresh_RowDefaultPropertyList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowDefaultPropertyList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetColumnCount(model.Item, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedColumns(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var prop = items[i] as Property;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, prop))
                            model.ChildModels[i] = NewPropertyModel(model, level, item, prop);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B5 RowUnusedChildRelationList  ===============================
        ModelAction RowUnusedChildRelationList_X;
        void Initialize_RowUnusedChildRelationList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowUnusedChildRelationList_X,
            };
        }
        void Refresh_RowUnusedChildRelationList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowUnusedChildRelationList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetChildRelationCount(model.Item, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedChildRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowChildRelation_M, level, item, rel, null, RowChildRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B6 RowUnusedParentRelationList  ==============================
        ModelAction RowUnusedParentRelationList_X;
        void Initialize_RowUnusedParentRelationList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowUnusedParentRelationList_X,
            };
        }
        void Refresh_RowUnusedParentRelationList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowUnusedParentRelationList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        GetParentRelationCount(model.Item, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedParentRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ItemModel(model, Trait.RowParentRelation_M, level, item, rel, null, RowParentRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6B7 RowComputeList  ===========================================
        ModelAction RowComputeList_X;
        void Initialize_RowComputeList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_RowComputeList_X,
            };
        }
        void Refresh_RowComputeList_X(RootModel root, ItemModel model)
        {
        }
        internal void RowComputeList_X(ItemModel model, RootModel root)
        {
            var item = model.Item;
            var sto = item.Owner;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;
                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var list = Store_ComputeX.GetChildren(sto);
                    for (int i = 0; i < N; i++)
                    {
                        var itm = list[i];
                        if (!TryGetOldModel(model, Trait.TextProperty_M, oldModels, i, item, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.TextProperty_M, level, item, itm, null, TextCompute_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion



        #region 6C1 QueryRootLink  ============================================
        ModelAction QueryRootLink_X;
        void Initialize_QueryRootLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryRootLink_X,
            };
        }
        void Refresh_QueryRootLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryRootLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryRootItem_M, level, itm, seg, null, QueryRootItem_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
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
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryPathHead_X,
            };
        }
        void Refresh_QueryPathHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryPathHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryPathLink_VX(model);
            }
        }
        #endregion

        #region 6C3 QueryPathLink  ============================================
        ModelAction QueryPathLink_X;
        void Initialize_QueryPathLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryPathLink_X,
            };
        }
        void Refresh_QueryPathLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryPathLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var seg = model.Aux1 as Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:


                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryPathLink_VX(model);
            }
        }
        private void QueryPathLink_VX(ItemModel model)
        {
            var seg = model.Query;

            Item[] items;
            if (seg.TryGetItems(out items))
            {
                var N = items.Length;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;
                model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathTail_M, level, itm, seg, null, QueryPathTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathStep_M, level, itm, seg, null, QueryPathStep_X);
                    }
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion

        #region 6C4 QueryGroupHead  ===========================================
        ModelAction QueryGroupHead_X;
        void Initialize_QueryGroupHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryGroupHead_X,
            };
        }
        void Refresh_QueryGroupHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryGroupHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryGroupLink_VX(model);
            }
        }
        #endregion

        #region 6C5 QueryGroupLink  ===========================================
        ModelAction QueryGroupLink_X;
        void Initialize_QueryGroupLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryGroupLink_X,
            };
        }
        void Refresh_QueryGroupLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryGroupLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryGroupLink_VX(model);
            }
        }
        private void QueryGroupLink_VX(ItemModel model)
        {
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Item[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupTail_M, level, itm, seg, null, QueryGroupTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupStep_M, level, itm, seg, null, QueryGroupStep_X);
                    }
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion

        #region 6C6 QueryEgressHead  ==========================================
        ModelAction QueryEgressHead_X;
        void Initialize_QueryEgressHead_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryEgressHead_X,
            };
        }
        void Refresh_QueryEgressHead_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryEgressHead_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryEgressLink_VX(model);
            }
        }
        #endregion

        #region 6C7 QueryEgressLink  ==========================================
        ModelAction QueryEgressLink_X;
        void Initialize_QueryEgressLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryEgressLink_X,
            };
        }
        void Refresh_QueryEgressLink_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryEgressLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryEgressLink_VX(model);
            }
        }
        private void QueryEgressLink_VX(ItemModel model)
        {
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Item[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressTail_M, level, itm, seg, null, QueryEgressTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressStep_M, level, itm, seg, null, QueryEgressStep_X);
                    }
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion



        #region 6D1 QueryRootItem  ============================================
        ModelAction QueryRootItem_X;
        void Initialize_QueryRootItem_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryRootItem_X,
            };
        }
        void Refresh_QueryRootItem_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryRootItem_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = (row.Owner as TableX).Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                ValidateQueryModel(model);
            }
        }
        private void ValidateQueryModel(ItemModel model)
        {
            var itm = model.Item;
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Query[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    seg = items[i];
                    if (seg.IsGraphLink)
                    {
                        if (!TryGetOldModel(model, Trait.QueryRootLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryRootLink_M, level, itm, seg, null, QueryRootLink_X);
                    }
                    else if (seg.IsPathHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathHead_M, level, itm, seg, null, QueryPathHead_X);
                    }
                    else if (seg.IsGroupHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupHead_M, level, itm, seg, null, QueryGroupHead_X);
                    }
                    else if (seg.IsSegueHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressHead_M, level, itm, seg, null, QueryEgressHead_X);
                    }
                    else
                        throw new Exception("Invalid Query");
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        #endregion

        #region 6D2 QueryPathStep  ============================================
        ModelAction QueryPathStep_X;
        void Initialize_QueryPathStep_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryPathStep_X,
            };
        }
        void Refresh_QueryPathStep_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryPathStep_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(model.KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryPathLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathLink_M, level, itm, seg, null, QueryPathLink_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6D3 QueryPathTail  ============================================
        ModelAction QueryPathTail_X;
        void Initialize_QueryPathTail_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryPathTail_X,
            };
        }
        void Refresh_QueryPathTail_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryPathTail_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(model.KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6D4 QueryGroupStep  ===========================================
        ModelAction QueryGroupStep_X;
        void Initialize_QueryGroupStep_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryGroupStep_X,
            };
        }
        void Refresh_QueryGroupStep_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryGroupStep_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryGroupLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupLink_M, level, itm, seg, null, QueryGroupLink_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6D5 QueryGroupTail  ===========================================
        ModelAction QueryGroupTail_X;
        void Initialize_QueryGroupTail_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryGroupTail_X,
            };
        }
        void Refresh_QueryGroupTail_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryGroupTail_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(model.KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6D6 QueryEgressStep  ==========================================
        ModelAction QueryEgressStep_X;
        void Initialize_QueryEgressStep_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryEgressStep_X,
            };
        }
        void Refresh_QueryEgressStep_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryEgressStep_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(model.KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryEgressLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressLink_M, level, itm, seg, null, QueryEgressLink_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6D7 QueryEgressTail  ==========================================
        ModelAction QueryEgressTail_X;
        void Initialize_QueryEgressTail_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_QueryEgressTail_X,
            };
        }
        void Refresh_QueryEgressTail_X(RootModel root, ItemModel model)
        {
        }
        internal void QueryEgressTail_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = $"{_localize(model.KindKey)} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 6E1 GraphXRef  ================================================
        ModelAction GraphXRef_X;
        void Initialize_GraphXRef_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphXRef_X,
            };
        }
        void Refresh_GraphXRef_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphXRef_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gx = model.Item as GraphX;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        root.ModelDrop = GraphXModDrop;
                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = gx.Summary;
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = gx.Trait.ToString();
                        root.ModelName = gx.Name;
                        root.ModelCount = gx.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.CreateCommand, CreateGraph));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gx = model.Item as GraphX;
                var N = model.IsExpandedLeft ? gx.Count : 0;

                if (N > 0)
                {
                    var items = gx.ToArray;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var g = items[i] as Graph;
                        if (!TryGetOldModel(model, Trait.Graph_M, oldModels, i, g))
                            model.ChildModels[i] = new ItemModel(model, Trait.Graph_M, level, g, null, null, Graph_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateGraph(ItemModel model)
        {
            var gx = model.Item as GraphX;
            CreateGraph(gx, out Graph g);

            model.IsExpandedLeft = true;
            MajorDelta += 1;

            var root = model.GetRootModel();
            root.UIRequest = UIRequest.CreateNewView(ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true);
        }
        private DropAction GraphXModDrop(ItemModel model, ItemModel drop, bool doDrop)
        {
            var gd = model.Item as GraphX;
            Graph g;
            Store tbl = null;

            if (GraphX_QueryX.ChildCount(gd) == 0) return DropAction.None;

            var items = GraphX_QueryX.GetChildren(gd);
            foreach (var item in items)
            {
                if (item.IsQueryGraphRoot && Store_QueryX.TryGetParent(item, out tbl) && drop.Item.Owner == tbl) break;
            }
            if (tbl == null) return DropAction.None;

            foreach (var tg in gd.ToArray)
            {
                if (tg.RootItem == drop.Item) return DropAction.None;
            }

            if (doDrop)
            {
                CreateGraph(gd, out g, drop.Item);

                model.IsExpandedLeft = true;
                MajorDelta += 1;

                var root = model.GetRootModel();
                root.UIRequest = UIRequest.CreateNewView(ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true);
            }
            return DropAction.Copy;
        }
        #endregion

        #region 6E2 GraphNodeList  ============================================
        ModelAction GraphNodeList_X;
        void Initialize_GraphNodeList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphNodeList_X,
            };
        }
        void Refresh_GraphNodeList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphNodeList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = model.Graph.NodeCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Nodes;
                var N = items.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphNode_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphNode_M, level, itm, null, null, GraphNode_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E3 GraphEdgeList  ============================================
        ModelAction GraphEdgeList_X;
        void Initialize_GraphEdgeList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphEdgeList_X,
            };
        }
        void Refresh_GraphEdgeList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphEdgeList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = model.Graph.EdgeCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Edges;
                var N = items.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphEdge_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphEdge_M, level, itm, null, null, GraphEdge_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E4 GraphRootList  ============================================
        ModelAction GraphRootList_X;
        void Initialize_GraphRootList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphRootList_X,
            };
        }
        void Refresh_GraphRootList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphRootList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = model.Graph.QueryCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = item.Forest;
                var N = item.QueryCount;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var seg = items[i];
                        var tbl = seg.Item;
                        if (!TryGetOldModel(model, Trait.GraphRoot_M, oldModels, i, tbl, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphRoot_M, level, tbl, seg, null, GraphRoot_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E5 GraphLevelList  ===========================================
        ModelAction GraphLevelList_X;
        void Initialize_GraphLevelList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphLevelList_X,
            };
        }
        void Refresh_GraphLevelList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphLevelList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as Graph;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = item.Levels.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as Graph;
                var items = item.Levels;
                var N = model.IsExpandedLeft ? items.Count : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Level;
                        if (!TryGetOldModel(model, Trait.GraphLevel_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphLevel_M, level, itm, null, null, GraphLevel_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E6 GraphLevel  ===============================================
        ModelAction GraphLevel_X;
        void Initialize_GraphLevel_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphLevel_X,
            };
        }
        void Refresh_GraphLevel_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphLevel_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item as Level;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = item.Name;
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item as Level;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Paths;
                var N = (model.IsExpandedLeft) ? items.Count : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSortAscending || model.IsSortDescending || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphPath_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphPath_M, level, itm, null, null, GraphPath_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E7 GraphPath  ================================================
        ModelAction GraphPath_X;
        void Initialize_GraphPath_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphPath_X,
            };
        }
        void Refresh_GraphPath_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphPath_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var path = model.Item as Path;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = GetPathKind(path);
                        root.ModelName = GetPathName(path);
                        root.ModelCount = path.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var path = model.Item as Path;
                var items = path.Items;
                var N = (model.IsExpandedLeft) ? path.Count : 0;
                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Path;
                        if (!TryGetOldModel(model, Trait.GraphPath_M, oldModels, i, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphPath_M, level, itm, null, null, GraphPath_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private string GetPathName(Path path)
        {
            return GetHeadTailName(path.Head, path.Tail);
        }
        private string GetHeadTailName(Item head, Item tail)
        {
            var headName = GetIdentity(head, IdentityStyle.Double);
            var tailName = GetIdentity(tail, IdentityStyle.Double);
            return $"{headName} --> {tailName}";
        }
        private string GetPathKind(Path path)
        {
            var name = _localize(path.NameKey);
            var kind = path.IsRadial ? _localize(GetKindKey(Trait.RadialPath)) : _localize(GetKindKey(Trait.LinkPath));
            return $"{name}{kind}";
        }
        #endregion

        #region 6E8 GraphRoot  ================================================
        ModelAction GraphRoot_X;
        void Initialize_GraphRoot_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphRoot_X,
            };
        }
        void Refresh_GraphRoot_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphRoot_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item as TableX;
                var seg = model.Aux1 as Query;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = tbl.Name;
                        root.ModelCount = seg.ItemCount;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryRootItem_M, level, itm, seg, null, QueryRootItem_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6E9 GraphNode  ================================================
        ModelAction GraphNode_X;
        void Initialize_GraphNode_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphNode_X,
            };
        }
        void Refresh_GraphNode_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphNode_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var nd = model.Item as Node;
                var g = nd.Graph;
                List<Edge> edges;

                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = GetIdentity(nd.Item, IdentityStyle.Double);
                        root.ModelCount = g.Node_Edges.TryGetValue(nd, out edges) ? edges.Count : 0;

                        model.CanExpandRight = true;
                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var level = (byte)(model.Level + 1);
                List<Edge> edges = null;
                var nd = model.Item as Node;
                var g = nd.Graph;
                var sp = new Property[] { _nodeCenterXYProperty, _nodeSizeWHProperty, _nodeOrientationProperty, _nodeFlipRotateProperty, _nodeLabelingProperty, _nodeResizingProperty, _nodeBarWidthProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;
                var L = (model.IsExpandedLeft && g.Node_Edges.TryGetValue(nd, out edges)) ? edges.Count : 0;
                var N = L + R;

                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        for (int i = R, j = 0; i < N; i++, j++)
                        {
                            var edge = edges[j];
                            if (!TryGetOldModel(model, Trait.GraphEdge_M, oldModels, i, edge))
                                model.ChildModels[i] = new ItemModel(model, Trait.GraphEdge_M, level, edge, null, null, GraphEdge_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 6EA GraphEdge  ================================================
        ModelAction GraphEdge_X;
        void Initialize_GraphEdge_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphEdge_X,
            };
        }
        void Refresh_GraphEdge_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphEdge_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var edge = model.Item as Edge;
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = GetEdgeName(edge);

                        model.CanExpandRight = true;
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _edgeFace1Property, _edgeFace2Property, _edgeGnarl1Property, _edgeGnarl2Property, _edgeConnect1Property, _edgeConnect2Property };
                var N = model.IsExpandedRight ? sp.Length : 0;

                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private string GetEdgeName(Edge edge)
        {
            return GetHeadTailName(edge.Node1.Item, edge.Node2.Item);
        }
        #endregion


        #region 6EB GraphOpenList  ============================================
        ModelAction GraphOpenList_X;
        void Initialize_GraphOpenList_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphOpenList_X,
            };
        }
        void Refresh_GraphOpenList_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphOpenList_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = model.Graph.OpenQuerys.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var g = model.Item as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = g.OpenQuerys.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var h = g.OpenQuerys[i].Query1;
                        var t = g.OpenQuerys[i].Query2;
                        if (!TryGetOldModel(model, Trait.GraphOpen_M, oldModels, i, g, h, t))
                            model.ChildModels[i] = new ItemModel(model, Trait.GraphOpen_M, level, g, h, t, GraphOpen_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        #endregion

        #region 6EC GraphOpen  ================================================
        ModelAction GraphOpen_X;
        void Initialize_GraphOpen_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_GraphOpen_X,
            };
        }
        void Refresh_GraphOpen_X(RootModel root, ItemModel model)
        {
        }
        internal void GraphOpen_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:
                        break;

                    case ModelActionX.PointerOver:
                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = GraphOpenKind(model);
                        root.ModelName = GraphOpenName(model);
                        break;

                    case ModelActionX.ModelSelect:
                        break;
                }
            }
        }
        private string GraphOpenKind(ItemModel model)
        {
            var g = model.Item as Graph;
            var h = model.Aux1 as Query;

            return GetIdentity(h.Item, IdentityStyle.Double);
        }
        private string GraphOpenName(ItemModel model)
        {
            var g = model.Item as Graph;
            var t = model.Aux2 as Query;
            Store head, tail;
            GetHeadTail(t.QueryX, out head, out tail);
            return $"{GetIdentity(t.Item, IdentityStyle.Double)}  -->  {GetIdentity(tail, IdentityStyle.Single)}: <?>";
        }

        #endregion


        #region 7D0 PrimeCompute  =============================================
        ModelAction PrimeCompute_X;
        void Initialize_PrimeCompute_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_PrimeCompute_X,
            };
        }
        void Refresh_PrimeCompute_X(RootModel root, ItemModel model)
        {
        }
        internal void PrimeCompute_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(GetSummaryKey(Trait.PrimeCompute_M));
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(GetNameKey(Trait.PrimeCompute_M));
                        root.ModelCount = GetPrimeComputeCount();

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(GetDescriptionKey(Trait.PrimeCompute_M));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? GetPrimeComputeCount() : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var list = GetPrimeComputeStores();

                    for (int i = 0; i < N; i++)
                    {
                        var sto = list[i];
                        if (!TryGetOldModel(model, Trait.ComputeStore_M, oldModels, i, sto))
                            model.ChildModels[i] = new ItemModel(model, Trait.ComputeStore_M, level, sto, null, null, ComputeStore_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
            int GetPrimeComputeCount()
            {
                var count = 0;
                foreach (var sto in _primeStores)
                {
                    if (Store_ComputeX.HasChildLink(sto)) count += 1;
                }
                return count;
            }
            List<Store> GetPrimeComputeStores()
            {
                var list = new List<Store>();
                foreach (var sto in _primeStores)
                {
                    if (Store_ComputeX.HasChildLink(sto)) list.Add(sto);
                }
                return list;
            }
        }
        #endregion

        #region 7D1 ComputeStore  =============================================
        ModelAction ComputeStore_X;
        void Initialize_ComputeStore_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_ComputeStore_X,
            };
        }
        void Refresh_ComputeStore_X(RootModel root, ItemModel model)
        {
        }
        internal void ComputeStore_X(ItemModel model, RootModel root)
        {
            var sto = model.Item as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(sto, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = GetIdentity(sto, IdentityStyle.Single);
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var list = Store_ComputeX.GetChildren(sto);
                    for (int i = 0; i < N; i++)
                    {
                        var itm = list[i];
                        if (!TryGetOldModel(model, Trait.TextProperty_M, oldModels, i, sto, itm))
                            model.ChildModels[i] = new ItemModel(model, Trait.TextProperty_M, level, sto, itm, null, TextCompute_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion


        #region 7F0 InternlStoreZ  ============================================
        ModelAction InternalStoreZ_X;
        void Initialize_InternalStoreZ_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_InternalStoreZ_X,
            };
        }
        void Refresh_InternalStoreZ_X(RootModel root, ItemModel model)
        {
        }
        internal void InternalStoreZ_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(GetSummaryKey(Trait.InternalStore_ZM));
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(GetNameKey(Trait.InternalStore_ZM));

                        model.CanExpandLeft = true;
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelDescription = _localize(GetDescriptionKey(Trait.InternalStore_ZM));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? 11 : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    int i = 0;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _viewXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _viewXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _enumXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _enumXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _tableXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _tableXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _graphXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _graphXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _queryXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _queryXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _symbolXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _symbolXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _columnXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _columnXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _relationXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _relationXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _computeXStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _computeXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _relationStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _relationStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _propertyStore))
                        model.ChildModels[i] = new ItemModel(model, Trait.InternalStore_M, level, _propertyStore, null, null, InternalStore_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F1 InternalStore  ============================================
        ModelAction InternalStore_X;
        void Initialize_InternalStore_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_InternalStore_X,
            };
        }
        void Refresh_InternalStore_X(RootModel root, ItemModel model)
        {
        }
        internal void InternalStore_X(ItemModel model, RootModel root)
        {
            var store = model.Item as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(store.NameKey);
                        root.ModelCount = store.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? store.Count : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var list = store.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetOldModel(model, Trait.StoreItem_M, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreItem_M, level, item, null, null, StoreItem_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F2 StoreItem  ================================================
        ModelAction StoreItem_X;
        void Initialize_StoreItem_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreItem_X,
            };
        }
        void Refresh_StoreItem_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreItem_X(ItemModel model, RootModel root)
        {
            var item = model.Item;
            var hasItems = (item is Store sto && sto.Count > 0) ? true : false;
            var hasLinks = (item is Relation rel && rel.GetLinksCount() > 0) ? true : false;
            var hasChildRels = (GetChildRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var hasParentRels = (GetParentRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var count = 0;
            if (hasItems) count++;
            if (hasLinks) count++;
            if (hasChildRels) count++;
            if (hasParentRels) count++;


            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.StoreItem);
                        root.ModelCount = count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Description);
                        if (item.IsExternal) root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else
            {
                if (model.IsExpandedLeft && count > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != count) model.ChildModels = new ItemModel[count];

                    int i = -1;
                    if (hasItems)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreItemItem_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreItemItem_ZM, level, item, null, null, StoreItemItemZ_X);
                    }
                    if (hasLinks)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreRelationLink_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreRelationLink_ZM, level, item, null, null, StoreRelationLinkZ_X);
                    }
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreChildRelation_ZM, level, item, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreParentRelation_ZM, level, item, null, null, StoreParentRelationZ_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F4 StoreItemItemZ  ===========================================
        ModelAction StoreItemItemZ_X;
        void Initialize_StoreItemItemZ_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreItemItemZ_X,
            };
        }
        void Refresh_StoreItemItemZ_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreItemItemZ_X(ItemModel model, RootModel root)
        {
            var store = model.Item as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = store.Count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? store.Count : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    var list = store.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetOldModel(model, Trait.StoreItemItem_M, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreItemItem_M, level, item, null, null, StoreItemItem_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F5 StoreRelationLinkZ  =======================================
        ModelAction StoreRelationLinkZ_X;
        void Initialize_StoreRelationLinkZ_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreRelationLinkZ_X,
            };
        }
        void Refresh_StoreRelationLinkZ_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreRelationLinkZ_X(ItemModel model, RootModel root)
        {
            var rel = model.Item as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = rel.GetLinksCount();

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var oldModels = model.ChildModels;
                model.ChildModels = null;

                Item[] parents = null;
                Item[] children = null;
                var N = (model.IsExpandedLeft) ? rel.GetLinks(out parents, out children) : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);
                    model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var parent = parents[i];
                        var child = children[i];
                        if (!TryGetOldModel(model, Trait.StoreRelationLink_M, oldModels, i, rel, parent, child))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreRelationLink_M, level, rel, parent, child, StoreRelationLink_X);
                    }
                }
            }
        }
        #endregion

        #region 7F6 StoreChildRelationZ  ======================================
        ModelAction StoreChildRelationZ_X;
        void Initialize_StoreChildRelationZ_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreChildRelationZ_X,
            };
        }
        void Refresh_StoreChildRelationZ_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreChildRelationZ_X(ItemModel model, RootModel root)
        {
            var item = model.Item;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GetChildRelationCount(item, SubsetType.Used);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedLeft && TryGetChildRelations(item, out Relation[] relations, SubsetType.Used))
                {
                    var N = relations.Length;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_M, oldModels, i, rel, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreChildRelation_M, level, rel, item, null, StoreChildRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F7 StoreParentRelationZ  =====================================
        ModelAction StoreParentRelationZ_X;
        void Initialize_StoreParentRelationZ_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreParentRelationZ_X,
            };
        }
        void Refresh_StoreParentRelationZ_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreParentRelationZ_X(ItemModel model, RootModel root)
        {
            var item = model.Item;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = _localize(model.SummaryKey);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelName = _localize(model.NameKey);
                        root.ModelCount = GetParentRelationCount(item, SubsetType.Used);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpandedLeft && TryGetParentRelations(item, out Relation[] relations, SubsetType.Used))
                {
                    var N = relations.Length;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_M, oldModels, i, rel, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreParentRelation_M, level, rel, item, null, StoreParentRelation_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F8 StoreItemItem  ============================================
        ModelAction StoreItemItem_X;
        void Initialize_StoreItemItem_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreItemItem_X,
            };
        }
        void Refresh_StoreItemItem_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreItemItem_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item;

                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = _localize(item.KindKey);
                        root.ModelName = GetIdentity(item, IdentityStyle.Double);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 7F9 StoreRelationLink  ========================================
        ModelAction StoreRelationLink_X;
        void Initialize_StoreRelationLink_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreRelationLink_X,
            };
        }
        void Refresh_StoreRelationLink_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreRelationLink_X(ItemModel model, RootModel root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item as Relation;
                var parent = model.Aux1;
                var child = model.Aux2;

                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = _localize(model.KindKey);
                        root.ModelName = $"({GetIdentity(parent, IdentityStyle.Double)}) --> ({GetIdentity(child, IdentityStyle.Double)})";
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 7FA StoreChildRelation  =======================================
        ModelAction StoreChildRelation_X;
        void Initialize_StoreChildRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreChildRelation_X,
            };
        }
        void Refresh_StoreChildRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreChildRelation_X(ItemModel model, RootModel root)
        {
            var rel = model.Item as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = GetKind(rel.Trait);
                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);
                        root.ModelCount = rel.ChildCount(model.Aux1);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var oldModels = model.ChildModels;
                model.ChildModels = null;

                if (model.IsExpandedLeft)
                {
                    Item[] items;
                    if (rel.TryGetChildren(model.Aux1, out items))
                    {
                        var N = items.Length;
                        var level = (byte)(model.Level + 1);
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.StoreRelatedItem_M, level, itm, null, null, StoreRelatedItem_X);
                        }
                    }
                }
            }
        }
        #endregion

        #region 7FA StoreParentRelation  ======================================
        ModelAction StoreParentRelation_X;
        void Initialize_StoreParentRelation_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreParentRelation_X,
            };
        }
        void Refresh_StoreParentRelation_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreParentRelation_X(ItemModel model, RootModel root)
        {
            var rel = model.Item as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:

                        root.ModelKind = GetKind(rel.Trait);
                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);
                        root.ModelCount = rel.ParentCount(model.Aux1);

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var oldModels = model.ChildModels;
                model.ChildModels = null;

                if (model.IsExpandedLeft)
                {
                    Item[] items;
                    if (rel.TryGetParents(model.Aux1, out items))
                    {
                        var N = items.Length;
                        var level = (byte)(model.Level + 1);
                        model.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ItemModel(model, Trait.StoreRelatedItem_M, level, itm, null, null, StoreRelatedItem_X);
                        }
                    }
                }
            }
        }
        #endregion

        #region 7FC StoreRelatedItem  =========================================
        ModelAction StoreRelatedItem_X;
        void Initialize_StoreRelatedItem_X()
        {
            DataChef_X = new ModelAction
            {
                Refresh = Refresh_StoreRelatedItem_X,
            };
        }
        void Refresh_StoreRelatedItem_X(RootModel root, ItemModel model)
        {
        }
        internal void StoreRelatedItem_X(ItemModel model, RootModel root)
        {
            var item = model.Item;
            var hasChildRels = (GetChildRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var hasParentRels = (GetParentRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var count = 0;
            if (hasChildRels) count++;
            if (hasParentRels) count++;


            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelActionX.DragOver:

                        break;

                    case ModelActionX.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelActionX.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.StoreItem);
                        root.ModelCount = count;

                        model.CanExpandLeft = (root.ChildCount > 0);
                        model.CanFilter = (root.ChildCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ChildCount > 1);
                        break;

                    case ModelActionX.ModelSelect:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Description);
                        if (item.IsExternal) root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else
            {
                var oldModels = model.ChildModels;
                model.ChildModels = null;

                if (model.IsExpandedLeft && count > 0)
                {
                    var level = (byte)(model.Level + 1);
                     model.ChildModels = new ItemModel[count];

                    int i = -1;
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreChildRelation_ZM, level, item, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ItemModel(model, Trait.StoreParentRelation_ZM, level, item, null, null, StoreParentRelationZ_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion
    }
}
