using System;

namespace ModelGraph.Internals
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
        private Value value = Chef.ValuesNone; // provides default behavior prior to validation

        internal CompuType CompuType; // type of computation
        internal NumericSet NumericSet; //specify a numeric calculation

        #region Constructors  =================================================
        internal ComputeX(StoreOf<ComputeX> owner)
        {
            Owner = owner;
            Trait = Trait.ComputeX;
            Guid = Guid.NewGuid();
            AutoExpandRight = true;

            owner.Append(this);
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
        internal string SelectString { get { return GetChef().GetSelectString(this); } set { GetChef().SetSelectString(this, value); } }
        #endregion
    }
}