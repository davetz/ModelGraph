using System;

namespace ModelGraph.Internals
{/*

 */
    public class ModelCommand
    {
        public Chef Chef;
        public ModelTree Model;
        public Action<ModelTree> Action;
        public Action<ModelTree, Object> Action1;
        public Object Parameter1;
        public string Name;
        public string Summary;
        internal Trait Trait;

        public ModelCommand(Chef chef, ModelTree model, Trait trait, Action<ModelTree> action)
        {
            Chef = chef;
            Model = model;
            Trait = trait;
            Action = action;
            Name = chef.GetName(trait);
            Summary = chef.GetSummary(trait); ;
        }
        public ModelCommand(Chef chef, ModelTree model, Trait trait, Action<ModelTree, Object> action)
        {
            Chef = chef;
            Model = model;
            Trait = trait;
            Action1 = action;
            Name = chef.GetName(trait);
            Summary = chef.GetSummary(trait); ;
        }

        public void Execute()
        {
            if (Model == null || Model.IsInvalid) return;

           if (IsInsertCommand) Model.IsExpandedLeft = true;
           Chef.PostExecuteCommand(this);
        }

        #region Trait  ========================================================
        public bool IsStorageFileParameter1 => (Trait & Trait.GetStorageFile) != 0;
        public bool IsSaveAsCommand => (Trait & Trait.KeyMask) == (Trait.SaveAsCommand & Trait.KeyMask);
        public bool IsInsertCommand => (Trait == Trait.InsertCommand);
        public bool IsRemoveCommand => (Trait == Trait.RemoveCommand);
        #endregion
    }
}

