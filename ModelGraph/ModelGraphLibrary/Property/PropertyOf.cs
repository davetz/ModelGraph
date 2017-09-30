using System;

namespace ModelGraphLibrary
{/*

 */
    public class PropertyOf<T> : Property where T : Item
    {
        static Func<Item, string> _getValue = (i) => { return string.Empty; };
        static Func<Item, string, bool> _trySetValue = (i, s) => { return false; };

        internal Func<Item, string> GetValueFunc = _getValue;
        internal Func<Item, string, bool> TrySetValueFunc = _trySetValue;
        internal Func<Item, string> GetItemNameFunc;

        private Value _values = Chef.ValuesNone;
        private ValueType _valueType;

        #region Constructor  ==================================================
        internal PropertyOf (Store owner, Trait trait, ValueType valueType)
        {
            Owner = owner;
            Trait = trait;
            _valueType = valueType;

            owner.Add(this);
        }
        #endregion

        #region Properties  ===================================================
        internal override Value Values { get { return _values; } set { _values = value; } }
        internal override ValueType ValueType => _valueType;
        internal bool Valid(Item item) { return (item is T); }
        internal bool Invalid(Item item) { return !(item is T); }
        internal T Cast(Item item) { return item as T; }

        internal override bool TrySetValue(Item itm, String val) { return Invalid(itm) ? false : TrySetValueFunc(itm, val); }
        internal override string GetValue(Item itm) { return Invalid(itm) ? Chef.InvalidItem : GetValueFunc(itm); }
        internal override string GetItemName(Item itm) { return Invalid(itm) ? Chef.InvalidItem : GetItemNameFunc(itm); }


        internal override bool GetBool(Item item) { return (bool.TryParse(GetValue(item), out bool val)) ? val : false;  }
        internal override bool HasItemName => (GetItemNameFunc != null);
    }
    #endregion
}

