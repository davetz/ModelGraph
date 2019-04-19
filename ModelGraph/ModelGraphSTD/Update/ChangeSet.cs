using System;

namespace ModelGraphSTD
{
    public class ChangeSet : StoreOf<ItemChange>
    {
        internal DateTime DateTime;
        internal int Sequence;

        #region Constructor  ==================================================
        internal ChangeSet(ChangeRoot owner, int seqno)
        {
            Owner = owner;
            Trait = Trait.ChangeSet;
            DateTime = DateTime.Now;
            Sequence = seqno;
            IsVirgin = true;
            SetCapacity(11);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal ChangeRoot ChangeRoot => Owner as ChangeRoot;
        internal bool CanUndo => (!IsCongealed && !IsUndone);
        internal bool CanRedo => (!IsCongealed && IsUndone);
        internal bool CanMerge => ChangeRoot.CanMerge(this); 
        internal void Merge() { ChangeRoot.Mege(this); }
        internal string Name => Sequence.ToString();
        #endregion
    }
}
