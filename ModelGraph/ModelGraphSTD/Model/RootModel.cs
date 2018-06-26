using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*
    The ModelGraphSTD has no direct knowledge of the UI controls, however, it does 
    initiates UI action through the interfaces IPageControl and IModelControl.

    The UI controls, on the other hand, do have direct access to the public methods and
    properties of ModelGraphSTD objects. This is especially important for the case
    of the ModelGraphControl.
 */
    public class RootModel : ItemModel
    {
        public Chef Chef { get; private set; }
        public IPageControl PageControl { get; set; } // reference the UI PageControl
        public IModelControl ModelControl { get; set; }

        public int ViewCapacity = 10; // initial minimum capacity
        public ItemModel SelectModel;
        public List<ItemModel> ViewModels = new List<ItemModel>();

        private readonly ConcurrentQueue<UIRequest> _requestQueue = new ConcurrentQueue<UIRequest>();

        public int MinorDelta;
        public int MajorDelta;

        public ControlType ControlType;

        #region Constructors  =================================================
        // AppRootChef: Created by PageControl.xaml.cs
        public RootModel()
        { 
            Trait = Trait.RootChef_M;
            Item = Chef = new Chef();
            Get = Chef.RootChef_X;
            Chef.AddRootModel(this);

            ControlType = ControlType.AppRootChef;
        }

        // (Primary & Secondary) RootModels: Created by PrimaryRoot
        public RootModel(UIRequest rq)
            : base(null, rq.Trait, 0, rq.Item, rq.Aux1, rq.Aux2, rq.Get)
        {
            Get = rq.Get;
            Chef = rq.Root.Chef;
            ControlType = rq.Type;
            if (rq.Item is Chef newChef) Chef = newChef;

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
        public void Reload()
        {
        }
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
