namespace ModelGraph.Internals
{/*

 */
    public enum CompuType : byte
    {
        RowValue = 0, // compose from an item's property values
        RelatedValue = 1,
        NumericValueSet = 2,
        CompositeString = 3,
        CompositeReversed = 4,
    }
}
