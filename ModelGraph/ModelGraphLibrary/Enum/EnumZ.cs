using System;

namespace ModelGraphLibrary
{/*
 */
    public class EnumZ : StoreOf<PairZ>
    {
        private Type _type;

        #region Constructor  ==================================================
        internal EnumZ(Store owner, Trait trait, Type type)
        {
            Owner = owner;
            Trait = trait;
            _type = type;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal string ActualValue(int index)
        {
            var values = Enum.GetValues(_type);
            if (index < values.Length) return values.GetValue(index).ToString();
            return Chef.InvalidItem;
        }
        #endregion

    }
}
