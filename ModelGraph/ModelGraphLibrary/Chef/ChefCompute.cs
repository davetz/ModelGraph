
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphLibrary
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

        #region ResetCacheValues  =============================================
        private void ResetCacheValues()
        {
            foreach (var cx in _computeXStore.Items) { cx.Values.Clear(); }
        }
        #endregion

        #region <Get/Set>SelectString  ========================================
        internal ValueType GetValueType(ComputeX cx)
        {
            var root = ComputeX_QueryX.GetChild(cx);
            switch (cx.CompuType)
            {
                case CompuType.RowValue:
                    return (root.HasSelect) ? root.Select.ValueType : ValueType.None; ;

                case CompuType.RelatedValue:
                    return (root.HasSelect) ? root.Select.ValueType : ValueType.None; ;

                case CompuType.NumericValueSet:
                    if (cx.NumericSet == NumericSet.Count)
                        return (AnyValueXHeads()) ? ValueType.String : ValueType.Invalid;
                    else
                        return (AnyValueXNumbers()) ? ValueType.String : ValueType.Invalid;

                case CompuType.CompositeString:
                    return (AnyValueXSelects()) ? ValueType.String : ValueType.Invalid;

                case CompuType.CompositeReversed:
                    return (AnyValueXSelects()) ? ValueType.String : ValueType.Invalid;
                default:
                    return ValueType.Invalid;
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

            bool IsNumber(ValueType nt)
            {
                switch(nt)
                {
                    case ValueType.Byte:
                    case ValueType.Int16:
                    case ValueType.Int32:
                    case ValueType.Int64:
                    case ValueType.Double:
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
        private bool TrySetComputeTypeProperty(ComputeX cx, string value)
        {
            if (!Enum.TryParse<CompuType>(value, out cx.CompuType))
            {
                return false;
            }
            cx.Values.Clear();
            return true;
        }
        #endregion

        #region TrySetNumericSetProperty  =====================================
        private bool TrySetNumericSetProperty(ComputeX cx, string value)
        {
            if (!Enum.TryParse<NumericSet>(value, out cx.NumericSet))
            {
                return false;
            }
            cx.Values.Clear();
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
                ValidateValueX(qx);
            }
            cx.Values.Clear();
            return true;
        }
        private bool SetComputeXSelect(ComputeX cx, string value)
        {
            var qx = ComputeX_QueryX.GetChild(cx) as QueryX;
            if (qx != null)
            {
                qx.SelectString = value;
                ValidateValueX(qx);
            }
            cx.Values.Clear();
            return true;
        }
        #endregion

        #region ValidateValueXChange  =========================================
        private void ValidateValueXChange(QueryX qx)
        {
            ValidateValueX(qx);

            while (qx != null)
            {
                var qx2 = QueryX_QueryX.GetParent(qx);
                if (qx2 == null)
                {
                    var cx = ComputeX_QueryX.GetParent(qx);
                    if (cx != null)
                    {
                        cx.Values.Clear();
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
                    if (vx == null || vx.Select == null || vx.Select.ValueType == ValueType.Invalid)
                        cx.Values = ValuesInvalid;
                    else
                        AllocateCache(vx);
                    break;

                case CompuType.RelatedValue:

                    var qx = ComputeX_QueryX.GetChild(cx) as QueryX;
                    while(QueryX_QueryX.HasChildLink(qx)) { qx = QueryX_QueryX.GetChild(qx); }
                    if (qx == null || qx.Select == null || qx.Select.ValueType == ValueType.Invalid)
                        cx.Values = ValuesInvalid;
                    else
                        AllocateCache(qx);
                    break;

                case CompuType.NumericValueSet:

                    cx.Values = Value.Create(ValueType.DoubleArray);
                    break;

                case CompuType.CompositeString:

                    cx.Values = Value.Create(ValueType.String);
                    break;

                case CompuType.CompositeReversed:

                    cx.Values = Value.Create(ValueType.String);
                    break;
            }

            void AllocateCache(QueryX vx)
            {
                var type = vx.Select.ValueType;
                if (type < ValueType.MaximumType)
                    cx.Values = Value.Create(type);
                else if (type == ValueType.None)
                    cx.Values = ValuesNone;
                else if (type == ValueType.Circular)
                    cx.Values = ValuesCircular;
                else if (type == ValueType.Unresolved)
                    cx.Values = ValuesUnresolved;
                else
                    cx.Values = ValuesInvalid;
            }
        }
        #endregion

        #region UpdateValueCache  =============================================
        // called when ComputeX needs to produce a value but there is none in the cache

        internal void UpdateValueCache(Item item, ComputeX cx , out bool value)
        {
            value = false;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfBool v) v.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out byte value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfByte v) v.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out int value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfInt32 v) v.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out short value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfInt16 v) v.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out long value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfInt64 v) v.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out double value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            if (cx.Values is ValueOfDouble v) v.SetValue(item, value);
        }
        private WhereSelect GetRootSelect(ComputeX cx)
        {
            var root = ComputeX_QueryX.GetChild(cx) as QueryX;
            if (root == null || root.Select == null)
                return null;
            else
                return root.Select;
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out string value)
        {
            value = string.Empty;
            switch (cx.CompuType)
            {
                case CompuType.RowValue:
                    value = null;
                    var selector = GetRootSelect(cx);
                    if (selector != null) selector.GetValue(item, out value);
                    break;

                case CompuType.RelatedValue:
                    GetRelatedResult(item, cx, out value);
                    break;

                case CompuType.NumericValueSet:
                    GetNumericResult(item, cx, out value);
                    break;

                case CompuType.CompositeString:
                    GetCompositeResult(item, cx, out value, true);
                    break;

                case CompuType.CompositeReversed:
                    GetCompositeResult(item, cx, out value);
                    break;
            }
            if (cx.Values is ValueOfString v) v.SetValue(item, value);
        }
        #endregion

        #region GetNumericResult  =============================================
        private void GetNumericResult(Item item, ComputeX cx, out string value)
        {
            value = InvalidItem;

            var tailQuerys = new List<Query>();
            var forest = GetForest(cx, item, tailQuerys);
            var root = ComputeX_QueryX.GetChild(cx) as QueryX;

            if (forest != null && root != null)
            {
                var values = new List<(Item Item, WhereSelect Select)>();
                var sb = new StringBuilder(120);

                double v, cnt = 0, min = double.MaxValue, max = double.MinValue, sum = 0, ave = 0, std = 0;

                switch (cx.NumericSet)
                {
                    case NumericSet.Count:

                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                        }
                        if (cx.Values is ValueOfDoubleArray v1) v1.SetValue(item, new double[] { cnt });
                        break;

                    case NumericSet.Count_Min_Max:

                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                            if (tailSeg.TryGetValues(values))
                            {
                                foreach (var val in values)
                                {
                                    val.Select.GetValue(val.Item, out v);
                                    if (v < min) min = v;
                                    if (v > max) max = v;
                                }
                            }
                        }
                        if (cx.Values is ValueOfDoubleArray v3) v3.SetValue(item, new double[] { cnt, min, max });

                        break;

                    case NumericSet.Count_Min_Max_Sum:

                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                            if (tailSeg.TryGetValues(values))
                            {
                                foreach (var val in values)
                                {
                                    val.Select.GetValue(val.Item, out v);
                                    if (v < min) min = v;
                                    if (v > max) max = v;
                                    sum += v;
                                }
                            }
                        }
                        if (cx.Values is ValueOfDoubleArray v4) v4.SetValue(item, new double[] { cnt, min, max, sum });
                        break;

                    case NumericSet.Count_Min_Max_Sum_Ave:

                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                            if (tailSeg.TryGetValues(values))
                            {
                                foreach (var val in values)
                                {
                                    val.Select.GetValue(val.Item, out v);
                                    if (v < min) min = v;
                                    if (v > max) max = v;
                                    sum += v;
                                }
                            }
                        }
                        if (cnt > 0) ave = sum / cnt;
                        if (cx.Values is ValueOfDoubleArray v5) v5.SetValue(item, new double[] { cnt, min, max, sum, ave });
                        break;

                    case NumericSet.Count_Min_Max_Sum_Ave_Std:

                        var vals = new List<double>();
                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                            if (tailSeg.TryGetValues(values))
                            {
                                foreach (var val in values)
                                {
                                    val.Select.GetValue(val.Item, out v);
                                    if (v < min) min = v;
                                    if (v > max) max = v;
                                    sum += v;
                                    vals.Add(v);
                                }
                            }
                        }
                        if (cnt > 0)
                        {
                            ave = sum / cnt;
                            var vsum = 0.0;
                            for (int i = 0; i < vals.Count; i++)
                            {
                                v = vals[i] - ave;
                                vsum += (v * v);
                            }
                            std = Math.Sqrt(vsum / vals.Count);
                        }
                        if (cx.Values is ValueOfDoubleArray v6) v6.SetValue(item, new double[] { cnt, min, max, sum, ave, std });
                        break;
                }

                sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Count)));
                sb.Append("=");
                sb.Append(cnt.ToString());
                sb.Append("  ");

                if (cx.NumericSet == NumericSet.Count_Min_Max || cx.NumericSet == NumericSet.Count_Min_Max_Sum || cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Min)));
                    sb.Append("=");
                    sb.Append(DecimalString(min));
                    sb.Append("  ");

                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Max)));
                    sb.Append("=");
                    sb.Append(DecimalString(max));
                    sb.Append("  ");
                }

                if (cx.NumericSet == NumericSet.Count_Min_Max_Sum || cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Sum)));
                    sb.Append("=");
                    sb.Append(DecimalString(sum));
                    sb.Append("  ");
                }

                if (cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Ave)));
                    sb.Append("=");
                    sb.Append(DecimalString(ave));
                    sb.Append("  ");
                }

                if (cx.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Std)));
                    sb.Append("=");
                    sb.Append(DecimalString(std));
                    sb.Append("  ");
                }

                value = sb.ToString();
            }

        }
        internal static string DecimalString(double val, int digits = 4)
        {
            if (Double.IsNaN(val) || Double.IsNegativeInfinity(val) || Double.IsPositiveInfinity(val) || val == double.MinValue || val == double.MaxValue) return "??";
            return Math.Round(val, digits, MidpointRounding.ToEven).ToString();
        }
        #endregion

        #region GetCompositeResult  ===========================================
        private void GetCompositeResult(Item item, ComputeX cd, out string value, bool isReversed = false)
        {
            value = InvalidItem;

            var tailQuerys = new List<Query>();
            var forest = GetForest(cd, item, tailQuerys);
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;

            if (forest != null && root != null)
            {
                var values = new List<(Item Item, WhereSelect Select)>();
                var sb = new StringBuilder(120);
                string str;
                if (isReversed)
                {
                    // use the first one that can produce values
                    if (root.HasSelect && root.Select.ValueType != ValueType.None)
                    {
                        root.Select.GetValue(item, out str);
                        sb.Append(str);
                    }
                    foreach (var tailSeg in tailQuerys)
                    {
                        if (tailSeg.TryGetValues(values))
                        {
                            values.Reverse();
                            foreach (var val in values)
                            {
                                val.Select.GetValue(val.Item, out str);
                                sb.Append(cd.Separator);
                                sb.Append(str);
                            }
                        }
                    }
                    value = sb.ToString();
                }
                else
                {
                    // use the first one that can produce values
                    foreach (var tailSeg in tailQuerys)
                    {
                        if (tailSeg.TryGetValues(values))
                        {
                            foreach (var val in values)
                            {
                                val.Select.GetValue(val.Item, out str);
                                sb.Append(str);
                                sb.Append(cd.Separator);
                            }
                        }
                    }
                    if (root.HasSelect && root.Select.ValueType != ValueType.None)
                    {
                        root.Select.GetValue(item, out str);
                        sb.Append(str);
                    }
                    value = sb.ToString();
                }
            }
        }
        #endregion

        #region GetRelatedResult  =============================================
        private void GetRelatedResult(Item item, ComputeX cd, out string value)
        {
            value = InvalidItem;

            var tailQuerys = new List<Query>();
            var forest = GetForest(cd, item, tailQuerys);
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;

            if (forest != null && root != null)
            {
                var values = new List<(Item Item, WhereSelect Select)>();
                // use the first one that can produce values
                foreach (var tailSeg in tailQuerys)
                {
                    if (tailSeg.TryGetValues(values))
                    {
                        foreach (var val in values)
                        {
                            val.Select.GetValue(val.Item1, out value);
                            if (string.IsNullOrEmpty(value)) continue;
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region GetSelectorName  ==============================================
        string GetSelectorName(ComputeX item)
        {
            var text = _resourceLoader.GetString("SelectClause"); //<=================== FIX THIS
            var tbl = Store_ComputeX.GetParent(item);
            var tblName = GetIdentity(tbl, IdentityStyle.Single);
            if (tbl != null) return $"{tblName} {text}";

            return text;
        }
        string GetNativeType(QueryX vd)
        {
            if (vd.Select == null) return GetEnumDisplayValue(_nativeTypeEnum, (int)ValueType.None);
            return GetEnumDisplayValue(_nativeTypeEnum, (int)vd.Select.ValueType);
        }
        #endregion

        #region ValidateComputeXStore  ========================================
        private void ValidateComputeXStore()
        {
            if (_computeXStore.Count > 0)
            {
                var cxList = _computeXStore.Items;
                var qxQueue = new Queue<QueryX>();
                var cxList2 = new List<ComputeX>();
                foreach (var cx in cxList) { cx.Values = ValuesNone; }
                var anyChange = true;
                while (anyChange)
                {
                    anyChange = false;
                    foreach (var cx in cxList)
                    {
                        if (cx.ValueType == ValueType.None)
                        {
                            var qx = ComputeX_QueryX.GetChild(cx);
                            if (qx == null)
                            {
                                cx.Values = ValuesInvalid;
                                anyChange = true;
                            }
                            else
                            {
                                qxQueue.Clear();
                                qxQueue.Enqueue(qx);
                                while(qxQueue.Count > 0)
                                {
                                    var qx2 = qxQueue.Dequeue();
                                    //if (qx2.HasSelect && qx2.Select.HasUnresolvedComputeX()) continue;
                                    var st = Store_ComputeX.GetParent(cx);
                                    if (st == null)
                                    {
                                        cx.Values = ValuesInvalid;
                                        anyChange = true;
                                    }
                                    else
                                    {
                                        qx.Select.Validate(st);
                                        anyChange = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region GetValue  =====================================================
        internal void GetValue(ComputeX cx, Item item, out bool value, int index)
        {
            value = false;
        }
        internal void GetValue(ComputeX cx, Item item, out byte value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out sbyte value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out short value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out ushort value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out int value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out uint value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out long value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out ulong value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out float value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out double value, int index)
        {
            value = 0;
        }
        internal void GetValue(ComputeX cx, Item item, out string value, int index)
        {
            value = string.Empty;
        }


        #endregion
    }
}