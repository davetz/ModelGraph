using System;

namespace ModelGraphLibrary
{/*
 */
    public class EnumX : StoreOf<PairX>
    {
        internal string Name;
        internal string Summary;
        internal string Description;

        #region Constructors  =================================================
        internal EnumX(StoreOf<EnumX> owner)
        {
            Owner = owner;
            Trait = Trait.EnumX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal EnumX(StoreOf<EnumX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.EnumX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion
    }
}
