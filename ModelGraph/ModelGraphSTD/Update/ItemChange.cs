
namespace ModelGraphSTD
{
    public abstract class ItemChange : Item
    {
        internal string Name;

        #region Properties/Methods  ===========================================
        internal ChangeSet ChangeSet => Owner as ChangeSet;
        internal bool CanUndo => !IsUndone;
        internal bool CanRedo => IsUndone;

        protected void UpdateDelta()
        {
            Owner.ModelDelta++;
            Owner.Owner.ModelDelta++;
        }
        #endregion
    }
}
