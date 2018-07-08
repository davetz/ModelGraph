using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public class ItemRemoved : ItemChange
    {
        internal Item Item;
        internal int AtIndex;  // remember the item's location before it was removed
        internal IList<ColumnX> Columns; // remember the item,s column values (only applies to RowX items) 
        internal List<String> Values;

        #region Constructor  ==================================================
        internal ItemRemoved(ChangeSet owner, Item item, int index, string name, IList<ColumnX> columns = null, List<String> values = null)
        {
            Owner = owner;
            Trait = Trait.ItemRemoved;
            Name = name;

            Item = item;
            AtIndex = index;
            Columns = columns;
            Values = values;

            owner.Add(this);
        }
        #endregion
    }
}
