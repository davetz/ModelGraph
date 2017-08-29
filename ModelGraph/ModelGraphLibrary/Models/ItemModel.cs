using System;

namespace ModelGraphLibrary
{/*

 */
    public class ItemModel
    {
        internal Item Item1;
        internal Item Item2;
        internal Item Item3;
        internal ItemModel ParentModel;   // allows bidirectional tree taversal
        internal ItemModel[] ChildModels; // hierarchal subtree of models 
        protected Action<ItemModel, RootModel> _getData;
        internal Trait Trait;
        private State1 _flags1;
        private State2 _flags2;
        internal byte Level;

        #region Constructor  ==================================================
        internal ItemModel(ItemModel parent, Trait trait = Trait.Empty, byte level = 0, Item item1 = null, Item item2 = null, Item item3 = null,
            Action<ItemModel, RootModel> getData = null)
        {
            Trait = trait;
            Level = level;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            ParentModel = parent;
            _getData = getData;
        }
        #endregion

        #region StaticStringKeys  =============================================
        internal string GetKindKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}K";
        internal string GetNameKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}N";
        internal string GetSummaryKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}S";
        internal string GetDescriptionKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}V";

        internal string KindKey =>  GetKindKey(IsProperty ? Item1.Trait: Trait);
        internal string NameKey => GetNameKey(IsProperty ? Item1.Trait : Trait);
        internal string SummaryKey => GetSummaryKey(IsProperty ? Item1.Trait : Trait);
        internal string DescriptionKey => GetDescriptionKey(IsProperty ? Item1.Trait : Trait);
        #endregion

        #region State  ========================================================
        [Flags]
        private enum State1 : ushort
        {
            IsChanged        = 0x8000,
            IsReadOnly       = 0x4000,
            IsMultiline      = 0x2000,

            IsAscendingSort  = 0x800,
            IsDescendingSort = 0x400,
            IsExpandFilter   = 0x200,
            SetFilterFocus   = 0x100,

            IsExpandLeft     = 0x80,
            IsExpandRight    = 0x40,
            IsUsedFilter     = 0x20,
            IsNotUsedFilter  = 0x10,
        }
        [Flags]
        private enum State2 : byte
        {
            CanDrag        = 0x80,
            CanSort        = 0x40,
            CanFilter      = 0x20,
            CanMultiline   = 0x10,

            CanExpandLeft  = 0x08,
            CanExpandRight = 0x04,
            CanFilterUsage = 0x02,
        }
        private bool GetFlag1(State1 state) => (_flags1 & state) != 0;
        private void SetFlag1(State1 state, bool value) { if (value) _flags1 |= state; else _flags1 &= ~state; }
        private bool GetFlag2(State2 state) => (_flags2 & state) != 0;
        private void SetFlag2(State2 state, bool value) { if (value) _flags2 |= state; else _flags2 &= ~state; }

        internal bool CanDrag { get { return GetFlag2(State2.CanDrag); } set { SetFlag2(State2.CanDrag, value); } }
        internal bool CanSort { get { return GetFlag2(State2.CanSort); } set { SetFlag2(State2.CanSort, value); } }
        internal bool CanFilter { get { return GetFlag2(State2.CanFilter); } set { SetFlag2(State2.CanFilter, value); } }
        internal bool CanMultiline { get { return GetFlag2(State2.CanMultiline); } set { SetFlag2(State2.CanMultiline, value); } }
        internal bool CanExpandLeft { get { return GetFlag2(State2.CanExpandLeft); } set { SetFlag2(State2.CanExpandLeft, value); } }
        internal bool CanExpandRight { get { return GetFlag2(State2.CanExpandRight); } set { SetFlag2(State2.CanExpandRight, value); } }
        internal bool CanFilterUsage { get { return GetFlag2(State2.CanFilterUsage); } set { SetFlag2(State2.CanFilterUsage, value); } }

        internal bool IsChanged { get { return !GetFlag1(State1.IsChanged); } set { SetFlag1(State1.IsChanged, !value); } }
        internal bool IsReadOnly { get { return GetFlag1(State1.IsReadOnly); } set { SetFlag1(State1.IsReadOnly, !value); } }
        internal bool IsMultiline { get { return GetFlag1(State1.IsMultiline); } set { SetFlag1(State1.IsMultiline, !value); } }
        internal bool IsExpandedLeft { get { return GetFlag1(State1.IsExpandLeft); } set { var prev = GetFlag1(State1.IsExpandLeft); SetFlag1(State1.IsExpandLeft, value); if (prev != value) IsChanged = true; if (!value) IsExpandedRight = false; } }
        internal bool IsExpandedRight { get { return GetFlag1(State1.IsExpandRight); } set { var prev = GetFlag1(State1.IsExpandRight); SetFlag1(State1.IsExpandRight, value); if (prev != value) IsChanged = true; } }
        internal bool IsFilterFocus { get { return GetFlag1(State1.SetFilterFocus); } set { SetFlag1(State1.SetFilterFocus, value); } }

        internal bool IsExpandedFilter { get { return GetFlag1(State1.IsExpandFilter); } set { var prev = GetFlag1(State1.IsExpandFilter); SetExpandedFilter(value);
                if (prev != value)
                {
                    IsChanged = true;
                    if (value) IsFilterFocus = true;
                } } }
        internal void SetExpandedFilter(bool value)
        {
            SetFlag1(State1.IsExpandFilter, value);

            if (value == false)
            {
                var root = GetRootModel();
                root.ViewFilter.Remove(this);
            }
        }
        internal bool IsSortAscending { get { return GetFlag1(State1.IsAscendingSort); } set { var prev = GetFlag1(State1.IsAscendingSort); SetFlag1(State1.IsAscendingSort, value); if (prev != value) IsChanged = true; } }
        internal bool IsSortDescending { get { return GetFlag1(State1.IsDescendingSort); } set { var prev = GetFlag1(State1.IsDescendingSort); SetFlag1(State1.IsDescendingSort, value); if (prev != value) IsChanged = true; } }
        internal bool IsUsedFilter { get { return GetFlag1(State1.IsUsedFilter); } set { var prev = GetFlag1(State1.IsUsedFilter); SetFlag1(State1.IsUsedFilter, value); if (prev != value) IsChanged = true; } }
        internal bool IsNotUsedFilter { get { return GetFlag1(State1.IsNotUsedFilter); } set { var prev = GetFlag1(State1.IsNotUsedFilter); SetFlag1(State1.IsNotUsedFilter, value); if (prev != value) IsChanged = true; } }
        #endregion

        #region Trait  ========================================================
        internal bool IsProperty => (IsTextProperty || IsComboProperty || IsCheckProperty);
        internal bool IsTextProperty => Trait == Trait.TextProperty_M;
        internal bool IsComboProperty => Trait == Trait.ComboProperty_M;
        internal bool IsCheckProperty => Trait == Trait.CheckProperty_M;

        internal bool IsRowChildRelationModel => Trait == Trait.RowChildRelation_M;
        internal bool IsRowParentRelationModel => Trait == Trait.RowParentRelation_M;
        #endregion

        #region Properties/Methods  ===========================================
       internal ViewRequest BuildViewRequest(ControlType controlType)
        {
            return new ViewRequest(controlType, Trait, Item1.GetChef(), Item1, Item2, Item3, _getData, true);
        }
        internal ViewRequest BuildViewRequest(ControlType controlType, Trait trait, Action<ItemModel, RootModel> getData)
        {
            return new ViewRequest(controlType, trait, Item1.GetChef(), Item1, Item2, Item3, getData, true);
        }
        internal int FilterCount { get { return (ChildModels == null) ? 0 : ChildModels.Length; } }
        internal bool HasError { get { return false; } }
        internal bool IsModified { get { return false; } }
        internal string ModelIdentity => GetModelIdentity();
        internal Action<ItemModel, RootModel> ModelGetData => _getData;

        internal bool IsExpanded => (IsExpandedLeft || IsExpandedRight);
        internal bool IsSorted => (IsSortAscending || IsSortDescending);

        internal bool IsInvalid => (Item1 == null || Item1.IsInvalid);
        internal void Validate() { _getData?.Invoke(this, null); }
        internal void GetData(RootModel root) { _getData?.Invoke(this, root); }

        internal int GetChildlIndex(ItemModel child)
        {
            if (ChildModels != null)
            {
                var N = ChildModels.Length;
                for (int i = 0; i < N; i++)
                {
                    if (ChildModels[i] == child) return i;
                }
            }
            return -1;
        }
        private string GetModelIdentity()
        {
            if (Item2 != null && Item2 is Property)
            {
                var code1 = (int)(Trait & Trait.KeyMask);
                var code2 = (int)(Item2.Trait & Trait.KeyMask);
                return $"{Trait.ToString()}  ({code1.ToString("X3")}){Environment.NewLine}{Item2.Trait.ToString()}  ({code2.ToString("X3")})";
            }
            else
            {
                var code = (int)(Trait & Trait.KeyMask);
                return $"{Trait.ToString()}  ({code.ToString("X3")})";
            }
        }
        internal Graph Graph { get { return Item1 as Graph; } }
        internal Relation Relation { get { return Item2 as Relation; } }
        internal Query Query { get { return Item2 as Query; } }

        internal bool IsChildModel(ItemModel model)
        {
            if (ChildModels == null) return false;
            foreach (var child in ChildModels)
            {
                if (child == model) return true;
            }
            return false;
        }
        internal bool IsSiblingModel(ItemModel model)
        {
            return (ParentModel == model.ParentModel);
        }

        internal RootModel GetRootModel()
        {
            var mdl = this;
            while (mdl.ParentModel != null) { mdl = mdl.ParentModel; }
            var root = mdl as RootModel;
            if (root != null) return root;
            throw new Exception("Corrupt ItemModel Hierachy");
        }
        #endregion
    }
}
