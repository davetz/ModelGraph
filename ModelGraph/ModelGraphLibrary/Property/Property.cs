namespace ModelGraphLibrary
{/*

 */
    public abstract class Property : Item
    {
        internal abstract bool GetBool(Item item);
        internal abstract bool HasItemName { get; }
        internal abstract string GetItemName(Item itm);

        internal abstract Value Values { get; set; }
        internal abstract ValueType ValueType { get; }
        internal abstract string GetValue(Item item);
        internal abstract bool TrySetValue(Item item, string value);
    }
}

