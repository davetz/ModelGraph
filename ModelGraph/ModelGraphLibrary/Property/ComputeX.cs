using System;

namespace ModelGraphLibrary
{/*

 */
    public class ComputeX : Property
    {
        internal const string DefaultSeparator = " : ";

        private Value _values = Chef.ValuesNone; // provides default behavior prior to validation
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;
        internal string Separator = DefaultSeparator;

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
        internal override Value Values { get { return _values; } set { _values = value; } }
        internal override ValueType ValueType => Values.ValueType;
        internal override bool HasItemName => false;
        internal override string GetItemName(Item itm) { return null; }
        internal override string GetValue(Item item) { Values.GetValue(item, out string val); return val; }
        internal string SelectString { get { return GetChef().GetSelectString(this); } set { GetChef().SetSelectString(this, value); } }

        internal override bool TrySetValue(Item item, string value) => false;
        internal override bool GetBool(Item item) => false;

        #endregion
    }
}
