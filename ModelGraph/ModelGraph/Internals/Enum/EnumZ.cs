using System;

namespace ModelGraph.Internals
{/*
 */
    public class EnumZ : StoreOf<PairZ>
    {
        private Type _type;

        #region Constructor  ==================================================
        internal EnumZ(StoreOf<EnumZ> owner, Trait trait, Type type)
        {
            Owner = owner;
            Trait = trait;
            _type = type;
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
