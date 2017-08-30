using System;
using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
    Data flow between ModelGraphLibrary <--> ModelGraph App

    Give the UI controls access to the ItemModels and also where it
    is necessary (within the ModelGraphControl) allow the control to
    directly interact with actual Item objects (Selector, Graph, Node, Edge,..)

    I want the ModelGraphLibrary to know as little as posible about the UI controls,
    however, it really simplifies things to allow one way access UI --> graph object.
   
    ModelGraphUnitTest can thoroughly test ModelGraphLibrary independant of any UI control.    
 */
    public class RootModel : ItemModel
    {
        public Chef Chef;
        public IPageControl Page;

        public int ModelCount;
        public int ValueIndex;
        public int MinorDelta;
        public int MajorDelta;

        public string ModelKind;
        public string ModelName;
        public string ModelInfo;
        public string ModelValue;
        public string ModelSummary;
        public string ModelDescription;
        public string[] ModelValueList;

        public List<ModelCommand> MenuCommands = new List<ModelCommand>();
        public List<ModelCommand> ButtonCommands = new List<ModelCommand>();
        public List<ModelCommand> AppButtonCommands = new List<ModelCommand>();

        public Func<ItemModel, ItemModel, bool, DropAction> ModelDrop;
        public Func<ItemModel, ItemModel, bool, DropAction> ReorderItems;

        public int ViewIndex1; //index of first visible model
        public int ViewIndex2; //index one beyond the last visible model
        public int ViewCapacity; //max number of visible models
        public ItemModel ViewSelectModel; //currently selected model
        public ItemModel[] ViewModels; //flattend list of itemModel tree
        public Dictionary<object, string> ViewFilter = new Dictionary<object, string>();


        public IModelControl ModelControl;
        public ControlType ControlType;
        public ModelAction ModelAction;

        public bool CloseModel;
        public bool ReloadModel;
        public bool ModelIsChecked;

        #region Constructors  =================================================
        // AppRootChef: Created by App.xaml
        public RootModel(IPageControl page)
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
        public RootModel(IPageControl page, ViewRequest rq)
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

        public ViewRequest BuildViewRequest() { return new ViewRequest(ControlType, Trait, Chef, Item1, Item2, Item3, _getData); }

        // Created by dataChef used for GetModelData
        public RootModel(Chef owner) : base(null, Trait.MockChef_M, 0, owner) { Chef = owner; }
        #endregion

        #region Properties/Methods  ===========================================
        public bool IsAppRootChef => (ControlType == ControlType.AppRootChef);
        public string TabName => Chef.GetAppTabName(this);
        public string TitleName => Chef.GetAppTitleName(this);
        public string TabSummary => Chef.GetAppTabSummary(this);
        public string TitleSummary => Chef.GetAppTitleSummary(this);
        public void Close()
        {
            Chef.RemoveRootModel(this);
        }
        #endregion

        #region ModelData  ====================================================

        // ModelAction.DragOver  ==============================================
        public void GetDragOverData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.DragOver;
            ModelDrop = ReorderItems = null;

            model.GetData(this);
        }

        // ModelAction.PointerOver  ===========================================
        public void GetPointerOverData(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.PointerOver;
            ModelSummary = null;

            model.GetData(this);

            if (string.IsNullOrEmpty(ModelSummary))
                ModelSummary = model.ModelIdentity;
        }

        // ModelAction.ModelSelect  ===========================================
        public void GetModelSelectData(ItemModel model)
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
        public void GetModelItemData(ItemModel model)
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

        public void PostModelDrop(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            var drop = Chef.DragDropSource;
            if (drop == null) return;

            if (model.IsSiblingModel(drop))
                Chef.PostDataAction(this, () => { ReorderItems?.Invoke(model, drop, true); });
            else
                Chef.PostDataAction(this, () => { ModelDrop?.Invoke(model, drop, true); });
        }
        public void SetDragDropSource(ItemModel model)
        {
            if (model == null | model.IsInvalid) return;

            Chef.DragDropSource = model;
        }

        public DropAction CanModelAcceptDrop(ItemModel model)
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
        public async void SaveSymbol()
        {
            //var editor = ModelControl as SymbolEditControl;
            //if (editor == null) return;
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { editor.Save(); });
        }
        public async void ReloadSymbol()
        {
            //var editor = ModelControl as SymbolEditControl;
            //if (editor == null) return;
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { editor.Reload(); });
        }
        #endregion

        #region PageDispatch  =================================================
        public ViewRequest ViewRequest { get { return _viewRequest; } set { if (_viewRequest == null) _viewRequest = value; } }
        private ViewRequest _viewRequest;
        public ItemModel[] ModelList { get { return _modelList; } set { if (_modelList == null) _modelList = value; } }
        private ItemModel[] _modelList;
        public int Capacity = 100;

        public async void PageDispatch()
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
        public void PageRefresh()
        {
            if (ModelControl == null) return;
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
        public async void ModelTreeRefreshAsync(ChangeType change)
        {
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            //{
            //    if (Chef.ValidateModelTree(this, change))
            //        ModelControl.Refresh();
            //});
        }
        public async void ModelGraphRefreshAsync()
        {
            //await Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            //{
            //    ModelControl.Refresh();
            //});
        }
        public async void SymbolEditorRefreshAsync()
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
}
