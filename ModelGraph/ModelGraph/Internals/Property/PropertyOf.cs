﻿using System;

namespace ModelGraph.Internals
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
        public bool GetVal(Item key, out T2 val) => (GetValFunc is null) ? Value.NoValue(out val) : GetVal(key, out val);
        public bool SetVal(Item key, T2 value) => (SetValFunc is null) ? false : SetValFunc(key, value);
        #endregion
    }
}
