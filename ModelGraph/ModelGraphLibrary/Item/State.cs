using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    /// <summary>
    /// Named item flag bit slots
    /// </summary>
    [Flags]
    public enum State : ushort
    {
        Empty = 0,
        Index = 0x7,
        //=================================================
        HasChoice = 0x1, // TableX
        IsChoice = 0x1, // ColumnX
        IsLimited = 0x1, // Relation
        IsUndone = 0x1, // ItemChange, ChangeSet

        IsVirgin = 0x2, // ChangeSet
        IsRequired = 0x2, // Relation

        IsCongealed = 0x4, // ChangeSet

        IsRoot = 0x8, // QueryX, Query

        //=================================================
        IsHead = 0x10, // QueryX, Query

        IsTail = 0x20, // QueryX, Query

        IsRadial = 0x40, // QueryX, Query, Path

        IsReversed = 0x80, // QueryX, Query, Path

        //=================================================
        IsBreakPoint = 0x100, // QueryX, Query

        IsPersistent = 0x200, // QueryX, Query

        B11_spare = 0x400, // currently unused

        B12_spare = 0x800, // currently unused

        //=================================================
        Mask = 0x0FFF,
        IsNew = 0x1000, // all items
        IsDeleted = 0x2000, // all items
        AutoExpandLeft = 0x4000, // all items
        AutoExpandRight = 0x8000, // all items
    }
}
