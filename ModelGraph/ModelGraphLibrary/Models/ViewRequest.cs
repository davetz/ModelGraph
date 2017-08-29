using System;

namespace ModelGraphLibrary
{/*

 */
    public class ViewRequest
    {
        internal Trait Trait;
        internal Chef Chef;
        internal Item Item1;
        internal Item Item2;
        internal Item Item3;
        internal ControlType Type;
        internal Action<ItemModel, RootModel> GetData;
        internal bool OpenInNewPage;

        internal ViewRequest(ControlType type, Trait trait, Chef chef, Item item1, Item item2 = null, Item item3 = null, Action<ItemModel, RootModel> getData = null, bool openInNewPage = false)
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
