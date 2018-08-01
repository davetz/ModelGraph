namespace ModelGraphSTD
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
            owner.Add(this); // we want to be in the dataChef's item tree hierarchy
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
                var item = Items[i];
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
                var item = Items[i];
                if (item == chg)
                    return item.CanRedo;
                else if (!item.CanUndo)
                    return false;
            }
            return false;
        }

        private bool TryMerge(ChangeSet cs, bool testIfCanMerge = false)
        {
            if (cs.IsCongealed) return false;

            var index = IndexOf(cs);
            if (index < 0) return false;
            var prev = index - 1;
            if (prev < 0) return false;

            var cs2 = Items[prev];
            if (cs2.IsCongealed) return false;
            if (cs2.IsUndone != cs.IsUndone) return false;

            if (testIfCanMerge) return true;

            cs.Owner.ModelDelta++;

            Remove(cs2);
            var items = cs2.Items;
            foreach (var item in items) { cs.Add(item); }
            cs2.RemoveAll();

            foreach (var cs3 in Items)
            {
                cs3.ChildDelta++;
                cs3.ModelDelta++;
            }
            return true;
        }

        internal void CongealChanges()
        {
            if (Count > 0)
            {
                ModelDelta++;
                ChildDelta++;
                ChangeSet save = null;
                var last = Count - 1;
                for (int i = last; i >= 0; i--)
                {
                    var cs = Items[i];
                    if (cs.IsCongealed) continue;
                    if (cs.IsUndone)
                        Remove(cs);
                    else if (save == null)
                        save = cs;
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
            foreach (var chg in Items)
            {
                if (chg.IsCongealed) break;
                if (!chg.IsVirgin) break;
                chg.AutoExpandLeft = true;
                foreach (var item in chg.Items)
                {
                    item.AutoExpandLeft = true;
                }
            }
        }
        #endregion
    }
}
