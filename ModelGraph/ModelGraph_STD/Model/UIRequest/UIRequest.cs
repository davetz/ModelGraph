using System;

namespace ModelGraphSTD
{/*
    This is how the ModelGraphSTD requests UI control action.
  
    rootModel.PageControl.Dispatch( UIRequest.Refresh(rootModel) );
 */
    public class UIRequest
    {
        public ModelRoot RootModel { get; private set; }

        // parameters for new view
        public Chef Chef { get; private set; }
        public Item Item1 { get; private set; }
        public Item Item2 { get; private set; }
        public Item Item3 { get; private set; }
        public ControlType Type { get; private set; }
        public Action<ModelTree, ModelRoot> GetData { get; private set; }
        public Trait Trait { get; private set; }

        public bool DoRefresh { get; private set; }
        public bool DoSaveModel { get; private set; }
        public bool DoCloseModel { get; private set; }
        public bool DoReloadModel { get; private set; }
        public bool DoCreateNewView { get; private set; }
        public bool DoCreateNewPage { get; private set; }

        #region Constructors  =================================================
        private UIRequest(ModelRoot model)
        {
            RootModel = model;
        }
        private UIRequest(ControlType type, Trait trait, Chef chef, Item item1, Item item2 = null, Item item3 = null, Action<ModelTree, ModelRoot> getData = null, bool openInNewPage = false)
        {
            Chef = chef;
            Type = type;
            Trait = trait;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            GetData = getData;
            DoCreateNewView = true;
            DoCreateNewPage = openInNewPage;
        }
        #endregion

        public static UIRequest Refresh(ModelRoot model)
        {
            return new UIRequest(model)
            {
                DoRefresh = true
            };
        }
        public static UIRequest SaveModel(ModelRoot model)
        {
            return new UIRequest(model)
            {
                DoSaveModel = true
            };
        }
        public static UIRequest CloseModel(ModelRoot model)
        {
            return new UIRequest(model)
            {
                DoCloseModel = true
            };
        }
        public static UIRequest ReloadModel(ModelRoot model)
        {
            return new UIRequest(model)
            {
                DoReloadModel = true
            };
        }
        public static UIRequest CreateNewView(ControlType type, Trait trait, Chef chef, Item item1, Item item2 = null, Item item3 = null, Action<ModelTree, ModelRoot> getData = null, bool openInNewPage = false)
        {
            return new UIRequest(type, trait, chef, item1, item2, item3, getData, openInNewPage);
        }
    }
}
