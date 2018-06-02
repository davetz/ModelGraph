using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public class ItemModel
    {
        public Item Item;
        public Item Aux1;
        public Item Aux2;
        public ItemModel ParentModel;   // allows bidirectional tree taversal
        public ItemModel[] ChildModels; // hierarchal subtree of models 
        internal ModelAction Get;
        internal Trait Trait;
        private State1 _flags1;
        private State2 _flags2;
        public byte Level;

        #region Constructor  ==================================================
        internal ItemModel(ItemModel parent, Trait trait, byte level, Item item, Item aux1, Item aux2, ModelAction action)
        {
            Trait = trait;
            Level = level;
            Item = item;
            Aux1 = aux1;
            Aux2 = aux2;
            Get = action;
            ParentModel = parent;
        }
        #endregion

        #region StringKeys  ===================================================
        internal string GetKindKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}K";
        internal string GetNameKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}N";
        internal string GetSummaryKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}S";
        internal string GetDescriptionKey(Trait trait) => $"{(int)(trait & Trait.KeyMask):X3}V";

        internal string KindKey => GetKindKey(IsProperty ? Item.Trait : Trait);
        internal string NameKey => GetNameKey(IsProperty ? Item.Trait : Trait);
        internal string SummaryKey => GetSummaryKey(IsProperty ? Item.Trait : Trait);
        internal string DescriptionKey => GetDescriptionKey(IsProperty ? Item.Trait : Trait);
        #endregion

        #region State  ========================================================
        [Flags]
        private enum State1 : ushort
        {
            IsChanged = 0x8000,
            IsReadOnly = 0x4000,
            IsMultiline = 0x2000,

            IsAscendingSort = 0x800,
            IsDescendingSort = 0x400,
            IsExpandFilter = 0x200,
            SetFilterFocus = 0x100,

            IsExpandLeft = 0x80,
            IsExpandRight = 0x40,
            IsUsedFilter = 0x20,
            IsNotUsedFilter = 0x10,
        }
        [Flags]
        private enum State2 : byte
        {
            CanDrag = 0x80,
            CanSort = 0x40,
            CanFilter = 0x20,
            CanMultiline = 0x10,

            CanExpandLeft = 0x08,
            CanExpandRight = 0x04,
            CanFilterUsage = 0x02,
        }
        private bool GetFlag1(State1 state) => (_flags1 & state) != 0;
        private void SetFlag1(State1 state, bool value) { if (value) _flags1 |= state; else _flags1 &= ~state; }
        private bool GetFlag2(State2 state) => (_flags2 & state) != 0;
        private void SetFlag2(State2 state, bool value) { if (value) _flags2 |= state; else _flags2 &= ~state; }

        public bool CanDrag { get { return GetFlag2(State2.CanDrag); } set { SetFlag2(State2.CanDrag, value); } }
        public bool CanSort { get { return GetFlag2(State2.CanSort); } set { SetFlag2(State2.CanSort, value); } }
        public bool CanFilter { get { return GetFlag2(State2.CanFilter); } set { SetFlag2(State2.CanFilter, value); } }
        public bool CanMultiline { get { return GetFlag2(State2.CanMultiline); } set { SetFlag2(State2.CanMultiline, value); } }
        public bool CanExpandLeft { get { return GetFlag2(State2.CanExpandLeft); } set { SetFlag2(State2.CanExpandLeft, value); } }
        public bool CanExpandRight { get { return GetFlag2(State2.CanExpandRight); } set { SetFlag2(State2.CanExpandRight, value); } }
        public bool CanFilterUsage { get { return GetFlag2(State2.CanFilterUsage); } set { SetFlag2(State2.CanFilterUsage, value); } }

        public bool IsChanged { get { return !GetFlag1(State1.IsChanged); } set { SetFlag1(State1.IsChanged, !value); } }
        public bool IsReadOnly { get { return GetFlag1(State1.IsReadOnly); } set { SetFlag1(State1.IsReadOnly, !value); } }
        public bool IsMultiline { get { return GetFlag1(State1.IsMultiline); } set { SetFlag1(State1.IsMultiline, !value); } }
        public bool IsExpandedLeft { get { return GetFlag1(State1.IsExpandLeft); } set { var prev = GetFlag1(State1.IsExpandLeft); SetFlag1(State1.IsExpandLeft, value); if (prev != value) IsChanged = true; if (!value) IsExpandedRight = false; } }
        public bool IsExpandedRight { get { return GetFlag1(State1.IsExpandRight); } set { var prev = GetFlag1(State1.IsExpandRight); SetFlag1(State1.IsExpandRight, value); if (prev != value) IsChanged = true; } }
        public bool IsFilterFocus { get { return GetFlag1(State1.SetFilterFocus); } set { SetFlag1(State1.SetFilterFocus, value); } }

        public bool IsExpandedFilter { get { return GetFlag1(State1.IsExpandFilter); } set { var prev = GetFlag1(State1.IsExpandFilter); SetExpandedFilter(value);
                if (prev != value)
                {
                    IsChanged = true;
                    if (value) IsFilterFocus = true;
                } } }
        public void SetExpandedFilter(bool value)
        {
            SetFlag1(State1.IsExpandFilter, value);

            if (value == false)
            {
                var root = GetRootModel();
                root.ViewFilter.Remove(this);
            }
        }
        public bool IsSortAscending { get { return GetFlag1(State1.IsAscendingSort); } set { var prev = GetFlag1(State1.IsAscendingSort); SetFlag1(State1.IsAscendingSort, value); if (prev != value) IsChanged = true; } }
        public bool IsSortDescending { get { return GetFlag1(State1.IsDescendingSort); } set { var prev = GetFlag1(State1.IsDescendingSort); SetFlag1(State1.IsDescendingSort, value); if (prev != value) IsChanged = true; } }
        public bool IsUsedFilter { get { return GetFlag1(State1.IsUsedFilter); } set { var prev = GetFlag1(State1.IsUsedFilter); SetFlag1(State1.IsUsedFilter, value); if (prev != value) IsChanged = true; } }
        public bool IsNotUsedFilter { get { return GetFlag1(State1.IsNotUsedFilter); } set { var prev = GetFlag1(State1.IsNotUsedFilter); SetFlag1(State1.IsNotUsedFilter, value); if (prev != value) IsChanged = true; } }
        #endregion

        #region Trait  ========================================================
        public bool IsProperty => (IsTextProperty || IsComboProperty || IsCheckProperty);
        public bool IsTextProperty => Trait == Trait.TextProperty_M;
        public bool IsComboProperty => Trait == Trait.ComboProperty_M;
        public bool IsCheckProperty => Trait == Trait.CheckProperty_M;

        public bool IsRowChildRelationModel => Trait == Trait.RowChildRelation_M;
        public bool IsRowParentRelationModel => Trait == Trait.RowParentRelation_M;
        #endregion

        #region Auxiliary Items  ==============================================
        public Graph Graph => Item as Graph;
        public Query Query => Aux1 as Query;
        public ColumnX ColumnX => Aux1 as ColumnX;
        public ComputeX ComputeX => Aux1 as ComputeX;
        public Property Property => Aux1 as Property;
        public Relation Relation => Aux1 as Relation;
        public Item RelatedItem => Aux2;
        public EnumX EnumX => Aux2 as EnumX;
        public EnumZ EnumZ => Aux2 as EnumZ;
        #endregion

        #region ModelAction  ==================================================
        public int ChildCount => (Get.ChildCount == null) ? 0 : Get.ChildCount(this);
        public string  ModelKind => (Get.ModelKind == null) ? null : Get.ModelKind(this);
        public string  ModelInfo => (Get.ModelInfo == null) ? null : Get.ModelInfo(this);
        public string ModelSummary => (Get.ModelSummary == null) ? null : Get.ModelSummary(this);
        public string ModelDescription => (Get.ModelDescription == null) ? null : Get.ModelDescription(this);

        public int IndexValue => (Get.IndexValue == null) ? 0 : Get.IndexValue(this);
        public bool BoolValue => (Get.BoolValue == null) ? false : Get.BoolValue(this);
        public string TextValue => (Get.TextValue == null) ? null : Get.TextValue(this);
        public string[] ListValue => (Get.ListValue == null) ? null : Get.ListValue(this);
        //=====================================================================
        public string ModelName
        {
            get
            {
                var val = (Get.ModelName == null) ? null : Get.ModelName(this);
                return (string.IsNullOrWhiteSpace(val)) ? "?_?_?" : val;
            }
        }
        public bool MenuComands(List<ModelCommand> list)
        {
            if (Get.MenuCommands == null) return false;
            
            list.Clear();
            Get.MenuCommands(this, list);
            return list.Count > 0;
        }
        public bool ButtonComands(List<ModelCommand> list)
        {
            if (Get.ButtonCommands == null) return false;

            list.Clear();
            Get.ButtonCommands(this, list);
            return list.Count > 0;
        }
        //=====================================================================
        public void DragStart() { Chef.DragDropSource = this; }
        public DropAction DragEnter() => ModelDrop(this, Chef.DragDropSource, false);
        public void DragDrop()
        {
            var drop = Chef.DragDropSource;
            Chef.DragDropSource = null;
            if (drop == null) return;

             PostAction( () => { ModelDrop(this, drop, true); } );
        }

        #region ModelDrop  ====================================================
        internal Func<ItemModel, ItemModel, bool, DropAction> ModelDrop
        {
            get
            {
                var drop = Chef.DragDropSource;
                if (drop == null) return DropActionNone;

                if (IsSiblingModel(drop))
                    return Get.ReorderItems ?? DropActionNone;
                else
                    return Get.ModelDrop ?? DropActionNone;
            }
        }
        DropAction DropActionNone(ItemModel model, ItemModel drop, bool doit) => DropAction.None;
        #endregion

        #region PostAction  ===================================================
        void PostModelRefresh() => DataChef?.PostRefresh(this);
        void PostSetValue(int value) => DataChef?.PostSetValue(this, value);
        void PostSetValue(bool value) => DataChef?.PostSetValue(this, value);
        void PostSetValue(string value) => DataChef?.PostSetValue(this, value);
        void PostAction(Action action) => DataChef?.PostAction(this, action);
        Chef DataChef
        {
            get
            {
                for (var item = Item; ; item = item.Owner)
                {
                    if (item == null) return null;
                    if (item.IsInvalid) return null;
                    if (item.IsDataChef) return item as Chef;
                }
            }
        }
        #endregion
        #endregion

        #region Properties/Methods  ===========================================
        public UIRequest BuildViewRequest(ControlType controlType)
        {
            return UIRequest.CreateNewView(controlType, Trait, Item.GetChef(), Item, Aux1, Aux2, Get, true);
        }
        internal UIRequest BuildViewRequest(ControlType controlType, Trait trait, ModelAction action)
        {
            return UIRequest.CreateNewView(controlType, trait, Item.GetChef(), Item, Aux1, Aux2, action, true);
        }
        public int FilterCount { get { return (ChildModels == null) ? 0 : ChildModels.Length; } }
        public bool HasError { get { return false; } }
        public bool IsModified { get { return false; } }
        public string ModelIdentity => GetModelIdentity();

        public bool IsExpanded => (IsExpandedLeft || IsExpandedRight);
        public bool IsSorted => (IsSortAscending || IsSortDescending);

        public bool IsInvalid => (Item == null || Item.IsInvalid);

        public int GetChildlIndex(ItemModel child)
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
            if (Aux1 != null && Aux1 is Property)
            {
                var code1 = (int)(Trait & Trait.KeyMask);
                var code2 = (int)(Aux1.Trait & Trait.KeyMask);
                return $"{Trait.ToString()}  ({code1.ToString("X3")}){Environment.NewLine}{Aux1.Trait.ToString()}  ({code2.ToString("X3")})";
            }
            else
            {
                var code = (int)(Trait & Trait.KeyMask);
                return $"{Trait.ToString()}  ({code.ToString("X3")})";
            }
        }

        public bool IsChildModel(ItemModel model)
        {
            if (ChildModels == null) return false;
            foreach (var child in ChildModels)
            {
                if (child == model) return true;
            }
            return false;
        }
        public bool IsSiblingModel(ItemModel model)
        {
            return (ParentModel == model.ParentModel);
        }

        public RootModel GetRootModel()
        {
            var mdl = this;
            while (mdl.ParentModel != null) { mdl = mdl.ParentModel; }
            if (mdl is RootModel root) return root;
            throw new Exception("Corrupt TreeModel Hierachy");
        }
        #endregion
    }
}
