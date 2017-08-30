using System;

namespace ModelGraphLibrary
{/*

 */
    public class ItemLinked : ItemChange
    {
        internal Item Item1;
        internal Item Item2;
        internal Relation Relation;
        internal IndexPair Index; // remember initial location in relation map lists

        #region Constructor  ==================================================
        internal ItemLinked(ChangeSet owner, Relation relation, Item item1, Item item2, IndexPair index, string name)
        {
            Owner = owner;
            Trait = Trait.ItemLinked;
            Name = name;

            Item1 = item1;
            Item2 = item2;
            Index = index;
            Relation = relation;

            owner.Append(this);
        }
        #endregion
    }
}
