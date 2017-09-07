
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        static InvalidCache InvalidCache = new InvalidCache();
        static CircularCache CyclicalCache = new CircularCache();
        static internal string BlankName = "???"; // indicates blank or missing name
        static internal string InvalidItem = "######"; // indicates invalid reference 
        static internal string CircularItem = "@@@@@@"; // indicates circular reference

        #region ResetCacheValues  =============================================
        private void ResetCacheValues()
        {
            foreach (var cx in _computeXStore.Items) { cx.ValueCache = null; cx.ValueCacheSet = null; }
        }
        #endregion

        #region <Get/Set>SelectString  ========================================
        internal NativeType GetNativeType(ComputeX cd)
        {
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;
            switch (cd.CompuType)
            {
                case CompuType.RowValue:
                    return (root.HasSelect) ? root.Select.NativeType : NativeType.None; ;

                case CompuType.RelatedValue:
                    return (root.HasSelect) ? root.Select.NativeType : NativeType.None; ;

                case CompuType.NumericValueSet:
                    if (cd.NumericSet == NumericSet.Count)
                        return (AnyValueXHeads()) ? NativeType.String : NativeType.Invalid;
                    else
                        return (AnyValueXNumbers()) ? NativeType.String : NativeType.Invalid;

                case CompuType.CompositeString:
                    return (AnyValueXSelects()) ? NativeType.String : NativeType.Invalid;

                case CompuType.CompositeReversed:
                    return (AnyValueXSelects()) ? NativeType.String : NativeType.Invalid;
                default:
                    return NativeType.Invalid;
            }
            bool AnyValueXHeads()
            {
                return QueryX_QueryX.HasChildLink(root);
            }

            bool AnyValueXSelects()
            {
                var vd = root;
                while (vd != null)
                {
                    if (vd.HasSelect) return true;
                    vd = QueryX_QueryX.GetChild(vd) as QueryX;
                }
                return false;
            }

            bool AnyValueXNumbers()
            {
                var vd = root;
                while (vd != null)
                {
                    if (vd.HasSelect && IsNumber(vd.Select.NativeType)) return true;
                    vd = QueryX_QueryX.GetChild(vd) as QueryX;
                }
                return false;
            }

            bool IsNumber(NativeType nt)
            {
                switch(nt)
                {
                    case NativeType.Byte:
                    case NativeType.Int16:
                    case NativeType.Int32:
                    case NativeType.Int64:
                    case NativeType.Double:
                        return true;
                    default:
                        return false;
                }
            }
        }
        internal string GetSelectString(ComputeX cd)
        {
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;
            return (root != null && root.HasSelect) ? root.SelectString : null;
        }
        internal void SetSelectString(ComputeX cd, string value)
        {
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;
            if (root != null) root.SelectString = value;
        }
        #endregion

        #region TrySetComputeTypeProperty  ====================================
        private bool TrySetComputeTypeProperty(ComputeX cd, string value)
        {
            if (!Enum.TryParse<CompuType>(value, out cd.CompuType))
            {
                return false;
            }
            cd.ValueCache = null;
            cd.ValueCacheSet = null;
            return true;
        }
        #endregion

        #region TrySetNumericSetProperty  =====================================
        private bool TrySetNumericSetProperty(ComputeX cd, string value)
        {
            if (!Enum.TryParse<NumericSet>(value, out cd.NumericSet))
            {
                return false;
            }
            cd.ValueCache = null;
            cd.ValueCacheSet = null;
            return true;
        }
        #endregion

        #region SetComputeXProperty  ==========================================
        private bool SetComputeXWhere(ComputeX cd, string value)
        {
            var vx = ComputeX_QueryX.GetChild(cd) as QueryX;
            if (vx != null)
            {
                vx.WhereString = value;
                ValidateValueX(vx);
            }
            cd.ValueCache = null;
            cd.ValueCacheSet = null;
            return true;
        }
        private bool SetComputeXSelect(ComputeX cd, string value)
        {
            var vx = ComputeX_QueryX.GetChild(cd) as QueryX;
            if (vx != null)
            {
                vx.SelectString = value;
                ValidateValueX(vx);
            }
            cd.ValueCache = null;
            cd.ValueCacheSet = null;
            return true;
        }
        #endregion

        #region ValidateValueXChange  =========================================
        private void ValidateValueXChange(QueryX vx)
        {
            ValidateValueX(vx);

            var qx = vx as QueryX;
            while (qx != null)
            {
                var qx2 = QueryX_QueryX.GetParent(qx);
                if (qx2 == null)
                {
                    var cx = ComputeX_QueryX.GetParent(qx);
                    if (cx != null)
                    {
                        cx.ValueCache = null;
                        cx.ValueCacheSet = null;
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
                    if (vx == null || vx.Select == null || vx.Select.NativeType == NativeType.Invalid)
                        cx.ValueCache = InvalidCache;
                    else
                        AllocateCache(vx);
                    break;

                case CompuType.RelatedValue:

                    var qx = ComputeX_QueryX.GetChild(cx) as QueryX;
                    while(QueryX_QueryX.HasChildLink(qx)) { qx = QueryX_QueryX.GetChild(qx); }
                    if (qx == null || qx.Select == null || qx.Select.NativeType == NativeType.Invalid)
                        cx.ValueCache = InvalidCache;
                    else
                        AllocateCache(qx);
                    break;

                case CompuType.NumericValueSet:

                    cx.ValueCache = new StringCache(); //display a composite of the numeric set values
                    AlocateCacheSet();
                    break;

                case CompuType.CompositeString:

                    cx.ValueCache = new StringCache();
                    break;

                case CompuType.CompositeReversed:

                    cx.ValueCache = new StringCache();
                    break;
            }

            void AllocateCache(QueryX vx)
            {
                switch (vx.Select.NativeType)
                {
                    case NativeType.Bool:
                        cx.ValueCache = new BoolCache();
                        break;
                    case NativeType.Byte:
                        cx.ValueCache = new ByteCache();
                        break;
                    case NativeType.Int16:
                        cx.ValueCache = new Int16Cache();
                        break;
                    case NativeType.Int32:
                        cx.ValueCache = new Int32Cache();
                        break;
                    case NativeType.Int64:
                        cx.ValueCache = new Int64Cache();
                        break;
                    case NativeType.Double:
                        cx.ValueCache = new DoubleCache();
                        break;
                    case NativeType.String:
                        cx.ValueCache = new StringCache();
                        break;
                    default:
                        cx.ValueCache = InvalidCache;
                        break;
                }
            }

            void AlocateCacheSet()
            {
                switch (cx.NumericSet)
                {
                    case NumericSet.Count:
                        cx.ValueCacheSet = new ICacheValue[] { new DoubleCache(), };
                        break;
                    case NumericSet.Count_Min_Max:
                        cx.ValueCacheSet = new ICacheValue[] { new DoubleCache(), new DoubleCache(), new DoubleCache(), };
                        break;
                    case NumericSet.Count_Min_Max_Sum:
                        cx.ValueCacheSet = new ICacheValue[] { new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), };
                        break;
                    case NumericSet.Count_Min_Max_Sum_Ave:
                        cx.ValueCacheSet = new ICacheValue[] { new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), };
                        break;
                    case NumericSet.Count_Min_Max_Sum_Ave_Std:
                        cx.ValueCacheSet = new ICacheValue[] { new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), new DoubleCache(), };
                        break;
                }
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
            cx.ValueCache.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out byte value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            cx.ValueCache.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out int value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            cx.ValueCache.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out short value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            cx.ValueCache.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out long value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null) selector.GetValue(item, out value);
            cx.ValueCache.SetValue(item, value);
        }
        internal void UpdateValueCache(Item item, ComputeX cx, out double value)
        {
            value = 0;
            var selector = GetRootSelect(cx);
            if (selector != null)
            {
                selector.GetValue(item, out value);
                cx.ValueCache.SetValue(item, value);
            }
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
            cx.ValueCache.SetValue(item, value);
        }
        #endregion

        #region GetNumericResult  =============================================
        private void GetNumericResult(Item item, ComputeX cd, out string value)
        {
            value = InvalidItem;

            var tailQuerys = new List<Query>();
            var forest = GetForest(cd, item, tailQuerys);
            var root = ComputeX_QueryX.GetChild(cd) as QueryX;

            if (forest != null && root != null)
            {
                var values = new List<(Item Item, WhereSelect Select)>();
                var sb = new StringBuilder(120);

                double v, cnt = 0, min = double.MaxValue, max = double.MinValue, sum = 0, ave = 0, std = 0;

                switch (cd.NumericSet)
                {
                    case NumericSet.Count:

                        foreach (var tailSeg in tailQuerys)
                        {
                            cnt += tailSeg.ItemCount;
                        }
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
                        break;
                }

                sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Count)));
                sb.Append("=");
                sb.Append(cnt.ToString());
                sb.Append("  ");
                cd.ValueCacheSet[(int)NumericTerm.Count].SetValue(item, cnt);

                if (cd.NumericSet == NumericSet.Count_Min_Max || cd.NumericSet == NumericSet.Count_Min_Max_Sum || cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Min)));
                    sb.Append("=");
                    sb.Append(DecimalString(min));
                    sb.Append("  ");
                    cd.ValueCacheSet[(int)NumericTerm.Min].SetValue(item, min);

                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Max)));
                    sb.Append("=");
                    sb.Append(DecimalString(max));
                    sb.Append("  ");
                    cd.ValueCacheSet[(int)NumericTerm.Max].SetValue(item, max);
                }

                if (cd.NumericSet == NumericSet.Count_Min_Max_Sum || cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Sum)));
                    sb.Append("=");
                    sb.Append(DecimalString(sum));
                    sb.Append("  ");
                    cd.ValueCacheSet[(int)NumericTerm.Sum].SetValue(item, sum);
                }

                if (cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave || cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Ave)));
                    sb.Append("=");
                    sb.Append(DecimalString(ave));
                    sb.Append("  ");
                    cd.ValueCacheSet[(int)NumericTerm.Ave].SetValue(item, ave);
                }

                if (cd.NumericSet == NumericSet.Count_Min_Max_Sum_Ave_Std)
                {
                    sb.Append(_resourceLoader.GetString(GetNameKey(Trait.NumericTerm_Std)));
                    sb.Append("=");
                    sb.Append(DecimalString(std));
                    sb.Append("  ");
                    cd.ValueCacheSet[(int)NumericTerm.Std].SetValue(item, std);
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
                    if (root.HasSelect && root.Select.NativeType != NativeType.None)
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
                    if (root.HasSelect && root.Select.NativeType != NativeType.None)
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
            if (vd.Select == null) return GetEnumDisplayValue(_nativeTypeEnum, (int)NativeType.None);
            return GetEnumDisplayValue(_nativeTypeEnum, (int)vd.Select.NativeType);
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
                foreach (var cx in cxList) { cx.SetNativeType(NativeType.None); }
                var anyChange = true;
                while (anyChange)
                {
                    anyChange = false;
                    foreach (var cx in cxList)
                    {
                        if (cx.NativeType == NativeType.None)
                        {
                            var qx = ComputeX_QueryX.GetChild(cx);
                            if (qx == null)
                            {
                                cx.SetNativeType(NativeType.Invalid);
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
                                        cx.SetNativeType(NativeType.Invalid);
                                        anyChange = true;
                                    }
                                    else
                                    {
                                        qx.Select.Validate(st);
                                        cx.SetNativeType(qx.Select.NativeType);
                                        anyChange = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}