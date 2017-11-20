using System;
using System.Linq;

namespace ModelGraph.Internals
{/*

 */
    public partial class Chef
    {
        private enum SubsetType
        {
            All,
            Used,
            Unused,
        }
        SubsetOf<ColumnX, Item> _inUseColumns = new SubsetOf<ColumnX, Item>((ColumnX col, Item key) => { return col.IsSpecific(key); });
        SubsetOf<Relation, Item> _inUseChildRelations = new SubsetOf<Relation, Item>( (Relation rel, Item key) => { return rel.HasKey1(key); });
        SubsetOf<Relation, Item> _inUseParentRelations = new SubsetOf<Relation, Item>((Relation rel, Item key) => { return rel.HasKey2(key); });

        #region InUseColumns  =================================================
        private int GetColumnCount(Item item, SubsetType type = SubsetType.All)
        {
            (int all, int used, int unused) = _inUseColumns.GetCount(item, TableX_ColumnX.GetChildren(item.Owner));

            switch (type)
            {
                case SubsetType.All:
                    return all;

                case SubsetType.Used:
                    return used;

                case SubsetType.Unused:
                    return unused;

                default:
                    return 0;
            }
        }
        private bool TryGetColumns(Item key, out ColumnX[] columns, SubsetType type = SubsetType.All)
        {
            columns = (key.IsRowX) ? TableX_ColumnX.GetChildren(key.Owner) : null;
            if (columns == null) return false;

            switch (type)
            {
                case SubsetType.All:
                    return true;

                case SubsetType.Used:
                    var u = _inUseColumns.GetIncluded(key, columns);
                    columns = u.Items;
                    return (u.Count > 0);

                case SubsetType.Unused:
                    var n = _inUseColumns.GetExcluded(key, columns);
                    columns = n.Items;
                    return (n.Count > 0);

                default:
                    return false;
            }
        }
        #endregion

        #region InUseChildRelation  ===========================================
        private int GetChildRelationCount(Item item, SubsetType type = SubsetType.All)
        {
            Relation[] relations;
            if (item.IsRowX)
                relations = TableX_ChildRelationX.GetChildren(item.Owner);
            else
                relations = Store_ChildRelation.GetChildren(item.Owner);

            (int all, int used, int unused) = _inUseChildRelations.GetCount(item, relations);

            switch (type)
            {
                case SubsetType.All:
                    return all;

                case SubsetType.Used:
                    return used;

                case SubsetType.Unused:
                    return unused;

                default:
                    return 0;
            }
        }
        private bool TryGetChildRelations(Item item, out Relation[] relations, SubsetType type = SubsetType.All)
        {
            relations = null;
            if (item.IsRowX)
                relations = TableX_ChildRelationX.GetChildren(item.Owner);
            else
                relations = Store_ChildRelation.GetChildren(item.Owner);
            if (relations == null) return false;

            switch (type)
            {
                case SubsetType.All:
                    return (relations != null);

                case SubsetType.Used:
                    var u = _inUseChildRelations.GetIncluded(item, relations);
                    relations = u.Items;
                    return (u.Count > 0);

                case SubsetType.Unused:
                    var n = _inUseChildRelations.GetExcluded(item, relations);
                    relations = n.Items;
                    return (n.Count > 0);

                default:
                    return false;
            }
        }
        #endregion

        #region InUseParentRelation  ==========================================
        private int GetParentRelationCount(Item item, SubsetType type = SubsetType.All)
        {
            Relation[] relations;
            if (item.IsRowX)
                relations = TableX_ParentRelationX.GetChildren(item.Owner);
            else
                relations = Store_ParentRelation.GetChildren(item.Owner);

            (int all, int used, int unused) = _inUseParentRelations.GetCount(item, relations);

            switch (type)
            {
                case SubsetType.All:
                    return all;

                case SubsetType.Used:
                    return used;

                case SubsetType.Unused:
                    return unused;

                default:
                    return 0;
            }
        }
        private bool TryGetParentRelations(Item item, out Relation[] relations, SubsetType type = SubsetType.All)
        {
            if (item.IsRowX)
                relations = TableX_ParentRelationX.GetChildren(item.Owner);
            else
                relations = Store_ParentRelation.GetChildren(item.Owner);
            if (relations == null) return false;


            switch (type)
            {
                case SubsetType.All:
                    return (relations != null);

                case SubsetType.Used:
                    var u = _inUseParentRelations.GetIncluded(item, relations);
                    relations = u.Items;
                    return (u.Count > 0);

                case SubsetType.Unused:
                    var n = _inUseParentRelations.GetExcluded(item, relations);
                    relations = n.Items;
                    return (n.Count > 0);

                default:
                    return false;
            }
        }
        #endregion

