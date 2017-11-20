using System;

namespace ModelGraph.Internals
{/*

 */
    public partial class Chef
    {
        bool IsValidEnumValue(Type type, int val)
        {
            var vals = Enum.GetValues(type);
            foreach (var item in vals)
            {
                if ((int)item == val) return true;
            }
            return false;
        }


        #region GetEnumZValues  ===============================================
        internal (string[], int[]) GetEnumZValues(EnumZ e)
        {
            if (e != null && e.Count > 0 )
            {
                var items = e.Items;
                var N = e.Count;

                var s = new string[N];
                var v = new int[N];

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    v[i] = (int)(p.Trait & Trait.EnumMask);
                    s[i] = _resourceLoader.GetString(p.NameKey);
                }
                return (s, v);
            }
            return (new string[] { "######" }, new int[] { 0 });
        }
        #endregion
    }
}
