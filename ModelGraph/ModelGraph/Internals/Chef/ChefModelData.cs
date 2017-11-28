using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Storage;
using ModelGraph.Helpers;

namespace ModelGraph.Internals
{/*

 */
    public partial class Chef
    {
        #region TryGetOldModel  ===============================================
        /// <summary>
        /// Try to find and reuse an existing model matching the callers parameters
        /// </summary>
        private static bool TryGetOldModel(ModelTree model, Trait trait, ModelTree[] oldModels, int index, Item itm1 = null, Item itm2 = null, Item itm3 = null)
        {
            if (oldModels == null || oldModels.Length == 0) return false;

            var N = oldModels.Length;
            for (int i = 0; i < N; i++)
            {
                var mod = oldModels[i];
                if (mod == null) continue;
                if (trait != Trait.Empty && trait != mod.Trait) continue;
                if (itm1 != null && itm1 != mod.Item1) continue;
                if (itm2 != null && itm2 != mod.Item2) continue;
                if (itm3 != null && itm3 != mod.Item3) continue;
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
            private (ModelTree[] Models, int Index)[] _stack;
            internal bool IsNotEmpty => (_count > 0);

            #region Constructor  ==============================================
            private const int minLength = 25;
            internal TreeModelStack(int capacity = minLength)
            {
                var length = (capacity < minLength) ? minLength : capacity;
                _stack = new(ModelTree[] Models, int Index)[length];
            }
            #endregion

            #region PushChildren  =============================================
            /// <summary>
            /// Push the ChildModels (if any)
            /// </summary>
            internal int PushChildren(ModelTree model)
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
                        _stack = new(ModelTree[] Models, int Index)[_count  * 2];
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
            internal ModelTree PopNext()
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
        public bool ValidateModelTree(ModelRoot root, ChangeType change = ChangeType.NoChange)
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
                var newModels = root.ViewModels = new ModelTree[newLen]; // create array of the correct size

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
                        ModelTree FindSelectParent()
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
                    int IndexOf(ModelTree model, int index1, int index2)
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
                    int OldDelta(ModelTree model, int index1, int index2)
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
            void ValidateModel(ModelTree model)
            {
                if (model.Item1.AutoExpandRight)
                {
                    model.IsExpandedRight = true;
                    model.Item1.AutoExpandRight = false;
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
        private void CheckFilterSort(ModelTree model)
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
                children = new ModelTree[count];
                for (int i = 0; i < count; i++)
                {
                    children[i] = items[i].Model;
                }
                model.ChildModels = children;
            }
        }
        private string GetFilterSortName(ModelTree model)
        {
            var refRoot = _selfReferenceModel;
            refRoot.GetModelItemData(model);
            var kind = string.IsNullOrEmpty(refRoot.ModelKind) ? " " : refRoot.ModelKind;
            var name = string.IsNullOrEmpty(refRoot.ModelName) ? " " : refRoot.ModelName;
            return $"{kind} {name}";
        }
        private bool TryGetFilter(ModelTree model, out Regex filter)
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
            internal ModelTree Model;

            internal FilterSortItem(ModelTree model, string name)
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
        private void AddProperyModels(ModelTree model, ModelTree[] oldModels, ColumnX[] cols)
        {
            var item = model.Item1;
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
        private void AddProperyModels(ModelTree model, ModelTree[] oldModels, Property[] props)
        {
            var item = model.Item1;
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
        private ModelTree NewPropertyModel(ModelTree model, byte level, Item item, ColumnX col)
        {
            if (EnumX_ColumnX.TryGetParent(col, out EnumX enu))
                return new ModelTree(model, Trait.ComboProperty_M, level, item, col, enu, ComboColomn_X);
            else if (col.Value.ValType == ValType.Bool)
                return new ModelTree(model, Trait.CheckProperty_M, level, item, col, null, CheckColumn_X);
            else
                return new ModelTree(model, Trait.TextProperty_M, level, item, col, null, TextColumn_M);
        }
        private ModelTree NewPropertyModel(ModelTree model, byte level, Item item, ComputeX cx)
        {
            if (EnumX_ColumnX.TryGetParent(cx, out EnumX enu))
                return new ModelTree(model, Trait.ComboProperty_M, level, item, cx, enu, ComboProperty_X);
            else if (cx.Value.ValType == ValType.Bool)
                return new ModelTree(model, Trait.CheckProperty_M, level, item, cx, null, CheckProperty_X);
            else
                return new ModelTree(model, Trait.TextProperty_M, level, item, cx, null, TextCompute_X);
        }
        private ModelTree NewPropertyModel(ModelTree model, byte level, Item item, Property prop)
        {
            if (Property_Enum.TryGetValue(prop, out EnumZ enu))
                return new ModelTree(model, Trait.ComboProperty_M, level, item, prop, enu, ComboProperty_X);
            else if (prop.Value.ValType == ValType.Bool)
                return new ModelTree(model, Trait.CheckProperty_M, level, item, prop, null, CheckProperty_X);
            else
                return new ModelTree(model, Trait.TextProperty_M, level, item, prop, null, TextProperty_X);
        }
        #endregion

        #region GetAppTabName  ================================================
        internal string GetAppTabName(ModelRoot root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return GetName(Trait.AppRootModelTab);

                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                case ControlType.GraphDisplay:
                case ControlType.SymbolEditor:
                    return GetShortStorageFileName();
            }
            return BlankName;
        }
        #endregion

        #region GetAppTabSummary  =============================================
        internal string GetAppTabSummary(ModelRoot root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return null;

                case ControlType.PrimaryTree:
                    if (_modelingFile == null)
                        return GetNameKey(Trait.NewModel).GetLocalized();

                    var name = _modelingFile.Name;
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
        internal string GetAppTitleName(ModelRoot root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return $"{GetName(Trait.ModelGraphTitle)} - {GetName(Trait.AppRootModelTab)}";

                case ControlType.PrimaryTree:
                    return GetShortStorageFileName();

                case ControlType.PartialTree:
                    return $"{GetShortStorageFileName()} - {GetName(root.Trait)}";

                case ControlType.GraphDisplay:
                    var g = root.Item1 as Graph;
                    var gx = g.GraphX;
                    if (g.RootItem == null)
                        return $"{gx.Name}";
                    else
                        return $"{gx.Name} - {GetIdentity(g.RootItem, IdentityStyle.Double)}";

                case ControlType.SymbolEditor:

                    return $"{GetName(Trait.EditSymbol)} : {GetIdentity(root.Item1, IdentityStyle.Single)}";
            }
            return BlankName;
        }
        #endregion

        #region GetAppTitleSummary  ===========================================
        internal string GetAppTitleSummary(ModelRoot root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    return null;

                case ControlType.PrimaryTree:
                    if (_modelingFile == null)
                        return GetNameKey(Trait.NewModel).GetLocalized();

                    var name = _modelingFile.Name;
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
        internal void GetAppCommands(ModelRoot root)
        {
            switch (root.ControlType)
            {
                case ControlType.AppRootChef:
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.NewCommand, AppRootNewModel));
                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.OpenCommand, AppRootOpenModel));
                    break;

                case ControlType.PrimaryTree:
                    if (root.Chef.ModelingFile == null)
                        root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.SaveAsCommand, AppRootSaveAsModel));
                    else
                        root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.SaveCommand, AppRootSaveModel));

                    root.AppButtonCommands.Add(new ModelCommand(this, root, Trait.CloseCommand, AppRootCloseModel));
                    if (root.Chef.ModelingFile != null)
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
        private void AppRootNewModel(ModelTree model)
        {
            var root = model as ModelRoot;
            var rootChef = root.Chef;
            var dataChef = new Chef(rootChef, null);

            root.UIRequest = UIRequest.CreateNewView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_M);
        }
        public void AppRootOpenModel(ModelTree model, Object parm1)
        {
            var file = parm1 as StorageFile;
            var root = model as ModelRoot;
            var rootChef = root.Chef;
            var dataChef = new Chef(rootChef, file);

            root.UIRequest = UIRequest.CreateNewView(ControlType.PrimaryTree, Trait.DataChef_M, dataChef, dataChef, null, null, dataChef.DataChef_M);
        }
        internal void AppRootSaveAsModel(ModelTree model, Object parm1)
        {
            var file = parm1 as StorageFile;
            var root = model as ModelRoot;
            var dataChef = root.Chef;

            MajorDelta += 1;
            dataChef.SaveAsStorageFile(file);
        }
        private void AppRootSaveModel(ModelTree model)
        {
            var root = model as ModelRoot;
            var dataChef = root.Chef;

            MajorDelta += 1;
            dataChef.SaveStorageFile();
        }
        private void AppRootCloseModel(ModelTree model)
        {
            var root = model as ModelRoot;
            root.UIRequest = UIRequest.CloseModel(root);
        }
        private void AppRootReloadModel(ModelTree model)
        {
            var root = model as ModelRoot;
            root.UIRequest = UIRequest.ReloadModel(root);
        }
        private void AppSaveSymbol(ModelTree model)
        {
            var root = model as ModelRoot;
            root.UIRequest = UIRequest.SaveModel(root);
        }
        private void AppReloadSymbol(ModelTree model)
        {
            var root = model as ModelRoot;
            root.UIRequest = UIRequest.ReloadModel(root);
        }
        #endregion



        #region 611 RootChef_M  ===============================================
        internal void RootChef_M(ModelTree model, ModelRoot root = null)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the model's child models
            {
                var chef = model.Item1 as Chef;
                var N = chef.Count;

                if (N > 0)
                {
                    var items = chef.Items;
                    var oldModels = model.ChildModels;
                    model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Chef;
                        if (!TryGetOldModel(model, Trait.MockChef_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.MockChef_M, 0, itm, null, null, MockChef_M);
                    }
                }
                if (N == 0) model.ChildModels = null;
                
            }
        }
        #endregion

        #region 612 DataChef_M  ===============================================
        internal void DataChef_M(ModelTree model, ModelRoot root)
        {
            var chef = model.Item1 as Chef;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:
                        
                        root.ModelName = model.NameKey.GetLocalized();

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the model's child models
            {
                var item = model.Item1;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = 4;

                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                var i = 0;
                if (!TryGetOldModel(model, Trait.ErrorRoot_M, oldModels, i))
                    model.ChildModels[i] = new ModelTree(model, Trait.ErrorRoot_M, level, _errorStore, null, null, ErrorRoot_X);

                i++;
                if (!TryGetOldModel(model, Trait.ChangeRoot_M, oldModels, i))
                    model.ChildModels[i] = new ModelTree(model, Trait.ChangeRoot_M, level, _changeRoot, null, null, ChangeRoot_X);

                i++;
                if (!TryGetOldModel(model, Trait.MetadataRoot_M, oldModels, i))
                    model.ChildModels[i] = new ModelTree(model, Trait.MetadataRoot_M, level, item, null, null, MetadataRoot_X);

                i++;
                if (!TryGetOldModel(model, Trait.ModelingRoot_M, oldModels, i))
                    model.ChildModels[i] = new ModelTree(model, Trait.ModelingRoot_M, level, item, null, null, ModelingRoot_X);
            }
        }

        #endregion

        #region 613 MockChef_M  ===============================================
        internal void MockChef_M(ModelTree model, ModelRoot root = null)
        {
            var chef = model.Item1 as Chef;
            if (root != null)
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = chef.GetLongStorageFileName();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = chef.GetShortStorageFileName();
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreatePrimaryModelTree));
                        break;
                }
            }
        }
        private void CreatePrimaryModelTree(ModelTree model)
        {
            var chef = model.Item1 as Chef;
            var root = model.GetRootModel();
            root.UIRequest = UIRequest.CreateNewView(ControlType.PrimaryTree, Trait.DataChef_M, chef, chef, null, null, chef.DataChef_M);
        }
        #endregion

        #region 614 TextColumn_M  =============================================
        internal void TextColumn_M(ModelTree model, ModelRoot root)
        {
            var itm = model.Item1;
            var col = model.Item2 as ColumnX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = col.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = col.Name;
                        root.ModelValue = col.Value.GetString(itm);
                        if (root.ModelValue == null) root.ModelValue = string.Empty;
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = col.Description;
                        break;
                }
            }
        }
        #endregion

        #region 615 CheckColumn  ==============================================
        internal void CheckColumn_X(ModelTree model, ModelRoot root)
        {
            var itm = model.Item1;
            var col = model.Item2 as ColumnX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = col.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = col.Name;
                        root.ModelIsChecked = col.Value.GetBool(itm);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = col.Description;
                        break;
                }
            }
        }
        #endregion

        #region 616 ComboColumn  ==============================================
        internal void ComboColomn_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var itm = model.Item1;
                var col = model.Item2 as ColumnX;
                var enu = model.Item3 as EnumX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = col.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = col.Name;
                        root.ModelValueList = GetEnumDisplayValues(enu);
                        root.ValueIndex = GetComboSelectedIndex(itm, col, enu);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = col.Description;
                        break;
                }
            }
        }
        //=====================================================================
        private string[] GetEnumDisplayValues(EnumX e)
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
        private string[] GetEnumActualValues(EnumX e)
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
        private int GetComboSelectedIndex(Item itm, Property col, EnumX enu)
        {
            var value = col.Value.GetString(itm);
            var values = GetEnumActualValues(enu);
            var len = (values == null) ? 0 : values.Length;
            for (int i = 0; i < len; i++) { if (value == values[i]) return i; }
            return -1;
        }
        #endregion

        #region 617 TextProperty  =============================================
        internal void TextProperty_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var itm = model.Item1;
                var pro = model.Item2 as Property;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = pro.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        if (pro.HasItemName)
                            root.ModelName = $"{pro.GetItemName(itm)} {pro.NameKey.GetLocalized()}";
                        else
                            root.ModelName = pro.NameKey.GetLocalized();

                        root.ModelValue = pro.Value.GetString(itm);
                        if (root.ModelValue == null) root.ModelValue = string.Empty;
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = pro.DescriptionKey.GetLocalized();
                        break;
                }
            }
        }
        #endregion

        #region 618 CheckProperty  ============================================
        internal void CheckProperty_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var itm = model.Item1;
                var pro = model.Item2 as Property;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = pro.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = pro.NameKey.GetLocalized();
                        root.ModelIsChecked = pro.Value.GetBool(itm);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = pro.DescriptionKey.GetLocalized();
                        break;
                }
            }
        }
        #endregion

        #region 619 ComboProperty  ============================================
        internal void ComboProperty_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var itm = model.Item1;
                var pro = model.Item2 as Property;
                var enu = model.Item3 as EnumZ;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = pro.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = pro.NameKey.GetLocalized();
                        root.ModelValueList = GetEnumDisplayValues(enu);
                        root.ValueIndex = GetComboSelectedIndex(itm, pro, enu);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = pro.DescriptionKey.GetLocalized();
                        break;
                }
            }
        }
        //=====================================================================
        private string[] GetEnumDisplayValues(EnumZ e)
        {
            string[] values = null;
            if (e != null && e.IsValid)
            {
                var items = e.Items;
                var count = e.Count;
                values = new string[count];

                for (int i = 0; i < count; i++)
                {
                    var p = items[i];
                    values[i] = p.NameKey.GetLocalized();
                }
            }
            return values;
        }
        //=====================================================================
        private string GetEnumDisplayValue(EnumZ e, int index)
        {
            string value = InvalidItem;
            if (e != null && e.IsValid)
            {
                var items = e.Items;
                var count = e.Count;
                if (index >= 0 && index < count)
                {
                    var p = items[index];
                    value = p.NameKey.GetLocalized();
                }
            }
            return value;
        }
        //=====================================================================
        private int GetComboSelectedIndex(Item itm, Property pro, EnumZ enu)
        {
            //var value = pro.Value.GetValue(itm);
            var values = GetEnumDisplayValues(enu);
            var len = (values == null) ? 0 : values.Length;
            //for (int i = 0; i < len; i++) { if (value == values[i]) return i; }
            return 0;
        }
        #endregion

        #region 61A TextCompute  ==============================================
        internal void TextCompute_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1;
                var cd = model.Item2 as ComputeX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = cd.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = cd.Name;
                        root.ModelValue = cd.Value.GetString(item);
                        if (root.ModelValue == null) root.ModelValue = string.Empty;
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = cd.Description;
                        break;
                }
            }
        }
        #endregion



        #region 621 ErrorRoot  ================================================
        internal void ErrorRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var store = model.Item1 as StoreOf<Error>;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = store.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else
            {
                var N = model.IsExpandedLeft ? _errorStore.Count : 0;

                if (N > 0)
                {
                    var items = _errorStore.Items;
                    var item = model.Item1;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Error;
                        if (!TryGetOldModel(model, Trait.ErrorType_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.ErrorType_M, level, itm);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        #endregion

        #region 622 ChangeRoot  ===============================================
        internal void ChangeRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var chg = model.Item1 as ChangeRoot;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        if (model.IsExpandedLeft)
                        {
                            _changeRootInfoItem = null;
                            _changeRootInfoText = string.Empty;
                        }
                        root.ModelInfo = _changeRootInfoText;
                        root.ModelCount = chg.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        if (_changeRoot.Count > 0 && model.IsExpandedLeft == false)
                            root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ExpandAllCommand, ExpandAllChangeSets));
                        break;
                }
            }
            else
            {
                var oldModels = model.ChildModels;
                model.ChildModels = null;

                if (model.IsExpandedLeft)
                {
                    var N = _changeRoot.Count;

                    if (N > 0)
                    {
                        var items = new List<ChangeSet>(_changeRoot.Items);
                        items.Reverse();
                        var level = (byte)(model.Level + 1);
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i] as ChangeSet;
                            if (!TryGetOldModel(model, Trait.ChangeSet_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.ChangeSet_M, level, itm, null, null, ChangeSet_X);
                        }
                    }
                }
            }
        }
        #endregion

        #region 623 MetadataRoot  =============================================
        internal void MetadataRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreateSecondaryMetadataTree));
                        break;
                }
            }
            else
            {
                var N = (model.IsExpandedLeft) ? 5 : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var i = 0;
                    if (!TryGetOldModel(model, Trait.ViewXView_ZM, oldModels, i, _viewXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.ViewXView_ZM, level, _viewXStore, null, null, ViewXView_ZM);

                    i++;
                    if (!TryGetOldModel(model, Trait.EnumX_ZM, oldModels, i, _enumZStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.EnumX_ZM, level, _enumZStore, null, null, EnumXList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.TableX_ZM, oldModels, i, _tableXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.TableX_ZM, level, _tableXStore, null, null, TableXList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphX_ZM, oldModels, i, _graphXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphX_ZM, level, _graphXStore, null, null, GraphXList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.InternalStore_ZM, oldModels, i, this))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_ZM, level, this, null, null, InternalStoreZ_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateSecondaryMetadataTree(ModelTree model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.PartialTree);
        }
        #endregion

        #region 624 ModelingRoot  =============================================
        internal void ModelingRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.ViewCommand, CreateSecondaryModelingTree));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = (model.IsExpandedLeft) ? 4 : 0;

                if (N > 0)
                {
                    var item = model.Item1;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var i = 0;
                    if (!TryGetOldModel(model, Trait.ViewView_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.ViewView_ZM, level, item, null, null, ViewView_ZM);

                    i++;
                    if (!TryGetOldModel(model, Trait.Table_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.Table_ZM, level, item, null, null, TableList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.Graph_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.Graph_ZM, level, item, null, null, GraphList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.PrimeCompute_M, oldModels, i, this))
                        model.ChildModels[i] = new ModelTree(model, Trait.PrimeCompute_M, level, this, null, null, PrimeCompute_X);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private void CreateSecondaryModelingTree(ModelTree model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.PartialTree);
        }
        #endregion

        #region 625 MetaRelationList  =========================================
        internal void MetaRelationList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                int N = (model.IsExpandedLeft) ? 2 : 0;

                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var i = 0;
                    if (!TryGetOldModel(model, Trait.NameColumnRelation_M, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.NameColumnRelation_M, level, item, TableX_NameProperty, null, NameColumnRelation_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.SummaryColumnRelation_M, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.SummaryColumnRelation_M, level, item, TableX_SummaryProperty, null, SummaryColumnRelation_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 626 ErrorType  ================================================
        internal void ErrorType_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as Error;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = item.NameKey.GetLocalized();
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:
                        break;
                }
            }
            else  // validate the list of child models
            {
                var error = model.Item1 as Error;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = (model.IsExpandedLeft) ? error.Count : 0;
                if (N > 0)
                {
                    if (oldModels == null || oldModels.Length != N)
                    {
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            model.ChildModels[i] = new ModelTree(model, Trait.ErrorText_M, level, error);
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
        internal void ErrorText_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:
                        break;

                    case ModelAction.PointerOver:
                        break;

                    case ModelAction.ModelRefresh:

                        var err = model.Item1 as Error;
                        var inx = model.ParentModel.GetChildlIndex(model);
                        if (inx < 0 || err.Count <= inx)
                            root.ModelName = InvalidItem;
                        else
                            root.ModelName = err.Errors[inx];
                        break;

                    case ModelAction.ModelSelect:
                        break;
                }
            }
        }
        #endregion

        #region 628 ChangeSet  ================================================
        internal void ChangeSet_X(ModelTree model, ModelRoot root)
        {
            var chg = model.Item1 as ChangeSet;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetChangeSetName(chg);
                        root.ModelCount = chg.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
                var cs = model.Item1 as ChangeSet;
                var N = model.IsExpandedLeft ? cs.Count : 0;

                if (N > 0)
                {
                    var items = (cs.IsReversed) ? cs.Items : cs.ItemsReversed;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as ItemChange;
                        if (!TryGetOldModel(model, Trait.ItemChange_M, oldModels, i, itm))
                        {
                            model.ChildModels[i] = new ModelTree(model, Trait.ItemChange_M, level, itm, null, null, ItemChanged_X);
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
                return chg.NameKey.GetLocalized();
            else
                return chg.Name;
        }
        private void ModelMerge(ModelTree model)
        {
            var chg = model.Item1 as ChangeSet;
            chg.Merge();
            MajorDelta += 1;
        }
        private void ModelUndo(ModelTree model)
        {
            var chg = model.Item1 as ChangeSet;
            Undo(chg);
        }
        private void ModelRedo(ModelTree model)
        {
            var chg = model.Item1 as ChangeSet;
            Redo(chg);
        }
        #endregion

        #region 629 ItemChanged  ==============================================
        internal void ItemChanged_X(ModelTree model, ModelRoot root)
        {
            var chg = model.Item1 as ItemChange;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = chg.KindKey.GetLocalized();
                        root.ModelName = chg.Name;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 631 ViewXView_ZM  =============================================
        internal void ViewXView_ZM(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ViewXView_ZM_Drop;
                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();

                        var views = _viewXStore.Items;
                        var count = 0;
                        foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ViewXView_ZM_Insert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    var views = _viewXStore.Items;
                    var roots = new List<ViewX>();
                    foreach (var view in views) { if (ViewX_ViewX.HasNoParent(view)) { roots.Add(view); N++; } } 
                    
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = roots[i];
                            if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewXView_M, level, itm, null, null, ViewXView_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXView_ZM_Insert(ModelTree model)
        {
            ItemCreated(new ViewX(_viewXStore));
        }
        private DropAction ViewXView_ZM_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            var vxDrop = drop.Item1 as ViewX;
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
        internal void ViewXView_M(ModelTree model, ModelRoot root)
        {
            var view = model.Item1 as ViewX;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ViewXView_M_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = view.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = view.Name;
                        var count = (ViewX_ViewX.ChildCount(view) + ViewX_QueryX.ChildCount(view) + ViewX_Property.ChildCount(view));
                        root.ModelCount = count;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                        model.ChildModels = new ModelTree[N];

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
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXProperty_M, level, px, ViewX_Property, view, ViewXProperty_M);
                            }
                        }
                        if (L2 > 0)
                        {
                            for (int i = (R + L1), j = 0; j < L2; i++, j++)
                            {
                                var qx = queryList[j];
                                if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qx))
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXQuery_M, level, qx, ViewX_QueryX, view, ViewXQuery_M);
                            }
                        }
                        if (L3 > 0)
                        {
                            for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                            {
                                var vx = viewList[j];
                                if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXView_M, level, vx, ViewX_ViewX, view, ViewXView_M);
                            }
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXView_M_Insert(ModelTree model)
        {
            var vx = new ViewX(_viewXStore);
            ItemCreated(vx);
            AppendLink(ViewX_ViewX, model.Item1, vx);
        }
        private DropAction ViewXView_M_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            var view = model.Item1 as ViewX;
            if (view != null)
            {
                var vx = drop.Item1 as ViewX;
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
                    var st = drop.Item1 as Store;
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
                        var re = drop.Item1 as Relation;
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
                            var pr = drop.Item1 as Property;
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
        internal void ViewXQuery_M(ModelTree model, ModelRoot root)
        {
            var qx = model.Item1 as QueryX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ViewXQuery_M_Drop;
                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:


                        var rel = Relation_QueryX.GetParent(qx);
                        if (rel != null)
                        {
                            root.ModelKind = qx.KindKey.GetLocalized();
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

                    case ModelAction.ModelSelect:

                        root.ModelDescription = model.DescriptionKey.GetLocalized();
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
                        model.ChildModels = new ModelTree[N];

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
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXProperty_M, level, px, QueryX_Property, qx, ViewXProperty_M);
                            }
                        }
                        if (L2 > 0)
                        {
                            for (int i = (R + L1), j = 0; j < L2; i++, j++)
                            {
                                var qr = queryList[j];
                                if (!TryGetOldModel(model, Trait.ViewXQuery_M, oldModels, i, qr))
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXQuery_M, level, qr, QueryX_QueryX, qx, ViewXQuery_M);
                            }
                        }
                        if (L3 > 0)
                        {
                            for (int i = (R + L1 + L2), j = 0; j < L3; i++, j++)
                            {
                                var vx = viewList[j];
                                if (!TryGetOldModel(model, Trait.ViewXView_M, oldModels, i, vx))
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewXView_M, level, vx, QueryX_ViewX, qx, ViewXView_M);
                            }
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        private void ViewXQuery_M_Insert(ModelTree model)
        {
            var vx = new ViewX(_viewXStore);
            ItemCreated(vx);
            AppendLink(QueryX_ViewX, model.Item1, vx);
        }

        private DropAction ViewXQuery_M_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            var query = model.Item1 as QueryX;
            if (drop.Item1 is Relation rel)
            {
                if (doDrop)
                {
                    CreateQueryX(query, rel, QueryType.View).AutoExpandRight = false;
                }
                return DropAction.Link;
            }
            else if (drop.Item1 is Property prop)
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
        internal void ViewXCommand_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        break;

                    case ModelAction.ModelSelect:

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
        internal void ViewXProperty_M(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as Property;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.Double);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion


        #region 63A ViewView_ZM  ==============================================
        internal void ViewView_ZM(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        var views = _viewXStore.Items;
                        var count = 0;
                        foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) count++; }
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft)
                {
                    var views = _viewXStore.Items;
                    var roots = new List<ViewX>();
                    foreach (var vx in views) { if (ViewX_ViewX.HasNoParent(vx)) { roots.Add(vx); N++; } }

                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = roots[i];
                            if (!TryGetOldModel(model, Trait.ViewView_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewView_M, level, itm, null, null, ViewView_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion

        #region 63B ViewView_M  ===============================================
        internal void ViewView_M(ModelTree model, ModelRoot root)
        {
            var view = model.Item1 as ViewX;
            var key = model.Item2; // may be null

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = view.Summary;
                        break;

                    case ModelAction.ModelRefresh:

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

                    case ModelAction.ModelSelect:

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
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];

                            if (!TryGetOldModel(model, Trait.ViewItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewItem_M, level, itm, queryList[0], null, ViewItem_M);
                        }
                    }
                    else if (key != null && L2 > 0)
                    {
                        N = L2;
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var qx = queryList[i];

                            if (!TryGetOldModel(model, Trait.ViewQuery_M, oldModels, i, qx))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewQuery_M, level, qx, key, null, ViewQuery_M);
                        }
                    }
                    else if (L3 > 0)
                    {
                        N = L3;
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var vx = viewList[i];

                            if (!TryGetOldModel(model, Trait.ViewView_M, oldModels, i, vx))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewView_M, level, vx, null, null, ViewView_M);
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
        internal void ViewItem_M(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;
            var query = model.Item2 as QueryX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = GetIdentity(item.Owner, IdentityStyle.Single);
                        root.ModelName = GetIdentity(item, IdentityStyle.Single);

                        var qc = GetQueryXChildren(query);
                        var count = (qc.L2 + qc.L3);
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanExpandRight = qc.L1 > 0;
                        model.CanFilterUsage = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                        model.ChildModels = new ModelTree[N];

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
                                    model.ChildModels[i] = new ModelTree(model, Trait.ViewQuery_M, level, item, qx, null, ViewQuery_M);
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
        internal void ViewQuery_M(ModelTree model, ModelRoot root)
        {
            var key = model.Item1;
            var query = model.Item2 as QueryX;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:
                        var count = TryGetQueryItems(query, out Item[] items, key) ? items.Length : 0;

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = GetIdentity(query, IdentityStyle.Single);
                        root.ModelCount = count;

                        model.CanExpandLeft = (count > 0);
                        model.CanFilter = (count > 2);
                        model.CanSort = (model.IsExpandedLeft && count > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.ViewItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.ViewItem_M, level, itm, query, null, ViewItem_M);
                        }
                    }
                }
                if (N == 0) model.ChildModels = null;
            }
        }
        #endregion




        #region 642 EnumXList  ================================================
        internal void EnumXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = _enumXStore.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, EnumDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _enumXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _enumXStore.Items;
                    var item = model.Item1;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as EnumX;
                        if (!TryGetOldModel(model, Trait.EnumX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.EnumX_M, level, itm, null, null, EnumX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void EnumDefListInsert(ModelTree model)
        {
            ItemCreated(new EnumX(_enumXStore));
        }
        #endregion

        #region 643 TableXList  ===============================================
        internal void TableXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = _tableXStore.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, TableDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _tableXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _tableXStore.Items;
                    var item = model.Item1;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as TableX;
                        if (!TryGetOldModel(model, Trait.TableX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.TableX_M, level, itm, null, null, TableX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private void TableDefListInsert(ModelTree model)
        {
            ItemCreated(new TableX(_tableXStore));
        }
        #endregion

        #region 644 GraphXList  ===============================================
        internal void GraphXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = _graphXStore.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, GraphXRootInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var N = _graphXStore.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _graphXStore.Items;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as GraphX;
                        if (!TryGetOldModel(model, Trait.GraphX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphX_M, level, itm, null, null, GraphX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private void GraphXRootInsert(ModelTree model)
        {
            ItemCreated(new GraphX(_graphXStore));
            model.IsExpandedLeft = true;
        }
        #endregion

        #region 645 SymbolXlList  =============================================
        internal void SymbolXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item1 as GraphX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = SymbolDef_Drop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GraphX_SymbolX.ChildCount(gd);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, SymbolListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gd = model.Item1 as GraphX;
                var N = GraphX_SymbolX.ChildCount(gd);

                if (model.IsExpandedLeft && N > 0)
                {
                    var syms = GraphX_SymbolX.GetChildren(gd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var sym = syms[i];
                        if (!TryGetOldModel(model, Trait.SymbolX_M, oldModels, i, sym))
                            model.ChildModels[i] = new ModelTree(model, Trait.SymbolX_M, level, sym, GraphX_SymbolX, gd, SymbolX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void SymbolListInsert(ModelTree model)
        {
            var gd = model.Item1 as GraphX;
            var sym = new SymbolX(_symbolXStore);
            ItemCreated(sym);
            AppendLink(GraphX_SymbolX, gd, sym);
            model.IsExpandedLeft = true;
        }
        private DropAction SymbolDef_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!drop.Item1.IsSymbolX) return DropAction.None;
            var src = drop.Item1 as SymbolX;
            if (doDrop)
            {
                var gd = model.Item1 as GraphX;
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
        internal void TableList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = _tableXStore.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = _tableXStore.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _tableXStore.Items;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.Table_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.Table_M, level, itm, null, null, Table_X);
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
        internal void GraphList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = _graphXStore.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = _graphXStore.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    var items = _graphXStore.Items;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var gx = items[i];
                        if (!TryGetOldModel(model, Trait.GraphXRef_M, oldModels, i, gx))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXRef_M, level, gx, null, null, GraphXRef_X);
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
        internal void PairX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as PairX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.ActualValue;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = item.DisplayValue;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
        internal void EnumX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as EnumX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = item.Name;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    var item = model.Item1 as EnumX;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.EnumValue_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.EnumValue_ZM, level, item, null, null, PairXList_Dx);

                        i++;
                        if (!TryGetOldModel(model, Trait.EnumColumn_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.EnumColumn_ZM, level, item, null, null, EnumColumnList_X);
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
        internal void TableX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item1 as TableX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = tbl.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = tbl.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    var item = model.Item1 as TableX;
                    var level = (byte)(model.Level + 1);
                    var oldModels = model.ChildModels;

                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.ColumnX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.ColumnX_ZM, level, item, null, null, ColumnXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ComputeX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.ComputeX_ZM, level, item, null, null, ComputeXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ChildRelationX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.ChildRelationX_ZM, level, item, null, null, ChildRelationXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.ParentRelatationX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.ParentRelatationX_ZM, level, item, null, null, ParentRelatationXList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.MetaRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.MetaRelation_ZM, level, item, null, null, MetaRelationList_X);
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
        internal void GraphX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item1 as GraphX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = gd.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = gd.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    var item = model.Item1 as GraphX;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    if (R > 0)
                    {
                        AddProperyModels(model, oldModels, sp);
                    }
                    if (L > 0)
                    {
                        var i = R;
                        if (!TryGetOldModel(model, Trait.GraphXColoring_M, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXColoring_M, level, item, null, null, GraphXColoring_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.GraphXRoot_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXRoot_ZM, level, item, null, null, GraphXRootList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.GraphXNode_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXNode_ZM, level, item, null, null, GraphXNodeList_X);

                        i++;
                        if (!TryGetOldModel(model, Trait.SymbolX_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.SymbolX_ZM, level, item, null, null, SymbolXList_X);
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
        internal void SymbolX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as SymbolX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = item.Name;

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateSecondarySymbolEdit(ModelTree model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.SymbolEditor);
        }
        #endregion

        #region 657 ColumnX  ==================================================
        internal void ColumnX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item1 as ColumnX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = col.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = col.Name;

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
        internal void ComputeX_M(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var cpd = model.Item1 as ComputeX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ComputedX_M_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = cpd.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = cpd.Name;
                        var sd = ComputeX_QueryX.GetChild(cpd);
                        if (sd != null) root.ModelCount = QueryX_QueryX.ChildCount(sd);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                if (model.IsExpanded)
                {
                    var cx = model.Item1 as ComputeX;

                    int R = 0;
                    Property[] sp = null;
                    if (model.IsExpandedRight)
                    {
                        switch (cx.CompuType)
                        {
                            case CompuType.RowValue:
                                sp = new Property[] { _computeXNameProperty, _computeXSummaryProperty, _computeXCompuTypeProperty, _computeXSelectProperty, _computeXValueTypeProperty };
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
                    var sd = ComputeX_QueryX.GetChild(cx);
                    var L = (model.IsExpandedLeft && sd != null) ? QueryX_QueryX.ChildCount(sd) : 0;

                    var N = L + R;
                    if (N > 0)
                    {
                        var oldModels = model.ChildModels;
                        if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                var itm = items[j] as QueryX;
                                if (!TryGetOldModel(model, Trait.ValueXHead_M, oldModels, i, itm))
                                    model.ChildModels[i] = new ModelTree(model, Trait.ValueXHead_M, level, itm, null, null, ValueXHead_X);
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

        private DropAction ComputedX_M_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(drop.Item1 is Relation rel)) return DropAction.None;

            var cd = model.Item1 as ComputeX;
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
        internal void ColumnXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = TableX_ColumnX.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ColumnDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ColumnX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ColumnX.GetChildren(tbl);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.ColumnX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.ColumnX_M, level, itm, TableX_ColumnX, tbl, ColumnX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ColumnDefListInsert(ModelTree model)
        {
            var col = new ColumnX(_columnXStore);
            ItemCreated(col); AppendLink(TableX_ColumnX, model.Item1, col);
        }
        #endregion

        #region 662 ChildRelationXList  =======================================
        internal void ChildRelationXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = TableX_ChildRelationX.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ChildRelationDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ChildRelationX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ChildRelationX.GetChildren(tbl);

                    var level = (byte)(model.Level + 1);
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetOldModel(model, Trait.ChildRelationX_M, oldModels, i, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.ChildRelationX_M, level, rel, TableX_ChildRelationX, tbl, ChildRelationX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ChildRelationDefListInsert(ModelTree model)
        {
            var rel = new RelationX(_relationXStore);
            ItemCreated(rel); AppendLink(TableX_ChildRelationX, model.Item1, rel);
        }
        #endregion

        #region 663 ParentRelatationXList  ====================================
        internal void ParentRelatationXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = TableX_ParentRelationX.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ParentRelationDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;

                var N = (model.IsExpandedLeft) ? TableX_ParentRelationX.ChildCount(tbl) : 0;
                if (N > 0)
                {
                    var items = TableX_ParentRelationX.GetChildren(tbl);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i];
                        if (!TryGetOldModel(model, Trait.ParentRelationX_M, oldModels, i, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.ParentRelationX_M, level, rel, TableX_ParentRelationX, tbl, ParentRelationX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ParentRelationDefListInsert(ModelTree model)
        {
            var rel = new RelationX(_relationXStore); ItemCreated(rel);
            AppendLink(TableX_ParentRelationX, model.Item1, rel);
        }
        #endregion

        #region 664 PairXList  ================================================
        internal void PairXList_Dx(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as EnumX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, EnumValueListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var enu = model.Item1 as EnumX;
                var N = enu.Count;

                if (model.IsExpandedLeft && N > 0)
                {
                    var items = enu.Items;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as PairX;
                        if (!TryGetOldModel(model, Trait.PairX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.PairX_M, level, itm, enu, null, PairX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void EnumValueListInsert(ModelTree model)
        {
            ItemCreated(new PairX(model.Item1 as EnumX));
        }
        #endregion

        #region 665 EnumColumnList  ===========================================
        internal void EnumColumnList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as EnumX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = EnumColumn_Drop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = EnumX_ColumnX.ChildCount(item);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var enu = model.Item1 as EnumX;

                var N = (model.IsExpandedLeft) ? EnumX_ColumnX.ChildCount(enu): 0;
                if (N > 0)
                {
                    var items = EnumX_ColumnX.GetChildren(enu);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var col = items[i];
                        var tbl = TableX_ColumnX.GetParent(col);
                        if (tbl != null)
                        {
                            if (!TryGetOldModel(model, Trait.EnumRelatedColumn_M, oldModels, i, col))
                                model.ChildModels[i] = new ModelTree(model, Trait.EnumRelatedColumn_M, level, col, tbl, enu, EnumRelatedColumn_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction EnumColumn_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!drop.Item1.IsColumnX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(EnumX_ColumnX, model.Item1, drop.Item1);
            }
            return DropAction.Link;
        }
        #endregion

        #region 666 ComputeXList  =============================================
        internal void ComputeXList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var sto = model.Item1 as Store;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, ComputedDefListInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sto = model.Item1 as Store;

                var N = (model.IsExpandedLeft) ? Store_ComputeX.ChildCount(sto) : 0;
                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);
                    var items = Store_ComputeX.GetChildren(sto);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.ComputeX_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.ComputeX_M, level, itm, null, null, ComputeX_M);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void ComputedDefListInsert(ModelTree model)
        {
            var st = model.Item1 as Store;
            var cx = new ComputeX(_computeXStore);
            ItemCreated(cx);
            AppendLink(Store_ComputeX, st, cx);

            CreateQueryX(cx, st);
        }
        #endregion



        #region 671 ChildRelationX  ===========================================
        internal void ChildRelationX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ChildRelationDef_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetIdentity(item, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                    var item = model.Item1 as RelationX;
                    var sp = item.IsLimited ? sp2 : sp1;
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private DropAction ChildRelationDef_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!drop.Item1.IsTableX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(TableX_ParentRelationX, drop.Item1, model.Item1);
            }
            return DropAction.Link;
        }
        #endregion

        #region 672 ParentRelationX  ==========================================
        internal void ParentRelationX_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item1 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ParentRelationDef_Drop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                    var item = model.Item1 as RelationX;
                    var sp = item.IsLimited ? sp2 : sp1;
                    var N = sp.Length;

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }

            }
        }
        private DropAction ParentRelationDef_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!drop.Item1.IsTableX) return DropAction.None;

            if (doDrop)
            {
                AppendLink(TableX_ChildRelationX, drop.Item1, model.Item1);
            }
            return DropAction.Link;
        }
        #endregion

        #region 673 NameColumnRelation  =======================================
        internal void NameColumnRelation_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = NameColumnRelation_Drop;
                        break;

                    case ModelAction.PointerOver:
                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = TableX_NameProperty.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;
                Property prop;

                if (model.IsExpandedLeft && TableX_NameProperty.TryGetChild(tbl, out prop))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ModelTree[1];

                    if (!TryGetOldModel(model, Trait.NameColumn_M, oldModels, 0, prop))
                        model.ChildModels[0] = new ModelTree(model, Trait.NameColumn_M, level, prop, tbl, null, NameColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction NameColumnRelation_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(drop.Item1 is Property)) return DropAction.None;

            if (doDrop)
            {
                if (model.IsChildModel(drop))
                    RemoveLink(TableX_NameProperty, model.Item1, drop.Item1);
                else
                {
                    AppendLink(TableX_NameProperty, model.Item1, drop.Item1);
                    model.IsExpandedLeft = true;
                }
            }
            return DropAction.Link;
        }
        #endregion

        #region 674 SummaryColumnRelation  ====================================
        internal void SummaryColumnRelation_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = SummaryColRelation_Drop;
                        break;

                    case ModelAction.PointerOver:
                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = TableX_SummaryProperty.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;
                Property prop = null;

                if (model.IsExpandedLeft && TableX_SummaryProperty.TryGetChild(tbl, out prop))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ModelTree[1];

                    if (!TryGetOldModel(model, Trait.SummaryColumn_M, oldModels, 0, prop))
                        model.ChildModels[0] = new ModelTree(model, Trait.SummaryColumn_M, level, prop, tbl, null, SummaryColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction SummaryColRelation_Drop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(drop.Item1 is Property)) return DropAction.None;

            if (doDrop)
            {
                if (model.IsChildModel(drop))
                    RemoveLink(TableX_SummaryProperty, model.Item1, drop.Item1);
                else
                {
                    AppendLink(TableX_SummaryProperty, model.Item1, drop.Item1);
                    model.IsExpandedLeft = true;
                }
            }
            return DropAction.Link;
        }
        #endregion

        #region 675 NameColumn  ===============================================
        internal void NameColumn_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        if (item.IsColumnX)
                            root.ModelSummary = (item as ColumnX).Summary;
                        else if (item.IsComputeX)
                            root.ModelSummary = (item as ComputeX).Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        if (item.IsColumnX)
                            root.ModelName = (item as ColumnX).Name;
                        else if (item.IsComputeX)
                            root.ModelName = (item as ComputeX).Name;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 676 SummaryColumn  ============================================
        internal void SummaryColumn_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        if (item.IsColumnX)
                            root.ModelSummary = (item as ColumnX).Summary;
                        else if (item.IsComputeX)
                            root.ModelSummary = (item as ComputeX).Summary;
                        break;

                    case ModelAction.ModelRefresh:


                        if (item.IsColumnX)
                            root.ModelName = (item as ColumnX).Name;
                        else if (item.IsComputeX)
                            root.ModelName = (item as ComputeX).Name;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 681 GraphXColoring  ===========================================
        internal void GraphXColoring_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gd = model.Item1 as GraphX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = GraphXColoringDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GraphX_ColorColumnX.ChildCount(gd);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = model.DescriptionKey.GetLocalized();
                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1;
                ColumnX col = null;
                TableX tbl = null;
                if (model.IsExpandedLeft && GraphX_ColorColumnX.TryGetChild(item, out col) && TableX_ColumnX.TryGetParent(col, out tbl))
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || oldModels.Length != 1) model.ChildModels = new ModelTree[1];

                    if (!TryGetOldModel(model, Trait.GraphXColorColumn_M, oldModels, 0, col, tbl))
                        model.ChildModels[0] = new ModelTree(model, Trait.GraphXColorColumn_M, level, col, tbl, null, GraphXColorColumn_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction GraphXColoringDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            var gd = model.Item1;
            var col = drop.Item1;
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
        internal void GraphXRootList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = GraphXRootListDrop;
                        break;

                    case ModelAction.PointerOver:
                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GraphX_QueryX.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = model.DescriptionKey.GetLocalized();
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gd = model.Item1 as GraphX;

                var N = (model.IsExpandedLeft) ? GraphX_QueryX.ChildCount(gd): 0;
                if (N > 0)
                {
                    var items = GraphX_QueryX.GetChildren(gd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphXRoot_M, oldModels, i, itm, gd))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXRoot_M, level, itm, gd, null, QueryXRoot_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction GraphXRootListDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1 is GraphX gx)) return DropAction.None;
            if (!(drop.Item1 is Store st)) return DropAction.None;
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
        internal void GraphXNodeList_X(ModelTree model, ModelRoot root)
        {
            var gx = model.Item1 as GraphX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GetNodeOwners(gx).Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
                        if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var sto = owners[i];
                            if (!TryGetOldModel(model, Trait.GraphXNode_M, oldModels, i, sto, gx))
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXNode_M, level, sto, gx, null, GraphXNode_X);
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
        internal void GraphXNode_X(ModelTree model, ModelRoot root)
        {
            var sto = model.Item1 as Store;
            var gx = model.Item2 as GraphX;

            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as TableX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = SymbolNodeOwnerDrop;
                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = item.Name;
                        root.ModelCount = GetSymbolQueryXCount(gx, sto);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    (var symbols, var querys) = GetSymbolXQueryX(gx, sto);
                    for (int i = 0; i < N; i++)
                    {
                        var seg = querys[i];
                        if (!TryGetOldModel(model, Trait.GraphXNodeSymbol_M, oldModels, i, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXNodeSymbol_M, level, seg, GraphX_SymbolQueryX, gx, GraphXNodeSymbol_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction SymbolNodeOwnerDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1 is Store st)) return DropAction.None;
            if (!(model.Item2 is GraphX gx)) return DropAction.None;
            if (!(drop.Item1 is SymbolX sx)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(gx, sx, st);
            }
            return DropAction.Link;
        }
        #endregion

        #region 685 GraphXColorColumn  ========================================
        internal void GraphXColorColumn_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item1 as ColumnX;
                var tbl = model.Item2 as TableX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = $"{tbl.Name} : {col.Name}";
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = model.DescriptionKey.GetLocalized();
                        break;
                }
            }
        }
        #endregion



        #region 691 QueryXRoot  ===============================================
        internal void QueryXRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXRootHeadDrop;
                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.NameKey.GetLocalized();
                        root.ModelName = QueryXRootName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = model.DescriptionKey.GetLocalized();
                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                int N = 0;
                if (model.IsExpandedLeft || model.IsExpandedRight)
                {
                    var qx = model.Item1 as QueryX;
                    var sp = new Property[] { };
                    var R = model.IsExpandedRight ? sp.Length : 0;

                    var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(qx) : 0;

                    N = L + R;
                    if (N > 0)
                    {
                        var level = (byte)(model.Level + 1);
                        var oldModels = model.ChildModels;
                        model.ChildModels = new ModelTree[N];

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
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathHead_M, level, child, null, null, QueryXPathHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathLink_M, level, child, null, null, QueryXPathLink_X);
                                    }
                                }
                                else if (child.IsGroup)
                                {
                                    if (child.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupHead_M, oldModels, i, child))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupHead_M, level, child, null, null, QueryXGroupHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupLink_M, level, child, null, null, QueryXGroupLink_X);
                                    }
                                }
                                else if (child.IsSegue)
                                {
                                    if (child.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressHead_M, oldModels, i, child))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressHead_M, level, child, null, null, QueryXEgressHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, child))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressLink_M, level, child, null, null, QueryXEgressLink_X);
                                    }
                                }
                                else if (child.IsRoot)
                                {
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, child))
                                        model.ChildModels[i] = new ModelTree(model, Trait.GraphXLink_M, level, child, null, null, QueryXLink_X);
                                }
                                else
                                {
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, child))
                                        model.ChildModels[i] = new ModelTree(model, Trait.GraphXLink_M, level, child, null, null, QueryXLink_X);
                                }
                            }
                        }
                    }
                }
                if (N == 0)    model.ChildModels = null;
            }
        }
        private DropAction QueryXRootHeadDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(drop.Item1 is Relation re)) return DropAction.None;
            if (!(model.Item1 is QueryX qx)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Graph);
            }
            return DropAction.Link;
        }
        private string QueryXRootName(ModelTree modle)
        {
            var sd = modle.Item1;
            var tb = Store_QueryX.GetParent(sd);
            return GetIdentity(tb, IdentityStyle.Single);
        }
        #endregion

        #region 692 QueryXLink  ===============================================
        internal void QueryXLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXRootLinkDrop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathHead_M, level, qx, null, null, QueryXPathHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXPathLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathLink_M, level, qx, null, null, QueryXPathLink_X);
                                    }
                                    break;

                                case QueryType.Group:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupHead_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupHead_M, level, qx, null, null, QueryXGroupHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXGroupLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupLink_M, level, qx, null, null, QueryXGroupLink_X);
                                    }
                                    break;

                                case QueryType.Segue:
                                    if (qx.IsHead)
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressHead_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressHead_M, level, qx, null, null, QueryXEgressHead_X);
                                    }
                                    else
                                    {
                                        if (!TryGetOldModel(model, Trait.GraphXEgressLink_M, oldModels, i, qx))
                                            model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressLink_M, level, qx, null, null, QueryXEgressLink_X);
                                    }
                                    break;

                                case QueryType.Graph:
                                    if (!TryGetOldModel(model, Trait.GraphXLink_M, oldModels, i, qx))
                                        model.ChildModels[i] = new ModelTree(model, Trait.GraphXLink_M, level, qx, null, null, QueryXLink_X);
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

        private string GetQueryXRelationName(ModelTree model)
        {
            Relation parent;
            if (Relation_QueryX.TryGetParent(model.Item1, out parent))
            {
                var rel = parent as RelationX;
                return GetRelationName(rel);
            }
            return null;
        }
        private DropAction QueryXRootLinkDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (drop.Item1.IsRelationX)
            {
                if (doDrop)
                {
                    var qx = model.Item1 as QueryX;
                    var re = drop.Item1 as Relation;
                    CreateQueryX(qx, re, QueryType.Graph);
                }
                return DropAction.Link;
            }
            else if (model.Item1.IsQueryGraphLink)
            {
                if (doDrop)
                {
                    RemoveItem(drop);
                }
                return DropAction.Link;
            }
            return DropAction.None;
        }
        string QueryXLinkName(ModelTree model)
        {
            return QueryXFilterName(model.Item1 as QueryX);
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
        internal void QueryXPathHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXPathDrop;
                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathLink_M, level, itm, null, null, QueryXPathLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        string QueryXHeadName(ModelTree model)
        {
            var sd = model.Item1 as QueryX;

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
        internal void QueryXPathLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXPathDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXIsBreakPointProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXPathLink_M, level, itm, null, null, QueryXPathLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXPathDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1 is QueryX qx)) return DropAction.None;
            if (!(drop.Item1 is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item1 as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Path);
            }
            return DropAction.Link;
        }
        #endregion

        #region 695 QueryXGroupHead  ==========================================
        internal void QueryXGroupHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXGroupDrop;
                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupLink_M, level, itm, null, null, QueryXGroupLink_X);
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
        internal void QueryXGroupLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXGroupDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXGroupLink_M, level, itm, null, null, QueryXGroupLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXGroupDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1 is QueryX qx)) return DropAction.None;
            if (!(drop.Item1 is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item1 as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Group);
            }
            return DropAction.Link;
        }
        #endregion

        #region 697 QueryXEgressHead  =========================================
        internal void QueryXEgressHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXBridgeDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXHeadName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressLink_M, level, itm, null, null, QueryXEgressLink_X);
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
        internal void QueryXEgressLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = QueryXBridgeDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var sd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(sd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphXEgressLink_M, level, itm, null, null, QueryXEgressLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction QueryXBridgeDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1 is QueryX qx)) return DropAction.None;
            if (!(drop.Item1 is Relation re)) return DropAction.None;
            if (!CanDropQueryXRelation(qx, drop.Item1 as RelationX)) return DropAction.None;

            if (doDrop)
            {
                CreateQueryX(qx, re, QueryType.Segue);
            }
            return DropAction.Link;
        }
        #endregion

        #region 699 GraphXNodeSymbol  =========================================
        internal void GraphXNodeSymbol_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetQueryXRelationName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXNodeSymbolName(model);

                        model.CanDrag = true;
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    AddProperyModels(model, oldModels, sp);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private string QueryXNodeSymbolName(ModelTree model)
        {
            var sd = model.Item1;
            SymbolX sym;
            return (SymbolX_QueryX.TryGetParent(sd, out sym)) ? sym.Name : null;
        }
        #endregion


        #region 69E ValueXHead  ===============================================
        internal void ValueXHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ValueXLinkDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = QueryXComputeName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var vd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(vd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var items = QueryX_QueryX.GetChildren(vd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.ValueXLink_M, level, itm, null, null, ValueXLink_X);
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
        internal void ValueXLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = ValueXLinkDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = QueryXComputeName(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryXLinkName(model);
                        root.ModelCount = QueryX_QueryX.ChildCount(model.Item1);

                        model.CanDrag = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var sp = new Property[] { _queryXRelationProperty, _queryXIsReversedProperty, _queryXRootWhereProperty, _queryXSelectProperty, _queryXValueTypeProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;

                var vd = model.Item1 as QueryX;
                var L = (model.IsExpandedLeft) ? QueryX_QueryX.ChildCount(vd) : 0;

                var N = L + R;
                if (N > 0)
                {
                    var items = QueryX_QueryX.GetChildren(vd);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.ValueXLink_M, level, itm, null, null, ValueXLink_X);
                        }
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction ValueXLinkDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(drop.Item1 is Relation re)) return DropAction.None;
            if (!(model.Item1 is QueryX qx)) return DropAction.None;

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
        string QueryXComputeName(ModelTree model)
        {
            var sd = model.Item1 as QueryX;

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
        internal void RowX_X(ModelTree model, ModelRoot root)
        {
            var row = model.Item1 as RowX;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderStoreItem;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(model.Item1, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetIdentity(model.Item1, IdentityStyle.Single);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        model.CanExpandRight = row.TableX.HasChoiceColumns;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, RemoveItem));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void RowX_VX(ModelTree model)
        {
            var row = model.Item1 as RowX;
            ColumnX[] cols = null;
            var R = (model.IsExpandedRight && TryGetChoiceColumns(row.Owner, out cols)) ? cols.Length : 0;
            var L = (model.IsExpandedLeft) ? 7 : 0;
            var N = R + L;

            if (N > 0)
            {
                var level = (byte)(model.Level + 1);

                var oldModels = model.ChildModels;
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];


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
                        model.ChildModels[i] = new ModelTree(model, Trait.RowProperty_ZM, level, row, TableX_ColumnX, null, RowPropertyList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowCompute_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowCompute_ZM, level, row, Store_ComputeX, null, RowComputeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowChildRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowChildRelation_ZM, level, row, TableX_ChildRelationX, null, RowChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowParentRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowParentRelation_ZM, level, row, TableX_ParentRelationX, null, RowParentRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowDefaultProperty_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowDefaultProperty_ZM, level, row, TableX_ColumnX, null, RowDefaultPropertyList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowUnusedChildRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowUnusedChildRelation_ZM, level, row, TableX_ChildRelationX, null, RowUnusedChildRelationList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.RowUnusedParentRelation_ZM, oldModels, i))
                        model.ChildModels[i] = new ModelTree(model, Trait.RowUnusedParentRelation_ZM, level, row, TableX_ParentRelationX, null, RowUnusedParentRelationList_X);
                }
            }
            else
            {
                model.ChildModels = null;
            }
        }
        private DropAction ReorderStoreItem(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (!(model.Item1.Owner is Store sto)) return DropAction.None;
            if (!model.IsSiblingModel(drop)) return DropAction.None;
            
            var item1 = drop.Item1;
            var item2 = model.Item1;
            var index1 = sto.IndexOf(item1);
            var index2 = sto.IndexOf(item2);
            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop)
            {
                ItemMoved(drop.Item1, index1, index2);
            }
            return DropAction.Move;
        }
        #endregion

        #region 6A3 View  =====================================================
        internal void View_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as ViewX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = item.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = item.Name;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6A4 TableX  ===================================================
        internal void Table_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item1 as TableX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = tbl.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = tbl.Name;
                        root.ModelCount = tbl.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.InsertCommand, TableInsert));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1 as TableX;
                var col = TableX_NameProperty.GetChild(tbl);
                var N = model.IsExpandedLeft ? tbl.Count : 0;

                if (N > 0)
                {
                    var items = tbl.Items;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var row = items[i];
                        if (!TryGetOldModel(model, Trait.Row_M, oldModels, i, row, tbl, col))
                            model.ChildModels[i] = new ModelTree(model, Trait.Row_M, level, row, tbl, col, RowX_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void TableInsert(ModelTree model)
        {
            var tbl = model.Item1 as TableX;
            ItemCreated(new RowX(tbl));
        }
        #endregion

        #region 6A5 Graph  ====================================================
        internal void Graph_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var g = model.Item1 as Graph;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = g.GraphX.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = g.Name;

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    var item = model.Item1 as Graph;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var i = 0;
                    if (!TryGetOldModel(model, Trait.GraphNode_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphNode_ZM, level, item, null, null, GraphNodeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphEdge_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphEdge_ZM, level, item, null, null, GraphEdgeList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphOpen_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphOpen_ZM, level, item, null, null, GraphOpenList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphRoot_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphRoot_ZM, level, item, null, null, GraphRootList_X);

                    i++;
                    if (!TryGetOldModel(model, Trait.GraphLevel_ZM, oldModels, i, item))
                        model.ChildModels[i] = new ModelTree(model, Trait.GraphLevel_ZM, level, item, null, null, GraphLevelList_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateSecondaryModelGraph(ModelTree model)
        {
            var root = model.GetRootModel();
            root.UIRequest = model.BuildViewRequest(ControlType.GraphDisplay, Trait.GraphRef_M, GraphRef_X);
        }
        #endregion

        #region 6A6 GraphRef  =================================================
        internal void GraphRef_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var graph = model.Item1 as Graph;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = graph.GraphX.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = graph.Name;
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else
            {
                var graph = model.Item1 as Graph;
                //graph.GraphRefValidate(model as RootModel);
            }
        }
        #endregion

        #region 6A7 RowChildRelation  =========================================
        internal void RowChildRelation_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item2 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = RowChildRelationDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = GetRelationName(rel);
                        root.ModelCount = model.Relation.ChildCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var row1 = model.Item1 as RowX;
                var rel = model.Item2 as RelationX;

                var N = (model.IsExpandedLeft) ? rel.ChildCount(row1) : 0;

                if (N > 0)
                {
                    var items = rel.GetChildren(row1);
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var row2 = items[i];
                        if (!TryGetOldModel(model, Trait.RowRelatedChild_M, oldModels, i, row2))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowRelatedChild_M, level, row2, rel, row1, RowRelatedChild_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction RowChildRelationDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            TableX expectedOwner;
            if (!drop.Item1.IsRowX) return DropAction.None;
            if (!model.Item1.IsRowX) return DropAction.None;
            if (!model.Item2.IsRelationX) return DropAction.None;
            if (!TableX_ParentRelationX.TryGetParent(model.Item2, out expectedOwner)) return DropAction.None;
            if (drop.Item1.Owner != expectedOwner) return DropAction.None;

            if (doDrop)
            {
                var rel = model.Item2 as RelationX;
                if (model.IsChildModel(drop))
                    RemoveLink(rel, model.Item1, drop.Item1);
                else
                    AppendLink(rel, model.Item1, drop.Item1);
            }
            return DropAction.Link;
        }
        #endregion

        #region 6A8 RowParentRelation  ========================================
        internal void RowParentRelation_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item2 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = RowParentRelationDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = rel.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = GetRelationName(rel);
                        root.ModelCount = model.Relation.ParentCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var rel = model.Item2 as RelationX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && rel.TryGetParents(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.RowRelatedParent_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowRelatedParent_M, level, itm, rel, item, RowRelatedParent_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private DropAction RowParentRelationDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            TableX expectedOwner;
            if (!drop.Item1.IsRowX) return DropAction.None;
            if (!model.Item1.IsRowX) return DropAction.None;
            if (!model.Item2.IsRelationX) return DropAction.None;
            if (!TableX_ChildRelationX.TryGetParent(model.Item2, out expectedOwner)) return DropAction.None;
            if (drop.Item1.Owner != expectedOwner) return DropAction.None;

            if (doDrop)
            {
                var rel = model.Item2 as RelationX;
                if (model.IsChildModel(drop))
                    RemoveLink(rel, drop.Item1, model.Item1);
                else
                    AppendLink(rel, drop.Item1, model.Item1);
            }
            return DropAction.Link;
        }
        #endregion

        #region 6A9 RowRelatedChild  ============================================
        internal void RowRelatedChild_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var rel = model.Item2 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedChild;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = RowSummary(model);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = row.TableX.Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = rel.ChildCount(row);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedChild));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void UnlinkRelatedChild(ModelTree model)
        {
            var key = model.Item3;
            var rel = model.Item2 as Relation;
            var item = model.Item1;
            RemoveLink(rel, key, item);
        }
        private void UnlinkRelatedRow(ModelTree model)
        {
            var parent = model.ParentModel;
            if (parent.IsRowChildRelationModel)
            {
                var row2 = model.Item1;
                var row1 = parent.Item1;
                var rel = parent.Item2 as Relation;
                RemoveLink(rel, row1, row2);
            }
            else if (parent.IsRowParentRelationModel)
            {
                var row1 = model.Item1;
                var row2 = parent.Item1;
                var rel = parent.Item2 as Relation;
                RemoveLink(rel, row1, row2);
            }
        }
        private DropAction ReorderRelatedChild (ModelTree model, ModelTree drop, bool doDrop)
        {
            if (model.Item3 == null) return DropAction.None;
            if (model.Item2 == null || !(model.Item2 is Relation rel)) return DropAction.None;

            var key = model.Item3;
            var item1 = drop.Item1;
            var item2 = model.Item1;
            (int index1, int index2) = rel.GetChildrenIndex(key, item1, item2);

            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop) ItemChildMoved(rel, key, item1, index1, index2);

            return DropAction.Move;
        }
        #endregion

        #region 6AA RowRelatedParent  ============================================
        internal void RowRelatedParent_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var rel = model.Item2 as RelationX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ReorderItems = ReorderRelatedParent;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = RowSummary(model);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = row.TableX.Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = rel.ParentCount(row);

                        model.CanDrag = true;
                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedParent));
                        break;
                }
            }
            else  // validate the list of child models
            {
                RowX_VX(model);
            }
        }
        private void UnlinkRelatedParent(ModelTree model)
        {
            var key = model.Item1;
            var rel = model.Item2 as Relation;
            var item = model.Item3;
            RemoveLink(rel, key, item);
        }
        private DropAction ReorderRelatedParent(ModelTree model, ModelTree drop, bool doDrop)
        {
            if (model.Item3 == null) return DropAction.None;
            if (model.Item2 == null || !(model.Item2 is Relation rel)) return DropAction.None;

            var key = model.Item3;
            var item1 = drop.Item1;
            var item2 = model.Item1;
            (int index1, int index2) = rel.GetParentsIndex(key, item1, item2);

            if (index1 < 0 || index2 < 0 || index1 == index2) return DropAction.None;

            if (doDrop) ItemParentMoved(rel, key, item1, index1, index2);
            
            return DropAction.Move;
        }
        #endregion

        #region 6AB EnumRelatedColumn  ========================================
        internal void EnumRelatedColumn_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var col = model.Item1 as ColumnX;
                var tbl = model.Item2 as TableX;

                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = $"{tbl.Name}: {col.Name}";
                        break;

                    case ModelAction.ModelSelect:

                        root.MenuCommands.Add(new ModelCommand(this, model, Trait.RemoveCommand, UnlinkRelatedColumn));
                        break;
                }
            }
        }
        private void UnlinkRelatedColumn(ModelTree model)
        {
            var col = model.Item1;
            var tbl = model.Item2;
            var enu = model.Item3;
            RemoveLink(EnumX_ColumnX, enu, col);
        }
        #endregion



        #region 6B1 RowPropertyList  ==========================================
        internal void RowPropertyList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetColumnCount(model.Item1, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var row = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedColumns(row, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
        internal void RowChildRelationList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetChildRelationCount(model.Item1, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedChildRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowChildRelation_M, level, item, rel, null, RowChildRelation_X);
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
        internal void RowParentRelationList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetParentRelationCount(model.Item1, out root.ModelCount, out n);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUsedParentRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowParentRelation_M, level, item, rel, null, RowParentRelation_X);
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
        internal void RowDefaultPropertyList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetColumnCount(model.Item1, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedColumns(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
        internal void RowUnusedChildRelationList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetChildRelationCount(model.Item1, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedChildRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowChildRelation_M, level, item, rel, null, RowChildRelation_X);
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
        internal void RowUnusedParentRelationList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                int n;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        GetParentRelationCount(model.Item1, out n, out root.ModelCount);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as RowX;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && TryGetUnusedParentRelations(item, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = items[i] as RelationX;

                        if (!TryGetOldModel(model, Trait.Empty, oldModels, i, item, rel))
                            model.ChildModels[i] = new ModelTree(model, Trait.RowParentRelation_M, level, item, rel, null, RowParentRelation_X);
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
        internal void RowComputeList_X(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;
            var sto = item.Owner;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var list = Store_ComputeX.GetChildren(sto);
                    for (int i = 0; i < N; i++)
                    {
                        var itm = list[i];
                        if (!TryGetOldModel(model, Trait.TextProperty_M, oldModels, i, item, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.TextProperty_M, level, item, itm, null, TextCompute_X);
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
        internal void QueryRootLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryRootItem_M, level, itm, seg, null, QueryRootItem_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private string QueryLinkName(ModelTree modle)
        {
            var s = modle.Query;
            return QueryXFilterName(s.QueryX);
        }
        #endregion

        #region 6C2 QueryPathHead  ============================================
        internal void QueryPathHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
        internal void QueryPathLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var seg = model.Item2 as Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:


                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryPathLink_VX(model);
            }
        }
        private void QueryPathLink_VX(ModelTree model)
        {
            var seg = model.Query;

            Item[] items;
            if (seg.TryGetItems(out items))
            {
                var N = items.Length;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;
                model.ChildModels = new ModelTree[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryPathTail_M, level, itm, seg, null, QueryPathTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryPathStep_M, level, itm, seg, null, QueryPathStep_X);
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
        internal void QueryGroupHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
        internal void QueryGroupLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryGroupLink_VX(model);
            }
        }
        private void QueryGroupLink_VX(ModelTree model)
        {
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Item[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryGroupTail_M, level, itm, seg, null, QueryGroupTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryGroupStep_M, level, itm, seg, null, QueryGroupStep_X);
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
        internal void QueryEgressHead_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

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
        internal void QueryEgressLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = QueryLinkName(model);
                        root.ModelCount = model.Query.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                QueryEgressLink_VX(model);
            }
        }
        private void QueryEgressLink_VX(ModelTree model)
        {
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Item[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                for (int i = 0; i < N; i++)
                {
                    var itm = items[i];
                    if (seg.IsTail)
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressTail_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryEgressTail_M, level, itm, seg, null, QueryEgressTail_X);
                    }
                    else
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressStep_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryEgressStep_M, level, itm, seg, null, QueryEgressStep_X);
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
        internal void QueryRootItem_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = (row.Owner as TableX).Name;
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                ValidateQueryModel(model);
            }
        }
        private void ValidateQueryModel(ModelTree model)
        {
            var itm = model.Item1;
            var seg = model.Query;
            var level = (byte)(model.Level + 1);
            var oldModels = model.ChildModels;

            Query[] items = null;
            var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
            if (N > 0)
            {
                if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                for (int i = 0; i < N; i++)
                {
                    seg = items[i];
                    if (seg.IsGraphLink)
                    {
                        if (!TryGetOldModel(model, Trait.QueryRootLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryRootLink_M, level, itm, seg, null, QueryRootLink_X);
                    }
                    else if (seg.IsPathHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryPathHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryPathHead_M, level, itm, seg, null, QueryPathHead_X);
                    }
                    else if (seg.IsGroupHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryGroupHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryGroupHead_M, level, itm, seg, null, QueryGroupHead_X);
                    }
                    else if (seg.IsSegueHead)
                    {
                        if (!TryGetOldModel(model, Trait.QueryEgressHead_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryEgressHead_M, level, itm, seg, null, QueryEgressHead_X);
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
        internal void QueryPathStep_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{model.KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item1;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryPathLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryPathLink_M, level, itm, seg, null, QueryPathLink_X);
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
        internal void QueryPathTail_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{model.KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6D4 QueryGroupStep  ===========================================
        internal void QueryGroupStep_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item1;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryGroupLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryGroupLink_M, level, itm, seg, null, QueryGroupLink_X);
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
        internal void QueryGroupTail_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{model.KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 6D6 QueryEgressStep  ==========================================
        internal void QueryEgressStep_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{model.KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var itm = model.Item1;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetQuerys(itm, out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        seg = items[i];
                        if (!TryGetOldModel(model, Trait.QueryEgressLink_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryEgressLink_M, level, itm, seg, null, QueryEgressLink_X);
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
        internal void QueryEgressTail_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var row = model.Item1 as RowX;
                var seg = model.Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = $"{model.KindKey.GetLocalized()} {(row.Owner as TableX).Name}";
                        root.ModelName = GetRowName(row);
                        root.ModelCount = seg.QueryCount(model.Item1);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion



        #region 6E1 GraphXRef  ================================================
        internal void GraphXRef_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var gx = model.Item1 as GraphX;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        root.ModelDrop = GraphXModDrop;
                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = gx.Summary;
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = gx.Trait.ToString();
                        root.ModelName = gx.Name;
                        root.ModelCount = gx.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        break;

                    case ModelAction.ModelSelect:

                        root.ButtonCommands.Add(new ModelCommand(this, model, Trait.CreateCommand, CreateGraph));
                        break;
                }
            }
            else  // validate the list of child models
            {
                var gx = model.Item1 as GraphX;
                var N = model.IsExpandedLeft ? gx.Count : 0;

                if (N > 0)
                {
                    var items = gx.Items;
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var g = items[i] as Graph;
                        if (!TryGetOldModel(model, Trait.Graph_M, oldModels, i, g))
                            model.ChildModels[i] = new ModelTree(model, Trait.Graph_M, level, g, null, null, Graph_X);
                    }
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        private void CreateGraph(ModelTree model)
        {
            var gx = model.Item1 as GraphX;
            CreateGraph(gx, out Graph g);

            model.IsExpandedLeft = true;
            MajorDelta += 1;

            var root = model.GetRootModel();
            root.UIRequest = UIRequest.CreateNewView(ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true);
        }
        private DropAction GraphXModDrop(ModelTree model, ModelTree drop, bool doDrop)
        {
            var gd = model.Item1 as GraphX;
            Graph g;
            Store tbl = null;

            if (GraphX_QueryX.ChildCount(gd) == 0) return DropAction.None;

            var items = GraphX_QueryX.GetChildren(gd);
            foreach (var item in items)
            {
                if (item.IsQueryGraphRoot && Store_QueryX.TryGetParent(item, out tbl) && drop.Item1.Owner == tbl) break;
            }
            if (tbl == null) return DropAction.None;

            foreach (var tg in gd.Items)
            {
                if (tg.RootItem == drop.Item1) return DropAction.None;
            }

            if (doDrop)
            {
                CreateGraph(gd, out g, drop.Item1);

                model.IsExpandedLeft = true;
                MajorDelta += 1;

                var root = model.GetRootModel();
                root.UIRequest = UIRequest.CreateNewView(ControlType.GraphDisplay, Trait.GraphRef_M, this, g, null, null, GraphRef_X, true);
            }
            return DropAction.Copy;
        }
        #endregion

        #region 6E2 GraphNodeList  ============================================
        internal void GraphNodeList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = model.Graph.NodeCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Nodes;
                var N = items.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphNode_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphNode_M, level, itm, null, null, GraphNode_X);
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
        internal void GraphEdgeList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = model.Graph.EdgeCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Edges;
                var N = items.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphEdge_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphEdge_M, level, itm, null, null, GraphEdge_X);
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
        internal void GraphRootList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = model.Graph.QueryCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Query[] items = item.Forest;
                var N = item.QueryCount;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var seg = items[i];
                        var tbl = seg.Item;
                        if (!TryGetOldModel(model, Trait.GraphRoot_M, oldModels, i, tbl, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphRoot_M, level, tbl, seg, null, GraphRoot_X);
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
        internal void GraphLevelList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as Graph;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = item.Levels.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as Graph;
                var items = item.Levels;
                var N = model.IsExpandedLeft ? items.Count : 0;

                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Level;
                        if (!TryGetOldModel(model, Trait.GraphLevel_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphLevel_M, level, itm, null, null, GraphLevel_X);
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
        internal void GraphLevel_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1 as Level;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = item.Name;
                        root.ModelCount = item.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var item = model.Item1 as Level;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var items = item.Paths;
                var N = (model.IsExpandedLeft) ? items.Count : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSortAscending || model.IsSortDescending || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.GraphPath_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphPath_M, level, itm, null, null, GraphPath_X);
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
        internal void GraphPath_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var path = model.Item1 as Path;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = GetPathKind(path);
                        root.ModelName = GetPathName(path);
                        root.ModelCount = path.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var path = model.Item1 as Path;
                var items = path.Items;
                var N = (model.IsExpandedLeft) ? path.Count : 0;
                if (N > 0)
                {
                    var level = (byte)(model.Level + 1);

                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i] as Path;
                        if (!TryGetOldModel(model, Trait.GraphPath_M, oldModels, i, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphPath_M, level, itm, null, null, GraphPath_X);
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
            var name = path.NameKey.GetLocalized();
            var kind = path.IsRadial ? GetKindKey(Trait.RadialPath).GetLocalized() : GetKindKey(Trait.LinkPath).GetLocalized();
            return $"{name}{kind}";
        }
        #endregion

        #region 6E8 GraphRoot  ================================================
        internal void GraphRoot_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var tbl = model.Item1 as TableX;
                var seg = model.Item2 as Query;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = tbl.Name;
                        root.ModelCount = seg.ItemCount;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var tbl = model.Item1;
                var seg = model.Query;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                Item[] items = null;
                var N = (model.IsExpandedLeft && seg.TryGetItems(out items)) ? items.Length : 0;
                if (N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var itm = items[i];
                        if (!TryGetOldModel(model, Trait.QueryRootItem_M, oldModels, i, itm, seg))
                            model.ChildModels[i] = new ModelTree(model, Trait.QueryRootItem_M, level, itm, seg, null, QueryRootItem_X);
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
        internal void GraphNode_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var nd = model.Item1 as Node;
                var g = nd.Graph;
                List<Edge> edges;

                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = GetIdentity(nd.Item, IdentityStyle.Double);
                        root.ModelCount = g.Node_Edges.TryGetValue(nd, out edges) ? edges.Count : 0;

                        model.CanExpandRight = true;
                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var level = (byte)(model.Level + 1);
                List<Edge> edges = null;
                var nd = model.Item1 as Node;
                var g = nd.Graph;
                var sp = new Property[] { _nodeCenterXYProperty, _nodeSizeWHProperty, _nodeOrientationProperty, _nodeFlipRotateProperty, _nodeLabelingProperty, _nodeResizingProperty, _nodeBarWidthProperty };
                var R = model.IsExpandedRight ? sp.Length : 0;
                var L = (model.IsExpandedLeft && g.Node_Edges.TryGetValue(nd, out edges)) ? edges.Count : 0;
                var N = L + R;

                if (N > 0)
                {
                    var oldModels = model.ChildModels;
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
                                model.ChildModels[i] = new ModelTree(model, Trait.GraphEdge_M, level, edge, null, null, GraphEdge_X);
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
        internal void GraphEdge_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var edge = model.Item1 as Edge;
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = GetEdgeName(edge);

                        model.CanExpandRight = true;
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

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
        internal void GraphOpenList_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = model.Graph.OpenQuerys.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
            else  // validate the list of child models
            {
                var g = model.Item1 as Graph;
                var level = (byte)(model.Level + 1);
                var oldModels = model.ChildModels;

                var N = g.OpenQuerys.Count;
                if (model.IsExpandedLeft && N > 0)
                {
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var h = g.OpenQuerys[i].Query1;
                        var t = g.OpenQuerys[i].Query2;
                        if (!TryGetOldModel(model, Trait.GraphOpen_M, oldModels, i, g, h, t))
                            model.ChildModels[i] = new ModelTree(model, Trait.GraphOpen_M, level, g, h, t, GraphOpen_X);
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
        internal void GraphOpen_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:
                        break;

                    case ModelAction.PointerOver:
                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = GraphOpenKind(model);
                        root.ModelName = GraphOpenName(model);
                        break;

                    case ModelAction.ModelSelect:
                        break;
                }
            }
        }
        private string GraphOpenKind(ModelTree model)
        {
            var g = model.Item1 as Graph;
            var h = model.Item2 as Query;

            return GetIdentity(h.Item, IdentityStyle.Double);
        }
        private string GraphOpenName(ModelTree model)
        {
            var g = model.Item1 as Graph;
            var t = model.Item3 as Query;
            Store head, tail;
            GetHeadTail(t.QueryX, out head, out tail);
            return $"{GetIdentity(t.Item, IdentityStyle.Double)}  -->  {GetIdentity(tail, IdentityStyle.Single)}: <?>";
        }

        #endregion


        #region 7D0 PrimeCompute  =============================================
        internal void PrimeCompute_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetSummaryKey(Trait.PrimeCompute_M).GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetNameKey(Trait.PrimeCompute_M).GetLocalized();
                        root.ModelCount = GetPrimeComputeCount();

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = GetDescriptionKey(Trait.PrimeCompute_M).GetLocalized();
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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var list = GetPrimeComputeStores();

                    for (int i = 0; i < N; i++)
                    {
                        var sto = list[i];
                        if (!TryGetOldModel(model, Trait.ComputeStore_M, oldModels, i, sto))
                            model.ChildModels[i] = new ModelTree(model, Trait.ComputeStore_M, level, sto, null, null, ComputeStore_X);
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
        internal void ComputeStore_X(ModelTree model, ModelRoot root)
        {
            var sto = model.Item1 as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(sto, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetIdentity(sto, IdentityStyle.Single);
                        root.ModelCount = Store_ComputeX.ChildCount(sto);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var list = Store_ComputeX.GetChildren(sto);
                    for (int i = 0; i < N; i++)
                    {
                        var itm = list[i];
                        if (!TryGetOldModel(model, Trait.TextProperty_M, oldModels, i, sto, itm))
                            model.ChildModels[i] = new ModelTree(model, Trait.TextProperty_M, level, sto, itm, null, TextCompute_X);
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
        internal void InternalStoreZ_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetSummaryKey(Trait.InternalStore_ZM).GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = GetNameKey(Trait.InternalStore_ZM).GetLocalized();

                        model.CanExpandLeft = true;
                        break;

                    case ModelAction.ModelSelect:

                        root.ModelDescription = GetDescriptionKey(Trait.InternalStore_ZM).GetLocalized();
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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    int i = 0;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _viewXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _viewXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _enumXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _enumXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _tableXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _tableXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _graphXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _graphXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _queryXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _queryXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _symbolXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _symbolXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _columnXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _columnXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _relationXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _relationXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _computeXStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _computeXStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _relationStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _relationStore, null, null, InternalStore_X);

                    i += 1;
                    if (!TryGetOldModel(model, Trait.InternalStore_M, oldModels, i, _propertyStore))
                        model.ChildModels[i] = new ModelTree(model, Trait.InternalStore_M, level, _propertyStore, null, null, InternalStore_X);
                }
                else
                {
                    model.ChildModels = null;
                }
            }
        }
        #endregion

        #region 7F1 InternalStore  ============================================
        internal void InternalStore_X(ModelTree model, ModelRoot root)
        {
            var store = model.Item1 as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = store.NameKey.GetLocalized();
                        root.ModelCount = store.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var list = store.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetOldModel(model, Trait.StoreItem_M, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreItem_M, level, item, null, null, StoreItem_X);
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
        internal void StoreItem_X(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;
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
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.StoreItem);
                        root.ModelCount = count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != count) model.ChildModels = new ModelTree[count];

                    int i = -1;
                    if (hasItems)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreItemItem_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreItemItem_ZM, level, item, null, null, StoreItemItemZ_X);
                    }
                    if (hasLinks)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreRelationLink_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreRelationLink_ZM, level, item, null, null, StoreRelationLinkZ_X);
                    }
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreChildRelation_ZM, level, item, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreParentRelation_ZM, level, item, null, null, StoreParentRelationZ_X);
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
        internal void StoreItemItemZ_X(ModelTree model, ModelRoot root)
        {
            var store = model.Item1 as Store;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = store.Count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    var list = store.GetItems();
                    for (int i = 0; i < N; i++)
                    {
                        var item = list[i];
                        if (!TryGetOldModel(model, Trait.StoreItemItem_M, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreItemItem_M, level, item, null, null, StoreItemItem_X);
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
        internal void StoreRelationLinkZ_X(ModelTree model, ModelRoot root)
        {
            var rel = model.Item1 as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = rel.GetLinksCount();

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var parent = parents[i];
                        var child = children[i];
                        if (!TryGetOldModel(model, Trait.StoreRelationLink_M, oldModels, i, rel, parent, child))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreRelationLink_M, level, rel, parent, child, StoreRelationLink_X);
                    }
                }
            }
        }
        #endregion

        #region 7F6 StoreChildRelationZ  ======================================
        internal void StoreChildRelationZ_X(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GetChildRelationCount(item, SubsetType.Used);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_M, oldModels, i, rel, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreChildRelation_M, level, rel, item, null, StoreChildRelation_X);
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
        internal void StoreParentRelationZ_X(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;

            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = model.SummaryKey.GetLocalized();
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelName = model.NameKey.GetLocalized();
                        root.ModelCount = GetParentRelationCount(item, SubsetType.Used);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (oldModels == null || model.IsSorted || oldModels.Length != N) model.ChildModels = new ModelTree[N];

                    for (int i = 0; i < N; i++)
                    {
                        var rel = relations[i];
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_M, oldModels, i, rel, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreParentRelation_M, level, rel, item, null, StoreParentRelation_X);
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
        internal void StoreItemItem_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var item = model.Item1;

                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = item.KindKey.GetLocalized();
                        root.ModelName = GetIdentity(item, IdentityStyle.Double);
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 7F9 StoreRelationLink  ========================================
        internal void StoreRelationLink_X(ModelTree model, ModelRoot root)
        {
            if (root != null)  // get the data for this root.ModelAction
            {
                var rel = model.Item1 as Relation;
                var parent = model.Item2;
                var child = model.Item3;

                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = model.KindKey.GetLocalized();
                        root.ModelName = $"({GetIdentity(parent, IdentityStyle.Double)}) --> ({GetIdentity(child, IdentityStyle.Double)})";
                        break;

                    case ModelAction.ModelSelect:

                        break;
                }
            }
        }
        #endregion

        #region 7FA StoreChildRelation  =======================================
        internal void StoreChildRelation_X(ModelTree model, ModelRoot root)
        {
            var rel = model.Item1 as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = GetKind(rel.Trait);
                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);
                        root.ModelCount = rel.ChildCount(model.Item2);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (rel.TryGetChildren(model.Item2, out items))
                    {
                        var N = items.Length;
                        var level = (byte)(model.Level + 1);
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.StoreRelatedItem_M, level, itm, null, null, StoreRelatedItem_X);
                        }
                    }
                }
            }
        }
        #endregion

        #region 7FA StoreParentRelation  ======================================
        internal void StoreParentRelation_X(ModelTree model, ModelRoot root)
        {
            var rel = model.Item1 as Relation;
            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(rel, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:

                        root.ModelKind = GetKind(rel.Trait);
                        root.ModelName = GetIdentity(rel, IdentityStyle.Single);
                        root.ModelCount = rel.ParentCount(model.Item2);

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                    if (rel.TryGetParents(model.Item2, out items))
                    {
                        var N = items.Length;
                        var level = (byte)(model.Level + 1);
                        model.ChildModels = new ModelTree[N];

                        for (int i = 0; i < N; i++)
                        {
                            var itm = items[i];
                            if (!TryGetOldModel(model, Trait.StoreRelatedItem_M, oldModels, i, itm))
                                model.ChildModels[i] = new ModelTree(model, Trait.StoreRelatedItem_M, level, itm, null, null, StoreRelatedItem_X);
                        }
                    }
                }
            }
        }
        #endregion

        #region 7FC StoreRelatedItem  =========================================
        internal void StoreRelatedItem_X(ModelTree model, ModelRoot root)
        {
            var item = model.Item1;
            var hasChildRels = (GetChildRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var hasParentRels = (GetParentRelationCount(item, SubsetType.Used) > 0) ? true : false;
            var count = 0;
            if (hasChildRels) count++;
            if (hasParentRels) count++;


            if (root != null)  // get the data for this root.ModelAction
            {
                switch (root.ModelAction)
                {
                    case ModelAction.DragOver:

                        break;

                    case ModelAction.PointerOver:

                        root.ModelSummary = GetIdentity(item, IdentityStyle.Summary);
                        break;

                    case ModelAction.ModelRefresh:
                        root.ModelKind = GetIdentity(item, IdentityStyle.Kind);
                        root.ModelName = GetIdentity(item, IdentityStyle.StoreItem);
                        root.ModelCount = count;

                        model.CanExpandLeft = (root.ModelCount > 0);
                        model.CanFilter = (root.ModelCount > 2);
                        model.CanSort = (model.IsExpandedLeft && root.ModelCount > 1);
                        break;

                    case ModelAction.ModelSelect:

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
                     model.ChildModels = new ModelTree[count];

                    int i = -1;
                    if (hasChildRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreChildRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreChildRelation_ZM, level, item, null, null, StoreChildRelationZ_X);
                    }
                    if (hasParentRels)
                    {
                        i++;
                        if (!TryGetOldModel(model, Trait.StoreParentRelation_ZM, oldModels, i, item))
                            model.ChildModels[i] = new ModelTree(model, Trait.StoreParentRelation_ZM, level, item, null, null, StoreParentRelationZ_X);
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
