
namespace ModelGraphSTD
{
    public partial class Chef
    {

        #region GetEnumZKey  ==================================================
        int GetEnumZKey(EnumZ e, string name)
        {
            if (e != null && e.Count > 0)
            {
                var items = e.Items;
                var N = e.Count;

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    if (name == _localize(p.NameKey))
                        return p.EnumKey;
                }
            }
            return 0;
        }
        #endregion

        #region GetEnumZIndex  ================================================
        int GetEnumZIndex(EnumZ e, string name)
        {
            if (e != null && e.Count > 0)
            {
                var items = e.Items;
                var N = e.Count;

                for (int i = 0; i < N; i++)
                {
                    var p = items[i];
                    if (name == _localize(p.NameKey))
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
                        return _localize(p.NameKey);
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
                    s[i] = _localize(p.NameKey);
                }
                return s;
            }
            return new string[0];
        }
        #endregion
    }
}
