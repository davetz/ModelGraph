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
        public string ViewFilter;       // UI imposed child model Kind/Name filter
        internal ModelAction Get;

        internal Trait Trait;
        private State _state;
        private Flags _flags;
        public byte Delta;
        public byte Depth;

        #region Constructor  ==================================================
        internal ItemModel() { }
        internal ItemModel(ItemModel parent, Trait trait, byte level, Item item, Item aux1, Item aux2, ModelAction action)
        {
            Trait = trait;
            Depth = level;
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

        public override string ToString()
        {
            var (kind, name, count, type) = ModelParms;
            return $"{NameKey} {Trait} {kind} {name} {count}";
        }
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
        enum State : ushort
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
        public RowX RowX => (Item is RowX item) ? item : (Aux1 is RowX aux1) ? aux1 : Aux2 as RowX;
        public Node Node => (Item is Node item) ? item : (Aux1 is Node aux1) ? aux1 : Aux2 as Node;
        public Edge Edge => (Item is Edge item) ? item : (Aux1 is Edge aux1) ? aux1 : Aux2 as Edge;
        public Path Path => (Item is Path item) ? item : (Aux1 is Path aux1) ? aux1 : Aux2 as Path;
        public Store Store => (Aux1 is Store aux1) ? aux1 : (Item is Store item) ? item : Aux2 as Store;
        public Level Level => (Item is Level item) ? item : (Aux1 is Level aux1) ? aux1 : Aux2 as Level;
        public Graph Graph => (Item is Graph item) ? item : (Aux1 is Graph aux1) ? aux1 : Aux2 as Graph;
        public Query Query => (Item is Query item) ? item : (Aux1 is Query aux1) ? aux1 : Aux2 as Query;
        public EnumX EnumX => (Item is EnumX item) ? item : (Aux2 is EnumX aux2) ? aux2 : Aux1 as EnumX;
        public EnumZ EnumZ => (Aux2 is EnumZ aux2) ? aux2 : (Aux1 is EnumZ aux1) ? aux1 : Item as EnumZ;
        public Error Error => (Item is Error item) ? item : (Aux1 is Error aux1) ? aux1 : Aux2 as Error;
        public ViewX ViewX => (Item is ViewX item) ? item : (Aux1 is ViewX aux1) ? aux1 : Aux2 as ViewX;
        public PairX PairX => (Item is PairX item) ? item : (Aux1 is PairX aux1) ? aux1 : Aux2 as PairX;
        public TableX TableX => (Item is TableX item) ? item : (Aux1 is TableX aux1) ? aux1 : Aux2 as TableX;
        public GraphX GraphX => (Item is GraphX item) ? item : (Aux1 is GraphX aux1) ? aux1 : Aux2 as GraphX;
        public QueryX QueryX => (Item is QueryX item) ? item : (Aux1 is QueryX aux1) ? aux1 : Aux2 as QueryX;
        public ColumnX ColumnX => (Item is ColumnX item) ? item : (Aux1 is ColumnX aux1) ? aux1 : Aux2 as ColumnX;
        public SymbolX SymbolX => (Item is SymbolX item) ? item : (Aux1 is SymbolX aux1) ? aux1 : Aux2 as SymbolX;
        public ComputeX ComputeX => (Item is ComputeX item) ? item : (Aux1 is ComputeX aux1) ? aux1 : Aux2 as ComputeX;
        public Property Property => (Aux1 is Property aux1) ? aux1 : (Item is Property item) ? item : Aux2 as Property;
        public Relation Relation => (Aux1 is Relation aux1) ? aux1 : (Item is Relation item) ? item : Aux2 as Relation;
        public ChangeSet ChangeSet => (Item is ChangeSet item) ? item : (Aux1 is ChangeSet aux1) ? aux1 : Aux2 as ChangeSet;
        public ItemChange ItemChange => (Item is ItemChange item) ? item : (Aux1 is ItemChange aux1) ? aux1 : Aux2 as ItemChange;
        public RelationX RelationX => (Item is RelationX item) ? item : (Aux1 is RelationX aux1) ? aux1 : Aux2 as RelationX;
        #endregion

        #region ModelAction  ==================================================
        public void Validate() => Get.Validate?.Invoke(this);
        public string ModelInfo => (Get.ModelInfo == null) ? null : Get.ModelInfo(this);
        public string ModelSummary => (Get.ModelSummary == null) ? null : Get.ModelSummary(this);
        public string ModelDescription => (Get.ModelDescription == null) ? null : Get.ModelDescription(this);

        public int IndexValue => (Get.IndexValue == null) ? 0 : Get.IndexValue(this);
        public bool BoolValue => (Get.BoolValue == null) ? false : Get.BoolValue(this);
        public string TextValue => (Get.TextValue == null) ? null : Get.TextValue(this);
        public string[] ListValue => (Get.ListValue == null) ? null : Get.ListValue(this);

        //=====================================================================
        public (string Kind, string Name, int Count, ModelType Type) ModelParms
        {
            get
            {
                if (Get.ModelParms == null) return (string.Empty, Chef.BlankName, 0, ModelType.Default);

                var (kind, name, count, type) = Get.ModelParms(this);

                if (kind == null) kind = string.Empty;
                if (string.IsNullOrWhiteSpace(name)) name = Chef.BlankName;

                return (kind, name, count, type);
            }
        }
        public bool MenuComands(List<ModelCommand> list)
        {
            list.Clear();
            if (Get.MenuCommands == null) return false;

            Get.MenuCommands(this, list);
            return list.Count > 0;
        }
        public bool ButtonComands(List<ModelCommand> list)
        {
            list.Clear();
            if (Get.ButtonCommands == null) return false;

            Get.ButtonCommands(this, list);
            return list.Count > 0;
        }
        //=====================================================================
        public void DragStart() { Chef.DragDropSource = this; }
        public DropAction DragEnter() => ModelDrop(this, Chef.DragDropSource, false);
        public void DragDrop()
        {
            var drop = Chef.DragDropSource;
            if (drop == null) return;

            PostAction(() => { ModelDrop(this, drop, true); });
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
        public void PostRefreshGraph() { }
        public void PostRefreshViewList(ItemModel select, int scroll = 0, ChangeType change = ChangeType.NoChange) => DataChef?.PostRefreshViewList(GetRootModel(), select, scroll, change);
        public void PostSetValue(int value) => DataChef?.PostSetValue(this, value);
        public void PostSetValue(bool value) => DataChef?.PostSetValue(this, value);
        public void PostSetValue(string value) => DataChef?.PostSetValue(this, value);
        public void PostAction(Action action) => DataChef?.PostAction(this, action);
        public static ItemModel FirstValidModel(List<ItemModel> viewList)
        {
            if (viewList.Count > 0)
            {
                foreach (var m in viewList)
                {
                    if (m.DataChef != null) return m;
                }
                foreach (var m in viewList)
                {
                    var p = m?.ParentModel;
                    if (p != null && p.DataChef != null) return p;
                }
                foreach (var m in viewList)
                {
                    var p = m.ParentModel;
                    var q = p?.ParentModel;
                    if (q != null && q.DataChef != null) return q;
                }
                foreach (var m in viewList)
                {
                    var p = m.ParentModel;
                    var q = p?.ParentModel;
                    var r = q?.ParentModel;
                    if (r != null && r.DataChef != null) return r;
                }
            }
            return null;
        }
        Chef DataChef
        {
            get
            {
                for (var item = Item; ; item = item.Owner)
                {
                    if (item == null) return null;
                    if (item.IsInvalid) return null;
                    if (item is Chef chef) return chef;
                }
            }
        }
        #endregion
        #endregion

        #region Properties/Methods  ===========================================
        public UIRequest BuildViewRequest(ControlType controlType)
        {
            return UIRequest.CreateView(GetRootModel(), controlType, Trait, Item.GetChef(), Item, Aux1, Aux2, Get, true);
        }
        internal UIRequest BuildViewRequest(ControlType controlType, Trait trait, ModelAction action)
        {
            return UIRequest.CreateView(GetRootModel(), controlType, trait, Item.GetChef(), Item, Aux1, Aux2, action, true);
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
        public int ChildModelCount => (ChildModels == null) ? 0 : ChildModels.Length;
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
