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
        private State _state;
        private Flags _flags;
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

        #region Trait  ========================================================
        public bool IsProperty => (IsTextProperty || IsComboProperty || IsCheckProperty);
        public bool IsTextProperty => Trait == Trait.TextProperty_M;
        public bool IsComboProperty => Trait == Trait.ComboProperty_M;
        public bool IsCheckProperty => Trait == Trait.CheckProperty_M;

        public bool IsRowChildRelationModel => Trait == Trait.RowChildRelation_M;
        public bool IsRowParentRelationModel => Trait == Trait.RowParentRelation_M;
        #endregion

        #region State  ========================================================
        [Flags]
        private enum State : ushort
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
        private bool GetState(State state) => (_state & state) != 0;
        private void SetState(State state, bool value) { if (value) _state |= state; else _state &= ~state; }

        public bool IsChanged { get { return !GetState(State.IsChanged); } set { SetState(State.IsChanged, !value); } }
        public bool IsReadOnly { get { return GetState(State.IsReadOnly); } set { SetState(State.IsReadOnly, !value); } }
        public bool IsMultiline { get { return GetState(State.IsMultiline); } set { SetState(State.IsMultiline, !value); } }
        public bool IsExpandedLeft { get { return GetState(State.IsExpandLeft); } set { var prev = GetState(State.IsExpandLeft); SetState(State.IsExpandLeft, value); if (prev != value) IsChanged = true; if (!value) IsExpandedRight = false; } }
        public bool IsExpandedRight { get { return GetState(State.IsExpandRight); } set { var prev = GetState(State.IsExpandRight); SetState(State.IsExpandRight, value); if (prev != value) IsChanged = true; } }
        public bool IsFilterFocus { get { return GetState(State.SetFilterFocus); } set { SetState(State.SetFilterFocus, value); } }

        public bool IsExpandedFilter
        {
            get { return GetState(State.IsExpandFilter); }
            set
            {
                var prev = GetState(State.IsExpandFilter); SetExpandedFilter(value);
                if (prev != value)
                {
                    IsChanged = true;
                    if (value) IsFilterFocus = true;
                }
            }
        }
        public void SetExpandedFilter(bool value)
        {
            SetState(State.IsExpandFilter, value);

            if (value == false)
            {
                var root = GetRootModel();
                root.ViewFilter.Remove(this);
            }
        }
        public bool IsSortAscending { get { return GetState(State.IsAscendingSort); } set { var prev = GetState(State.IsAscendingSort); SetState(State.IsAscendingSort, value); if (prev != value) IsChanged = true; } }
        public bool IsSortDescending { get { return GetState(State.IsDescendingSort); } set { var prev = GetState(State.IsDescendingSort); SetState(State.IsDescendingSort, value); if (prev != value) IsChanged = true; } }
        public bool IsUsedFilter { get { return GetState(State.IsUsedFilter); } set { var prev = GetState(State.IsUsedFilter); SetState(State.IsUsedFilter, value); if (prev != value) IsChanged = true; } }
        public bool IsNotUsedFilter { get { return GetState(State.IsNotUsedFilter); } set { var prev = GetState(State.IsNotUsedFilter); SetState(State.IsNotUsedFilter, value); if (prev != value) IsChanged = true; } }
        #endregion

        #region Flags  ========================================================
        [Flags]
        private enum Flags : byte
        {
            CanDrag = 0x01,
            CanSort = 0x02,
            CanFilter = 0x04,
            CanMultiline = 0x08,

            CanExpandLeft = 0x10,
            CanExpandRight = 0x20,
            CanFilterUsage = 0x40,
        }
        private bool GetFlag(Flags flag) => (_flags & flag) != 0;
        private void SetFlag(Flags flag, bool value) { if (value) _flags |= flag; else _flags &= ~flag; }
        public bool CanDrag { get { return GetFlag(Flags.CanDrag); } set { SetFlag(Flags.CanDrag, value); } }
        public bool CanSort { get { return GetFlag(Flags.CanSort); } set { SetFlag(Flags.CanSort, value); } }
        public bool CanFilter { get { return GetFlag(Flags.CanFilter); } set { SetFlag(Flags.CanFilter, value); } }
        public bool CanMultiline { get { return GetFlag(Flags.CanMultiline); } set { SetFlag(Flags.CanMultiline, value); } }
        public bool CanExpandLeft { get { return GetFlag(Flags.CanExpandLeft); } set { SetFlag(Flags.CanExpandLeft, value); } }
        public bool CanExpandRight { get { return GetFlag(Flags.CanExpandRight); } set { SetFlag(Flags.CanExpandRight, value); } }
        public bool CanFilterUsage { get { return GetFlag(Flags.CanFilterUsage); } set { SetFlag(Flags.CanFilterUsage, value); } }

        #endregion

        #region Auxiliary Items  ==============================================
        public Graph Graph => Item as Graph;
        public Query Query => (Aux1 is Query aux) ? aux : Item as Query;
        public ColumnX ColumnX => (Aux1 is ColumnX aux) ? aux : Item as ColumnX;
        public ComputeX ComputeX => (Aux1 is ComputeX aux) ? aux : Item as ComputeX;
        public Property Property => (Aux1 is Property aux) ? aux : Item as Property;
        public Relation Relation => (Aux1 is Relation aux) ? aux : Item as Relation;
        public EnumX EnumX => (Aux2 is EnumX aux) ? aux : Item as EnumX;
        public EnumZ EnumZ => Aux2 as EnumZ;
        public ChangeSet ChangeSet => Item as ChangeSet;
        public Error Error => Item as Error;
        public ViewX ViewX => Item as ViewX;
        public PairX PairX => Item as PairX;
        public TableX TableX => Item as TableX;
        public GraphX GraphX => Item as GraphX;
        public SymbolX SymbolX => Item as SymbolX;
        public RelationX RelationX => Item as RelationX;
        public Store Store => Item as Store;
        #endregion

        #region ModelAction  ==================================================
        public string  ModelInfo => (Get.ModelInfo == null) ? null : Get.ModelInfo(this);
        public string ModelSummary => (Get.ModelSummary == null) ? null : Get.ModelSummary(this);
        public string ModelDescription => (Get.ModelDescription == null) ? null : Get.ModelDescription(this);

        public int IndexValue => (Get.IndexValue == null) ? 0 : Get.IndexValue(this);
        public bool BoolValue => (Get.BoolValue == null) ? false : Get.BoolValue(this);
        public string TextValue => (Get.TextValue == null) ? null : Get.TextValue(this);
        public string[] ListValue => (Get.ListValue == null) ? null : Get.ListValue(this);
        //=====================================================================
        const string BlankName = "?-?-?";
        public (string Kind, string Name, int Count, ModelType Type) ModelParms
        {
            get
            {
                if (Get.ModelParms == null) return (string.Empty, BlankName, 0, ModelType.Default);

                var (kind, name, count, type) = Get.ModelParms(this);

                if (kind == null) kind = string.Empty;
                if (string.IsNullOrWhiteSpace(name)) name = BlankName;

                return (kind, name, count, type);
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
