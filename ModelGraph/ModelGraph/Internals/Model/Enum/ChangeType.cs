﻿namespace ModelGraph.Internals
{/*
 */
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
}