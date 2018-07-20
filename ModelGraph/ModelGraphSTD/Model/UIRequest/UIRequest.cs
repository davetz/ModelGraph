
namespace ModelGraphSTD
{/*
    This is how the ModelGraphSTD requests UI control action.
 */
    public class UIRequest
    {
        // parameters for new view
        public RootModel Root { get; private set; }
        internal Item Item { get; private set; }
        internal Item Aux1 { get; private set; }
        internal Item Aux2 { get; private set; }
        public ControlType Type { get; private set; }
        internal ModelAction Get { get; private set; }
        internal Trait Trait { get; private set; }
        public RequestType RequestType { get; private set; }

        private UIRequest() { }

        internal static UIRequest SaveModel(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.Save
            };
        }
        internal static UIRequest CloseModel(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.Close
            };
        }
        internal static UIRequest ReloadModel(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.Reload
            };
        }
        internal static UIRequest SaveSymbol(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.SaveSymbol
            };
        }
        internal static UIRequest ReloadSymbol(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.ReloadSymbol
            };
        }
        internal static UIRequest RefreshModel(RootModel root)
        {
            return new UIRequest()
            {
                Root = root,
                RequestType = RequestType.Refresh
            };
        }
        internal static UIRequest CreateView(RootModel root, ControlType type, Trait trait, Item item, ModelAction modelAction)
        {
            return new UIRequest()
            {
                Root = root,
                Type = type,
                Trait = trait,
                Item = item,
                Get = modelAction,
                RequestType = RequestType.CreateView,
            };
        }
        internal static UIRequest CreatePage(RootModel root, ControlType type, Trait trait, Item item, Item aux1, Item aux2, ModelAction modelAction)
        {
            return new UIRequest()
            {
                Root = root,
                Type = type,
                Trait = trait,
                Item = item,
                Aux1 = aux1,
                Aux2 = aux2,
                Get = modelAction,
                RequestType = RequestType.CreatePage,
            };
        }
    }
}
