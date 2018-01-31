
namespace ModelGraphSTD
{
    public abstract class Property : Item
    {
        internal Value Value = Chef.ValuesNone;

        internal abstract bool HasItemName { get; }
        internal abstract string GetItemName(Item itm);
    }
}
