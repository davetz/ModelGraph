using System;

namespace ModelGraphLibrary
{/*

 */
    internal class ModelCommand
    {
        internal Chef Chef;
        internal ItemModel Model;
        internal Action<ItemModel> Action;
        internal Action<ItemModel, Object> Action1;
        internal Object Parameter1;
        internal string Name;
        internal string Summary;
        internal Trait Trait;

        internal ModelCommand(Chef chef, ItemModel model, Trait trait, Action<ItemModel> action)
        {
            Chef = chef;
            Model = model;
            Trait = trait;
            Action = action;
            Name = chef.GetName(trait);
            Summary = chef.GetSummary(trait); ;
        }
        internal ModelCommand(Chef chef, ItemModel model, Trait trait, Action<ItemModel, Object> action)
        {
            Chef = chef;
            Model = model;
            Trait = trait;
            Action1 = action;
            Name = chef.GetName(trait);
            Summary = chef.GetSummary(trait); ;
        }

        internal void Execute()
        {
            if (Model == null || Model.IsInvalid) return;

           if (IsInsertCommand) Model.IsExpandedLeft = true;
           Chef.PostExecuteCommand(this);
        }

        #region Trait  ========================================================
        internal bool IsStorageFileParameter1 => (Trait & Trait.GetStorageFile) != 0;
        internal bool IsSaveAsCommand => (Trait & Trait.KeyMask) == (Trait.SaveAsCommand & Trait.KeyMask);
        internal bool IsInsertCommand => (Trait == Trait.InsertCommand);
        internal bool IsRemoveCommand => (Trait == Trait.RemoveCommand);
        #endregion
    }
}

