using System;

namespace ModelGraphLibrary
{
    public class RelationX : RelationOf<RowX, RowX>
    {
        internal string Name;
        internal string Summary;
        internal string Description;

        #region Constructors  =================================================
        internal RelationX(StoreOf<RelationX> owner)
        {
            Guid = Guid.NewGuid();
            Owner = owner;
            Trait = Trait.RelationX;
            Pairing = Pairing.OneToMany;

            AutoExpandRight = true;

            owner.Append(this);
        }

        internal RelationX(StoreOf<RelationX> owner, Guid guid)
        {
            Guid = guid;
            Owner = owner;
            Trait = Trait.RelationX;
            Pairing = Pairing.OneToMany;

            owner.Add(this);
        }
        #endregion
    }
}
