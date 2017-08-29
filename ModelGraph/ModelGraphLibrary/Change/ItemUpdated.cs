﻿namespace ModelGraphLibrary
{/*

 */
    public class ItemUpdated : ItemChange
    {
        internal Item Item;
        internal Property Property;
        internal string OldValue;
        internal string NewValue;

        #region Constructor  ==================================================
        internal ItemUpdated(ChangeSet owner, Item item, Property property, string oldValue, string newValue, string name)
        {
            Owner = owner;
            Trait = Trait.ItemUpdated;
            Name = name;

            Item = item;
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;

            owner.Append(this);
        }
        #endregion
    }
}
