using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public class QueryX : Item
    {
        internal Guid Guid;
        internal WhereSelect Where;
        internal WhereSelect Select;
        internal Connect Connect1;
        internal Connect Connect2;
        internal byte ExclusiveKey;

        #region Constructor  ==================================================
        internal QueryX() { } // parameterless constructor required for ValueX
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

        #region Property/Method  ==============================================
        internal bool IsExclusive => ExclusiveKey != 0;

        internal bool HasWhere => (Where != null);
        internal bool HasSelect => (Select != null);

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
