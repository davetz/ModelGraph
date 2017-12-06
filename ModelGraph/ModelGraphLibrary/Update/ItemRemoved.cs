using System;

namespace ModelGraphLibrary
{/*

 */
    public class ItemRemoved : ItemChange
    {
        internal Item Item;
        internal int Index;  // remember the item's location before it was removed
        internal ColumnX[] Columns; // remember the item,s column values (only applies to RowX items) 
        internal String[] Values;

        #region Constructor  ==================================================
        internal ItemRemoved(ChangeSet owner, Item item, int index, string name, ColumnX[] columns = null, String[] values = null)
        {
            Owner = owner;
            Trait = Trait.ItemRemoved;
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
