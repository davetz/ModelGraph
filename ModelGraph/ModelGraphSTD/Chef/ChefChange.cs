using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private string _changeRootInfoText;
        private Item _changeRootInfoItem;
        private int _changeRootInfoCount;

        #region IsSameValue  ==================================================
        private bool IsSameValue(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a))
            {
                if (string.IsNullOrWhiteSpace(b)) return true;
                return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(b)) return false;
                return (string.Compare(a, b) == 0);
            }
        }
        private bool IsNotSameValue(string a, string b) => !IsSameValue(a, b);
        #endregion

        #region CongealChanges  ===============================================
        // Consolidate the current change items and freeze them so that the changes
        // can not be undone, also remove any change items wich have been undon. 
        private void CongealChanges()
        {
            _changeRoot.CongealChanges();
            _changeRoot.ChildDelta++;
        }
        #endregion

        #region Expand All  ===================================================
        private void ExpandAllChangeSets(ItemModel model)
        {
            if (_changeRoot == null) return;
            _changeRoot.AutoExpandChanges();
            model.IsExpandedLeft = true;
        }
        #endregion

        #region ChangeSet  ====================================================
        internal void CheckChanges()
        {
            if (_changeSet.Count > 0)
            {
                var item = _changeSet.Items[_changeSet.Count - 1];
                var changeText = $"{_localize(item.KindKey)}  {item.Name}";
                if (_changeRootInfoItem != null && item.Trait == _changeRootInfoItem.Trait)
                    _changeRootInfoCount += 1;
                else
                {
                    _changeRootInfoItem = item;
                    _changeRootInfoCount = 1;
                }
                if (_changeRootInfoCount > 1)
                    _changeRootInfoText = $"{GetName(item.Trait)} ({_changeRootInfoCount.ToString()})  {changeText}";
                else
                    _changeRootInfoText = $"{GetName(item.Trait)}  {changeText}";

                _changeRoot.Add(_changeSet);
                _changeSequence += 1;
                _changeSet = new ChangeSet(_changeRoot, _changeSequence);
                ResetCacheValues();
                RefreshAllGraphs();
            }
            else
            {
                _changeRootInfoText = string.Empty;
            }
        }

        internal bool CanDelete(ChangeSet chng)
        {
            var items = chng.Items;
            foreach (var item in items)
            {
                if (!(item as ItemChange).IsUndone)
                    return false;
            }
            return true;
        }
        internal void Delete(ChangeSet chng)
        {
            _changeRoot.Remove(chng);
        }
        internal void Undo(ChangeSet chng)
        {
            var items = chng.Items;
            foreach (var item in items)
            {
                if (item.IsItemUpdated) Undo(item as ItemUpdated);
                else if (item.IsItemCreated) Undo(item as ItemCreated);
                else if (item.IsItemRemoved) Undo(item as ItemRemoved);
                else if (item.IsItemLinked) Undo(item as ItemLinked);
                else if (item.IsItemUnlinked) Undo(item as ItemUnLinked);
                else if (item.IsItemMoved) Undo(item as ItemMoved);
                else if (item.IsItemLinkMoved) Undo(item as ItemChildMoved);
            }
            chng.IsUndone = true;
        }

        internal void Redo(ChangeSet chng)
        {
            var items = chng.Items;
            foreach (var item in items)
            {
                if (item.IsItemUpdated) Redo(item as ItemUpdated);
                else if (item.IsItemCreated) Redo(item as ItemCreated);
                else if (item.IsItemRemoved) Redo(item as ItemRemoved);
                else if (item.IsItemLinked) Redo(item as ItemLinked);
                else if (item.IsItemUnlinked) Redo(item as ItemUnLinked);
                else if (item.IsItemMoved) Redo(item as ItemMoved);
                else if (item.IsItemLinkMoved) Redo(item as ItemChildMoved);
            }
            chng.IsUndone = false;
        }
        #endregion

        #region ItemMoved  ====================================================
        internal void ItemMoved(Item item, int index1, int index2)
        {
            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $"{GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemMoved(_changeSet, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemMoved chng)
        {
            var item = chng.Item;
            var store = item.Owner as Store;
            store.Move(item, chng.Index1);
        }

        internal void Redo(ItemMoved chng)
        {
            var item = chng.Item;
            var store = item.Owner as Store;
            store.Move(item, chng.Index2);
        }
        #endregion

        #region ItemCreated  ==================================================
        internal void ItemCreated(Item item)
        {
            string name = GetIdentity(item, IdentityStyle.ChangeLog);
            var store = item.Owner as Store;

            if (item.IsRowX)
            {
                var row = item as RowX;
                var tbl = item.Owner as TableX;

                if (TableX_ColumnX.TryGetChildren(tbl, out IList<ColumnX> cols))
                {
                    var vals = new List<string>(cols.Count);
                    foreach (var col in cols)
                    {
                        vals.Add(col.Value.GetString(row));
                    }
                    new ItemCreated(_changeSet, item, store.IndexOf(item), name, cols, vals);
                    return;
                }
            }
            new ItemCreated(_changeSet, item, store.IndexOf(item), name);
        }
        internal void Undo(ItemCreated chng)
        {
            var item = chng.Item;

            if (item.IsRowX)
            {
                var rx = item as RowX;
                var tx = rx.TableX;

                tx.Remove(rx);
                if (TableX_ColumnX.TryGetChildren(tx, out IList<ColumnX> lst)) { foreach (var cx in lst) { cx.Value.Remove(rx); } }
            }
            else
            {
                var store = item.Owner as Store;
                store.Remove(item);
            }
        }

        internal void Redo(ItemCreated chng)
        {
            var item = chng.Item;
            var index = chng.AtIndex;

            if (item.IsRowX)
            {
                var rx = item as RowX;
                var tx = rx.TableX;

                tx.Insert(rx, index);
                if (chng.Columns != null)
                {
                    var N = chng.Columns.Count;
                    for (int i = 0; i < N; i++)
                    {
                        chng.Columns[i].Value.SetString(rx, chng.Values[i]);
                    }
                }
            }
            else
            {
                var store = item.Owner as Store;
                store.Insert(item, index);
            }
        }
        #endregion

        #region ItemUpdated  ==================================================
        private void SetValue(ItemModel m, string newValue)
        {
            if (m.Property.IsCovert)
            {
                m.Property.Value.SetString(m.Item, newValue);
            }
            else
            {
                m.Item.ModelDelta++;
                var oldValue = m.Property.Value.GetString(m.Item);
                if (IsNotSameValue(oldValue, newValue))
                {
                    var name = $"{GetIdentity(m.Item, IdentityStyle.ChangeLog)}    {GetIdentity(m.Property, IdentityStyle.Single)}:  old<{oldValue}>  new<{newValue}>";
                    if (m.Property.Value.SetString(m.Item, newValue))
                    {
                        new ItemUpdated(_changeSet, m.Item, m.Property, oldValue, newValue, name);
                    }
                }
            }
        }
        internal void Undo(ItemUpdated chng)
        {
            if (chng.Item.IsValid && chng.CanUndo && chng.Property.Value.SetString(chng.Item, chng.OldValue))
            {
                chng.Item.ModelDelta++;
                chng.IsUndone = true;
            }
        }

        internal void Redo(ItemUpdated chng)
        {
            if (chng.Item.IsValid && chng.CanRedo && chng.Property.Value.SetString(chng.Item, chng.NewValue))
            {
                chng.Item.ModelDelta++;
                chng.IsUndone = false;
            }
        }
        #endregion

        #region ItemRemoved  ==================================================
        internal void MarkItemRemoved(Item item)
        {
            if (!(item.Owner is Store sto)) return;

            var inx = (sto == null) ? -1 : sto.IndexOf(item);
            var name = GetIdentity(item, IdentityStyle.ChangeLog);

            if (item.IsRowX && TableX_ColumnX.TryGetChildren(sto, out IList<ColumnX> cols))
            {
                var vals = new List<string>(cols.Count);
                foreach (var col in cols)
                {
                    vals.Add(col.Value.GetString(item));
                }
                new ItemRemoved(_changeSet, item, inx, name, cols, vals);
            }
            else
                new ItemRemoved(_changeSet, item, inx, name);
        }
        internal void Redo(ItemRemoved cg)
        {
            var itm = cg.Item;
            var sto = itm.Store;
            sto.Remove(itm);

            var N = (cg.Columns != null) ? cg.Columns.Count : 0;
            for (int i = 0; i < N; i++) { cg.Columns[i].Value.Remove(itm); }

            itm.IsDeleted = true;
            cg.IsUndone = false;
        }
        internal void Undo(ItemRemoved cg)
        {
            var itm = cg.Item;
            var sto = itm.Store;
            sto.Insert(itm, cg.AtIndex);

            var N = (cg.Columns != null) ? cg.Columns.Count : 0;
            for (int i = 0; i < N; i++) { cg.Columns[i].Value.SetString(itm, cg.Values[i]); }

            cg.Item.IsDeleted = false;
            cg.IsUndone = true;
        }
        #endregion

        #region ItemLinked  ===================================================
        internal void ItemLinked(Relation rel, Item item1, Item item2)
        {
            var nam1 = GetIdentity(item1, IdentityStyle.Double);
            var nam2 = GetIdentity(item2, IdentityStyle.Double);
            var rnam = GetIdentity(rel, IdentityStyle.Single);

            var name = $" [{rnam}]   ({nam1}) --> ({nam2})";
            (int parentIndex, int chilldIndex) = rel.AppendLink(item1, item2);
            var chg = new ItemLinked(_changeSet, rel, item1, item2, parentIndex, chilldIndex, name);
        }
        internal void Undo(ItemLinked chng)
        {
            chng.Relation.RemoveLink(chng.Parent, chng.Child);
        }

        internal void Redo(ItemLinked chng)
        {
            chng.Relation.AppendLink(chng.Parent, chng.Child);
        }

        #endregion

        #region ItemUnlinked  =================================================
        internal void MarkItemUnlinked(Relation rel, Item item1, Item item2)
        {
            (int parentIndex, int childIndex) = rel.GetIndex(item1, item2);
            if (parentIndex < 0 || childIndex < 0) return;

            var nam1 = GetIdentity(item1, IdentityStyle.Double);
            var nam2 = GetIdentity(item2, IdentityStyle.Double);
            var rnam = GetIdentity(rel, IdentityStyle.Single);

            var name = $" [{rnam}]   ({nam1}) --> ({nam2})";
            var chg = new ItemUnLinked(_changeSet, rel, item1, item2, parentIndex, childIndex, name);
        }

        internal void Redo(ItemUnLinked cg)
        {
            cg.Relation.RemoveLink(cg.Parent, cg.Child);
            cg.IsUndone = false;
        }

        internal void Undo(ItemUnLinked cg)
        {
            cg.Relation.InsertLink(cg.Parent, cg.Child, cg.ParentIndex, cg.ChildIndex);
            cg.IsUndone = true;
        }
        #endregion

        #region ItemChildMoved  ===============================================
        internal void ItemChildMoved(Relation relation, Item key, Item item, int index1, int index2)
        {
            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $" [{GetIdentity(relation, IdentityStyle.Single)}]     {GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemChildMoved(_changeSet, relation, key, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemChildMoved chng)
        {
            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index1);
        }

        internal void Redo(ItemChildMoved chng)
        {
            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index2);
        }
        #endregion

        #region ItemParentMoved  ==============================================
        internal void ItemParentMoved(Relation relation, Item key, Item item, int index1, int index2)
        {
            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $" [{GetIdentity(relation, IdentityStyle.Single)}]     {GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemParentMoved(_changeSet, relation, key, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemParentMoved chng)
        {
            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index1);
        }

        internal void Redo(ItemParentMoved chng)
        {
            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index2);
        }
        #endregion

        #region RemoveItem  ===================================================
        private void RemoveItem(Item target)
        {
            var relItems = new Dictionary<Relation, Dictionary<Item, List<Item>>>();
            var hitList = new List<Item>();
            FindDependents(target);
            hitList.Reverse();

            foreach (var item in hitList)
            {
                if (item is Relation r)
                {
                    var N = r.GetLinks(out List<Item> parents, out List<Item> children);

                    for (int i = 0; i < N; i++) { TryMarkItemUnlinked(r, parents[i], children[i]); }
                }
                if (TryGetParentRelations(item, out IList<Relation> relations))
                {
                    foreach (var rel in relations)
                    {
                        if (!rel.TryGetParents(item, out List<Item> parents)) continue;

                        foreach (var parent in parents) { TryMarkItemUnlinked(rel, parent, item); }
                    }
                }
                if (TryGetChildRelations(item, out relations))
                {
                    foreach (var rel in relations)
                    {
                        if (!rel.TryGetChildren(item, out List<Item> children)) continue;

                        foreach (var child in children) { TryMarkItemUnlinked(rel, item, child); }
                    }
                }
            }

            foreach (var item in hitList) { MarkItemRemoved(item); }
            Redo(_changeSet);

            #region PrivateMethods  ===========================================

            void FindDependents(Item target2)
            {
                hitList.Add(target2);
                if (target2 is Store store)
                {
                    var items = store.GetItems();
                    foreach (var item in items) FindDependents(item);
                }
                if (TryGetChildRelations(target2, out IList<Relation> relations))
                {
                    foreach (var rel in relations)
                    {
                        if (rel.IsRequired && rel.TryGetChildren(target2, out List<Item> children))
                        {
                            foreach (var child in children)
                            {
                                FindDependents(child);
                            }
                        }
                    }
                }
            }

            bool TryGetChildRelations(Item item, out IList<Relation> relations)
            {
                if (item.Owner is TableX tx)
                {
                    if (TableX_ChildRelationX.TryGetChildren(tx, out IList<RelationX> txRelations))
                    {
                        relations = new List<Relation>(txRelations);
                        return true;
                    }
                    relations = null;
                    return false;
                }
                return Store_ChildRelation.TryGetChildren(item.Owner, out relations);
            }

            bool TryGetParentRelations(Item item, out IList<Relation> relations)
            {
                if (item.Owner is TableX tx)
                {
                    if (TableX_ParentRelationX.TryGetChildren(tx, out IList<RelationX> txRelations))
                    {
                        relations = new List<Relation>(txRelations);
                        return true;
                    }
                    relations = null;
                    return false;
                }
                return Store_ParentRelation.TryGetChildren(item.Owner, out relations);
            }

            bool TryMarkItemUnlinked(Relation rel, Item item1, Item item2)
            {
                List<Item> items;

                if (relItems.TryGetValue(rel, out Dictionary<Item, List<Item>> itemItems))
                {
                    if (itemItems.TryGetValue(item1, out items))
                    {
                        if (items.Contains(item2)) return false;
                        items.Add(item2);
                    }
                    else
                    {
                        items = new List<Item>(2) { item2 };
                        itemItems.Add(item1, items);
                    }
                }
                else
                {
                    itemItems = new Dictionary<Item, List<Item>>(4);
                    items = new List<Item>(2) { item2 };
                    itemItems.Add(item1, items);
                    relItems.Add(rel, itemItems);
                }
                MarkItemUnlinked(rel, item1, item2);
                return true;
            }
            #endregion
        }
        #endregion

        #region MutuallyExclusive  ============================================
        internal void RemoveMutuallyExclusiveLinks(Relation relation, Item parent, Item child)
        {
        }
        #endregion
    }
}
