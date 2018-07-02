using System;

namespace ModelGraphSTD
{/*
 */
    public class RowX : Item
    {
        internal Guid Guid;

        #region Constructors  =================================================
        internal RowX(TableX owner) // user created row
        {
            Owner = owner;
            Trait = Trait.RowX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal RowX(TableX owner, Guid guid) // used by Load operation
        {
            Owner = owner;
            Trait = Trait.RowX;
            Guid = guid;

            owner.Add(this); // no need to check for null
        }
        #endregion

        #region Properies/Methods  ============================================
        internal TableX TableX => (Owner as TableX);
        internal override void RefChanged() => TableX.RefDelta++;
        #endregion
    }
}
