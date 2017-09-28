using System;

namespace ModelGraphLibrary
{/*

 */
    public class ComputeX : Property
    {
        internal const string DefaultSeparator = " : ";

        internal Value Values = Chef.ValuesNone; // provides default behavior prior to validation

        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;
        internal string Separator = DefaultSeparator;

        internal CompuType CompuType; // type of computation
        internal NumericSet NumericSet; //specify a numeric calculation

        private ValueType _nativeType; // is set by ValidateComputeXStore

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
        internal bool IsNumericSet => (CompuType == CompuType.NumericValueSet);
        internal bool IsPathComposite => (CompuType == CompuType.CompositeReversed || CompuType == CompuType.CompositeString);
        internal bool IsUnresolved => _nativeType == ValueType.Unresolved;
        internal override bool HasItemName => false;
        internal override string GetItemName(Item itm) { return null; }
        internal override ValueType ValueType => ValueType.String;
        internal override string GetValue(Item item, NumericTerm term = NumericTerm.None)
        {
            GetValue(item, term, out string value);
            return value;
        }
        internal void SetNativeType(ValueType type) { _nativeType = type; }
        internal string SelectString { get { return GetChef().GetSelectString(this); } set { GetChef().SetSelectString(this, value); } }

        internal override bool TrySetValue(Item item, string value) => false;
        internal override bool GetBool(Item item) => false;

        internal override void GetValue(Item item, out bool value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out byte value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out sbyte value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out uint value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out int value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out ushort value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out short value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out ulong value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out long value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, NumericTerm term, out double value) => Values.GetValue(this, item, out value);
        internal override void GetValue(Item item, NumericTerm term, out string value) => Values.GetValue(this, item, out value);

        #endregion
    }
}
