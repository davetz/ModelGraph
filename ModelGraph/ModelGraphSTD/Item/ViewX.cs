using System;

namespace ModelGraphSTD
{/*
 */
    public class ViewX : Item
    {
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;

        #region Constructor  ======================================================
        internal ViewX(StoreOf<ViewX> owner)
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
        internal override void RefChanged() => (Owner as StoreOf<ViewX>).RefDelta++;
        #endregion
    }
}
