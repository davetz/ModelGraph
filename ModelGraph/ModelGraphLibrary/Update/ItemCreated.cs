using System;

namespace ModelGraphSTD
{/*

 */
    public class ItemCreated : ItemChange
    {
        internal Item Item;
        internal int Index; // remember item's location in the Items list 
        internal ColumnX[] Columns; // remember the item,s column values (only applies to RowX items) 
        internal String[] Values;

        #region Constructor  ==================================================
        internal ItemCreated(ChangeSet owner, Item item, int index, string name, ColumnX[] columns = null, String[] values = null)
        {
            Owner = owner;
            Trait = Trait.ItemCreated;
            Name = name;
            Item = item;
            Index = index;
            Columns = columns;
            Values = values;

            owner.Append(this);
        }
        #endregion

    }
}
