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
        public Chef Chef;
        public IPageControl PageControl { get; set; } // reference the UI PageControl
        public IModelControl ModelControl { get; set; }

        public readonly ConcurrentQueue<UIRequest> UIRequestQueue = new ConcurrentQueue<UIRequest>();

        public int ViewCapacity = 10; // initial minimum capacity
        public ItemModel SelectModel;
        public List<ItemModel> ViewModels = new List<ItemModel>();

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
            Chef = rq.Chef;
            Get = rq.Get;
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

        #region PageDispatch  =================================================
        internal void PageDispatch()
        {
            if (PageControl != null)
            {
                while (UIRequestQueue.TryDequeue(out UIRequest request))
                {
                    PageControl.Dispatch(request);
                }
            }
        }
        #endregion
    }
}
