using System;

namespace ModelGraphLibrary
{/*

 */
    public class ComputeX : Property
    {
        internal const string DefaultSeparator = " : ";

        private Value _values;
        internal Guid Guid;
        internal string Name;
        internal string Summary;
        internal string Description;
        internal string Separator = DefaultSeparator;

        internal ICacheValue ValueCache;
        internal ICacheValue[] ValueCacheSet;

        internal CompuType CompuType; // type of computation
        internal NumericSet NumericSet; //specify a numeric calculation

        private NativeType _nativeType; // is set by ValidateComputeXStore

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
        internal bool IsUnresolved => _nativeType == NativeType.Unresolved;
        internal override bool HasItemName => false;
        internal override string GetItemName(Item itm) { return null; }
        internal override ValueType ValueType => ValueType.String;
        internal override string GetValue(Item item, NumericTerm term = NumericTerm.None)
        {
            GetValue(item, term, out string value);
            return value;
        }
        internal override NativeType NativeType => _nativeType;
        internal void SetNativeType(NativeType type) { _nativeType = type; }
        internal string SelectString { get { return GetChef().GetSelectString(this); } set { GetChef().SetSelectString(this, value); } }

        internal override bool TrySetValue(Item item, string value) => false;
        internal override bool GetBool(Item item) => false;

        internal override void GetValue(Item item, out bool value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out byte value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out sbyte value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out uint value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out int value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out ushort value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out short value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out ulong value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, out long value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, NumericTerm term, out double value) => _values.GetValue(this, item, out value);
        internal override void GetValue(Item item, NumericTerm term, out string value) => _values.GetValue(this, item, out value);

        #endregion
    }
}
