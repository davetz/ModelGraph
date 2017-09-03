using System;

namespace ModelGraphLibrary
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

        internal override bool TrySetValue(Item item, string value) { return false; }

        internal override bool GetBool(Item item) => false;


        internal override void GetValue(Item item, out bool value)
        {
            if (ValueCache == null || !ValueCache.GetValue(item, out value))
            {
                var chef = GetChef();
                if (ValueCache == null) chef.AllocateValueCache(this);
                chef.UpdateValueCache(item, this, out value);
            }
        }

        internal override void GetValue(Item item, out byte value)
        {
            if (ValueCache == null || !ValueCache.GetValue(item, out value))
            {
                var chef = GetChef();
                if (ValueCache == null) chef.AllocateValueCache(this);
                chef.UpdateValueCache(item, this, out value);
            }
        }

        internal override void GetValue(Item item, out int value)
        {
            if (ValueCache == null || !ValueCache.GetValue(item, out value))
            {
                var chef = GetChef();
                if (ValueCache == null) chef.AllocateValueCache(this);
                chef.UpdateValueCache(item, this, out value);
            }
        }

        internal override void GetValue(Item item, out short value)
        {
            if (ValueCache == null || !ValueCache.GetValue(item, out value))
            {
                var chef = GetChef();
                if (ValueCache == null) chef.AllocateValueCache(this);
                chef.UpdateValueCache(item, this, out value);
            }
        }

        internal override void GetValue(Item item, out long value)
        {
            if (ValueCache == null || !ValueCache.GetValue(item, out value))
            {
                var chef = GetChef();
                if (ValueCache == null) chef.AllocateValueCache(this);
                chef.UpdateValueCache(item, this, out value);
            }
        }

        internal override void GetValue(Item item, NumericTerm term, out double value)
        {
            if (CompuType == CompuType.NumericValueSet)
            {
                if (ValueCache == null)
                {
                    var chef = GetChef();
                    chef.AllocateValueCache(this);
                    chef.UpdateValueCache(item, this, out string _);

                    if (term == NumericTerm.None)
                        chef.UpdateValueCache(item, this, out value);
                    else
                        ValueCacheSet[(int)term].GetValue(item, out value);
                }
                else if (term == NumericTerm.None)
                    ValueCache.GetValue(item, out value);
                else
                    ValueCacheSet[(int)term].GetValue(item, out value);
            }
            else
            {
                if (ValueCache == null || !ValueCache.GetValue(item, out value))
                {
                    var chef = GetChef();
                    if (ValueCache == null) chef.AllocateValueCache(this);
                    chef.UpdateValueCache(item, this, out value);
                }
            }
        }

        internal override void GetValue(Item item, NumericTerm term, out string value)
        {
            if (CompuType == CompuType.NumericValueSet)
            {
                if (ValueCache == null)
                {
                    var chef = GetChef();
                    chef.AllocateValueCache(this);
                    chef.UpdateValueCache(item, this, out string _);

                    if (term == NumericTerm.None)
                        chef.UpdateValueCache(item, this, out value);
                    else
                        ValueCacheSet[(int)term].GetValue(item, out value);
                }
                else if (term == NumericTerm.None)
                    ValueCache.GetValue(item, out value);
                else
                    ValueCacheSet[(int)term].GetValue(item, out value);
            }
            else
            {
                if (ValueCache == null || !ValueCache.GetValue(item, out value))
                {
                    var chef = GetChef();
                    if (ValueCache == null) chef.AllocateValueCache(this);
                    chef.UpdateValueCache(item, this, out value);
                }
            }
        }
        #endregion
    }
}
