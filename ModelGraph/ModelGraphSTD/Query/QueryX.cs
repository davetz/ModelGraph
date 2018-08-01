using System;

namespace ModelGraphSTD
{
    public class QueryX : Item
    {
        internal Guid Guid;
        internal WhereSelect Where;
        internal WhereSelect Select;
        internal Connect Connect1;
        internal Attach Attach1;
        internal Connect Connect2;
        internal Attach Attatch2;
        internal byte ExclusiveKey;
        private PFlag _flags;

        #region Constructor  ==================================================
        internal QueryX(Chef owner) //referenced in _graphParams for nodes
        {
            Owner = owner;
            Trait = Trait.NodeParm;
            Guid = new Guid("96E6DDD7-4BBA-4DFF-A233-3CEDBD18C5D7");
        }
        internal QueryX(StoreOf<QueryX> owner, QueryType kind, bool isRoot = false, bool isHead = false)
        {
            Owner = owner;
            Trait = Trait.QueryX;
            Guid = Guid.NewGuid();
            QueryKind = kind;
            IsRoot = isRoot;
            IsHead = isHead;
            IsTail = true;
            AutoExpandRight = true;

            owner.Add(this);
        }
        internal QueryX(StoreOf<QueryX> owner, Guid guid)
        {
            Owner = owner;
            Trait = Trait.QueryX;
            Guid = guid;

            owner.Add(this);
        }
        #endregion

        #region Validation  ===================================================
        [Flags]
        private enum PFlag : byte //private validation state
        {
            Reset = 0,
            Completed = 0x1,
            InvalidRef = 0x2,
            CircularRef = 0x4,
            UnresolvedRef = 0x8,
            InvalidSyntax = 0x10,
            ErrorMask = InvalidRef | CircularRef | InvalidSyntax,
        } 
        private bool GetFlag(PFlag flag) => (_flags & flag) != 0;
        private void SetFlag(bool val, PFlag flag) { if (val) _flags |= flag; else _flags &= ~flag; }

        internal bool IsResolved => HasCompleted || HasError; 
        internal bool HasError => (_flags & PFlag.ErrorMask) != 0;

        internal bool HasCompleted { get { return GetFlag(PFlag.Completed); } set { SetFlag(value, PFlag.Completed); } }
        internal bool HasInvalidRef { get { return GetFlag(PFlag.InvalidRef); } set { SetFlag(value, PFlag.InvalidRef); } }
        internal bool HasCircularRef { get { return GetFlag(PFlag.CircularRef); } set { SetFlag(value, PFlag.CircularRef); } }
        internal bool HasUnresolvedRef { get { return GetFlag(PFlag.UnresolvedRef); } set { SetFlag(value, PFlag.UnresolvedRef); } }
        internal bool HasInvalidSyntax { get { return GetFlag(PFlag.InvalidSyntax); } set { SetFlag(value, PFlag.InvalidSyntax); } }
        internal void Validate(Store store, bool firstPass = false)
        {
            if (firstPass)
            {
                _flags = PFlag.Reset;
                if (Where != null && !Where.TryValidate(store)) HasInvalidSyntax = true;
                if (Select != null && !Select.TryValidate(store)) HasInvalidSyntax = true;
                return;
            }
            else
            {

            }
        }
        #endregion

        #region Property/Method  ==============================================
        internal bool IsExclusive => ExclusiveKey != 0;

        internal bool HasWhere => (Where != null);
        internal bool HasSelect => (Select != null);
        internal bool HasValidSelect => (Select != null && Select.IsValid);

        internal bool AnyChange => (HasWhere && Where.AnyChange) || (HasSelect && Select.AnyChange);

        internal string WhereString { get { return Where?.InputString; } set { SetWhereString(value); } }
        internal string SelectString { get { return Select?.InputString; } set { SetSelectString(value); } }
        private void SetWhereString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                Where = null;
            else
                Where = new WhereSelect(value);
        }
        private void SetSelectString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                Select = null;
            else
                Select = new WhereSelect(value);
        }
        #endregion
    }
}
