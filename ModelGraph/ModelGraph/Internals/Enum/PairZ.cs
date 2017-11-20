

namespace ModelGraph.Internals
{/*
 */
    public class PairZ : Item
{
    internal PairZ(EnumZ owner, Trait trait)
    {
        Owner = owner;
        Trait = trait;

        owner.Append(this);
    }
}
}
