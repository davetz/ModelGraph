using System;

namespace ModelGraphLibrary
{/*
 */
    public class ViewX : Item
    {
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;

        #region Constructor  ======================================================
        internal ViewX(Store owner)
        {
            Owner = owner;
            Trait = Trait.ViewX;
            Guid = Guid.NewGuid();

            owner.Add(this);
        }
        internal ViewX(Store owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.ViewX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion
    }
}
