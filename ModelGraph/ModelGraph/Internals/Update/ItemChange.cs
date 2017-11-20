using System;

namespace ModelGraph.Internals
{/*

 */
    public abstract class ItemChange : Item
    {
        internal string Name;

        #region Properties/Methods  ===========================================
        internal ChangeSet ChangeSet => Owner as ChangeSet;
        internal bool CanUndo => !IsUndone;
        internal bool CanRedo => IsUndone;
        #endregion
    }
}
