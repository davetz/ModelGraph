using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraph.Internals
{
    public class ColumnX : Property
    {
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Initial;
        internal string Description;

        internal bool IsSpecific(Item key)
        {
            throw new NotImplementedException();
        }


        #region Constructors  =================================================
        internal ColumnX(StoreOf<ColumnX> owner)
        {
            Owner = owner;
            Trait = Trait.ColumnX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Append(this);
        }
        internal ColumnX(StoreOf<ColumnX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.ColumnX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion


        #region Property  =====================================================
        internal override bool HasItemName => false;
        internal override string GetItemName(Item key) => null;
        #endregion
    }
}
