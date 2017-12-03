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

        internal bool IsSpecific(Item key) => false;


        #region Constructors  =================================================
        internal ColumnX(StoreOf<ColumnX> owner)
        {
            Owner = owner;
            Trait = Trait.ColumnX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            Value = Value.Create(ValType.String);

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

        internal void Initialize(ValType type, string defaultVal, int rowCount)
        {
            Value = Value.Create(type, rowCount, defaultVal);
        }
        #endregion
    }
}
