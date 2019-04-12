using System;

namespace ModelGraphSTD
{/*

 */
    public class ComputeX : Property
    {
        internal const string DefaultSeparator = " : ";

        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;
        internal string Separator = DefaultSeparator;

        internal CompuType CompuType; // type of computation

        #region Constructors  =================================================
        internal ComputeX(StoreOf<ComputeX> owner)
        {
            Owner = owner;
            Trait = Trait.ComputeX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal ComputeX(StoreOf<ComputeX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.ComputeX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion

        #region Property/Methods  =============================================
        internal override bool HasItemName => false;
        internal override string GetItemName(Item itm) { return null; }
        #endregion
    }
}