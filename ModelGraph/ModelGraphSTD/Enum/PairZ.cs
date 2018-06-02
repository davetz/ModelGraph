

namespace ModelGraphSTD
{/*
 */
    public class PairZ : Item
    {
        internal PairZ(EnumZ owner, Trait trait)
        {
            Owner = owner;
            Trait = trait;

            owner.Add(this);
        }

        internal int EnumKey => (int)(Trait & Trait.EnumMask);
    }
}
