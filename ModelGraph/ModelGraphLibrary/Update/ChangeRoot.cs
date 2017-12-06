namespace ModelGraphLibrary
{/*

 */
    public class ChangeRoot : StoreOf<ChangeSet>
    {
        #region Constructor  ==================================================
        internal ChangeRoot(Chef owner)
        {
            Owner = owner;
            Trait = Trait.ChangeRoot;

            if (owner.IsRootChef) return;
            owner.Append(this); // we want to be in the dataChef's item tree hierarchy
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal bool CanMerge(ChangeSet chg) { return TryMerge(chg, true); }
        internal void Mege(ChangeSet chg) { TryMerge(chg); }

        internal bool CanUndo(ChangeSet chg)
        {
            var last = Count - 1;
            for (int i = last; i >= 0; i--)
            {
                var item = Items[i] as ChangeSet;
                if (item == chg)
                    return item.CanUndo;
                else if (!item.CanRedo)
                    return false;
            }
            return false;
        }

        internal bool CanRedo(ChangeSet chg)
        {
            var last = Count - 1;
            for (int i = last; i >= 0; i--)
            {
                var item = Items[i] as ChangeSet;
                if (item == chg)
                    return item.CanRedo;
                else if (!item.CanUndo)
                    return false;
            }
            return false;
        }

        private bool TryMerge(ChangeSet chg, bool onlyTestMerge = false)
        {
            if (chg.IsCongealed) return false;

            var index = _items.IndexOf(chg);
            if (index < 0) return false;
            var prev = index - 1;
            if (prev < 0) return false;

            var chg2 = Items[prev] as ChangeSet;
            if (chg2.IsCongealed) return false;
            if (chg2.IsUndone != chg.IsUndone) return false;

            if (onlyTestMerge) return true;

            _items.Remove(chg2);
            var items = chg2.Items;
            foreach (var item in items) { chg.Append(item); }
            chg2.RemoveAll();

            return true;
        }

        internal void CongealChanges()
        {
            if (Count > 0)
            {
                ChangeSet save = null;
                foreach (var item in Items)
                {
                    var chg = item as ChangeSet;
                    if (chg.IsCongealed) continue;
                    if (chg.IsUndone)
                    {
                        _items.Remove(chg);
                    }
                    else
                        save = chg;
                }
                if (save != null)
                {
                    while (TryMerge(save)) { }
                    save.IsCongealed = true;
                }
            }
        }

        internal void AutoExpandChanges()
        {
            foreach (var item in Items)
            {
                var chg = item as ChangeSet;
                if (chg.IsCongealed) break;
                if (!chg.IsVirgin) break;
                chg.AutoExpandLeft = true;
            }
        }
        #endregion
    }
}
