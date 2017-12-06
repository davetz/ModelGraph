using System;

namespace ModelGraphLibrary
{/*
 */
    public class PairX : Item
    {
        internal Guid Guid;
        internal string DisplayValue;
        internal string ActualValue;

        #region Constructors  =================================================
        internal PairX(EnumX owner)
        {
            Owner = owner;
            Trait = Trait.PairX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Append(this);
        }
        internal PairX(EnumX owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.PairX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion
    }
}
