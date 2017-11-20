using System;

namespace ModelGraph.Internals
{/*

 */
    class SubsetOf<T1, T2>
    {
        Func<T1, T2, bool> _isIncluded;

        #region Constructor  ==================================================
        internal SubsetOf(Func<T1, T2, bool> isIncluded)
        {
            _isIncluded = isIncluded;
        }
        #endregion

        #region GetCount  =====================================================
        internal (int All, int Included, int Excluded) GetCount(T2 key, T1[] list)
        {
            if (list != null)
            {
                var M = list.Length;
                if (M > 0)
                {
                    var N = 0;
                    for (int i = 0; i < M; i++)
                    {
                        if (_isIncluded(list[i], key)) { N++; }
                    }
                    return (M, N, (M - N));
                }
            }
            return (0, 0, 0);
        }
        internal (int All, int Included, int Excluded, bool[] Include) GetResult(T2 key, T1[] list)
        {
            if (list != null)
            {
                var M = list.Length;
                if (M > 0)
                {
                    var include = new bool[M];
                    var N = 0;
                    for (int i = 0; i < M; i++)
                    {
                        if (_isIncluded(list[i], key)) { N++; include[i] = true; }
                    }
                    return (M, N, (M - N), include);
                }
            }
            return (0, 0, 0, null);
        }
        #endregion

        #region GetValues  ====================================================
        internal (int Count, T1[] Items) GetIncluded(T2 key, T1[] list)
        {
            (int len, int included, int excluded, bool[] include) = GetResult(key, list);
            if (included == 0)
            {
                return (0, null);
            }
            else if (excluded == 0)
            {
                return (included, list);
            }
            else
            {
                var items = new T1[included];
                for (int i = 0, j = 0; i < len; i++)
                {
                    if (include[i]) items[j++] = list[i];
                }
                return (included, items);
            }
        }
        internal (int Count, T1[] Items) GetExcluded(T2 key, T1[] list)
        {
            (int len, int included, int excluded, bool[] include) = GetResult(key, list);
            if (excluded == 0)
            {
                return (0, null);
            }
            else if (included == 0)
            {
                return (excluded, list);
            }
            else
            {
                var items = new T1[excluded];
                for (int i = 0, j = 0; i < len; i++)
                {
                    if (!include[i]) items[j++] = list[i];
                }
                return (excluded, items);
            }
        }
        #endregion
    }
}