        internal void GetColumnCount(Item owner, out int used, out int unused)
        {
            GetUsedUnsedCount(owner.Owner, out used, out unused, TableX_ColumnX, (Item item) => { var col = item as ColumnX; return col.IsSpecific(owner); });
        }
        internal bool TryGetUsedColumns(Item owner, out Item[] items)
        {
            return TryGetUsed(owner.Owner, out items, TableX_ColumnX, (Item item) => { var col = item as ColumnX; return col.IsSpecific(owner); });
        }
        internal bool TryGetUnusedColumns(Item owner, out Item[] items)
        {
            return TryGetUnused(owner.Owner, out items, TableX_ColumnX, (Item item) => { var col = item as ColumnX; return col.IsSpecific(owner); });
        }

        internal void GetChildRelationCount(Item owner, out int used, out int unused)
        {
            GetUsedUnsedCount(owner.Owner, out used, out unused, TableX_ChildRelationX, (Item item) => { return (item as Relation).HasKey1(owner); });
        }
        internal bool TryGetUsedChildRelations(Item owner, out Item[] items)
        {
            return TryGetUsed(owner.Owner, out items, TableX_ChildRelationX, (Item item) => { return (item as Relation).HasKey1(owner); });
        }
        internal bool TryGetUnusedChildRelations(Item owner, out Item[] items)
        {
            return TryGetUnused(owner.Owner, out items, TableX_ChildRelationX, (Item item) => { return (item as Relation).HasKey1(owner); });
        }

        internal void GetParentRelationCount(Item owner, out int used, out int unused)
        {
            GetUsedUnsedCount(owner.Owner, out used, out unused, TableX_ParentRelationX, (Item item) => { return (item as Relation).HasKey2(owner); });
        }
        internal bool TryGetUsedParentRelations(Item owner, out Item[] items)
        {
            return TryGetUsed(owner.Owner, out items, TableX_ParentRelationX, (Item item) => { return (item as Relation).HasKey2(owner); });
        }
        internal bool TryGetUnusedParentRelations(Item owner, out Item[] items)
        {
            return TryGetUnused(owner.Owner, out items, TableX_ParentRelationX, (Item item) => { return (item as Relation).HasKey2(owner); });
        }

        private void GetUsedUnsedCount(Item owner, out int used, out int unused, Relation relation, Func<Item, bool> isUsed)
        {
            Item[] titm;
            used = unused = 0;
            if (relation.TryGetChildren(owner, out titm))
            {
                for (int i = 0; i < titm.Length; i++)
                {
                    if (isUsed(titm[i])) used += 1;
                    else unused += 1;
                }
            }
        }
        private bool TryGetUsed(Item owner, out Item[] items, Relation relation, Func<Item, bool> isUsed)
        {
            Item[] titm;
            if (relation.TryGetChildren(owner, out titm))
            {
                int count = 0;
                for (int i = 0; i < titm.Length; i++)
                {
                    if (isUsed(titm[i])) count += 1;
                }
                if (count > 0)
                {
                    int j = 0;
                    items = new Item[count];
                    for (int i = 0; i < titm.Length; i++)
                    {
                        if (!isUsed(titm[i])) continue;
                        items[j] = titm[i]; j += 1;
                    }
                    return true;
                }
            }
            items = null;
            return false;
        }
        private bool TryGetUnused(Item owner, out Item[] items, Relation relation, Func<Item, bool> isUsed)
        {
            Item[] titm;
            if (relation.TryGetChildren(owner, out titm))
            {
                int count = 0;
                for (int i = 0; i < titm.Length; i++)
                {
                    if (!isUsed(titm[i])) count += 1;
                }
                if (count > 0)
                {
                    int j = 0;
                    items = new Item[count];
                    for (int i = 0; i < titm.Length; i++)
                    {
                        if (isUsed(titm[i])) continue;
                        items[j] = titm[i]; j += 1;
                    }
                    return true;
                }
            }
            items = null;
            return false;
        }
    }
}