using System;
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

        public int MinorDelta;
        public int MajorDelta;

        public int ViewIndex1; //index of first visible model
        public int ViewIndex2; //index one beyond the last visible model
        public int ViewCapacity; //max number of visible models
        public ItemModel ViewSelectModel; //currently selected model
        public ItemModel[] ViewModels; //flattend list of itemModel tree
        public Dictionary<object, string> ViewFilter = new Dictionary<object, string>();

        public ControlType ControlType;

        #region Constructors  =================================================
        // AppRootChef: Created by PageControl.xaml.cs
        public RootModel()
            : base(null, Trait.RootChef_M, 0, new Chef())
        {
            Chef = Item as Chef;
            Chef.AddRootModel(this);

            _getData = Chef.RootChef_M;
            ControlType = ControlType.AppRootChef;
        }

        // (Primary & Secondary) RootModels: Created by PrimaryRoot
        public RootModel(UIRequest rq)
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
    }
}
