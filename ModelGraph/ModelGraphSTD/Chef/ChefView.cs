namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private bool TryGetQueryItems(QueryX query, out Item[] items, Item key = null)
        {
            items = null;
            if (query != null)
            {
                if (key == null)
                {
                    var sto = Store_QueryX.GetParent(query);
                    if (sto != null && sto.Count > 0)
                    {
                        items = sto.GetItems();
                    }
                }
                else
                {
                    var rel = Relation_QueryX.GetParent(query);
                    if (rel != null)
                    {
                        if (query.IsReversed)
                            rel.TryGetParents(key, out items);
                        else
                            rel.TryGetChildren(key, out items);
                    }
                }
                if (query.HasWhere) items = ApplyFilter(query, items);
            }
            return (items != null && items.Length > 0);
        }

        private (int L1, Property[] PropertyList, int L2, QueryX[] QueryList, int L3, ViewX[] ViewList) GetQueryXChildren(QueryX query)
        {
            Property[] PropertyList = QueryX_Property.GetChildren(query);
            int L1 = (PropertyList == null) ? 0 : PropertyList.Length;

            QueryX[] QueryList = QueryX_QueryX.GetChildren(query);
            int L2 = (QueryList == null) ? 0 : QueryList.Length;

            ViewX[] ViewList = QueryX_ViewX.GetChildren(query);
            int L3 = (ViewList == null) ? 0 : ViewList.Length;

            return (L1, PropertyList, L2, QueryList, L3, ViewList);
        }

    }
}
