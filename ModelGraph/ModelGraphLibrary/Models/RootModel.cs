using System;
using System.Collections.Generic;
interface IPage { }
namespace ModelGraphLibrary
{/*

 */
    public class RootModel : ItemModel
    {
        internal Chef Chef;
        internal IPage Page;

        internal int ModelCount;
        internal int ValueIndex;
        internal int MinorDelta;
        internal int MajorDelta;

        internal string ModelKind;
        internal string ModelName;
        internal string ModelInfo;
        internal string ModelValue;
        internal string ModelSummary;
        internal string ModelDescription;
        internal string[] ModelValueList;

        internal List<ModelCommand> MenuCommands = new List<ModelCommand>();
        internal List<ModelCommand> ButtonCommands = new List<ModelCommand>();
        internal List<ModelCommand> AppButtonCommands = new List<ModelCommand>();

        internal Func<ItemModel, ItemModel, bool, DropAction> ModelDrop;
        internal Func<ItemModel, ItemModel, bool, DropAction> ReorderItems;

        internal int ViewIndex1; //index of first visible model
        internal int ViewIndex2; //index one beyond the last visible model
        internal int ViewCapacity; //max number of visible models
        internal ItemModel ViewSelectModel; //currently selected model
        internal ItemModel[] ViewModels; //flattend list of itemModel tree
        internal Dictionary<object, string> ViewFilter = new Dictionary<object, string>();


        //internal IModelControl ModelControl;
        internal ControlType ControlType;
        internal ModelAction ModelAction;

        internal bool CloseModel;
        internal bool ReloadModel;
        internal bool ModelIsChecked;

        #region Constructors  =================================================
        // AppRootChef: Created by App.xaml
        internal RootModel(IPage page)
            : base(null, Trait.RootChef_M, 0, new Chef())
        {
            Chef = Item1 as Chef;
            Page = page;
            Chef.AddRootModel(this);

            ControlType = ControlType.AppRootChef;
            //ModelControl = new ModelTreeControl(this);
            _getData = Chef.RootChef_M;

            IsExpandedLeft = true;
        }

        // (Primary & Secondary) RootModels: Created by PrimaryRoot
        internal RootModel(IPage page, ViewRequest rq)
            : base(null, rq.Trait, 0, rq.Item1, rq.Item2, rq.Item3, rq.GetData)
        {
            Page = page;
            Chef = rq.Chef;
            Chef.AddRootModel(this);

            ControlType = rq.Type;
            //if (ControlType == ControlType.PrimaryTree)
            //    ModelControl = new ModelTreeControl(this);
            //else if (ControlType == ControlType.PartialTree)
            //    ModelControl = new ModelTreeControl(this);
            //else if (ControlType == ControlType.GraphDisplay)
            //    ModelControl = new ModelGraphControl(this);
            //else if (ControlType == ControlType.SymbolEditor)
            //    ModelControl = new SymbolEditControl(this);

            IsExpandedLeft = true;
        }

        internal ViewRequest BuildViewRequest() { return new ViewRequest(ControlType, Trait, Chef, Item1, Item2, Item3, _getData); }

        // Created by dataChef used for GetModelData
        internal RootModel(Chef owner) : base(null, Trait.MockChef_M, 0, owner) { Chef = owner; }
        #endregion

        #region Properties/Methods  ===========================================
        internal bool IsAppRootChef => (ControlType == ControlType.AppRootChef);
        internal string TabName => Chef.GetAppTabName(this);
        internal string TitleName => Chef.GetAppTitleName(this);
        internal string TabSummary => Chef.GetAppTabSummary(this);
        internal string TitleSummary => Chef.GetAppTitleSummary(this);
        internal void Close()
        {
            Chef.RemoveRootModel(this);
        }
        #endregion

        #region ModelData  ====================================================

        // ModelAction.DragOver  ==============================================
        internal void GetDragOverData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.DragOver;
            ModelDrop = ReorderItems = null;

            model.GetData(this);
        }

        // ModelAction.PointerOver  ===========================================
        internal void GetPointerOverData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.PointerOver;
            ModelSummary = null;

            model.GetData(this);

