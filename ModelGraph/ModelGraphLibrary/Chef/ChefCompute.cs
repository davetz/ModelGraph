using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        static internal string BlankName = "???"; // indicates blank or missing name
        static internal string InvalidItem = "######"; // indicates invalid reference 

        static internal ValueNone ValuesNone = new ValueNone();
        static internal ValueInvalid ValuesInvalid = new ValueInvalid();
        static internal ValueCircular ValuesCircular = new ValueCircular();
        static internal ValueUnresolved ValuesUnresolved = new ValueUnresolved();
        static internal LiteralUnresolved LiteralUnresolved = new LiteralUnresolved();

        #region ResetCacheValues  =============================================
        private void ResetCacheValues()
        {
            foreach (var cx in _computeXStore.Items) { cx.Value.Clear(); }
        }
        #endregion 

        #region <Get/Set>SelectString  ========================================
        internal ValType GetValueType(ComputeX cx)
        {
            var root = ComputeX_QueryX.GetChild(cx);
            switch (cx.CompuType)
            {
                case CompuType.RowValue:
                    return (root.HasSelect) ? root.Select.ValueType : ValType.IsUnknown; ;

                case CompuType.RelatedValue:
                    return (root.HasSelect) ? root.Select.ValueType : ValType.IsUnknown; ;

                case CompuType.NumericValueSet:
                    if (cx.NumericSet == NumericSet.Count)
                        return (AnyValueXHeads()) ? ValType.String : ValType.IsInvalid;
                    else
                        return (AnyValueXNumbers()) ? ValType.String : ValType.IsInvalid;

                case CompuType.CompositeString:
                    return (AnyValueXSelects()) ? ValType.String : ValType.IsInvalid;

                case CompuType.CompositeReversed:
                    return (AnyValueXSelects()) ? ValType.String : ValType.IsInvalid;
                default:
                    return ValType.IsInvalid;
            }
            bool AnyValueXHeads()
            {
                return QueryX_QueryX.HasChildLink(root);
            }

            bool AnyValueXSelects()
            {
                var qx = root;
                while (qx != null)
                {
                    if (qx.HasSelect) return true;
                    qx = QueryX_QueryX.GetChild(qx) as QueryX;
                }
                return false;
            }

            bool AnyValueXNumbers()
            {
                var qx = root;
                while (qx != null)
                {
                    if (qx.HasSelect && IsNumber(qx.Select.ValueType)) return true;
                    qx = QueryX_QueryX.GetChild(qx);
                }
                return false;
            }

            bool IsNumber(ValType nt)
            {
                switch(nt)
                {
                    case ValType.Byte:
                    case ValType.Int16:
                    case ValType.Int32:
                    case ValType.Int64:
                    case ValType.Double:
                        return true;
                    default:
                        return false;
                }
            }
        }
        internal string GetSelectString(ComputeX cx)
        {
            var root = ComputeX_QueryX.GetChild(cx);
            return (root != null && root.HasSelect) ? root.SelectString : null;
        }
        internal void SetSelectString(ComputeX cx, string value)
        {
            var root = ComputeX_QueryX.GetChild(cx);
            if (root != null) root.SelectString = value;
            
        }
        #endregion

        #region TrySetComputeTypeProperty  ====================================
        private bool TrySetComputeTypeProperty(ComputeX cx, int val)
        {
            var type = (CompuType)val;
            if (cx.CompuType != type)
            {
                cx.CompuType = type;
                cx.Value.Clear();
            }
            return true;
        }
        #endregion

        #region TrySetNumericSetProperty  =====================================
        private bool TrySetNumericSetProperty(ComputeX cx, int val)
        {
            var type = (NumericSet)val;
            if (cx.NumericSet != type)
            {
                cx.NumericSet = type;
                cx.Value.Clear();
            }
            return true;
        }
        #endregion

        #region SetComputeXProperty  ==========================================
        private bool SetComputeXWhere(ComputeX cx, string value)
        {
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx != null)
            {
                qx.WhereString = value;
                ValidateQueryX(qx);
            }
            cx.Value.Clear();
            return true;
        }
        private bool SetComputeXSelect(ComputeX cx, string value)
        {
            return(ComputeX_QueryX.TryGetChild(cx, out QueryX qx) && TrySetQueryXSelectProperty(qx, value));
        }
        #endregion

        #region ValidateValueXChange  =========================================
        private void ValidateValueXChange(QueryX qx)
        {
            ValidateQueryX(qx);

            while (qx != null)
            {
                var qx2 = QueryX_QueryX.GetParent(qx);
                if (qx2 == null)
                {
                    var cx = ComputeX_QueryX.GetParent(qx);
                    if (cx != null)
                    {
                        cx.Value.Clear();
                    }
                }
                qx = qx2;
            }
        }
        #endregion

        #region AllocateValueCache  ===========================================
        // called when the compuDef needs to produce a value, but its ValueCache is null
        internal void AllocateValueCache(ComputeX cx)
        {
            switch (cx.CompuType)
            {
                case CompuType.RowValue:

                    var vx = ComputeX_QueryX.GetChild(cx) as QueryX;
                    if (vx == null || vx.Select == null || vx.Select.ValueType == ValType.IsInvalid)
                        cx.Value = ValuesInvalid;
                    else
                        AllocateCache(vx);
                    break;

                case CompuType.RelatedValue:

                    var qx = ComputeX_QueryX.GetChild(cx) as QueryX;
                    while(QueryX_QueryX.HasChildLink(qx)) { qx = QueryX_QueryX.GetChild(qx); }
                    if (qx == null || qx.Select == null || qx.Select.ValueType == ValType.IsInvalid)
                        cx.Value = ValuesInvalid;
                    else
                        AllocateCache(qx);
                    break;

                case CompuType.NumericValueSet:

                    cx.Value = Value.Create(ValType.DoubleArray);
                    break;

                case CompuType.CompositeString:

                    cx.Value = Value.Create(ValType.String);
                    break;

                case CompuType.CompositeReversed:

                    cx.Value = Value.Create(ValType.String);
                    break;
            }

            void AllocateCache(QueryX vx)
            {
                var type = vx.Select.ValueType;
                if (type < ValType.MaximumType)
                    cx.Value = Value.Create(type);
                else if (type == ValType.IsUnknown)
                    cx.Value = ValuesNone;
                else if (type == ValType.IsCircular)
                    cx.Value = ValuesCircular;
                else if (type == ValType.IsUnresolved)
                    cx.Value = ValuesUnresolved;
                else
                    cx.Value = ValuesInvalid;
            }
        }
        #endregion

        #region GetSelectorName  ==============================================
        string GetSelectorName(ComputeX item)
        {
            var text = "Select"; //<=================== FIX THIS
            var tbl = Store_ComputeX.GetParent(item);
            var tblName = GetIdentity(tbl, IdentityStyle.Single);
            if (tbl != null) return $"{tblName} ";

            return text;
        }
        int GetValueType(QueryX qx)
        {
            if (qx.Select == null) return (int)ValType.IsUnknown;
            return (int)qx.Select.ValueType;
        }
        #endregion

        #region ValidateComputeXStore  ========================================
        private void ValidateComputeX(ComputeX cx, QueryX qx)
        {
            if (cx.Value.ValType == ValType.IsUnknown)
            {
                if (cx.CompuType == CompuType.RowValue)
                {
                    if (qx.Select != null && qx.Select.ValueType < ValType.MaximumType)
                    {
                        cx.Value = Value.Create(qx.Select.ValueType);
                    }
                }
            }
        }
        #endregion
    }
}
