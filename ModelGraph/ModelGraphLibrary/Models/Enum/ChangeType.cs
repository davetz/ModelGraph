namespace ModelGraphLibrary
{/*
    Data Interface between ModelGraphLibrary <--> ModelGraph App
 */
    #region RelatedEnums  =====================================================
    // only return the data that is needed for the occasion
    public enum ChangeType
    {
        NoChange,

        ToggleLeft,
        ExpandLeft,
        CollapseLeft,
        ExpandLeftAll,

        ToggleRight,
        ExpandRight,
        CollapseRight,

        ToggleFilter,
        ExpandFilter,
        CollapseFilter,

        FilterSortChanged,
    }
    #endregion
}
