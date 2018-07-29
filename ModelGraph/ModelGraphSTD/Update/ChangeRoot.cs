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

        private bool TryMerge(ChangeSet chg, bool onlyTestMerge = false)
        {
            if (chg.IsCongealed) return false;

            var index = IndexOf(chg);
            if (index < 0) return false;
            var prev = index - 1;
            if (prev < 0) return false;

            var chg2 = Items[prev] as ChangeSet;
            if (chg2.IsCongealed) return false;
            if (chg2.IsUndone != chg.IsUndone) return false;

            if (onlyTestMerge) return true;

            Remove(chg2);
            var items = chg2.Items;
            foreach (var item in items) { chg.Add(item); }
            chg2.RemoveAll();

            ModelDelta++;
            ChildDelta++;
            foreach (var cs in Items)
            {
                cs.ChildDelta++;
                cs.ModelDelta++;
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
                foreach (var chg in Items)
                {
                    chg.ModelDelta++;
                    if (chg.IsCongealed) continue;
                    if (chg.IsUndone)
                    {
                        Remove(chg);
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
