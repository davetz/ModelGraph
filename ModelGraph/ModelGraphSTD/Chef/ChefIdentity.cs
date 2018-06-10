using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private Func<string, string> _localize = (s) => s; //dummy default localizer
        public void SetLocalizer(Func<string, string> localizer) => _localize = localizer;

        private Dictionary<Trait, Func<Item, IdentityStyle, string>> _itemIdentity;

        #region GetIdentity  ==================================================
        public string GetIdentity(Item item, IdentityStyle style)
        {
            if (item != null && _itemIdentity.TryGetValue(item.Trait, out Func<Item, IdentityStyle, string> id))
                return id(item, style);
            else if (item is Relation)
                return Relation_Identity(item, style);
            else if (item is Property)
                return Property_Identity(item, style);
            else
                return Undefined_Identity(item, style);
        }
        #endregion

        #region InitializeItemIdentity  =======================================
        private void InitializeItemIdentity()
        {
            _itemIdentity = new Dictionary<Trait, Func<Item, IdentityStyle, string>>(73);

            _itemIdentity.Add(Trait.RowX, RowX_Identity);
            _itemIdentity.Add(Trait.ViewX, ViewX_Identity);
            _itemIdentity.Add(Trait.PairX, PairX_Identity);
            _itemIdentity.Add(Trait.EnumX, EnumX_Identity);
            _itemIdentity.Add(Trait.TableX, TableX_Identity);
            _itemIdentity.Add(Trait.GraphX, GraphX_Identity);
            _itemIdentity.Add(Trait.QueryX, QueryX_Identity);
            _itemIdentity.Add(Trait.SymbolX, SymbolX_Identity);
            _itemIdentity.Add(Trait.ColumnX, ColumnX_Identity);
            _itemIdentity.Add(Trait.ComputeX, ComputeX_Identity);
            //_itemIdentity.Add(Trait.CommandX, CommandX_Identity);
            _itemIdentity.Add(Trait.RelationX, RelationX_Identity);
        }
        #endregion

        #region Undefined  ====================================================
        private string Undefined_Identity(Item item, IdentityStyle style)
        {
            //if (item.TraitKey == 0x1AD) item.SetTrait(Trait.QueryXNodeSymbol);
            return $"{BlankName} {item.Trait.ToString()}";
        }
        private string Index_Identity(Item item)
        {
            var own = item.Owner as Store;
            if (own == null) return InvalidItem;

            var inx = own.IndexOf(item);
            if (inx < 0) return InvalidItem;

            return $"{inx}";
        }
        #endregion

        #region RowX_Identity  ================================================
        private string RowX_Identity(Item item, IdentityStyle style = IdentityStyle.Single)
        {
            if (item is RowX itm)
            {
                var pa = itm.Owner as TableX;
                var nc = TableX_NameProperty.GetChild(pa);
                var ix = pa.IndexOf(itm);

                string name = (nc == null) ? null : nc.Value.GetString(itm);
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var parent = pa.Name;
                if (string.IsNullOrWhiteSpace(parent)) parent = $"{_localize(pa.KindKey)} {Index_Identity(pa)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} {ix}:  {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {parent} : {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);

                    case IdentityStyle.Summary:
                        var ns = TableX_SummaryProperty.GetChild(pa);
                        return (ns == null) ? name : ns.Value.GetString(itm);

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region PairX_Identity  ===============================================
        private string PairX_Identity(Item item, IdentityStyle style)
        {
            if (item is PairX itm)
            {
                var name = itm.DisplayValue;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var pa = itm.Owner as EnumX;
                var parent = pa.Name;
                if (string.IsNullOrWhiteSpace(parent)) parent = $"{_localize(pa.KindKey)} {Index_Identity(pa)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {parent} : {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.ActualValue;

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region ViewX_Identity  ===============================================
        private string ViewX_Identity(Item item, IdentityStyle style)
        {
            if (item is ViewX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return name;

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return itm.Description;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region EnumX_Identity  ===============================================
        private string EnumX_Identity(Item item, IdentityStyle style)
        {
            if (item is EnumX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return name;

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return itm.Description;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region TableX_Identity  ==============================================
        private string TableX_Identity(Item item, IdentityStyle style)
        {
            if (item is TableX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.StoreItem:
                        return name;

                    case IdentityStyle.Double:
                        return $"{GetIdentity(item, IdentityStyle.Kind)} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return itm.Description;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region GraphX_Identity  ==============================================
        private string GraphX_Identity(Item item, IdentityStyle style)
        {
            if (item is GraphX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return name;

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return itm.Description;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region QueryX_Identity  ==============================================
        private string QueryX_Identity(Item item, IdentityStyle style)
        {
            if (item is QueryX qx)
            {
                Trait trait = Trait.QueryIsCorrupt;
                switch (qx.QueryKind)
                {
                    case QueryType.View:
                        if (qx.IsHead)
                            trait = Trait.QueryViewHead;
                        else if (qx.IsRoot)
                            trait = Trait.QueryViewRoot;
                        else
                            trait = Trait.QueryViewLink;
                        break;
                    case QueryType.Path:
                        if (qx.IsHead)
                            trait = Trait.QueryPathHead;
                        else
                            trait = Trait.QueryPathLink;
                        break;
                    case QueryType.Group:
                        if (qx.IsHead)
                            trait = Trait.QueryGroupHead;
                        else
                            trait = Trait.QueryGroupLink;
                        break;
                    case QueryType.Segue:
                        if (qx.IsHead)
                            trait = Trait.QuerySegueHead;
                        else
                            trait = Trait.QuerySegueLink;
                        break;
                    case QueryType.Graph:
                        if (qx.IsRoot)
                            trait = Trait.QueryGraphRoot;
                        else
                            trait = Trait.QueryGraphLink;
                        break;
                    case QueryType.Value:
                        if (qx.IsHead)
                            trait = Trait.QueryValueHead;
                        else if (qx.IsRoot)
                            trait = Trait.QueryValueRoot;
                        else
                            trait = Trait.QueryValueLink;
                        break;
                    case QueryType.Symbol:
                        trait = Trait.QueryNodeSymbol;
                        break;
                }
                var kind = _localize(GetKindKey(trait));
                var name = string.Empty;
                if (qx.IsRoot)
                {
                    var st = Store_QueryX.GetParent(qx);
                    if (st == null)
                        name = Chef.InvalidItem;
                    else
                        name = GetIdentity(st, IdentityStyle.Double);
                }
                else
                {
                    var re = Relation_QueryX.GetParent(qx);
                    if (re == null)
                        name = Chef.InvalidItem;
                    else
                    {
                        GetHeadTail(qx, out Store head, out Store tail);
                        name = $"{GetIdentity(head, IdentityStyle.Single)} --> {GetIdentity(tail, IdentityStyle.Single)}";                       
                    }
                }

                if (qx.HasSelect || qx.HasWhere || qx.IsRoot || qx.IsHead || qx.IsTail)
                {
                    name = $"{name}      [";
                    if (qx.HasWhere) name = $"{name}{GetName(Trait.QueryWhere)}( {qx.WhereString} )";
                    if (qx.HasSelect) name = $"{name} {GetName(Trait.QuerySelect)}( {qx.SelectString} )";

                    if (qx.IsRoot || qx.IsHead || qx.IsTail)
                    {
                        name = $"{name} ";
                        if (qx.IsRoot) name = $"{name}R";
                        if (qx.IsHead) name = $"{name}H";
                        if (qx.IsTail) name = $"{name}T";
                    }
                    name = $"{name}]";
                }


                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return name;

                    case IdentityStyle.ChangeLog:
                        return $"{kind} {Index_Identity(qx)}: {name}";

                    case IdentityStyle.Kind:
                        return kind;

                    case IdentityStyle.Summary:
                        return _localize(GetSummaryKey(trait));

                    case IdentityStyle.Description:
                        return _localize(GetDescriptionKey(trait));
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region SymbolX_Identity  =============================================
        private string SymbolX_Identity(Item item, IdentityStyle style)
        {
            if (item is SymbolX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var pa = GraphX_SymbolX.GetParent(itm);
                var parent = (pa == null) ? BlankName : pa.Name;
                if (string.IsNullOrWhiteSpace(parent)) parent = $"{_localize(pa.KindKey)} {Index_Identity(pa)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {parent} : {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region ColumnX_Identity  =============================================
        private string ColumnX_Identity(Item item, IdentityStyle style)
        {
            if (item is ColumnX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var pa = TableX_ColumnX.GetParent(itm);
                var parent = (pa == null) ? BlankName : pa.Name;
                if (string.IsNullOrWhiteSpace(parent)) parent = $"{_localize(pa.KindKey)} {Index_Identity(pa)}";

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)}: {parent} : {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region ComputeX_Identity  ============================================
        private string ComputeX_Identity(Item item, IdentityStyle style)
        {
            if (item is ComputeX itm)
            {
                var name = itm.Name;
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var pa = Store_ComputeX.GetParent(itm);
                var parent = (pa == null) ? BlankName : GetIdentity(pa, IdentityStyle.Single);

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)} {Index_Identity(itm)} : {parent} : {name}";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);;

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region RelationX_Identity  ===========================================
        private string RelationX_Identity(Item item, IdentityStyle style)
        {
            if (item is RelationX itm)
            {
                var name = string.IsNullOrWhiteSpace(itm.Name) ? string.Empty : $"({itm.Name})";
                if (string.IsNullOrWhiteSpace(name)) name = $"{_localize(itm.KindKey)} {Index_Identity(itm)}";

                var ch = TableX_ParentRelationX.GetParent(itm);
                var pa = TableX_ChildRelationX.GetParent(itm);

                var child = (ch == null) ? BlankName : GetIdentity(ch, IdentityStyle.Single);
                var parent = (pa == null) ? BlankName : GetIdentity(pa, IdentityStyle.Single);

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.StoreItem:
                        return $"{parent} --> {child}    {name}";

                    case IdentityStyle.Double:
                    case IdentityStyle.ChangeLog:
                        return $"{_localize(itm.KindKey)}:  {parent} --> {child}    ({name})";

                    case IdentityStyle.Kind:
                        return _localize(itm.KindKey);

                    case IdentityStyle.Summary:
                        return itm.Summary;

                    case IdentityStyle.Description:
                        return null;
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region Relation_Identity  ============================================
        private string Relation_Identity(Item item, IdentityStyle style)
        {
            if (item is Relation itm)
            {
                var kind = _localize(GetKindKey(Trait.Relation));
                var name = _localize(itm.NameKey);

                var parts = name.Split("_".ToCharArray());
                var child = parts[1];
                var parent = parts[0];

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.StoreItem:
                        return $"{parent} --> {child}";

                    case IdentityStyle.Double:
                    case IdentityStyle.ChangeLog:
                        return $"{kind} {Index_Identity(itm)}:  {parent} --> {child}";

                    case IdentityStyle.Kind:
                        return kind;

                    case IdentityStyle.Summary:
                        return _localize(itm.SummaryKey);

                    case IdentityStyle.Description:
                        return _localize(itm.DescriptionKey);
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region Property_Identity  ============================================
        private string Property_Identity(Item item, IdentityStyle style)
        {
            if (item is Property itm)
            {
                var kind = _localize(GetKindKey(Trait.Property));
                var name = _localize(itm.NameKey);

                var pa = Store_Property.GetParent(itm);
                if (pa == null) pa = item.Owner as Store;
                var parent = (pa == null) ? BlankName : _localize(pa.NameKey);

                switch (style)
                {
                    case IdentityStyle.Single:
                        return name;

                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                        return $"{parent} : {name}";

                    case IdentityStyle.ChangeLog:
                        return $"{kind} {Index_Identity(itm)}: {parent} : {name}";

                    case IdentityStyle.Kind:
                        return kind;

                    case IdentityStyle.Summary:
                        return _localize(itm.SummaryKey);

                    case IdentityStyle.Description:
                        return _localize(itm.DescriptionKey);
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion

        #region Store_Identity  ===============================================
        private string Store_Identity(Item item, IdentityStyle style)
        {
            if (item is Store itm)
            {
                var kind = _localize(item.KindKey);
                var name = _localize(itm.NameKey);

                switch (style)
                {
                    case IdentityStyle.Single:
                    case IdentityStyle.Double:
                    case IdentityStyle.StoreItem:
                    case IdentityStyle.ChangeLog:
                        return name;

                    case IdentityStyle.Kind:
                        return kind;

                    case IdentityStyle.Summary:
                        return _localize(itm.SummaryKey);

                    case IdentityStyle.Description:
                        return _localize(itm.DescriptionKey);
                }
            }
            return $"{BlankName} {item.Trait.ToString()}";
        }
        #endregion


        #region Legacy  =======================================================
        internal string GetRowName(RowX row)
        {
            if (TableX_NameProperty.TryGetChild(row.Owner, out Property prop))
            {
                return prop.Value.GetString(row);
            }
            var tbl = row.Owner as TableX;
            var inx = tbl.IndexOf(row);
            return $"#{inx.ToString()}";
        }
        internal string RowSummary(ItemModel model)
        {
            return RowSummary(model.Item as RowX);
        }
        internal string RowSummary(RowX row)
        {
            if (TableX_SummaryProperty.TryGetChild(row.Owner, out Property prop))
            {
                return prop.Value.GetString(row);
            }
            return null;
        }

        #endregion

        #region GetName  ======================================================
        internal string GetKind(Trait trait)
        {
            return _localize(GetKindKey(trait));
        }
        internal string GetName(Trait trait)
        {
            return _localize(GetNameKey(trait));
        }
        internal string GetSummary(Trait trait)
        {
            return _localize(GetSummaryKey(trait));
        }
        internal string GetDescription(Trait trait)
        {
            return _localize(GetDescriptionKey(trait));
        }
        #endregion
    }

    #region IdentityStyle  ====================================================
    public enum IdentityStyle
    {
        Kind,
        Single,
        Double,
        Summary,
        StoreItem,
        ChangeLog,
        Description,
    }
    #endregion
}
