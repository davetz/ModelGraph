using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class Query : Item
    {
        internal Item Item;
        internal List<Item> Items;

        internal Query Parent;
        internal List<List<Query>> Children;

        #region Constructor  ==================================================
        internal Query(QueryX qx, Query parent, Item item, List<Item> items)
        {
            Owner = qx;
            Trait = Trait.Query;

            Item = item;
            Items = items;
            Parent = parent;

            SetFlags(qx.GetFlags()); // copy the queryX flags
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal Query GetHeadQuery() { var q = this; while (!q.IsHead && q.Parent != null) { q = q.Parent; } return q; }
        internal QueryX QueryX { get { return Owner as QueryX; } }

        internal bool IsEmpty => (Items == null || Items.Count == 0);
        internal Item GetItem(int index) { return Items[index]; }

        /// <summary>
        /// Try to populate a list of stepValues (they will be in reverse order)
        /// </summary>
        internal bool TryGetValues(List<(Item, WhereSelect)> values)
        {
            values.Clear();
            var q = this;
            while (q != null) // don't run off the end
            {
                var select = q.QueryX.Select;
                if (select != null && q.Items != null)
                {
                    foreach (var item in q.Items)
                    {
                        if (item == null) continue;
                        values.Add((item, select));
                    }
                }
                if (q.IsHead) return true;
                q = q.Parent;
            }
            return false; // we've failed
        }

        internal int ItemCount => (Items == null) ? 0 : Items.Count;

        internal bool TryGetItems(out List<Item> items)
        {
            if (IsEmpty)
            {
                items = null;
                return false;
            }
            items = Items;
            return true;
        }
        internal bool TryGetQuerys(Item item, out List<Query> querys)
        {
            querys = null;
            if (IsEmpty || Children == null) return false;

            for (int i = 0; i < Items.Count; i++)
            {
                if (item != Items[i]) continue;
                querys = Children[i];
                break;
            }

            return (querys != null);
        }
        internal int QueryCount(Item item)
        {
            if (!IsEmpty && Children != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (item != Items[i]) continue;
                    return (Children[i] != null) ? Children[i].Count : 0;
                }
            }
            return 0;
        }
        #endregion
    }
}
