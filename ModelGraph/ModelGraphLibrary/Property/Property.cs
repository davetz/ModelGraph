namespace ModelGraphLibrary
{/*

 */
    public abstract class Property : Item
    {
        internal abstract bool GetBool(Item item);
        internal abstract bool HasItemName { get; }
        internal abstract string GetItemName(Item itm);
        internal abstract string GetValue(Item item, NumericTerm term = NumericTerm.None);
        internal abstract void GetValue(Item item, out bool value);
        internal abstract void GetValue(Item item, out byte value);
        internal abstract void GetValue(Item item, out sbyte value);
        internal abstract void GetValue(Item item, out uint value);
        internal abstract void GetValue(Item item, out int value);
        internal abstract void GetValue(Item item, out ushort value);
        internal abstract void GetValue(Item item, out short value);
        internal abstract void GetValue(Item item, out ulong value);
        internal abstract void GetValue(Item item, out long value);
        internal abstract void GetValue(Item item, NumericTerm term, out double value);
        internal abstract void GetValue(Item item, NumericTerm term, out string value);

        internal abstract bool TrySetValue(Item item, string value);
        internal abstract ValueType ValueType { get; }
    }
}