            if (string.IsNullOrEmpty(ModelSummary))
                ModelSummary = model.ModelIdentity;
        }

        // ModelAction.ModelSelect  ===========================================
        internal void GetModelSelectData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.ModelSelect;
            ModelDescription = null;
            MenuCommands.Clear();
            ButtonCommands.Clear();

            model.GetData(this);

            if (string.IsNullOrEmpty(ModelSummary))
                ModelSummary = model.ModelIdentity;
        }

        // ModelAction.ModelRefresh  ==========================================
        internal void GetModelItemData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.ModelRefresh;

            ModelKind = string.Empty;
            ModelName = string.Empty;
            ModelInfo = string.Empty;

            ModelValue = string.Empty;
            ModelValueList = null;
            ModelCount = 0;
            ValueIndex = 0;
            ModelIsChecked = false;

            model.GetData(this);

            if (string.IsNullOrEmpty(ModelName)) ModelName = "???";
        }

        internal void PostModelDrop(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            var drop = Chef.DragDropSource;
            if (drop == null) return;

            if (model.IsSiblingModel(drop))
                Chef.PostDataAction(this, () => { ReorderItems?.Invoke(model, drop, true); });
            else
                Chef.PostDataAction(this, () => { ModelDrop?.Invoke(model, drop, true); });
        }
        internal void SetDragDropSource(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            Chef.DragDropSource = model;
        }

        internal DropAction CanModelAcceptDrop(ItemModel model)
        {
            if (model == null | model.IsInvalid) return DropAction.None;

            var drop = Chef.DragDropSource;
            if (drop == null) return DropAction.None;

            GetDragOverData(model);

            if (model.IsSiblingModel(drop))
                return (ReorderItems == null) ? DropAction.None : ReorderItems(model, drop, false);
            else
                return (ModelDrop == null) ? DropAction.None : ModelDrop(model, drop, false);
        }
        #endregion

        #region SymbolEditor  =================================================
        internal async void SaveSymbol()
        {
            //var editor = ModelControl as SymbolEditControl;
            //if (editor == null) return;
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { editor.Save(); });
        }
        internal async void ReloadSymbol()
        {
            //var editor = ModelControl as SymbolEditControl;
            //if (editor == null) return;
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { editor.Reload(); });
        }
        #endregion

        #region PageDispatch  =================================================
        internal ViewRequest ViewRequest { get { return _viewRequest; } set { if (_viewRequest == null) _viewRequest = value; } }
        private ViewRequest _viewRequest;
        internal ItemModel[] ModelList { get { return _modelList; } set { if (_modelList == null) _modelList = value; } }
        private ItemModel[] _modelList;
        internal int Capacity = 100;

        internal async void PageDispatch()
        {
            if (Page != null)
            {
                //if (CloseModel)
                //{
                //    await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { Page.CloseModel(this); });
                //}
                //else if (ReloadModel)
                //{
                //    await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { Page.ReloadModel(this); });
                //}
                //else
                //{
                //    if (_viewRequest != null)
                //    {
                //        var rq = _viewRequest;
                //        _viewRequest = null;
                //        await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { Page.CreateView(rq); });
                //    }
                //    else
                //    {
                //        PageRefresh();
                //    }
                //}
            }
        }
        #endregion

        #region PageRefresh  ==================================================
        internal void PageRefresh()
        {
            //if (ModelControl == null) return;
            if (MajorDelta == Chef.MajorDelta && MinorDelta == Chef.MinorDelta) return;
            switch (ControlType)
            {
                case ControlType.AppRootChef:
                case ControlType.PrimaryTree:
                case ControlType.PartialTree:
                    ModelTreeRefreshAsync(ChangeType.NoChange);
                    break;
                case ControlType.GraphDisplay:
                    ModelGraphRefreshAsync();
                    break;
                case ControlType.SymbolEditor:
                    SymbolEditorRefreshAsync();
                    break;
            }
        }
        internal async void ModelTreeRefreshAsync(ChangeType change)
        {
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            //{
            //    if (Chef.ValidateModelTree(this, change))
            //        ModelControl.Refresh();
            //});
        }
        internal async void ModelGraphRefreshAsync()
        {
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            //{
            //    ModelControl.Refresh();
            //});
        }
        internal async void SymbolEditorRefreshAsync()
        {
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            //{
            //    ModelControl.Refresh();
            //});
        }
        #endregion

        #region UIControl  ====================================================

        public void GetAppCommands()
        {
            AppButtonCommands.Clear();
            Chef.GetAppCommands(this);
        }

        public void PostModelRefresh() { Chef.PostModelRefresh(this); }

        public void PostSetValue(ItemModel model, string value)
        {
            Chef.PostModelSetValue(model, value);
        }
        public void PostSetIsChecked(ItemModel model, bool value)
        {
            Chef.PostModelSetIsChecked(model, value);
        }
        public void PostSetValueIndex(ItemModel model, int value)
        {
            model.GetData(this);
            Chef.PostModelSetValueIndex(model, value);
        }
        #endregion
    }

    #region RelatedEnums  =====================================================
    // only return the data that is needed for the occasion
    internal enum ChangeType
    {
        NoChange,

        ToggleLeft,
        ExpandLeft,
        CollapseLeft,
        ExpandLeftAll,

        ToggleRight,
        ExpandRight,
        CollapseRight,

        ToggleFilter,
        ExpandFilter,
        CollapseFilter,

        FilterSortChanged,
    }
    internal enum ModelAction
    {
        DragOver,
        PointerOver,
        ModelSelect,
        ModelRefresh,
    }
    internal enum DropAction
    {
        None,
        Move,
        Copy,
        Link,
        Unlink,
    }
    internal enum ControlType
    {
        AppRootChef,
        PrimaryTree,
        PartialTree,
        SymbolEditor,
        GraphDisplay,
    }
    #endregion
}
