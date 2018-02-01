using System;

namespace ModelGraphSTD
{/*

 */
    public class ItemParentMoved : ItemChange
    {
        internal Item Key;
        internal Item Item;
        internal Relation Relation;
        internal int Index1;
        internal int Index2;

        #region Constructor  ==================================================
        internal ItemParentMoved(ChangeSet owner, Relation relation, Item key, Item item, int index1, int index2, string name)
        {
            Owner = owner;
            Trait = Trait.ItemChildMoved;
            Name = name;

            Key = key;
            Item = item;
            Relation = relation;
            Index1 = index1;
            Index2 = index2;

            owner.Add(this);
        }
        #endregion
    }
}
