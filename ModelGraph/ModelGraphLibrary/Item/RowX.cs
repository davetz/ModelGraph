using System;

namespace ModelGraphLibrary
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

            owner.Append(this);
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
        #endregion
    }
}
