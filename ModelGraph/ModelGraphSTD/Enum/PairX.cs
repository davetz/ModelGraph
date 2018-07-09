using System;

namespace ModelGraphSTD
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

            owner.Add(this);
        }
        internal PairX(EnumX owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.PairX;
            Guid = guid;

            owner.Add(this);
        }
        internal EnumX EnumX => Owner as EnumX;
        #endregion
    }
}
