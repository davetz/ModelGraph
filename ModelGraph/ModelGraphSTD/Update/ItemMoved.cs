namespace ModelGraphSTD
{/*

 */
    public class ItemMoved : ItemChange
    {
        internal Item Item;
        internal int Index1; // the initial location before the move
        internal int Index2; // the ending location after the move

        #region Constructor  ==================================================
        internal ItemMoved(ChangeSet owner, Item item, int index1, int index2, string name)
        {
            Owner = owner;
            Trait = Trait.ItemMoved;
            Name = name;

            Item = item;
            Index1 = index1;
            Index2 = index2;

            owner.Append(this);
        }
        #endregion
    }
}
