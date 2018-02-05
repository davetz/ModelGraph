using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        static internal string BlankName = "???"; // indicates blank or missing name
        static internal string InvalidItem = "######"; // indicates invalid reference 

        static internal ValueUnknown ValuesUnknown = new ValueUnknown();
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
        internal string GetWhereProperty(ComputeX cx)
        {
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null) return InvalidItem;

            return (qx.HasWhere) ? qx.WhereString : null;
        }
        internal bool TrySetWhereProperty(ComputeX cx, string value)
        {
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null) return false;

            return TrySetWhereProperty(qx, value);
        }
        internal string GetSelectProperty(ComputeX cx)
        {
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null) return InvalidItem;

            return (qx.HasSelect) ? qx.SelectString : null;
        }
        internal bool TrySetSelectProperty(ComputeX cx, string value)
        {
            MajorDelta += 1;
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null) return false;

            return TrySetSelectProperty(qx, value);
        }
        #endregion

        #region TrySet...Property  ============================================
        private bool TrySetComputeTypeProperty(ComputeX cx, int val)
        {
            MajorDelta += 1;
            var type = (CompuType)val;
            if (cx.CompuType != type)
            {
                cx.CompuType = type;
                cx.Value.Clear();
            }
            return true;
        }
        private bool TrySetNumericSetProperty(ComputeX cx, int val)
        {
            MajorDelta += 1;
            var type = (NumericSet)val;
            if (cx.NumericSet != type)
            {
                cx.NumericSet = type;
                cx.Value.Clear();
            }
            return true;
        }
        private bool TrySetResultsProperty(ComputeX cx, int val)
        {
            MajorDelta += 1;
            var type = (Results)val;
            if (cx.Results != type)
            {
                cx.Results = type;
                cx.Value.Clear();
            }
            return true;
        }
        private bool TrySetSortingProperty(ComputeX cx, int val)
        {
            MajorDelta += 1;
            var type = (Sorting)val;
            if (cx.Sorting != type)
            {
                cx.Sorting = type;
                cx.Value.Clear();
            }
            return true;
        }
        private bool TrySetTakeSetProperty(ComputeX cx, int val)
        {
            MajorDelta += 1;
            var type = (TakeSet)val;
            if (cx.TakeSet != type)
            {
                cx.TakeSet = type;
                cx.Value.Clear();
            }
            return true;
        }
        #endregion

        #region TryGetComputedValue  ==========================================
        internal bool TryGetComputedValue(ComputeX cx, Item key)
        {/*
            This method is called by valueDictionary when a key-value-pair does
            not exist for the callers key.
         */
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null || cx.Value.IsEmpty)
                return false;

            switch (cx.CompuType)
            {
                case CompuType.RowValue: return TryGetRowValue();

                case CompuType.RelatedValue: return TryGetRelated();

                case CompuType.NumericValueSet: return TryGetNumericSet();

                case CompuType.CompositeString: return TryGetCompositeString();

                case CompuType.CompositeReversed: return TryGetCompositeReversed();
            }
            return false;

            //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

            bool TryGetRowValue()
            {
                if (!qx.HasValidSelect || !qx.Select.GetValue(key, out string val))
                    return false;

                cx.Value.SetValue(key, val);
                return true;
            }

            bool TryGetRelated()
            {
                var selectors = new List<Query>();
                var forest = GetForest(cx, key, selectors);
                if (selectors.Count == 0)
                    return false;

                return cx.Value.LoadCache(cx, key, selectors);
            }

            bool TryGetNumericSet()
            {
                return cx.Value.SetValue(key, "0, 1, 0, 1");
            }

            bool TryGetCompositeString(bool reverse = false)
            {
                var selectors = new List<Query>();
                var forest = GetForest(cx, key, selectors);
                if (forest == null || forest.Length == 0 || selectors.Count == 0)
                    return false;

                var sb = new StringBuilder(128);

                if (reverse) selectors.Reverse();

                var seperator = cx.Separator;
                if (string.IsNullOrEmpty(seperator)) seperator = null;

                foreach (var q in selectors)
                {
                    if (q.Items == null) continue;
                    var qt = q.QueryX;

                    foreach (var k in q.Items)
                    {
                        if (k == null) continue;
                        if (!qt.Select.GetValue(k, out string text)) continue;
                        if (string.IsNullOrEmpty(text)) text = " ? ";
                        if (sb.Length > 0 && seperator != null)
                            sb.Append(seperator);
                        sb.Append(text);
                    }
                }

                return cx.Value.SetValue(key, sb.ToString());
            }

            bool TryGetCompositeReversed()
            {
                return TryGetCompositeString(true);
            }

        }
        #endregion

        #region AllocateValueCache  ===========================================
        // called when the computeX needs to produce a value, but its ValueCache is null
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

                    cx.Value = Value.Create(GetRelatedValueType(cx));
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
            cx.Value.SetOwner(cx);

            void AllocateCache(QueryX vx)
            {
                var type = vx.Select.ValueType;
                if (type < ValType.MaximumType)
                    cx.Value = Value.Create(type);
                else if (type == ValType.IsUnknown)
                    cx.Value = ValuesUnknown;
                else if (type == ValType.IsCircular)
                    cx.Value = ValuesCircular;
                else if (type == ValType.IsUnresolved)
                    cx.Value = ValuesUnresolved;
                else
                    cx.Value = ValuesInvalid;
            }
        }
        #endregion

        #region GetRelatedValueType  ==========================================
        ValType GetRelatedValueType(ComputeX cx)
        {
            var qx = ComputeX_QueryX.GetChild(cx);
            if (qx == null)
                return ValType.IsInvalid; //computeX must have a root queryX reference

            var children = QueryX_QueryX.GetChildren(qx);
            if (children == null || children.Length == 0)
                return ValType.IsInvalid; //computeX must have atleast one queryX reference

            var workQueue = new Queue<QueryX>(children);
            var isMultiple = children.Length > 1;

            var vTypes = new HashSet<ValType>();

            while(workQueue.Count > 0)
            {/*
                deapth first traversal of queryX true
             */
                var qt = workQueue.Dequeue();

                if (qt.HasValidSelect && qt.Select.ValueType < ValType.MaximumType)
                {
                    vTypes.Add(qt.Select.ValueType);
                    var r = Relation_QueryX.GetParent(qt);
                    if (r != null && (r.Pairing == Pairing.ManyToMany || (!qt.IsReversed && r.Pairing == Pairing.OneToMany)))
                        isMultiple = true;
                }

                children = QueryX_QueryX.GetChildren(qt);
                if (children != null)
                {
                    isMultiple |= children.Length > 1;
                    foreach (var child in children) { workQueue.Enqueue(child); }
                }                
            }

            if (vTypes.Count == 0)
                return ValType.IsInvalid; //computeX must have atleast valid related value

            var vType = ValType.IsInvalid;
            var vGroup = ValGroup.None;
            foreach (var vt in vTypes)
            {
                vGroup |= Value.GetValGroup(vt); // compose an aggregate of value group bit flags
                if (vType == ValType.IsInvalid) vType = vt; // get the first valType
            }
            if (vGroup == ValGroup.None)
                return ValType.IsInvalid; //computeX must have atleast valid related value

            if (vGroup.HasFlag(ValGroup.ArrayGroup))
                isMultiple = true;

            if (vTypes.Count == 1)
            {
                if (isMultiple)
                    vType |= ValType.IsArray;
                else
                    vType &= ~ValType.IsArray;
            }
            else
            {
                if (vGroup == ValGroup.DateTime )
                {
                    vType = (isMultiple) ? ValType.DateTimeArray : ValType.DateTime;
                }
                else if (vGroup == ValGroup.DateTimeArray)
                {
                    vType = ValType.DateTimeArray;
                }
                else if (vGroup.HasFlag(ValGroup.DateTime) || vGroup.HasFlag(ValGroup.DateTimeArray))
                {
                    vType = ValType.StringArray;
                }
                else if (vGroup.HasFlag(ValGroup.ScalarGroup) && !vGroup.HasFlag(ValGroup.ArrayGroup))
                {
                    if (vGroup.HasFlag(ValGroup.String))
                        vType = ValType.StringArray;
                    else if (vGroup.HasFlag(ValGroup.Double))
                        vType = ValType.DoubleArray;
                    else if (vGroup.HasFlag(ValGroup.Long))
                        vType = ValType.Int64Array;
                    else if (vGroup.HasFlag(ValGroup.Int))
                        vType = ValType.Int32Array;
                    else if (vGroup.HasFlag(ValGroup.Bool))
                        vType = ValType.BoolArray;
                }
                else
                {
                    vType = ValType.StringArray;
                }
            }

            return vType;
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

    }
}
