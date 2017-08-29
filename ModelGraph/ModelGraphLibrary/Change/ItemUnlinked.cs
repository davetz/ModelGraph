namespace ModelGraphLibrary
{/*

 */
    public class ItemUnLinked : ItemChange
    {
        internal Item Child;
        internal Item Parent;
        internal Relation Relation;
        internal IndexPair Index;  // remember initial location in the relation map lists

        #region Constructor  ==================================================
        internal ItemUnLinked(ChangeSet owner, Relation relation, Item parent, Item child, IndexPair index, string name)
        {
            Owner = owner;
            Trait = Trait.ItemUnlinked;
            Name = name;

            Index = index;
            Child = child;
            Parent = parent;
            Relation = relation;

            owner.Append(this);
        }
        #endregion
    }
}
