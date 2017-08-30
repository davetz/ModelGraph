using System;

namespace ModelGraphLibrary
{/*

 */
    public class ViewRequest
    {
        internal Trait Trait;
        public Chef Chef;
        public Item Item1;
        public Item Item2;
        public Item Item3;
        public ControlType Type;
        public Action<ItemModel, RootModel> GetData;
        public bool OpenInNewPage;

        public ViewRequest(ControlType type, Trait trait, Chef chef, Item item1, Item item2 = null, Item item3 = null, Action<ItemModel, RootModel> getData = null, bool openInNewPage = false)
        {
            Chef = chef;
            Type = type;
            Trait = trait;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            GetData = getData;
            OpenInNewPage = openInNewPage;
        }
    }
}
