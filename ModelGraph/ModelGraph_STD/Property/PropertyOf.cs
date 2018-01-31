using System;

namespace ModelGraphSTD
{/*

 */
    public class PropertyOf<T1, T2> : Property, IValueStore<T2> where T1 : Item
    {
        internal EnumZ EnumZ;
        internal Func<Item, T2> GetValFunc;
        internal Func<Item, T2, bool> SetValFunc;
        internal Func<Item, string> GetItemNameFunc;


        #region Constructor  ==================================================
        internal PropertyOf(Store owner, Trait trait, EnumZ enumZ = null)
        {
            Owner = owner;
            Trait = trait;
            EnumZ = enumZ;

            owner.Add(this);
        }
        #endregion

        #region Property  =====================================================
        internal T1 Cast(Item item) { return item as T1; }

        internal bool Valid(Item item) { return (item is T1); }
        internal bool Invalid(Item item) { return !(item is T1); }

        internal override bool HasItemName => (GetItemNameFunc != null);
        internal override string GetItemName(Item itm) { return Invalid(itm) ? Chef.InvalidItem : GetItemNameFunc(itm); }
        #endregion

        #region IValueStore  ==================================================
        public int Count => 0;
        public void Clear() { }
        public void Remove(Item key) { }
        public void SetOwner(ComputeX cx) { }

        public bool GetVal(Item key, out T2 val) { if (GetValFunc == null) return Value.NoValue(out val); val = GetValFunc(key); return true; }
        public bool SetVal(Item key, T2 value) => (SetValFunc is null) ? false : SetValFunc(key, value);
        #endregion
    }
}

