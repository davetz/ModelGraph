using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class ItemCreated : ItemChange
    {
        internal Item Item;
        internal int Index; // remember item's location in the Items list 
        internal IList<ColumnX> Columns; // remember the item,s column values (only applies to RowX items) 
        internal List<String> Values;

        #region Constructor  ==================================================
        internal ItemCreated(ChangeSet owner, Item item, int index, string name, IList<ColumnX> columns = null, List<String> values = null)
        {
            Owner = owner;
            Trait = Trait.ItemCreated;
            Name = name;
            Item = item;
            Index = index;
            Columns = columns;
            Values = values;

            owner.Add(this);
        }
        #endregion

    }
}
