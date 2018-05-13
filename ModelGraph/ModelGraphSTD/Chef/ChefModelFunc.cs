using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    struct ChefModelFunc
    {
        public Action<RootModel, ItemModel> DragOver;
        public Action<RootModel, ItemModel> PointerOver;
        public Action<RootModel, ItemModel> ModelSelect;
        public Action<RootModel, ItemModel> ModelRefresh;
        public Action<RootModel, ItemModel> ModelValidate;

        public Func<ItemModel, (string Kind, string Name, int Count)> GetData;
        public Func<ItemModel, Item> GetParent;
        public Func<ItemModel, Property> GetProperty;
        public Func<ItemModel, Relation> GetRelation;

        public Func<ItemModel, bool> HasPropertyList;
        public Func<ItemModel, ItemModel[]> GetPropertyList;
    }
}
