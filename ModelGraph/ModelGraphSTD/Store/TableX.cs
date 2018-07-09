using System;

namespace ModelGraphSTD
{/*
 */
    public class TableX : StoreOf<RowX>
    {
        internal string Name;
        internal string Summary;
        internal string Description;

        #region Constructors  =================================================
        internal TableX(StoreOf<TableX> owner)
        {
            Owner = owner;
            Trait = Trait.TableX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal TableX(StoreOf<TableX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.TableX;
            Guid = guid;

            owner.Add(this);
        }
        internal override void RefChanged() => (Owner as StoreOf<TableX>).RefDelta++;
        #endregion
    }
}
