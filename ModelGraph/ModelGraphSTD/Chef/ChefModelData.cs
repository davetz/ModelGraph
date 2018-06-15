using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ModelGraphSTD
{/*
    depth Level
 */
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
                        _stack = new(ItemModel[] Models, int Index)[_count * 2];
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

        #region RefreshViewList  ==============================================
        internal void RefreshViewList(RootModel root, int scroll = 0, ChangeType change = ChangeType.NoChange)
        {
            var select = root.SelectModel;
            var viewList = root.ViewModels;
            var capacity = root.ViewCapacity;

            var first = ItemModel.FirstValidModel(viewList);
            var start = (first == null);

            viewList.Clear();
            UpdateSelectModel(select, change);

            ValidateModel(root);
            var modelStack = new TreeModelStack();
            modelStack.PushChildren(root);

            var S = (scroll < 0) ? -scroll : scroll;
            var N = capacity;
            var buffer = new CircularBuffer(N, S);

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

            root.UIRequestQueue.Enqueue(UIRequest.RefreshModel());
        }

        #region CircularBuffer  ===============================================
        class CircularBuffer
        {
            ItemModel[] _buffer;
            int _first;
            int _count;
            int _scroll;
            int _length;


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
                if (first < 0)  first = 0;
                CopyBuffer(first, list);
            }

            internal void GetTail(List<ItemModel> list)
            {
                var first = (_count < _buffer.Length) ? 0 : ((_count - _first) < _length) ? (_count - _length + _scroll): _first + _scroll;
                CopyBuffer(first, list);
            }

            #region PrivateMethods  ===========================================
            int Index(int inx) => inx % _buffer.Length;
            int Limit(int num) => (num < 0) ? 0 : (num < _buffer.Length) ? num : _buffer.Length;
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

        #region UpdateSelectModel  ========================================
        void UpdateSelectModel(ItemModel m, ChangeType change)
        {
            if (m != null)
            {
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

        #region ValidateModel  ============================================
        bool ValidateModel(ItemModel model)
        {
            if (model.Item.AutoExpandRight)
            {
                model.IsExpandedRight = true;
                model.Item.AutoExpandRight = false;
            }

            if (model.IsChanged)
            {
                model.IsChanged = false;

                if (model.IsExpandedLeft || model.IsExpandedRight || model.ChildModels != null)
                {
                    model.Validate();
                    return FilterSort(model);
                }
            }
            return (model.ChildModels != null && model.ChildModels.Length > 0);
        }
        #endregion
        #endregion

        #region ValidateModelTree =============================================
        public bool ValidateModelTree(RootModel root, ChangeType change = ChangeType.NoChange)
        {
            return false;
            //var oldModels = root.ViewList;
            //var oldLen = (oldModels == null) ? 0 : oldModels.Length;

            //var rebuildTree = (root.MajorDelta != MajorDelta) || (oldModels == null) || (change != ChangeType.NoChange);
            //var anyChange = (root.MinorDelta != MinorDelta) || rebuildTree;
            //if (!anyChange) return false;

            //root.MajorDelta = MajorDelta;
            //root.MinorDelta = MinorDelta;

            //UpdateSelectModel();

            //var modelStack = new TreeModelStack();
            //var validFilter = new HashSet<object>();
            //if (rebuildTree)
            //{
            //    //=============================================================
            //    // rebuild the itemModel tree
            //    //=============================================================
            //    root.Validate();
            //    var newLen = modelStack.PushChildren(root); // count the number of child models
            //    while (modelStack.IsNotEmpty)
            //    {
            //        var model = modelStack.PopNext();
            //        if (root.ViewFilter.ContainsKey(model)) validFilter.Add(model);

            //        ValidateModel(model);
            //        newLen += modelStack.PushChildren(model); // count the numer of child models
            //    }
            //    ValidateViewFilters();
            //    //=============================================================
            //    // build a flat array representation of the itemModel tree
            //    //=============================================================
            //    var n = 0;
            //    var newModels = root.ViewList = new ItemModel[newLen]; // create array of the correct size

            //    modelStack.PushChildren(root);
            //    while (modelStack.IsNotEmpty)
            //    {
            //        var model = newModels[n++] = modelStack.PopNext();
            //        modelStack.PushChildren(model);
            //    }
            //    //=============================================================
            //    // try to scroll to the previously visible location
            //    //=============================================================
            //    if (newLen > 0)
            //    {
            //        if (oldLen > 0)
            //        {
            //            var index1 = root.ViewIndex1;
            //            var index2 = root.ViewIndex2;
            //            var select = root.SelectModel;
            //            var index = IndexOf(select, index1, index2);
            //            var delta = OldDelta(select, index1, index2);

            //            if (IndexOf(oldModels[index1], index1, index2) < 0)
            //            {
            //                if (index < 0)
            //                {
            //                    index = IndexOf(oldModels[index2 - 1], index1, index2);
            //                    if (index < 0)
            //                    {
            //                        root.SelectModel = FindSelectParent();
            //                        if (index < 0)
            //                            SetScroll(0);
            //                        else
            //                            SetScroll(index);
            //                    }
            //                    else // did not find newModels[index1] or select, but did find newModels[index2 - 1]
            //                    {
            //                        if (index < 0 && delta > 0) // old select was deleted ?
            //                            root.SelectModel = root.SelectModel = oldModels[delta - 1];
            //                        SetScroll(index2 - root.ViewCapacity);
            //                    }
            //                }
            //                else // did not find newModels[index1], but did find select
            //                {
            //                    SetScroll(index - delta);
            //                }
            //            }
            //            else // found newModels[index1]
            //            {
            //                if (index < 0 && delta > 0) // old select was deleted ?
            //                {
            //                    var model = oldModels[delta - 1];
            //                    if (IndexOf(model, index1, index2) < 0)
            //                        model = FindSelectParent();
            //                    root.SelectModel = model;
            //                }
            //                SetScroll(index1);
            //            }

            //            #region FindSelectParent  =============================
            //            ItemModel FindSelectParent()
            //            {
            //                var mod = select;
            //                while (mod != null && mod.ParentModel != null)
            //                {
            //                    mod = mod.ParentModel;
            //                    index = IndexOf(mod, 0, newLen);
            //                    if (index >= 0) break;
            //                }
            //                return mod;
            //            }
            //            #endregion
            //        }
            //        else
            //        {
            //            SetScroll(0);
            //        }

            //        #region IndexOf  ==========================================
            //        int IndexOf(ItemModel model, int index1, int index2)
            //        {
            //            if (index2 > newLen) index2 = newLen;

            //            for (int i = index1; i < index2; i++)
            //            {
            //                if (model == newModels[i]) return i;
            //            }
            //            return -1;
            //        }
            //        #endregion

            //        #region OldDelta  =========================================
            //        int OldDelta(ItemModel model, int index1, int index2)
            //        {
            //            if (index2 > oldLen) index2 = oldLen;
            //            for (int i = index1; i < index2; i++)
            //            {
            //                if (model == oldModels[i]) return (i - index1);
            //            }
            //            return -1;
            //        }
            //        #endregion

            //        #region SetScroll  ========================================
            //        void SetScroll(int index1)
            //        {
            //            if (index1 < 0) index1 = 0;

            //            if (root.ViewCapacity < newLen)
            //            {
            //                if ((index1 + root.ViewCapacity) < newLen)
            //                {
            //                    root.ViewIndex1 = index1;
            //                    root.ViewIndex2 = index1 + root.ViewCapacity;
            //                }
            //                else
            //                {
            //                    root.ViewIndex1 = newLen - root.ViewCapacity;
            //                    root.ViewIndex2 = newLen;
            //                }
            //            }
            //            else
            //            {
            //                root.ViewIndex1 = 0;
            //                root.ViewIndex2 = newLen;
            //            }
            //            if (IndexOf(root.SelectModel, root.ViewIndex1, root.ViewIndex2) < 0)
            //                root.SelectModel = root.ViewList[root.ViewIndex1];
            //        }
            //        #endregion
            //    }
            //    else
            //    {
            //        root.ViewIndex1 = root.ViewIndex2 = 0;
            //        root.SelectModel = null;
            //    }
            //}

            //return anyChange;


            //#region ValidateViewFilters  ======================================
            //void ValidateViewFilters()
            //{
            //    var invalidFilter = new HashSet<object>();
            //    foreach (var e in root.ViewFilter)
            //    {
            //        if (validFilter.Contains(e.Key)) continue;
            //        invalidFilter.Add(e.Key);
            //    }
            //    foreach (var key in invalidFilter)
            //    {
            //        root.ViewFilter.Remove(key);
            //    }
            //}
            //#endregion
        }
        #endregion

        #region FilterSort  ===================================================
        bool FilterSort(ItemModel model)
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
            return (count > 0);
        }

        string GetFilterSortName(ItemModel m)
        {
            var (kind, name, count, type) = m.ModelParms;
            return $"{kind} {name}";
        }
        bool TryGetFilter(ItemModel m, out Regex filter)
        {
            filter = (!m.IsExpandedFilter || string.IsNullOrWhiteSpace(m.ViewFilter) ? null : 
                new Regex(".*" + m.ViewFilter + ".*", RegexOptions.Compiled | RegexOptions.IgnoreCase));

            return (filter != null);
        }
        class FilterSortItem
        {
            internal string Name;
            internal ItemModel Model;

            internal FilterSortItem(ItemModel model, string name)
            {
                Name = name;
                Model = model;
            }
        }
        class AlphabeticOrder : IComparer<FilterSortItem>
        {
            int IComparer<FilterSortItem>.Compare(FilterSortItem x, FilterSortItem y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        static AlphabeticOrder alphabeticOrder = new AlphabeticOrder();
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
                    model.ChildModels[i] = NewPropertyModel(model, model.Depth, item, col);
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

                Validate = (m) =>
                {
                    var chef = m.Item as Chef;
                    var N = chef.Count;

                    if (N > 0)
                    {
                        var items = chef.Items;
                        var oldModels = m.ChildModels;
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var child = items[i] as Chef;
                            if (!TryGetOldModel(m, Trait.MockChef_M, oldModels, i, child))
                                m.ChildModels[i] = new ItemModel(m, Trait.DataChef_M, 0, child, null, null, child.DataChef_X);
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };

            
            #region ButtonCommands  ===========================================
            void NewModel(ItemModel model)
            {
                var root = model as RootModel;
                var rootChef = root.Chef;
                var dataChef = new Chef(rootChef, null);

                root.UIRequestQueue.Enqueue(UIRequest.CreateView(root, ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_X));
                root.UIRequestQueue.Enqueue(UIRequest.RefreshModel());
            }
            void OpenModel(ItemModel model, Object parm1)
            {
                var repo = parm1 as IRepository;
                var root = model as RootModel;
                var rootChef = root.Chef;
                var dataChef = new Chef(rootChef, repo);

                root.UIRequestQueue.Enqueue(UIRequest.CreateView(root, ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_X));
                root.UIRequestQueue.Enqueue(UIRequest.RefreshModel());
            }
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
            void CloseModel(ItemModel model)
            {
                var root = model as RootModel;
                root.UIRequestQueue.Enqueue(UIRequest.CloseModel());
            }
            void ReloadModel(ItemModel model)
            {
                var root = model as RootModel;
                root.UIRequestQueue.Enqueue(UIRequest.ReloadModel());
            }
            void AppSaveSymbol(ItemModel model)
            {
                var root = model as RootModel;
                root.UIRequestQueue.Enqueue(UIRequest.SaveModel());
            }
            void AppReloadSymbol(ItemModel model)
            {
                var root = model as RootModel;
                root.UIRequestQueue.Enqueue(UIRequest.ReloadModel());
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
                    m.CanExpandLeft = true;

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var N = 4;

                    if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                    var i = 0;
                    if (!TryGetOldModel(m, Trait.ErrorRoot_M, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.ErrorRoot_M, depth, _errorStore, null, null, ErrorRoot_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.ChangeRoot_M, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.ChangeRoot_M, depth, _changeRoot, null, null, ChangeRoot_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.MetadataRoot_M, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.MetadataRoot_M, depth, item, null, null, MetadataRoot_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.ModelingRoot_M, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.ModelingRoot_M, depth, item, null, null, ModelingRoot_X);
                },
            };
        }
        #endregion

        #region 614 TextColumn_X  =============================================
        ModelAction TextColumn_X;
        void Initialize_TextColumn_X()
        {
            TextColumn_X = new ModelAction
            {
                ModelParms = (m) => (null, m.ColumnX.Name, 0, ModelType.TextProperty),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ColumnX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ColumnX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

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
                ModelParms = (m) => (null, m.ColumnX.Name, 0, ModelType.CheckProperty),

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
                ModelParms = (m) => (null, m.ColumnX.Name, 0, ModelType.ComboProperty),

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
                ModelParms = (m) =>
                {
                    var name = _localize(m.Property.NameKey);
                    var fullname = m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {name}" : name;

                    return (null, fullname, 0, ModelType.TextProperty);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Property.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.Property.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

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
                ModelParms = (m) =>
                {
                    var name = _localize(m.Property.NameKey);
                    var fullname = m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {name}" : name;

                    return (null, fullname, 0, ModelType.CheckProperty);
                },

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
                    var name = _localize(m.Property.NameKey);
                    var fullname = m.Property.HasItemName ? $"{m.Property.GetItemName(m.Item)} {name}" : name;

                    return (null, fullname, 0, ModelType.ComboProperty);
                },

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
                ModelParms = (m) => (null, m.ComputeX.Name, 0, ModelType.TextProperty),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ComputeX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => m.ComputeX.Description,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                TextValue = (m) => m.ComputeX.Value.GetString(m.Item),
            };
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

                    m.CanExpandLeft = count > 0;

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = m.IsExpandedLeft ? _errorStore.Count : 0;

                    if (N > 0)
                    {
                        var items = _errorStore.Items;
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as Error;
                            if (!TryGetOldModel(m, Trait.ErrorType_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ErrorType_M, depth, itm, null, null, ErrorType_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                },
            };
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

                    m.CanExpandLeft = count > 0;

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelInfo = (m) => m.IsExpandedLeft ? null : _changeRootInfoText,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, list) =>
                {
                    if (_changeRoot.Count > 0 && m.IsExpandedLeft == false)
                        list.Add(new ModelCommand(this, m, Trait.ExpandAllCommand, ExpandAllChangeSets));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var oldModels = m.ChildModels;
                    m.ChildModels = null;

                    if (m.IsExpandedLeft)
                    {
                        var N = _changeRoot.Count;

                        if (N > 0)
                        {
                            var items = new List<ChangeSet>(_changeRoot.ToArray);
                            items.Reverse();
                            var depth = (byte)(m.Depth + 1);
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = items[i] as ChangeSet;
                                if (!TryGetOldModel(m, Trait.ChangeSet_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ChangeSet_M, depth, itm, null, null, ChangeSet_X);
                            }
                        }
                    }
                },
            };
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
                    m.CanExpandLeft = true;

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryMetadataTree));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = (m.IsExpandedLeft) ? 5 : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetOldModel(m, Trait.ViewXView_ZM, oldModels, i, _viewXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_ZM, depth, _viewXStore, null, null, ViewXView_ZX);

                        i++;
                        if (!TryGetOldModel(m, Trait.EnumX_ZM, oldModels, i, _enumZStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.EnumX_ZM, depth, _enumZStore, null, null, EnumXList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.TableX_ZM, oldModels, i, _tableXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.TableX_ZM, depth, _tableXStore, null, null, TableXList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.GraphX_ZM, oldModels, i, _graphXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphX_ZM, depth, _graphXStore, null, null, GraphXList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.InternalStore_ZM, oldModels, i, this))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_ZM, depth, this, null, null, InternalStoreZ_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

            void CreateSecondaryMetadataTree(ItemModel model)
            {
                var root = model.GetRootModel();
                root.UIRequestQueue.Enqueue(model.BuildViewRequest(ControlType.PartialTree));
            }
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
                    m.CanExpandLeft = true;

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryModelingTree));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = (m.IsExpandedLeft) ? 4 : 0;

                    if (N > 0)
                    {
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetOldModel(m, Trait.ViewView_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.ViewView_ZM, depth, item, null, null, ViewXView_ZX);

                        i++;
                        if (!TryGetOldModel(m, Trait.Table_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.Table_ZM, depth, item, null, null, TableList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.Graph_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.Graph_ZM, depth, item, null, null, GraphList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.PrimeCompute_M, oldModels, i, this))
                            m.ChildModels[i] = new ItemModel(m, Trait.PrimeCompute_M, depth, this, null, null, PrimeCompute_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondaryModelingTree(ItemModel model)
            {
                var root = model.GetRootModel();
                root.UIRequestQueue.Enqueue(model.BuildViewRequest(ControlType.PartialTree));
            }
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
                    m.CanExpandLeft = true;

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    int N = (m.IsExpandedLeft) ? 2 : 0;

                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetOldModel(m, Trait.NameColumnRelation_M, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.NameColumnRelation_M, depth, item, TableX_NameProperty, null, NameColumnRelation_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.SummaryColumnRelation_M, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.SummaryColumnRelation_M, depth, item, TableX_SummaryProperty, null, SummaryColumnRelation_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;

                    return (null, _localize(m.Item.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.Item.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var error = m.Error;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var N = (m.IsExpandedLeft) ? error.Count : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || oldModels.Length != N)
                        {
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                m.ChildModels[i] = new ItemModel(m, Trait.ErrorText_M, depth, error, null, null, ErrorText_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var e = m.Error;
                    var i = m.ParentModel.GetChildlIndex(m);
                    var name = (i < 0 || e.Count <= i) ? InvalidItem : e.Errors[i];

                    return (null, name, 0, ModelType.Default);
                },
            };
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
                    var cs = m.ChangeSet;
                    var count = cs.Count;
                    var name = cs.IsCongealed ? _localize(cs.NameKey) : cs.Name;

                    m.CanExpandLeft = count > 0;

                    return (null, name, count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var cs = m.ChangeSet;
                    var N = m.IsExpandedLeft ? cs.Count : 0;

                    if (N > 0)
                    {
                        var items = (cs.IsReversed) ? cs.ToArray : cs.ItemsReversed;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as ItemChange;
                            if (!TryGetOldModel(m, Trait.ItemChange_M, oldModels, i, itm))
                            {
                                m.ChildModels[i] = new ItemModel(m, Trait.ItemChange_M, depth, itm, null, null, ItemChanged_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                ModelParms = (m) => (_localize(m.Item.KindKey), _localize(m.Item.NameKey), 0, ModelType.Default),
            };
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
                    var count = 0;
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }

                    m.CanExpandLeft = count > 0;

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    int N = 0;
                    if (m.IsExpandedLeft)
                    {
                        var views = _viewXStore.ToArray;
                        var roots = new List<ViewX>();
                        foreach (var view in views) { if (ViewX_ViewX.HasNoParent(view)) { roots.Add(view); N++; } }

                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = roots[i];
                                if (!TryGetOldModel(m, Trait.ViewXView_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, itm, null, null, ViewXViewM_X);
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };

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
                    var count = (ViewX_ViewX.ChildCount(vx) + ViewX_QueryX.ChildCount(vx) + ViewX_Property.ChildCount(vx));

                    m.CanDrag = true;
                    m.CanSort = count > 1;
                    m.CanFilter = count > 2;
                    m.CanExpandLeft = count > 0;
                    m.CanExpandRight = true;

                    return (null, _localize(vx.Name), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var view = m.ViewX;

                    int N = 0;
                    if (m.IsExpandedLeft || m.IsExpandedRight)
                    {
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
                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            if (R > 0)
                            {
                                AddProperyModels(m, oldModels, props);
                            }
                            if (L1 > 0)
                            {
                                for (int i = R, j = 0; j < L1; i++, j++)
                                {
                                    var px = propertyList[j];

                                    if (!TryGetOldModel(m, Trait.ViewXProperty_M, oldModels, i, px))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXProperty_M, depth, px, ViewX_Property, view, ViewXProperty_X);
                                }
                            }
                            if (L2 > 0)
                            {
                                for (int i = (R + L1), j = 0; j < L2; i++, j++)
                                {
                                    var qx = queryList[j];
                                    if (!TryGetOldModel(m, Trait.ViewXQuery_M, oldModels, i, qx))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXQuery_M, depth, qx, ViewX_QueryX, view, ViewXQuery_X);
                                }
                            }
                            if (L3 > 0)
                            {
                                for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                                {
                                    var vx = viewList[j];
                                    if (!TryGetOldModel(m, Trait.ViewXView_M, oldModels, i, vx))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, vx, ViewX_ViewX, view, ViewXViewM_X);
                                }
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }

            };

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
                    var count = (QueryX_ViewX.ChildCount(qx) + QueryX_QueryX.ChildCount(qx) + QueryX_Property.ChildCount(qx));

                    m.CanSort = (m.IsExpandedLeft && count > 1);
                    m.CanFilter = count > 2;
                    m.CanExpandLeft = count > 0;

                    var rel = Relation_QueryX.GetParent(qx);
                    if (rel != null)
                    {
                        return (_localize(m.KindKey), GetIdentity(qx, IdentityStyle.Single), count, ModelType.Default);
                    }
                    else
                    {
                        m.CanDrag = true;
                        m.CanExpandRight = true;

                        var sto = Store_QueryX.GetParent(qx);
                        return (GetIdentity(sto, IdentityStyle.Kind), GetIdentity(sto, IdentityStyle.Double), count, ModelType.Default);
                    }
                },

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

                Validate = (m) =>
                {
                    var qx = m.QueryX;

                    int N = 0;
                    if (m.IsExpandedLeft || m.IsExpandedRight)
                    {
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
                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            if (R > 0)
                            {
                                AddProperyModels(m, oldModels, props);
                            }
                            if (L1 > 0)
                            {
                                for (int i = R, j = 0; j < L1; i++, j++)
                                {
                                    var px = propertyList[j];

                                    if (!TryGetOldModel(m, Trait.ViewXProperty_M, oldModels, i, px))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXProperty_M, depth, px, QueryX_Property, qx, ViewXProperty_X);
                                }
                            }
                            if (L2 > 0)
                            {
                                for (int i = (R + L1), j = 0; j < L2; i++, j++)
                                {
                                    var qr = queryList[j];
                                    if (!TryGetOldModel(m, Trait.ViewXQuery_M, oldModels, i, qr))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXQuery_M, depth, qr, QueryX_QueryX, qx, ViewXQuery_X);
                                }
                            }
                            if (L3 > 0)
                            {
                                for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                                {
                                    var vx = viewList[j];
                                    if (!TryGetOldModel(m, Trait.ViewXView_M, oldModels, i, vx))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewXView_M, depth, vx, QueryX_ViewX, qx, ViewXViewM_X);
                                }
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }

            };

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
                Validate = (m) =>
                {
                    int N = 0;
                    if (m.IsExpandedLeft)
                    {
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
        }
        #endregion

        #region 635 ViewXProperty_M  ==========================================
        ModelAction ViewXProperty_X;
        void Initialize_ViewXProperty_X()
        {
            ViewXProperty_X = new ModelAction
            {
                ModelParms = (m) => (GetIdentity(m.Item, IdentityStyle.Kind), GetIdentity(m.Item, IdentityStyle.Double), 0, ModelType.Default),
            };
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
                    var count = 0;
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    int N = 0;
                    if (m.IsExpandedLeft)
                    {
                        var views = _viewXStore.ToArray;
                        var roots = new List<ViewX>();
                        foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) { roots.Add(vx); N++; } }

                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = roots[i];
                                if (!TryGetOldModel(m, Trait.ViewView_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewView_M, depth, itm, null, null, ViewView_X);
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
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

                    return (null, vx.Name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ViewX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var vx = m.ViewX;
                    var key = m.Aux1; // may be null
                    int N = 0;
                    if (m.IsExpandedLeft)
                    {
                        var propertyList = ViewX_Property.GetChildren(vx);
                        var queryList = ViewX_QueryX.GetChildren(vx);
                        var viewList = ViewX_ViewX.GetChildren(vx);

                        var L1 = (propertyList == null) ? 0 : propertyList.Length;
                        var L2 = (queryList == null) ? 0 : queryList.Length;
                        var L3 = (viewList == null) ? 0 : viewList.Length;

                        if (L2 == 1 && Store_QueryX.HasParentLink(queryList[0]) && TryGetQueryItems(queryList[0], out Item[] items))
                        {
                            N = items.Length;
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = items[i];

                                if (!TryGetOldModel(m, Trait.ViewItem_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewItem_M, depth, itm, queryList[0], null, ViewItem_X);
                            }
                        }
                        else if (key != null && L2 > 0)
                        {
                            N = L2;
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var qx = queryList[i];

                                if (!TryGetOldModel(m, Trait.ViewQuery_M, oldModels, i, qx))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewQuery_M, depth, qx, key, null, ViewQuery_X);
                            }
                        }
                        else if (L3 > 0)
                        {
                            N = L3;
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var v = viewList[i];

                                if (!TryGetOldModel(m, Trait.ViewView_M, oldModels, i, v))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewView_M, depth, v, null, null, ViewView_X);
                            }
                        }
                        //N = L1 + L2 + L3;
                        //if (N > 0)
                        //{
                        //    var depth = (byte)(model.Level + 1);
                        //    var oldModels = model.ChildModels;
                        //    model.ChildModels = new TreeModel[N];

                        //    if (L1 > 0)
                        //    {
                        //        for (int i = 0, j = 0; j < L1; i++, j++)
                        //        {
                        //            var px = propertyList[j];

                        //            if (!TryGetOldModel(model, Trait.ViewXProperty_M, oldModels, i, px))
                        //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXProperty_M, depth, px, ViewX_P, view, ViewXProperty_M);
                        //        }
                        //    }
                        //    if (L2 > 0)
                        //    {
                        //        for (int i = (L1), j = 0; j < L2; i++, j++)
                        //        {
                        //            var qx = queryList[j];
                        //            if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qx))
                        //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXQuery_M, depth, qx, ViewX_QueryX, view, ViewQuery_M);
                        //        }
                        //    }
                        //    if (L3 > 0)
                        //    {
                        //        for (int i = (L1 + L2), j = 0; j < L3; i++, j++)
                        //        {
                        //            var vx = viewList[j];
                        //            if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                        //                model.ChildModels[i] = new TreeModel(model, Trait.ViewXView_M, depth, vx, ViewX_ViewX, view, ViewView_M);
                        //        }
                        //    }
                        //}
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
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
                    var count = (L2 + L3);

                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = L1 > 0;
                    m.CanFilterUsage = (m.IsExpandedLeft && count > 1);

                    return (GetIdentity(item.Owner, IdentityStyle.Single), GetIdentity(item, IdentityStyle.Single), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var qx = m.Aux1 as QueryX;

                    int N = 0;
                    if (m.IsExpandedLeft || m.IsExpandedRight)
                    {
                        var (L1, PropertyList, L2, QueryList, L3, ViewList) = GetQueryXChildren(qx);

                        int R = (m.IsExpandedRight) ? L1 : 0;
                        int L = (m.IsExpandedLeft) ? (L2 + L3) : 0;

                        N = R + L;
                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            if (R > 0)
                            {
                                AddProperyModels(m, oldModels, PropertyList);
                            }

                            if (L > 0)
                            {
                                int i = R;
                                for (int j = 0; j < L2; i++, j++)
                                {
                                    var q = QueryList[j];
                                    if (!TryGetOldModel(m, Trait.ViewQuery_M, oldModels, i, item, q))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ViewQuery_M, depth, item, q, null, ViewQuery_X);
                                }
                                for (int j = 0; j < L3; i++, j++)
                                {

                                }
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
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
                    var qx = m.Aux1 as QueryX;
                    var count = TryGetQueryItems(qx, out Item[] items, key) ? items.Length : 0;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (_localize(m.KindKey), GetIdentity(qx, IdentityStyle.Single), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var key = m.Item;
                    var qx = m.Aux1 as QueryX;
                    int N = 0;
                    if (m.IsExpandedLeft)
                    {
                        N = TryGetQueryItems(qx, out Item[] items, key) ? items.Length : 0;

                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = items[i];
                                if (!TryGetOldModel(m, Trait.ViewItem_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ViewItem_M, depth, itm, qx, null, ViewItem_X);
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
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
                    var count = _enumXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = _enumXStore.Count;

                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = _enumXStore.ToArray;
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as EnumX;
                            if (!TryGetOldModel(m, Trait.EnumX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.EnumX_M, depth, itm, null, null, EnumX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = _tableXStore.Count;

                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = _tableXStore.ToArray;
                        var item = m.Item;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as TableX;
                            if (!TryGetOldModel(m, Trait.TableX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.TableX_M, depth, itm, null, null, TableX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }

                }
            };

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
                    var count = _graphXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.InsertCommand, Insert));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = _graphXStore.Count;

                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = _graphXStore.ToArray;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as GraphX;
                            if (!TryGetOldModel(m, Trait.GraphX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphX_M, depth, itm, null, null, GraphX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = GraphX_SymbolX.ChildCount(gx);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var gd = m.Item as GraphX;
                    var N = GraphX_SymbolX.ChildCount(gd);

                    if (m.IsExpandedLeft && N > 0)
                    {
                        var syms = GraphX_SymbolX.GetChildren(gd);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var sym = syms[i];
                            if (!TryGetOldModel(m, Trait.SymbolX_M, oldModels, i, sym))
                                m.ChildModels[i] = new ItemModel(m, Trait.SymbolX_M, depth, sym, GraphX_SymbolX, gd, SymbolX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = _tableXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var N = _tableXStore.Count;
                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = _tableXStore.ToArray;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.Table_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.Table_M, depth, itm, null, null, Table_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = _graphXStore.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var N = _graphXStore.Count;
                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = _graphXStore.ToArray;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var gx = items[i];
                            if (!TryGetOldModel(m, Trait.GraphXRef_M, oldModels, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXRef_M, depth, gx, null, null, GraphXRef_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var name = m.PairX.DisplayValue;

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (null, name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.PairX.ActualValue,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderRelatedChild,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    if (m.IsExpandedRight)
                    {
                        var sp = new Property[] { _pairXTextProperty, _pairXValueProperty };
                        var N = sp.Length;

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var ex = m.Item as EnumX;

                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (null, ex.Name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => (m.Item as EnumX).Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as EnumX;
                    var sp = new Property[] { _enumXNameProperty, _enumXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 2 : 0;
                    var N = R + L;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var i = R;
                            if (!TryGetOldModel(m, Trait.EnumValue_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.EnumValue_ZM, depth, item, null, null, PairXList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.EnumColumn_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.EnumColumn_ZM, depth, item, null, null, EnumColumnList_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (null, m.TableX.Name, 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _tableXNameProperty, _tableXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 5 : 0;
                    var N = R + L;

                    if (N > 0)
                    {
                        var item = m.Item as TableX;
                        var depth = (byte)(m.Depth + 1);
                        var oldModels = m.ChildModels;

                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var i = R;
                            if (!TryGetOldModel(m, Trait.ColumnX_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.ColumnX_ZM, depth, item, null, null, ColumnXList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.ComputeX_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.ComputeX_ZM, depth, item, null, null, ComputeXList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.ChildRelationX_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.ChildRelationX_ZM, depth, item, null, null, ChildRelationXList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.ParentRelatationX_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.ParentRelatationX_ZM, depth, item, null, null, ParentRelationXList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.MetaRelation_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.MetaRelation_ZM, depth, item, null, null, MetaRelationList_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (null, m.GraphX.Name, 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _graphXNameProperty, _graphXSummaryProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = m.IsExpandedLeft ? 4 : 0;
                    var N = R + L;

                    if (N > 0)
                    {
                        var gx = m.GraphX;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var i = R;
                            if (!TryGetOldModel(m, Trait.GraphXColoring_M, oldModels, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXColoring_M, depth, gx, null, null, GraphXColoring_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.GraphXRoot_ZM, oldModels, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXRoot_ZM, depth, gx, null, null, GraphXRootList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.GraphXNode_ZM, oldModels, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXNode_ZM, depth, gx, null, null, GraphXNodeList_X);

                            i++;
                            if (!TryGetOldModel(m, Trait.SymbolX_ZM, oldModels, i, gx))
                                m.ChildModels[i] = new ItemModel(m, Trait.SymbolX_ZM, depth, gx, null, null, SymbolXList_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = true;

                    return (null, m.SymbolX.Name, 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    if (m.IsExpandedRight)
                    {
                        var sp = new Property[] { _symbolXNameProperty };
                        var N = sp.Length;

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondarySymbolEdit(ItemModel model)
            {
                var root = model.GetRootModel();
                root.UIRequestQueue.Enqueue(model.BuildViewRequest(ControlType.SymbolEditor));
            }
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
                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (null, m.ColumnX.Name, 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    if (m.IsExpandedRight)
                    {
                        var sp = new Property[] { _columnXNameProperty, _columnXSummaryProperty, _columnXTypeOfProperty, _columnXIsChoiceProperty, _columnXInitialProperty };
                        var N = sp.Length;

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = (qx == null) ? 0 : QueryX_QueryX.ChildCount(qx);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (null, cx.Name, count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    if (m.IsExpanded)
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
                        if (N > 0)
                        {
                            var oldModels = m.ChildModels;
                            if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                            if (R > 0)
                            {
                                AddProperyModels(m, oldModels, sp);
                            }
                            if (L > 0)
                            {
                                var items = QueryX_QueryX.GetChildren(qx);
                                var depth = (byte)(m.Depth + 1);
                                for (int i = R, j = 0; i < N; i++, j++)
                                {
                                    var itm = items[j] as QueryX;
                                    if (!TryGetOldModel(m, Trait.ValueXHead_M, oldModels, i, itm))
                                        m.ChildModels[i] = new ItemModel(m, Trait.ValueXHead_M, depth, itm, null, null, ValueHead_X);
                                }
                            }
                        }
                        else
                        {
                            m.ChildModels = null;
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = TableX_ColumnX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var tbl = m.Item as TableX;

                    var N = (m.IsExpandedLeft) ? TableX_ColumnX.ChildCount(tbl) : 0;
                    if (N > 0)
                    {
                        var items = TableX_ColumnX.GetChildren(tbl);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.ColumnX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ColumnX_M, depth, itm, TableX_ColumnX, tbl, ColumnX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = TableX_ChildRelationX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var tbl = m.Item as TableX;

                    var N = (m.IsExpandedLeft) ? TableX_ChildRelationX.ChildCount(tbl) : 0;
                    if (N > 0)
                    {
                        var items = TableX_ChildRelationX.GetChildren(tbl);

                        var depth = (byte)(m.Depth + 1);
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i];
                            if (!TryGetOldModel(m, Trait.ChildRelationX_M, oldModels, i, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.ChildRelationX_M, depth, rel, TableX_ChildRelationX, tbl, ChildRelationX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = TableX_ParentRelationX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var tx = m.TableX;

                    var N = (m.IsExpandedLeft) ? TableX_ParentRelationX.ChildCount(tx) : 0;
                    if (N > 0)
                    {
                        var items = TableX_ParentRelationX.GetChildren(tx);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i];
                            if (!TryGetOldModel(m, Trait.ParentRelationX_M, oldModels, i, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.ParentRelationX_M, depth, rel, TableX_ParentRelationX, tx, ParentRelationX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = ex.Count;

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var enu = m.Item as EnumX;
                    var N = enu.Count;

                    if (m.IsExpandedLeft && N > 0)
                    {
                        var items = enu.ToArray;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as PairX;
                            if (!TryGetOldModel(m, Trait.PairX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.PairX_M, depth, itm, enu, null, PairX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = EnumX_ColumnX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var ex = m.Item as EnumX;

                    var N = (m.IsExpandedLeft) ? EnumX_ColumnX.ChildCount(ex) : 0;
                    if (N > 0)
                    {
                        var items = EnumX_ColumnX.GetChildren(ex);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var col = items[i];
                            var tbl = TableX_ColumnX.GetParent(col);
                            if (tbl != null)
                            {
                                if (!TryGetOldModel(m, Trait.EnumRelatedColumn_M, oldModels, i, col))
                                    m.ChildModels[i] = new ItemModel(m, Trait.EnumRelatedColumn_M, depth, col, tbl, ex, EnumRelatedColumn_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = Store_ComputeX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sto = m.Store;

                    var N = (m.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;
                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);
                        var items = Store_ComputeX.GetChildren(sto);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.ComputeX_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.ComputeX_M, depth, itm, Store_ComputeX, sto, ComputeX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (null, GetIdentity(m.RelationX, IdentityStyle.Single), 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    if (m.IsExpandedRight)
                    {
                        var sp1 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty };
                        var sp2 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty };

                        var item = m.Item as RelationX;
                        var sp = item.IsLimited ? sp2 : sp1;
                        var N = sp.Length;

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    return (null, GetIdentity(m.RelationX, IdentityStyle.Single), 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    if (m.IsExpandedRight)
                    {
                        var sp1 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty, _relationXIsLimitedProperty };
                        var sp2 = new Property[] { _relationXNameProperty, _relationXSummaryProperty, _relationXPairingProperty, _relationXIsRequiredProperty, _relationXIsLimitedProperty, _relationXMinOccuranceProperty, _relationXMaxOccuranceProperty };

                        var item = m.Item as RelationX;
                        var sp = item.IsLimited ? sp2 : sp1;
                        var N = sp.Length;

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }

                }
            };
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
                    var count = TableX_NameProperty.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var tbl = m.Item as TableX;

                    if (m.IsExpandedLeft && TableX_NameProperty.TryGetChild(tbl, out Property prop))
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || oldModels.Length != 1) m.ChildModels = new ItemModel[1];

                        if (!TryGetOldModel(m, Trait.NameColumn_M, oldModels, 0, prop))
                            m.ChildModels[0] = new ItemModel(m, Trait.NameColumn_M, depth, prop, tbl, null, NameColumn_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = TableX_SummaryProperty.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ReorderItems = ReorderStoreItem,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                },

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

                Validate = (m) =>
                {
                    var tbl = m.Item as TableX;

                    if (m.IsExpandedLeft && TableX_SummaryProperty.TryGetChild(tbl, out Property prop))
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || oldModels.Length != 1) m.ChildModels = new ItemModel[1];

                        if (!TryGetOldModel(m, Trait.SummaryColumn_M, oldModels, 0, prop))
                            m.ChildModels[0] = new ItemModel(m, Trait.SummaryColumn_M, depth, prop, tbl, null, SummaryColumn_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    if (m.Item.IsColumnX) return (null, m.ColumnX.Name, 0, ModelType.Default);
                    if (m.Item.IsComputeX) return (null, m.ComputeX.Name, 0, ModelType.Default);
                    throw new Exception("Corrupt ItemModelTree");
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) =>
                {
                    if (m.Item.IsColumnX) return m.ColumnX.Summary;
                    if (m.Item.IsComputeX) return m.ComputeX.Summary;
                    throw new Exception("Corrupt ItemModelTree");
                },
            };
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
                    if (m.Item.IsColumnX) return (null, m.ColumnX.Name, 0, ModelType.Default);
                    if (m.Item.IsComputeX) return (null, m.ComputeX.Name, 0, ModelType.Default);
                    throw new Exception("Corrupt ItemModelTree");
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) =>
                {
                    if (m.Item.IsColumnX) return m.ColumnX.Summary;
                    if (m.Item.IsComputeX) return m.ComputeX.Summary;
                    throw new Exception("Corrupt ItemModelTree");
                },
            };
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
                    var count = GraphX_ColorColumnX.ChildCount(m.GraphX);

                    m.CanExpandLeft = (count > 0);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(m.DescriptionKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDrop = (m, d, doDrop) =>
                {
                    var gx = m.GraphX;
                    var col = d.ColumnX;
                    if (!col.IsColumnX) return DropAction.None;
                    if (!gx.IsGraphX) return DropAction.None;

                    if (doDrop)
                    {
                        AppendLink(GraphX_ColorColumnX, gx, col);
                    }
                    return DropAction.Link;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;

                    if (m.IsExpandedLeft && GraphX_ColorColumnX.TryGetChild(item, out ColumnX col) && TableX_ColumnX.TryGetParent(col, out TableX tbl))
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || oldModels.Length != 1) m.ChildModels = new ItemModel[1];

                        if (!TryGetOldModel(m, Trait.GraphXColorColumn_M, oldModels, 0, col, tbl))
                            m.ChildModels[0] = new ItemModel(m, Trait.GraphXColorColumn_M, depth, col, tbl, null, GraphXColorColumn_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };        
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
                    var count = GraphX_QueryX.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var gd = m.Item as GraphX;

                    var N = (m.IsExpandedLeft) ? GraphX_QueryX.ChildCount(gd) : 0;
                    if (N > 0)
                    {
                        var items = GraphX_QueryX.GetChildren(gd);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.GraphXRoot_M, oldModels, i, itm, gd))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXRoot_M, depth, itm, gd, null, QueryXRoot_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = GetNodeOwners(m.GraphX).Count;

                    m.CanExpandLeft = (count > 0);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var gx = m.GraphX;

                    if (m.IsExpandedLeft)
                    {
                        var owners = GetNodeOwners(gx).ToArray();
                        var N = owners.Length;
                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);

                            var oldModels = m.ChildModels;
                            if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var sto = owners[i];
                                if (!TryGetOldModel(m, Trait.GraphXNode_M, oldModels, i, sto, gx))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXNode_M, depth, sto, gx, null, GraphXNode_X);
                            }
                        }
                        else
                        {
                            m.ChildModels = null;
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = GetSymbolQueryXCount(m.GraphX, m.Store);

                    m.CanExpandLeft = (count > 0);

                    return (_localize(m.KindKey), GetIdentity(m.Item, IdentityStyle.Single), 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var gx = m.GraphX;
                    var st = m.Store;

                    var N = (m.IsExpandedLeft) ? GetSymbolQueryXCount(gx, st) : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        (var symbols, var querys) = GetSymbolXQueryX(gx, st);
                        for (int i = 0; i < N; i++)
                        {
                            var seg = querys[i];
                            if (!TryGetOldModel(m, Trait.GraphXNodeSymbol_M, oldModels, i, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXNodeSymbol_M, depth, seg, GraphX_SymbolQueryX, gx, GraphXNodeSymbol_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var col = m.Item as ColumnX;
                    var tbl = m.Aux1 as TableX;

                    return (_localize(m.KindKey), $"{tbl.Name} : {col.Name}", 0, ModelType.Default);
                },
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    var tb = Store_QueryX.GetParent(m.Item);

                    return (_localize(m.NameKey), GetIdentity(tb, IdentityStyle.Single), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    int N = 0;
                    if (m.IsExpandedLeft || m.IsExpandedRight)
                    {
                        var qx = m.Item as QueryX;
                        var sp = new Property[] { };
                        var R = m.IsExpandedRight ? sp.Length : 0;

                        var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                        N = L + R;
                        if (N > 0)
                        {
                            var depth = (byte)(m.Depth + 1);
                            var oldModels = m.ChildModels;
                            m.ChildModels = new ItemModel[N];

                            if (R > 0)
                            {
                                AddProperyModels(m, oldModels, sp);
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
                                            if (!TryGetOldModel(m, Trait.GraphXPathHead_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathHead_M, depth, child, null, null, QueryXPathHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXPathLink_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, child, null, null, QueryXPathLink_X);
                                        }
                                    }
                                    else if (child.IsGroup)
                                    {
                                        if (child.IsHead)
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXGroupHead_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupHead_M, depth, child, null, null, QueryXGroupHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXGroupLink_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, child, null, null, QueryXGroupLink_X);
                                        }
                                    }
                                    else if (child.IsSegue)
                                    {
                                        if (child.IsHead)
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXEgressHead_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressHead_M, depth, child, null, null, QueryXEgressHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXEgressLink_M, oldModels, i, child))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, child, null, null, QueryXEgressLink_X);
                                        }
                                    }
                                    else if (child.IsRoot)
                                    {
                                        if (!TryGetOldModel(m, Trait.GraphXLink_M, oldModels, i, child))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, child, null, null, QueryXLink_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(m, Trait.GraphXLink_M, oldModels, i, child))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, child, null, null, QueryXLink_X);
                                    }
                                }
                            }
                        }
                    }
                    if (N == 0) m.ChildModels = null;
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
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
                                            if (!TryGetOldModel(m, Trait.GraphXPathHead_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathHead_M, depth, qx, null, null, QueryXPathHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXPathLink_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, qx, null, null, QueryXPathLink_X);
                                        }
                                        break;

                                    case QueryType.Group:
                                        if (qx.IsHead)
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXGroupHead_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupHead_M, depth, qx, null, null, QueryXGroupHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXGroupLink_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, qx, null, null, QueryXGroupLink_X);
                                        }
                                        break;

                                    case QueryType.Segue:
                                        if (qx.IsHead)
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXEgressHead_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressHead_M, depth, qx, null, null, QueryXEgressHead_X);
                                        }
                                        else
                                        {
                                            if (!TryGetOldModel(m, Trait.GraphXEgressLink_M, oldModels, i, qx))
                                                m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, qx, null, null, QueryXEgressLink_X);
                                        }
                                        break;

                                    case QueryType.Graph:
                                        if (!TryGetOldModel(m, Trait.GraphXLink_M, oldModels, i, qx))
                                            m.ChildModels[i] = new ItemModel(m, Trait.GraphXLink_M, depth, qx, null, null, QueryXLink_X);
                                        break;

                                    default:
                                        throw new Exception("Invalid item trait");
                                }
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXHeadName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXConnect1Property, _queryXConnect2Property, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXPathLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, itm, null, null, QueryXPathLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXPathLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXPathLink_M, depth, itm, null, null, QueryXPathLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXHeadName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXGroupLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, itm, null, null, QueryXGroupLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXGroupLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXGroupLink_M, depth, itm, null, null, QueryXGroupLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXHeadName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXEgressLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, itm, null, null, QueryXEgressLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var sd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            var items = QueryX_QueryX.GetChildren(sd);
                            var depth = (byte)(m.Depth + 1);

                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.GraphXEgressLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphXEgressLink_M, depth, itm, null, null, QueryXEgressLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandRight = true;

                    var name = (SymbolX_QueryX.TryGetParent(m.Item, out SymbolX sym)) ? sym.Name : null;

                    return (_localize(m.KindKey), name, 0, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXWhereProperty };
                    var N = m.IsExpandedRight ? sp.Length : 0;

                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var qx = m.Item as QueryX;
                    var sp1 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty };
                    var sp2 = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                    var sp = qx.HasSelect ? sp2 : sp1;
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(qx);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.ValueXLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ValueXLink_M, depth, itm, null, null, ValueLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = QueryX_QueryX.ChildCount(m.Item);

                    m.CanDrag = true;
                    m.CanExpandLeft = (count > 0);
                    m.CanExpandRight = true;

                    return (_localize(m.KindKey), QueryXLinkName(m), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;

                    var vd = m.Item as QueryX;
                    var L = (m.IsExpandedLeft) ? QueryX_QueryX.ChildCount(vd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var items = QueryX_QueryX.GetChildren(vd);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var itm = items[j];
                                if (!TryGetOldModel(m, Trait.ValueXLink_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.ValueXLink_M, depth, itm, null, null, ValueLink_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandLeft = true;
                    m.CanExpandRight = m.RowX.TableX.HasChoiceColumns;

                    return (null, GetIdentity(m.Item, IdentityStyle.Single), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, RemoveItem));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    RowX_VX(m);
                }
            };
        }
        private void RowX_VX(ItemModel m)
        {
            var row = m.Item as RowX;
            ColumnX[] cols = null;
            var R = (m.IsExpandedRight && TryGetChoiceColumns(row.Owner, out cols)) ? cols.Length : 0;
            var L = (m.IsExpandedLeft) ? 7 : 0;
            var N = R + L;

            if (N > 0)
            {
                var depth = (byte)(m.Depth + 1);

                var oldModels = m.ChildModels;
                if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];


                if (R > 0)
                {
                    AddProperyModels(m, oldModels, cols);
                }
                if (L > 0)
                {
                    GetColumnCount(row, out int usedColumnCount, out int unusedColumnCount);
                    GetChildRelationCount(row, out int usedChidRelationCount, out int unusedChildRelationCount);
                    GetParentRelationCount(row, out int usedParentRelationCount, out int unusedParentRelationCount);

                    int i = R;
                    if (!TryGetOldModel(m, Trait.RowProperty_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowProperty_ZM, depth, row, TableX_ColumnX, null, RowPropertyList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowCompute_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowCompute_ZM, depth, row, Store_ComputeX, null, RowComputeList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowChildRelation_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_ZM, depth, row, TableX_ChildRelationX, null, RowChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowParentRelation_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_ZM, depth, row, TableX_ParentRelationX, null, RowParentRelationList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowDefaultProperty_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowDefaultProperty_ZM, depth, row, TableX_ColumnX, null, RowDefaultPropertyList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowUnusedChildRelation_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowUnusedChildRelation_ZM, depth, row, TableX_ChildRelationX, null, RowUnusedChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(m, Trait.RowUnusedParentRelation_ZM, oldModels, i))
                        m.ChildModels[i] = new ItemModel(m, Trait.RowUnusedParentRelation_ZM, depth, row, TableX_ParentRelationX, null, RowUnusedParentRelationList_X);
                }
            }
            else
            {
                m.ChildModels = null;
            }
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
                ModelParms = (m) => (null, m.ViewX.Name, 0, ModelType.Default),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.ViewX.Summary,
            };
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
                    var count = m.TableX.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, m.TableX.Name, count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var tbl = m.Item as TableX;
                    var col = TableX_NameProperty.GetChild(tbl);
                    var N = m.IsExpandedLeft ? tbl.Count : 0;

                    if (N > 0)
                    {
                        var items = tbl.ToArray;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var row = items[i];
                            if (!TryGetOldModel(m, Trait.Row_M, oldModels, i, row, tbl, col))
                                m.ChildModels[i] = new ItemModel(m, Trait.Row_M, depth, row, tbl, col, RowX_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void Insert(ItemModel model)
            {
                var tbl = model.Item as TableX;
                ItemCreated(new RowX(tbl));
            }
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
                    m.CanExpandLeft = true;

                    return (_localize(m.KindKey), m.Graph.GraphX.Name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.Graph.GraphX.Summary,

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ButtonCommands = (m, bc) =>
                {
                    bc.Add(new ModelCommand(this, m, Trait.ViewCommand, CreateSecondaryModelGraph));
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = (m.IsExpandedLeft) ? 5 : 0;
                    if (N > 0)
                    {
                        var item = m.Item as Graph;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var i = 0;
                        if (!TryGetOldModel(m, Trait.GraphNode_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphNode_ZM, depth, item, null, null, GraphNodeList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.GraphEdge_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphEdge_ZM, depth, item, null, null, GraphEdgeList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.GraphOpen_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphOpen_ZM, depth, item, null, null, GraphOpenList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.GraphRoot_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphRoot_ZM, depth, item, null, null, GraphRootList_X);

                        i++;
                        if (!TryGetOldModel(m, Trait.GraphLevel_ZM, oldModels, i, item))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphLevel_ZM, depth, item, null, null, GraphLevelList_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void CreateSecondaryModelGraph(ItemModel m)
            {
                var root = m.GetRootModel();
                root.UIRequestQueue.Enqueue(m.BuildViewRequest(ControlType.GraphDisplay, Trait.GraphRef_M, GraphRef_X));
            }
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
                    return (_localize(m.KindKey), m.Graph.GraphX.Name, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => m.Graph.GraphX.Summary,
            };
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
                    var count = m.Relation.ChildCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (_localize(m.KindKey), GetRelationName(m.RelationX), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var row1 = m.Item as RowX;
                    var rel = m.Aux1 as RelationX;

                    var N = (m.IsExpandedLeft) ? rel.ChildCount(row1) : 0;

                    if (N > 0)
                    {
                        var items = rel.GetChildren(row1);
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var row2 = items[i];
                            if (!TryGetOldModel(m, Trait.RowRelatedChild_M, oldModels, i, row2))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowRelatedChild_M, depth, row2, rel, row1, RowRelatedChild_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Relation.ParentCount(m.Item);

                    m.CanExpandLeft = (count > 0);
                    m.CanFilter = (count > 2);
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (_localize(m.KindKey), GetRelationName(m.RelationX), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var rel = m.Aux1 as RelationX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && rel.TryGetParents(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.RowRelatedParent_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowRelatedParent_M, depth, itm, rel, item, RowRelatedParent_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanDrag = true;
                    m.CanExpandLeft = true;

                    return (m.RowX.TableX.Name, GetRowName(m.RowX), m.RelationX.ChildCount(m.RowX), ModelType.Default);
                },

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

                Validate = (m) => RowX_VX(m),
            };

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

            void UnlinkRelatedChild(ItemModel m)
            {
                var key = m.Aux2;
                var rel = m.Aux1 as Relation;
                var item = m.Item;
                RemoveLink(rel, key, item);
            }
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

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

        #region 6AA RowRelatedParent  =========================================
        ModelAction RowRelatedParent_X;
        void Initialize_RowRelatedParent_X()
        {
            RowRelatedParent_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Relation.ParentCount(m.RowX);

                    m.CanDrag = true;
                    m.CanExpandLeft = count > 0;

                    return (m.RowX.TableX.Name, GetRowName(m.RowX), count, ModelType.Default);
                },

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

                Validate = (m) => RowX_VX(m),
            };

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
                ModelParms = (m) => (_localize(m.KindKey), $"{m.TableX.Name}: {m.ColumnX.Name}", 0, ModelType.Default),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                MenuCommands = (m, mc) =>
                {
                    mc.Add(new ModelCommand(this, m, Trait.RemoveCommand, UnlinkRelatedColumn));
                },
            };

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
                    GetColumnCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var row = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUsedColumns(row, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var col = items[i] as ColumnX;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, row, col))
                                m.ChildModels[i] = NewPropertyModel(m, depth, row, col);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetChildRelationCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUsedChildRelations(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i] as RelationX;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, item, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_M, depth, item, rel, null, RowChildRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetParentRelationCount(m.Item, out int count, out int _);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUsedParentRelations(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i] as RelationX;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, item, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_M, depth, item, rel, null, RowParentRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetColumnCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUnusedColumns(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var prop = items[i] as Property;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, item, prop))
                                m.ChildModels[i] = NewPropertyModel(m, depth, item, prop);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetChildRelationCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUnusedChildRelations(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i] as RelationX;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, item, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowChildRelation_M, depth, item, rel, null, RowChildRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetParentRelationCount(m.Item, out int _, out int count);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as RowX;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && TryGetUnusedParentRelations(item, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = items[i] as RelationX;

                            if (!TryGetOldModel(m, Trait.Empty, oldModels, i, item, rel))
                                m.ChildModels[i] = new ItemModel(m, Trait.RowParentRelation_M, depth, item, rel, null, RowParentRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = Store_ComputeX.ChildCount(m.Item.Owner);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    var sto = item.Owner;

                    var N = (m.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;
                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var list = Store_ComputeX.GetChildren(sto);
                        for (int i = 0; i < N; i++)
                        {
                            var itm = list[i];
                            if (!TryGetOldModel(m, Trait.TextProperty_M, oldModels, i, item, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.TextProperty_M, depth, item, itm, null, TextCompute_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryRootItem_M, depth, itm, seg, null, QueryRootItem_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryPathLink_VX(m),
            };
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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryPathLink_VX(m),
            };
        }
        private void QueryPathLink_VX(ItemModel model)
        {
            var seg = model.Query;

            if (seg.TryGetItems(out Item[] items))
            {
                var N = items.Length;
                var depth = (byte)(model.Depth + 1);
                var oldModels = model.ChildModels;
                model.ChildModels = new ItemModel[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathTail_M, depth, itm, seg, null, QueryPathTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryPathStep_M, depth, itm, seg, null, QueryPathStep_X);
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
            QueryGroupHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryGroupLink_VX(m),
            };
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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryGroupLink_VX(m),
            };
        }
        private void QueryGroupLink_VX(ItemModel model)
        {
            var seg = model.Query;
            var depth = (byte)(model.Depth + 1);
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
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupTail_M, depth, itm, seg, null, QueryGroupTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryGroupStep_M, depth, itm, seg, null, QueryGroupStep_X);
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
            QueryEgressHead_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryEgressLink_VX(m),
            };
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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;

                    return (_localize(m.KindKey), QueryXFilterName(m.Query.QueryX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) => QueryEgressLink_VX(m),
            };
        }
        private void QueryEgressLink_VX(ItemModel model)
        {
            var seg = model.Query;
            var depth = (byte)(model.Depth + 1);
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
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressTail_M, depth, itm, seg, null, QueryEgressTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ItemModel(model, Trait.QueryEgressStep_M, depth, itm, seg, null, QueryEgressStep_X);
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
            QueryRootItem_X = new ModelAction
            {
                ModelParms = (m) =>
                {
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return (m.RowX.TableX.Name, GetRowName(m.RowX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var itm = m.Item;
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Query[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            seg = items[i];
                            if (seg.IsGraphLink)
                            {
                                if (!TryGetOldModel(m, Trait.QueryRootLink_M, oldModels, i, itm, seg))
                                    m.ChildModels[i] = new ItemModel(m, Trait.QueryRootLink_M, depth, itm, seg, null, QueryRootLink_X);
                            }
                            else if (seg.IsPathHead)
                            {
                                if (!TryGetOldModel(m, Trait.QueryPathHead_M, oldModels, i, itm, seg))
                                    m.ChildModels[i] = new ItemModel(m, Trait.QueryPathHead_M, depth, itm, seg, null, QueryPathHead_X);
                            }
                            else if (seg.IsGroupHead)
                            {
                                if (!TryGetOldModel(m, Trait.QueryGroupHead_M, oldModels, i, itm, seg))
                                    m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupHead_M, depth, itm, seg, null, QueryGroupHead_X);
                            }
                            else if (seg.IsSegueHead)
                            {
                                if (!TryGetOldModel(m, Trait.QueryEgressHead_M, oldModels, i, itm, seg))
                                    m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressHead_M, depth, itm, seg, null, QueryEgressHead_X);
                            }
                            else
                                throw new Exception("Invalid Query");
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var itm = m.Item;
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Query[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            seg = items[i];
                            if (!TryGetOldModel(m, Trait.QueryPathLink_M, oldModels, i, itm, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryPathLink_M, depth, itm, seg, null, QueryPathLink_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var itm = m.Item;
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Query[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            seg = items[i];
                            if (!TryGetOldModel(m, Trait.QueryGroupLink_M, oldModels, i, itm, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryGroupLink_M, depth, itm, seg, null, QueryGroupLink_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var itm = m.Item;
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Query[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            seg = items[i];
                            if (!TryGetOldModel(m, Trait.QueryEgressLink_M, oldModels, i, itm, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryEgressLink_M, depth, itm, seg, null, QueryEgressLink_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Query.QueryCount(m.Item);

                    m.CanExpandLeft = count > 0;

                    return ($"{_localize(m.KindKey)} {m.RowX.TableX.Name}", GetRowName(m.RowX), count, ModelType.Default);
                },
            };
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
                    var count = m.GraphX.Count;

                    m.CanExpandLeft = count > 0;

                    return (m.GraphX.Trait.ToString(), m.GraphX.Name, count, ModelType.Default);
                },

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
                    Store tbl = null;

                    if (GraphX_QueryX.ChildCount(gx) == 0) return DropAction.None;

                    var items = GraphX_QueryX.GetChildren(gx);
                    foreach (var item in items)
                    {
                        if (item.IsQueryGraphRoot && Store_QueryX.TryGetParent(item, out tbl) && d.Item.Owner == tbl) break;
                    }
                    if (tbl == null) return DropAction.None;

                    foreach (var tg in gx.ToArray)
                    {
                        if (tg.RootItem == d.Item) return DropAction.None;
                    }

                    if (doDrop)
                    {
                        CreateGraph(gx, out Graph g, d.Item);

                        m.IsExpandedLeft = true;
                        MajorDelta += 1;

                        var root = m.GetRootModel();
                        root.UIRequestQueue.Enqueue(UIRequest.CreateView(root, ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true));
                    }
                    return DropAction.Copy;
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var gx = m.Item as GraphX;
                    var N = m.IsExpandedLeft ? gx.Count : 0;

                    if (N > 0)
                    {
                        var items = gx.ToArray;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var g = items[i] as Graph;
                            if (!TryGetOldModel(m, Trait.Graph_M, oldModels, i, g))
                                m.ChildModels[i] = new ItemModel(m, Trait.Graph_M, depth, g, null, null, Graph_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
        }

        void CreateGraph(ItemModel m)
        {
            CreateGraph(m.GraphX, out Graph g);

            m.IsExpandedLeft = true;
            MajorDelta += 1;

            var root = m.GetRootModel();
            root.UIRequestQueue.Enqueue(UIRequest.CreateView(root, ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true));
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
                    var count = m.Graph.NodeCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as Graph;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var items = item.Nodes;
                    var N = items.Count;
                    if (m.IsExpandedLeft && N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.GraphNode_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphNode_M, depth, itm, null, null, GraphNode_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Graph.EdgeCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as Graph;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var items = item.Edges;
                    var N = items.Count;
                    if (m.IsExpandedLeft && N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.GraphEdge_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphEdge_M, depth, itm, null, null, GraphEdge_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Graph.QueryCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
            {
                var item = m.Item as Graph;
                var depth = (byte)(m.Depth + 1);
                var oldModels = m.ChildModels;

                Query[] items = item.Forest;
                var N = item.QueryCount;
                if (m.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                    for (int i = 0; i < N; i++)
                    {
                        var seg = items[i];
                        var tbl = seg.Item;
                        if (!TryGetOldModel(m, Trait.GraphRoot_M, oldModels, i, tbl, seg))
                            m.ChildModels[i] = new ItemModel(m, Trait.GraphRoot_M, depth, tbl, seg, null, GraphRoot_X);
                    }
                }
                else
                {
                    m.ChildModels = null;
                }
            }
            };
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
                    var count = m.Graph.Levels.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as Graph;
                    var items = item.Levels;
                    var N = m.IsExpandedLeft ? items.Count : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as Level;
                            if (!TryGetOldModel(m, Trait.GraphLevel_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphLevel_M, depth, itm, null, null, GraphLevel_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Level.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (_localize(m.KindKey), m.Level.Name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item as Level;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var items = item.Paths;
                    var N = (m.IsExpandedLeft) ? items.Count : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSortAscending || m.IsSortDescending || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.GraphPath_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphPath_M, depth, itm, null, null, GraphPath_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Path.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (GetPathKind(m.Path), GetPathName(m.Path), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var path = m.Item as Path;
                    var items = path.Items;
                    var N = (m.IsExpandedLeft) ? path.Count : 0;
                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as Path;
                            if (!TryGetOldModel(m, Trait.GraphPath_M, oldModels, i, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphPath_M, depth, itm, null, null, GraphPath_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
                    var count = m.Query.ItemCount;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, m.TableX.Name, count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var tbl = m.Item;
                    var seg = m.Query;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    Item[] items = null;
                    var N = (m.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
                    if (N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(m, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                                m.ChildModels[i] = new ItemModel(m, Trait.QueryRootItem_M, depth, itm, seg, null, QueryRootItem_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Node.Graph.Node_Edges.TryGetValue(m.Node, out List<Edge> edges) ? edges.Count : 0;

                    m.CanExpandRight = true;
                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (_localize(m.KindKey), GetIdentity(m.Node.Item, IdentityStyle.Double), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var depth = (byte)(m.Depth + 1);
                    List<Edge> edges = null;
                    var nd = m.Item as Node;
                    var g = nd.Graph;
                    var sp = new Property[] { _nodeCenterXYProperty, _nodeSizeWHProperty, _nodeOrientationProperty, _nodeFlipRotateProperty, _nodeLabelingProperty, _nodeResizingProperty, _nodeBarWidthProperty };
                    var R = m.IsExpandedRight ? sp.Length : 0;
                    var L = (m.IsExpandedLeft && g.Node_Edges.TryGetValue(nd, out edges)) ? edges.Count : 0;
                    var N = L + R;

                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        if (R > 0)
                        {
                            AddProperyModels(m, oldModels, sp);
                        }
                        if (L > 0)
                        {
                            for (int i = R, j = 0; i < N; i++, j++)
                            {
                                var edge = edges[j];
                                if (!TryGetOldModel(m, Trait.GraphEdge_M, oldModels, i, edge))
                                    m.ChildModels[i] = new ItemModel(m, Trait.GraphEdge_M, depth, edge, null, null, GraphEdge_X);
                            }
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var edge = m.Edge;

                    m.CanExpandRight = true;
                    

                    return (_localize(m.KindKey), GetHeadTailName(edge.Node1.Item, edge.Node2.Item), 0, ModelType.Default);
                },
                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var sp = new Property[] { _edgeFace1Property, _edgeFace2Property, _edgeGnarl1Property, _edgeGnarl2Property, _edgeConnect1Property, _edgeConnect2Property };
                    var N = m.IsExpandedRight ? sp.Length : 0;

                    if (N > 0)
                    {
                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        AddProperyModels(m, oldModels, sp);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = m.Graph.OpenQuerys.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var g = m.Item as Graph;
                    var depth = (byte)(m.Depth + 1);
                    var oldModels = m.ChildModels;

                    var N = g.OpenQuerys.Count;
                    if (m.IsExpandedLeft && N > 0)
                    {
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var h = g.OpenQuerys[i].Query1;
                            var t = g.OpenQuerys[i].Query2;
                            if (!TryGetOldModel(m, Trait.GraphOpen_M, oldModels, i, g, h, t))
                                m.ChildModels[i] = new ItemModel(m, Trait.GraphOpen_M, depth, g, h, t, GraphOpen_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    GetHeadTail(m.Query.QueryX, out Store head, out Store tail);
                    var name = $"{GetIdentity(m.Item, IdentityStyle.Double)}  -->  {GetIdentity(tail, IdentityStyle.Single)}: <?>";

                    return (GetIdentity(m.Item, IdentityStyle.Double), name, 0, ModelType.Default);
                },
            };
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
                    var count = 0;
                    foreach (var sto in _primeStores) { if (Store_ComputeX.HasChildLink(sto)) count += 1; }

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(GetNameKey(Trait.PrimeCompute_M)), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(GetSummaryKey(Trait.PrimeCompute_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelDescription = (m) => _localize(GetDescriptionKey(Trait.PrimeCompute_M)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var childList = new List<Store>();
                    foreach (var sto in _primeStores) { if (Store_ComputeX.HasChildLink(sto)) childList.Add(sto); }
                    var N = childList.Count;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var list = new List<Store>();
                        foreach (var sto in _primeStores) { if (Store_ComputeX.HasChildLink(sto)) list.Add(sto); }

                        for (int i = 0; i < N; i++)
                        {
                            var sto = childList[i];
                            if (!TryGetOldModel(m, Trait.ComputeStore_M, oldModels, i, sto))
                                m.ChildModels[i] = new ItemModel(m, Trait.ComputeStore_M, depth, sto, null, null, ComputeStore_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = Store_ComputeX.ChildCount(st);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, GetIdentity(st, IdentityStyle.Single), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Store, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var st = m.Store;
                    var N = (m.IsExpandedLeft) ? Store_ComputeX.ChildCount(st) : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var list = Store_ComputeX.GetChildren(st);
                        for (int i = 0; i < N; i++)
                        {
                            var itm = list[i];
                            if (!TryGetOldModel(m, Trait.TextProperty_M, oldModels, i, st, itm))
                                m.ChildModels[i] = new ItemModel(m, Trait.TextProperty_M, depth, st, itm, null, TextCompute_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    m.CanExpandLeft = true;

                    return (null, _localize(GetNameKey(Trait.InternalStore_ZM)), 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(GetSummaryKey(Trait.InternalStore_ZM)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =


                ModelDescription = (m) => _localize(GetDescriptionKey(Trait.InternalStore_ZM)),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var N = (m.IsExpandedLeft) ? 11 : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        int i = 0;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _viewXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _viewXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _enumXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _enumXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _tableXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _tableXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _graphXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _graphXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _queryXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _queryXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _symbolXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _symbolXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _columnXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _columnXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _relationXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _relationXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _computeXStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _computeXStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _relationStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _relationStore, null, null, InternalStore_X);

                        i += 1;
                        if (!TryGetOldModel(m, Trait.InternalStore_M, oldModels, i, _propertyStore))
                            m.ChildModels[i] = new ItemModel(m, Trait.InternalStore_M, depth, _propertyStore, null, null, InternalStore_X);
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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
                    var count = st.Count;

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(st.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var st = m.Item as Store;
                    var N = (m.IsExpandedLeft) ? st.Count : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var list = st.GetItems();
                        for (int i = 0; i < N; i++)
                        {
                            var item = list[i];
                            if (!TryGetOldModel(m, Trait.StoreItem_M, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreItem_M, depth, item, null, null, StoreItem_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (GetIdentity(item, IdentityStyle.Kind), GetIdentity(item, IdentityStyle.StoreItem), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var item = m.Item;
                    var (hasItems, hasLinks, hasChildRels, hasParentRels, count) = GetItemParms(item);

                    if (m.IsExpandedLeft && count > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != count) m.ChildModels = new ItemModel[count];

                        int i = -1;
                        if (hasItems)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreItemItem_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreItemItem_ZM, depth, item, null, null, StoreItemItemZ_X);
                        }
                        if (hasLinks)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreRelationLink_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreRelationLink_ZM, depth, item, null, null, StoreRelationLinkZ_X);
                        }
                        if (hasChildRels)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreChildRelation_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_ZM, depth, item, null, null, StoreChildRelationZ_X);
                        }
                        if (hasParentRels)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreParentRelation_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_ZM, depth, item, null, null, StoreParentRelationZ_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var st = m.Store;
                    var N = (m.IsExpandedLeft) ? st.Count : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        var list = st.GetItems();
                        for (int i = 0; i < N; i++)
                        {
                            var item = list[i];
                            if (!TryGetOldModel(m, Trait.StoreItemItem_M, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreItemItem_M, depth, item, null, null, StoreItemItem_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var rel = m.Relation;
                    var oldModels = m.ChildModels;
                    m.ChildModels = null;

                    Item[] parents = null;
                    Item[] children = null;
                    var N = (m.IsExpandedLeft) ? rel.GetLinks(out parents, out children) : 0;

                    if (N > 0)
                    {
                        var depth = (byte)(m.Depth + 1);
                        m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var parent = parents[i];
                            var child = children[i];
                            if (!TryGetOldModel(m, Trait.StoreRelationLink_M, oldModels, i, rel, parent, child))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreRelationLink_M, depth, rel, parent, child, StoreRelationLink_X);
                        }
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    if (m.IsExpandedLeft && TryGetChildRelations(item, out Relation[] relations, SubsetType.Used))
                    {
                        var N = relations.Length;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = relations[i];
                            if (!TryGetOldModel(m, Trait.StoreChildRelation_M, oldModels, i, rel, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_M, depth, rel, item, null, StoreChildRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
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

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (null, _localize(m.NameKey), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => _localize(m.SummaryKey),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var item = m.Item;
                    if (m.IsExpandedLeft && TryGetParentRelations(item, out Relation[] relations, SubsetType.Used))
                    {
                        var N = relations.Length;
                        var depth = (byte)(m.Depth + 1);

                        var oldModels = m.ChildModels;
                        if (oldModels == null || m.IsSorted || oldModels.Length != N) m.ChildModels = new ItemModel[N];

                        for (int i = 0; i < N; i++)
                        {
                            var rel = relations[i];
                            if (!TryGetOldModel(m, Trait.StoreParentRelation_M, oldModels, i, rel, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_M, depth, rel, item, null, StoreParentRelation_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };
        }
        #endregion

        #region 7F8 StoreItemItem  ============================================
        ModelAction StoreItemItem_X;
        void Initialize_StoreItemItem_X()
        {
            StoreItemItem_X = new ModelAction
            {
                ModelParms = (m) => (_localize(m.Item.KindKey), GetIdentity(m.Item, IdentityStyle.Double), 0, ModelType.Default),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Item, IdentityStyle.Summary),
            };
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
                    var parent = m.Aux1;
                    var child = m.Aux2;
                    var name = $"({GetIdentity(parent, IdentityStyle.Double)}) --> ({GetIdentity(child, IdentityStyle.Double)})";

                    return (_localize(m.KindKey), null, 0, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),
            };
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
                    var rel = m.Relation;
                    var count = rel.ChildCount(m.Aux1);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (GetKind(rel.Trait), GetIdentity(rel, IdentityStyle.Single), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var rel = m.Item as Relation;
                    var oldModels = m.ChildModels;
                    m.ChildModels = null;

                    if (m.IsExpandedLeft)
                    {
                        if (rel.TryGetChildren(m.Aux1, out Item[] items))
                        {
                            var N = items.Length;
                            var depth = (byte)(m.Depth + 1);
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = items[i];
                                if (!TryGetOldModel(m, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.StoreRelatedItem_M, depth, itm, null, null, StoreRelatedItem_X);
                            }
                        }
                    }
                }
            };
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
                    var rel = m.Item as Relation;
                    var count = rel.ParentCount(m.Aux1);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (GetKind(rel.Trait), GetIdentity(rel, IdentityStyle.Single), count, ModelType.Default);
                },

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                ModelSummary = (m) => GetIdentity(m.Relation, IdentityStyle.Summary),

                //= = = = = = = = = = = = = = = = = = = = = = = = = = = =

                Validate = (m) =>
                {
                    var rel = m.Item as Relation;
                    var oldModels = m.ChildModels;
                    m.ChildModels = null;

                    if (m.IsExpandedLeft)
                    {
                        if (rel.TryGetParents(m.Aux1, out Item[] items))
                        {
                            var N = items.Length;
                            var depth = (byte)(m.Depth + 1);
                            m.ChildModels = new ItemModel[N];

                            for (int i = 0; i < N; i++)
                            {
                                var itm = items[i];
                                if (!TryGetOldModel(m, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                    m.ChildModels[i] = new ItemModel(m, Trait.StoreRelatedItem_M, depth, itm, null, null, StoreRelatedItem_X);
                            }
                        }
                    }
                }
            };
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
                    var item = m.Item;
                    var (hasChildRels, hasParentRels, count) = GetItemParms(item);

                    m.CanExpandLeft = count > 0;
                    m.CanFilter = count > 2;
                    m.CanSort = (m.IsExpandedLeft && count > 1);

                    return (GetIdentity(item, IdentityStyle.Kind), GetIdentity(item, IdentityStyle.StoreItem), count, ModelType.Default);
                },

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

                Validate = (m) =>
                {
                    var item = m.Item;
                    var (hasChildRels, hasParentRels, count) = GetItemParms(item);
                    var oldModels = m.ChildModels;
                    m.ChildModels = null;

                    if (m.IsExpandedLeft && count > 0)
                    {
                        var depth = (byte)(m.Depth + 1);
                        m.ChildModels = new ItemModel[count];

                        int i = -1;
                        if (hasChildRels)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreChildRelation_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreChildRelation_ZM, depth, item, null, null, StoreChildRelationZ_X);
                        }
                        if (hasParentRels)
                        {
                            i++;
                            if (!TryGetOldModel(m, Trait.StoreParentRelation_ZM, oldModels, i, item))
                                m.ChildModels[i] = new ItemModel(m, Trait.StoreParentRelation_ZM, depth, item, null, null, StoreParentRelationZ_X);
                        }
                    }
                    else
                    {
                        m.ChildModels = null;
                    }
                }
            };

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
