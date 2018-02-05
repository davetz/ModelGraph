
namespace ModelGraphSTD
{
    public abstract class Property : Item
    {
        internal Value Value = Chef.ValuesUnknown;

        internal abstract bool HasItemName { get; }
        internal abstract string GetItemName(Item itm);
    }
}
