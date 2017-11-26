using ModelGraph.Helpers;
using System.Collections.Generic;

namespace ModelGraph.Internals
{/*

 */
    public partial class Chef
    {
        private string _changeRootInfoText;
        private Item _changeRootInfoItem;
        private int _changeRootInfoCount;

        internal int MinorDelta = 1; // incremented each time am item property changes
        internal int MajorDelta = 1; // incremented each time an item collection changes


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
        #endregion

        #region CongealChanges  ===============================================
        // Consolidate the current change items and freeze them so that the changes
        // can not be undone, also remove any change items wich have been undon. 
        private void CongealChanges()
        {
            _changeRoot.CongealChanges();
        }
        #endregion

        #region Expand All  ===================================================
        private void ExpandAllChangeSets(ModelTree model)
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
                var changeText = $"{item.KindKey.GetLocalized()}  {item.Name}";
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

                _changeRoot.Append(_changeSet);
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
            MajorDelta += 1;

            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $"{GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemMoved(_changeSet, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemMoved chng)
        {
            MajorDelta += 1;

            var item = chng.Item;
            var store = item.Owner as Store;
            store.Move(item, chng.Index1);
        }

        internal void Redo(ItemMoved chng)
        {
            MajorDelta += 1;

            var item = chng.Item;
            var store = item.Owner as Store;
            store.Move(item, chng.Index2);
        }
        #endregion

        #region ItemCreated  ==================================================
        internal void ItemCreated(Item item)
        {
            MajorDelta += 1;

            string name = GetIdentity(item, IdentityStyle.ChangeLog);
            var store = item.Owner as Store;

            if (item.IsRowX)
            {
                var row = item as RowX;
                var tbl = item.Owner as TableX;

                var N = TableX_ColumnX.ChildCount(tbl);
                if (N > 0)
                {
                    var cols = TableX_ColumnX.GetChildren(tbl);
                    string[] vals = new string[N];
                    for (int i = 0; i < N; i++)
                    {
                        vals[i] = cols[i].Value.GetString(row);
                    }
                    _changeSet.IsReversed = true; //we want the order of create and remove to look consistant
                    new ItemCreated(_changeSet, item, store.IndexOf(item), name, cols, vals);
                    return;
                }
            }
            new ItemCreated(_changeSet, item, store.IndexOf(item), name);
        }
        internal void Undo(ItemCreated chng)
        {
            MajorDelta += 1;

            var item = chng.Item;

            if (item.IsRowX)
            {
                var row = item as RowX;
                var tbl = row.Owner as TableX;

                tbl.Remove(row);

                var cols = TableX_ColumnX.GetChildren(tbl);
                if (cols != null) { foreach (var col in cols) { col.Value.Remove(row); } }
            }
            else
            {
                var store = item.Owner as Store;
                store.Remove(item);
            }
        }

