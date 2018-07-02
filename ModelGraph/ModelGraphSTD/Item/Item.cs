using System;

namespace ModelGraphSTD
{
    public class Item
    {
        internal Item Owner;
        internal Trait Trait;
        private State _flags;

        #region Trait  ========================================================
        internal bool IsExternal => (Trait & Trait.IsExternal) != 0;
        internal bool IsRootChef => (Trait == Trait.RootChef);
        internal bool IsDataChef => (Trait == Trait.DataChef);
        internal bool IsViewX => (Trait == Trait.ViewX);
        internal bool IsPairX => (Trait == Trait.PairX);
        internal bool IsRowX => (Trait == Trait.RowX);
        internal bool IsEnumX => (Trait == Trait.EnumX);
        internal bool IsTableX => (Trait == Trait.TableX);
        internal bool IsGraphX => (Trait == Trait.GraphX);
        internal bool IsQueryX => (Trait == Trait.QueryX);
        internal bool IsSymbolX => (Trait == Trait.SymbolX);
        internal bool IsColumnX => (Trait == Trait.ColumnX);
        internal bool IsComputeX => (Trait == Trait.ComputeX);
        //internal bool IsCommandX => (Trait == Trait.CommandX);
        internal bool IsRelationX => (Trait == Trait.RelationX);
        internal bool IsGraph => (Trait == Trait.Graph);
        internal bool IsNode => (Trait == Trait.Node);
        internal bool IsEdge => (Trait == Trait.Edge);

        internal bool IsItemMoved => Trait == Trait.ItemMoved;
        internal bool IsItemCreated => Trait == Trait.ItemCreated;
        internal bool IsItemUpdated => Trait == Trait.ItemUpdated;
        internal bool IsItemRemoved => Trait == Trait.ItemRemoved;
        internal bool IsItemLinked => Trait == Trait.ItemLinked;
        internal bool IsItemUnlinked => Trait == Trait.ItemUnlinked;
        internal bool IsItemLinkMoved => Trait == Trait.ItemChildMoved;


        internal bool IsReadOnly => (Trait & Trait.IsReadOnly) != 0;
        internal bool CanMultiline => (Trait & Trait.CanMultiline) != 0;

        internal byte TraitIndex => (byte)(Trait & Trait.IndexMask);
        #endregion

        #region Flags  ========================================================
        private bool GetFlag(State flag) => (_flags & flag) != 0;
        private void SetFlag(State flag, bool value = true) { if (value) _flags |= flag; else _flags &= ~flag; }
        internal bool HasFlags() => (_flags & State.Mask) != 0;
        internal ushort GetFlags() => (ushort)(_flags & State.Mask);
        internal void SetFlags(ushort flags) { _flags = ((State)flags & State.Mask); }
        internal QueryType QueryKind { get { return (QueryType)(_flags & State.Index); } set { _flags = ((_flags & ~State.Index) | (State)(value)); } }

        internal bool IsHead { get { return GetFlag(State.IsHead); } set { SetFlag(State.IsHead, value); } }
        internal bool IsTail { get { return GetFlag(State.IsTail); } set { SetFlag(State.IsTail, value); } }
        internal bool IsRoot { get { return GetFlag(State.IsRoot); } set { SetFlag(State.IsRoot, value); } }
        internal bool IsPath => (QueryKind == QueryType.Path);
        internal bool IsGroup => (QueryKind == QueryType.Group);
        internal bool IsSegue => (QueryKind == QueryType.Segue);
        internal bool IsValue => (QueryKind == QueryType.Value);
        internal bool IsReversed { get { return GetFlag(State.IsReversed); } set { SetFlag(State.IsReversed, value); } }
        internal bool IsRadial { get { return GetFlag(State.IsRadial); } set { SetFlag(State.IsRadial, value); } }

        internal bool IsBreakPoint { get { return GetFlag(State.IsBreakPoint); } set { SetFlag(State.IsBreakPoint, value); } }
        internal bool IsPersistent { get { return GetFlag(State.IsPersistent); } set { SetFlag(State.IsPersistent, value); } }

        internal bool IsLimited { get { return GetFlag(State.IsLimited); } set { SetFlag(State.IsLimited, value); } }
        internal bool IsRequired { get { return GetFlag(State.IsRequired); } set { SetFlag(State.IsRequired, value); } }

        internal bool IsUndone { get { return GetFlag(State.IsUndone); } set { SetFlag(State.IsUndone, value); } }
        internal bool IsVirgin { get { return GetFlag(State.IsVirgin); } set { SetFlag(State.IsVirgin, value); } }
        internal bool IsCongealed { get { return GetFlag(State.IsCongealed); } set { SetFlag(State.IsCongealed, value); } }

        internal bool IsNew { get { return GetFlag(State.IsNew); } set { SetFlag(State.IsNew, value); } }
        internal bool IsDeleted { get { return GetFlag(State.IsDeleted); } set { SetFlag(State.IsDeleted, value); } }
        internal bool AutoExpandLeft { get { return GetFlag(State.AutoExpandLeft); } set { SetFlag(State.AutoExpandLeft, value); } }
        internal bool AutoExpandRight { get { return GetFlag(State.AutoExpandRight); } set { SetFlag(State.AutoExpandRight, value); } }

        internal bool IsChoice { get { return GetFlag(State.IsChoice); } set { SetFlag(State.IsChoice, value); } }


        internal bool IsQueryGraphLink => !IsRoot && QueryKind == QueryType.Graph;
        internal bool IsQueryGraphRoot => IsRoot && QueryKind == QueryType.Graph;

        internal bool IsValueXHead => IsHead && QueryKind == QueryType.Value;// Trait == Trait.QueryXValueHead;

        internal bool IsGraphLink => (!IsRoot && QueryKind == QueryType.Graph);
        internal bool IsPathHead => IsHead && QueryKind == QueryType.Path;
        internal bool IsGroupHead => IsHead && QueryKind == QueryType.Group;
        internal bool IsSegueHead => IsHead && QueryKind == QueryType.Segue;


        #endregion

        #region StringKeys  ===================================================
        internal string KindKey => GetKindKey(Trait);
        internal string NameKey => GetNameKey(Trait);
        internal string SummaryKey => GetSummaryKey(Trait);
        internal string DescriptionKey => GetDescriptionKey(Trait);

        internal string GetKindKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}K";
        internal string GetNameKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}N";
        internal string GetSummaryKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}S";
        internal string GetDescriptionKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}V";
        #endregion

        #region Property/Methods ==============================================
        internal bool IsInvalid => Owner == null || IsDeleted;
        internal bool IsValid => !IsInvalid;
        internal Store Store => Owner as Store;
        internal virtual void RefChanged() { } // for optimization of ItemModel tree construction

        /// <summary>
        /// Walk up item tree hierachy to find the parent DataChef
        /// </summary>
        internal Chef GetChef()
        {
            var item = this;
            while (item != null) { if (item.IsDataChef) return item as Chef; item = item.Owner; }
            throw new Exception("Corrupted item hierarchy"); // I seriously hope this never happens
        }
        #endregion
    }
}
