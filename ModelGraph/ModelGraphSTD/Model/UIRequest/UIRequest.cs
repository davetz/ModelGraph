
namespace ModelGraphSTD
{/*
    This is how the ModelGraphSTD requests UI control action.
 */
    public class UIRequest
    {
        // parameters for new view
        public RootModel Root { get; private set; }
        public Chef Chef { get; private set; }
        public Item Item { get; private set; }
        public Item Aux1 { get; private set; }
        public Item Aux2 { get; private set; }
        public ControlType Type { get; private set; }
        internal ModelAction Get { get; private set; }
        public Trait Trait { get; private set; }

        public RequestType RequestType;

        private UIRequest() { }

        public static UIRequest SaveModel()
        {
            return new UIRequest()
            {
                RequestType = RequestType.Save
            };
        }
        public static UIRequest CloseModel()
        {
            return new UIRequest()
            {
                RequestType = RequestType.Close
            };
        }
        public static UIRequest ReloadModel()
        {
            return new UIRequest()
            {
                RequestType = RequestType.Reload
            };
        }
        public static UIRequest RefreshModel()
        {
            return new UIRequest()
            {
                RequestType = RequestType.Refresh
            };
        }
        internal static UIRequest CreateView(RootModel root, ControlType type, Trait trait, Chef chef, Item item, Item aux1, Item aux2, ModelAction modelAction, bool openInNewPage = false)
        {
            return new UIRequest()
            {
                Root = root,
                Chef = chef,
                Type = type,
                Trait = trait,
                Item = item,
                Aux1 = aux1,
                Aux2 = aux2,
                Get = modelAction,
                RequestType = (openInNewPage) ? RequestType.CreatePage : RequestType.CreateView,
            };
        }
    }
}