        internal void Redo(ItemCreated chng)
        {
            MajorDelta += 1;

            var item = chng.Item;
            var index = chng.Index;

            if (item.IsRowX)
            {
                var row = item as RowX;
                var tbl = row.Owner as TableX;

                tbl.Insert(row, index);
                if (chng.Columns != null)
                {
                    var N = chng.Columns.Length;
                    for (int i = 0; i < N; i++)
                    {
                        chng.Columns[i].Value.SetString(row, chng.Values[i]);
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
        private void SetValue(ModelTree model, string newValue)
        {
            MinorDelta += 1;

            var itm1 = model.Item1;
            var itm2 = model.Item2;
            var prop = itm2 as Property;
            var oldValue = prop.Value.GetString(itm1);

            string name = $"{GetIdentity(itm1, IdentityStyle.ChangeLog)}    {GetIdentity(itm2, IdentityStyle.Single)}:  old<{oldValue}>  new<{newValue}>";


            if (IsSameValue(oldValue, newValue)) return;

            if (prop.Value.SetString(itm1, newValue))
            {
                ItemUpdated(itm1, name, prop, oldValue, newValue);
            }
        }
        internal void ItemUpdated(Item item, string name, Property property, string oldValue, string newValue)
        {
            new ItemUpdated(_changeSet, item, property, oldValue, newValue, name);
        }
        internal void Undo(ItemUpdated chng)
        {
            if (chng.Item.IsValid && chng.CanUndo && chng.Property.Value.SetString(chng.Item, chng.OldValue))
            {
                MinorDelta += 1;

                chng.IsUndone = true;
            }
        }

        internal void Redo(ItemUpdated chng)
        {
            if (chng.Item.IsValid && chng.CanRedo && chng.Property.Value.SetString(chng.Item, chng.NewValue))
            {
                MinorDelta += 1;

                chng.IsUndone = false;
            }
        }
        #endregion

        #region ItemRemoved  ==================================================
        internal void MarkItemRemoved(Item item)
        {
            MajorDelta += 1;

            var sto = item.Owner as Store;
            if (sto == null) return;

            var inx = (sto == null) ? -1 : sto.IndexOf(item);
            var name = GetIdentity(item, IdentityStyle.ChangeLog);

            if (item.IsRowX && TableX_ColumnX.HasChildLink(sto))
            {
                var columns = TableX_ColumnX.GetChildren(sto);
                string[] values;
                var N = columns.Length;
                values = new string[N];
                for (int i = 0; i < N; i++)
                {
                    var col = columns[i];
                    values[i] = col.Value.GetString(item);
                }
                new ItemRemoved(_changeSet, item, inx, name, columns, values);
            }
            else
                new ItemRemoved(_changeSet, item, inx, name);
        }
        internal void Redo(ItemRemoved cg)
        {
            MajorDelta += 1;

            var itm = cg.Item;
            var sto = itm.Store;
            (sto).Remove(itm);

            var N = (cg.Columns != null) ? cg.Columns.Length : 0;
            for (int i = 0; i < N; i++) { cg.Columns[i].Value.Remove(itm); }

            itm.IsDeleted = true;
            cg.IsUndone = false;
        }
        internal void Undo(ItemRemoved cg)
        {
            MajorDelta += 1;

            var itm = cg.Item;
            var sto = cg.Store;
            sto.Insert(itm, cg.Index);

            var N = (cg.Columns != null) ? cg.Columns.Length : 0;
            for (int i = 0; i < N; i++) { cg.Columns[i].Value.SetString(itm, cg.Values[i]); }

            cg.Item.IsDeleted = false;
            cg.IsUndone = true;
        }
        #endregion

        #region ItemLinked  ===================================================
        internal void ItemLinked(Relation relation, Item item1, Item item2)
        {
            MajorDelta += 1;

            var nam1 = GetIdentity(item1, IdentityStyle.ChangeLog);
            var nam2 = GetIdentity(item2, IdentityStyle.ChangeLog);
            var rnam = GetIdentity(relation, IdentityStyle.ChangeLog);

            var name = $"{rnam}  ;  ({nam1}) --> ({nam2})";
            (int parentIndex, int chilldIndex) = relation.AppendLink(item1, item2);
            var chg = new ItemLinked(_changeSet, relation, item1, item2, parentIndex, chilldIndex, name);
        }
        internal void Undo(ItemLinked chng)
        {
            MajorDelta += 1;

            chng.Relation.RemoveLink(chng.Parent, chng.Child);
        }

        internal void Redo(ItemLinked chng)
        {
            MajorDelta += 1;

            chng.Relation.AppendLink(chng.Parent, chng.Child);
        }

        #endregion

        #region ItemUnlinked  =================================================
        internal void MarkItemUnlinked(Relation rel, Item item1, Item item2)
        {
            MajorDelta += 1;

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
            MajorDelta += 1;

            cg.Relation.RemoveLink(cg.Parent, cg.Child);
            cg.IsUndone = false;
        }

        internal void Undo(ItemUnLinked cg)
        {
            MajorDelta += 1;

            cg.Relation.InsertLink(cg.Parent, cg.Child, cg.ParentIndex, cg.ChildIndex);
            cg.IsUndone = true;
        }
        #endregion

        #region ItemChildMoved  ===============================================
        internal void ItemChildMoved(Relation relation, Item key, Item item, int index1, int index2)
        {
            MajorDelta += 1;

            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $" [{GetIdentity(relation, IdentityStyle.Single)}]     {GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemChildMoved(_changeSet, relation, key, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemChildMoved chng)
        {
            MajorDelta += 1;

            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index1);
        }

        internal void Redo(ItemChildMoved chng)
        {
            MajorDelta += 1;

            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index2);
        }
        #endregion

        #region ItemParentMoved  ==============================================
        internal void ItemParentMoved(Relation relation, Item key, Item item, int index1, int index2)
        {
            MajorDelta += 1;

            var n1 = index1 + 1;
            var n2 = index2 + 1;
            var name = $" [{GetIdentity(relation, IdentityStyle.Single)}]     {GetIdentity(item, IdentityStyle.Double)}     {n1.ToString()}->{n2.ToString()}";
            var chg = new ItemParentMoved(_changeSet, relation, key, item, index1, index2, name);
            Redo(chg);
        }
        internal void Undo(ItemParentMoved chng)
        {
            MajorDelta += 1;

            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index1);
        }

        internal void Redo(ItemParentMoved chng)
        {
            MajorDelta += 1;

            chng.Relation.MoveChild(chng.Key, chng.Item, chng.Index2);
        }
        #endregion

        #region RemoveItem  ===================================================
        /// <summary>
        /// Remove and Unlink the target item and all of its dependents 
        /// </summary>
        internal void RemoveItem(ModelTree model)
        {
            RemoveItem(model.Item1);
        }
        private void RemoveItem(Item target)
        {
            var hitList = new List<Item>();
            FindDependents(target, hitList);
            hitList.Reverse();

            Item[] items;
            Item[] parents;
            Item[] children;

            var relItems = new Dictionary<Relation, Dictionary<Item, List<Item>>>();

            foreach (var item in hitList)
            {
                if (item is Relation r)
                {
                    var N = r.GetLinks(out parents, out children);
                    for (int i = 0; i < N; i++)
                    {
                        var item1 = parents[i];
                        var item2 = children[i];
                        if (IsAreadyRemoved(r, item1, item2, relItems)) continue;
                        MarkItemUnlinked(r, item1, item2);
                    }
                }
                if (TryGetParentRelations(item, out Relation[] relations))
                {
                    foreach (var rel in relations)
                    {
                        if (rel.TryGetParents(item, out items))
                        {
                            foreach (var parent in items)
                            {
                                if (IsAreadyRemoved(rel, parent, item, relItems)) continue;
                                MarkItemUnlinked(rel, parent, item);
                            }
                        }
                    }
                }
                if (TryGetChildRelations(item, out relations))
                {
                    foreach (var rel in relations)
                    {
                        if (rel.TryGetChildren(item, out items))
                        {
                            foreach (var child in items)
                            {
                                if (IsAreadyRemoved(rel, item, child, relItems)) continue;
                                MarkItemUnlinked(rel, item, child);
                            }
                        }
                    }
                }
            }

            foreach (var item in hitList) { MarkItemRemoved(item); }
            Redo(_changeSet);
        }
        private bool IsAreadyRemoved(Relation rel, Item item1, Item item2, Dictionary<Relation, Dictionary<Item, List<Item>>> relItems)
        {
            List<Item> items;
            Dictionary<Item, List<Item>> itemItems;

            if (relItems.TryGetValue(rel, out itemItems))
            {
                if (itemItems.TryGetValue(item1, out items))
                {
                    if (items.Contains(item2)) return true;
                    items.Add(item2);
                }
                else
                {
                    items = new List<Item>(2);
                    items.Add(item2);
                    itemItems.Add(item1, items);
                }
            }
            else
            {
                itemItems = new Dictionary<Item, List<Item>>(4);
                items = new List<Item>(2);
                items.Add(item2);
                itemItems.Add(item1, items);
                relItems.Add(rel, itemItems);
            }
            return false;
        }
        /// <summary>
        /// Recursively find all of the target's dependents
        /// </summary>
        private void FindDependents(Item target, List<Item> hitList)
        {
            Item[] children;
            Relation[] relations;

            hitList.Add(target);
            if (target is Store store)
            {
                var items = store.GetItems();
                foreach (var item in items) FindDependents(item, hitList);
            }
            if (TryGetChildRelations(target, out relations))
            {
                foreach (var rel in relations)
                {
                    if (rel.IsRequired && rel.TryGetChildren(target, out children))
                    {
                        foreach (var child in children)
                        {
                            FindDependents(child, hitList);
                        }
                    }
                }
            }
        }
        #endregion

        #region MutuallyExclusive  ============================================
        internal void RemoveMutuallyExclusiveLinks(Relation relation, Item parent, Item child)
        {
        }
        #endregion
    }
}
