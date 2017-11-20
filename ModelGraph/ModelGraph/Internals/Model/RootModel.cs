﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace ModelGraph.Internals
{/*
    Data flow and control between ModelGraph.Internals <--> ModelGraph App

    The ModelGraph.Internals has no direct knowledge of the UI controls, however, it does 
    initiates UI action through the interfaces IPageControl and IModelControl.

    The UI controls, on the other hand, do have direct access to the public methods and
    properties of ModelGraph.Internals objects. This is especially important for the case
    of the ModelGraphControl.
    
    The ModelGraphUnitTest tests the ModelGraph.Internals's essential function
    independant of the UI controls.    
 */
    public class RootModel : ItemModel
    {
        public Chef Chef;
        public IPageControl PageControl { get; set; } // reference the UI PageControl
        public IViewControl ViewControl { get; set; } // created and used exclusively by PageControl

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

        public ModelAction ModelAction;

        private ItemModel[] _modelList;
        public ItemModel[] ModelList { get { return _modelList; } set { if (_modelList == null) _modelList = value; } }
        public int Capacity { get; set; } = 100;

        public bool ModelIsChecked;

        #region Constructors  =================================================
        // AppRootChef: Created by App.xaml
        public RootModel(IPageControl pageControl)
            : base(null, Trait.RootChef_M, 0, new Chef())
        {
            PageControl = pageControl;

            Chef = Item1 as Chef;
            Chef.AddRootModel(this);

            _getData = Chef.RootChef_M;

            IsExpandedLeft = true;
        }

        // (Primary & Secondary) RootModels: Created by PrimaryRoot
        public RootModel(IPageControl pageControl, UIRequest rq)
            : base(null, rq.Trait, 0, rq.Item1, rq.Item2, rq.Item3, rq.GetData)
        {
            PageControl = pageControl;

            Chef = rq.Chef;
            Chef.AddRootModel(this);

            IsExpandedLeft = true;
        }

        // Created by dataChef used for GetModelData
        public RootModel(Chef owner) : base(null, Trait.MockChef_M, 0, owner) { Chef = owner; }
        #endregion

        #region Properties/Methods  ===========================================
        public bool IsAppRootChef => (ViewControl.ControlType == ControlType.AppRootChef);
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

        #region PageDispatch  =================================================
        private UIRequest _uiRequest;
        internal UIRequest UIRequest { get { return _uiRequest; } set { if (_uiRequest == null && value != null) _uiRequest = value; } }
        
        internal void PageDispatch()
        {
            if (_uiRequest != null)
            {
                var request = _uiRequest;
                _uiRequest = null;

                PageControl.Dispatch(request);
            }
        }
        #endregion

        #region CalledFromUI  =================================================
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

        public UIRequest BuildViewRequest() => UIRequest.CreateNewView(ViewControl.ControlType, Trait, Chef, Item1, Item2, Item3, _getData, true);

        #endregion
    }
}