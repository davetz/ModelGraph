using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ModelAction
    {
        internal Action<ItemModel> Validate;

        internal Func<ItemModel, int> ChildCount; // (1=) allow expand left but don't show the count

        internal Func<ItemModel, string> ModelKind;
        internal Func<ItemModel, string> ModelName;
        internal Func<ItemModel, string> ModelInfo;
        internal Func<ItemModel, string> ModelSummary;
        internal Func<ItemModel, string> ModelDescription;

        internal Func<ItemModel, int> IndexValue;
        internal Func<ItemModel, bool> BoolValue;
        internal Func<ItemModel, string> TextValue;
        internal Func<ItemModel, string[]> ListValue;

        internal Action<ItemModel, List<ModelCommand>> MenuCommands;
        internal Action<ItemModel, List<ModelCommand>> ButtonCommands;

        internal Func<ItemModel, ItemModel, bool, DropAction> ModelDrop;
        internal Func<ItemModel, ItemModel, bool, DropAction> ReorderItems;
    }
}
