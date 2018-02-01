using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    Data flow and control between ModelGraphSTD <--> ModelGraph

    The ModelGraphSTD has no direct knowledge of the UI controls, however, it does 
    initiates UI action through the interfaces IPageControl and IModelControl.

    The UI controls, on the other hand, do have direct access to the public methods and
    properties of ModelGraphSTD objects. This is especially important for the case
    of the ModelGraphControl.
 */
    public class ModelRoot : ModelTree
    {
        public Chef Chef;
        public IPageControl PageControl { get; set; } // reference the UI PageControl
        public IModelControl ModelControl { get; set; }
        
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

        public Func<ModelTree, ModelTree, bool, DropAction> ModelDrop;
        public Func<ModelTree, ModelTree, bool, DropAction> ReorderItems;

        public int ViewIndex1; //index of first visible model
        public int ViewIndex2; //index one beyond the last visible model
        public int ViewCapacity; //max number of visible models
        public ModelTree ViewSelectModel; //currently selected model
        public ModelTree[] ViewModels; //flattend list of itemModel tree
        public Dictionary<object, string> ViewFilter = new Dictionary<object, string>();

        public ModelAction ModelAction;

        private ModelTree[] _modelList;
        public ModelTree[] ModelList { get { return _modelList; } set { if (_modelList == null) _modelList = value; } }
        public int Capacity { get; set; } = 100;

        public bool ModelIsChecked;

        public ControlType ControlType;

        #region Constructors  =================================================
        // AppRootChef: Created by PageControl.xaml.cs
        public ModelRoot()
            : base(null, Trait.RootChef_M, 0, new Chef())
        {
            Chef = Item1 as Chef;
            Chef.AddRootModel(this);

            _getData = Chef.RootChef_M;
            ControlType = ControlType.AppRootChef;

            IsExpandedLeft = true;
        }

        // (Primary & Secondary) RootModels: Created by PrimaryRoot
        public ModelRoot(UIRequest rq)
            : base(null, rq.Trait, 0, rq.Item1, rq.Item2, rq.Item3, rq.GetData)
        {
            Chef = rq.Chef;
            ControlType = rq.Type;

            Chef.AddRootModel(this);

            IsExpandedLeft = true;
        }
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
        public void GetDragOverData(ModelTree model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.DragOver;
            ModelDrop = ReorderItems = null;

            model.GetData(this);
        }

        // ModelAction.PointerOver  ===========================================
        public void GetPointerOverData(ModelTree model)
        {
            if (model == null | model.IsInvalid) return;

            ModelAction = ModelAction.PointerOver;
            ModelSummary = null;

            model.GetData(this);

            if (string.IsNullOrEmpty(ModelSummary))
                ModelSummary = model.ModelIdentity;
        }

        // ModelAction.ModelSelect  ===========================================
        public void GetModelSelectData(ModelTree model)
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
        public void GetModelItemData(ModelTree model)
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

        public void PostModelDrop(ModelTree model)
        {
            if (model == null | model.IsInvalid) return;

            var drop = Chef.DragDropSource;
            if (drop == null) return;

            if (model.IsSiblingModel(drop))
                Chef.PostDataAction(this, () => { ReorderItems?.Invoke(model, drop, true); });
            else
                Chef.PostDataAction(this, () => { ModelDrop?.Invoke(model, drop, true); });
        }
        public void SetDragDropSource(ModelTree model)
        {
            if (model == null | model.IsInvalid) return;

            Chef.DragDropSource = model;
        }

        public DropAction CanModelAcceptDrop(ModelTree model)
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
            if (PageControl != null)
            {
                if (_uiRequest == null)
                {
                    if (MajorDelta != Chef.MajorDelta || MinorDelta != Chef.MinorDelta)
                    {
                        PageControl.Dispatch(UIRequest.Refresh(this));
                    }
                }
                else
                {
                    var request = _uiRequest;
                    _uiRequest = null;

                    PageControl.Dispatch(request);
                }
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

        public void PostSetValue(ModelTree model, string value)
        {
            Chef.PostModelSetValue(model, value);
        }
        public void PostSetIsChecked(ModelTree model, bool value)
        {
            Chef.PostModelSetIsChecked(model, value);
        }
        public void PostSetValueIndex(ModelTree model, int value)
        {
            model.GetData(this);
            Chef.PostModelSetValueIndex(model, value);
        }
        #endregion
    }
}
