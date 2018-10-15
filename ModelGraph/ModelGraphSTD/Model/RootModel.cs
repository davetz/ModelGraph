﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    ModelGraphSTD has no direct knowledge of the UI controls, however, it does 
    initiates UI action through the interfaces IPageControl and IModelControl.

    The UI controls, on the other hand, do have direct access to the public methods and
    properties of ModelGraphSTD objects. This is especially important for the case
    of the ModelGraphControl.
 */
    public class RootModel : ItemModel
    {
        public Chef Chef { get; private set; }
        public IPageControl PageControl { get; set; } // reference the UI PageControl

        // used by the ModelTreeControl
        public int ViewCapacity = 10; // updated when the view screen size changes
        public ItemModel SelectModel;   // the user selected model
        public List<ItemModel> ViewFlatList = new List<ItemModel>(); // flat list of models visible to the user 
        internal bool HasFlatList => ControlType == ControlType.PartialTree || ControlType == ControlType.PartialTree;

        private ConcurrentQueue<UIRequest> _requestQueue = new ConcurrentQueue<UIRequest>();

        public int MinorDelta;
        public int MajorDelta;

        public ControlType ControlType;

        #region Constructors  =================================================

        // Primary-RootModel: Created by ModelPageService
        public RootModel(IRepository repository = null)
        {
            Trait = Trait.DataChef_M;
            Item = Chef = new Chef(repository);
            Get = Chef.DataChef_X;

            ControlType = ControlType.PrimaryTree;
            Chef.AddRootModel(this);
        }
        // Secondary-RootModel: Created by Primary-RootModel
        public RootModel(UIRequest rq)
        {
            Trait = rq.Trait;
            Item = rq.Item;
            Aux1 = rq.Aux1;
            Aux2 = rq.Aux2;
            Get = rq.Get;
            Chef = rq.Root.Chef;
            ControlType = rq.Type;
            if (rq.Item is Chef newChef) Chef = newChef;

            Chef.AddRootModel(this);
            IsExpandedLeft = true;
        }
        public void Release()
        {
            ItemModel.Release(this);
            Chef?.Release();
            ViewFlatList.Clear();
            ViewFlatList = null;
            _requestQueue = null;
            PageControl = null;
            SelectModel = null;
            Chef = null;
        }
        #endregion

        #region Properties/Methods  ===========================================
        public bool IsAppRootChef => (ControlType == ControlType.AppRootChef);
        public string TitleName => Chef.GetAppTitleName(this);
        public string TabSummary => Chef.GetAppTabSummary(this);
        public string TitleSummary => Chef.GetAppTitleSummary(this);
        #endregion

        #region UIRequest  ====================================================
        internal void UIRequestSaveModel() => _requestQueue.Enqueue(UIRequest.SaveModel(this));
        internal void UIRequestCloseModel() => _requestQueue.Enqueue(UIRequest.CloseModel(this));
        internal void UIRequestReloadModel() => _requestQueue.Enqueue(UIRequest.ReloadModel(this));
        internal void UIRequestRefreshModel() => _requestQueue.Enqueue(UIRequest.RefreshModel(this));

        internal void UIRequestCreateView(ControlType type, Trait trait, Item item, ModelAction get) =>
            _requestQueue.Enqueue(UIRequest.CreateView(this, type, trait, item, get));

        internal void UIRequestCreatePage(ControlType type, ItemModel m) =>
            _requestQueue.Enqueue(UIRequest.CreatePage(this, type, m.Trait, m.Item, m.Aux1, m.Aux2, m.Get));

        internal void UIRequestCreatePage(ControlType type, Trait trait, ModelAction get, ItemModel m) =>
            _requestQueue.Enqueue(UIRequest.CreatePage(this, type, trait, m.Item, m.Aux1, m.Aux2, get));
        internal void UIRequestCreatePage(ControlType type, Trait trait, Item item, ModelAction get) =>
            _requestQueue.Enqueue(UIRequest.CreatePage(this, type, trait, item, null, null, get));


        internal void UIRequestSaveSymbol() => _requestQueue.Enqueue(UIRequest.SaveModel(this));
        internal void UIRequestReloadSymbol() => _requestQueue.Enqueue(UIRequest.ReloadModel(this));
        #endregion

        #region PageDispatch  =================================================
        internal void PageDispatch()
        {
            if (PageControl != null)
            {
                while (_requestQueue.TryDequeue(out UIRequest request))
                {
                    PageControl.Dispatch(request);
                }
            }
        }
        #endregion
    }
}
