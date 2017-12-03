using System;
using ModelGraph.Helpers;

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

        #region GetEnumZKey  ==================================================
        int GetEnumZKey(EnumZ e, string name)
        {
            if (e != null && e.Count > 0)
            {
                var items = e.Items;
                var N = e.Count;

                var s = new string[N];
                var v = new int[N];

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    if (name == p.NameKey.GetLocalized())
                        return p.EnumKey;
                }
            }
            return 0;
        }
        #endregion

        #region GetEnumZIndex  ================================================
        int GetEnumZIndex(EnumZ e, int key)
        {
            if (e != null && e.Count > 0)
            {
                var items = e.Items;
                var N = e.Count;

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    if (p.EnumKey == key)
                        return i;
                }
            }
            return 0;
        }
        #endregion

        #region GetEnumZName  =================================================
        string GetEnumZName(EnumZ e, int key)
        {
            if (e != null && e.Count > 0)
            {
                var items = e.Items;
                var N = e.Count;

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    if (p.EnumKey == key)
                        return p.NameKey.GetLocalized();
                }
            }
            return "######";
        }
        #endregion

        #region GetEnumZNames  ================================================
        string[] GetEnumZNames(EnumZ e)
        {
            if (e != null && e.Count > 0 )
            {
                var items = e.Items;
                var N = e.Count;

                var s = new string[N];

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    s[i] = p.NameKey.GetLocalized();
                }
                return s;
            }
            return new string[0];
        }
        #endregion
    }
}
