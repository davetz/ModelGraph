using System.Collections.Generic;

namespace ModelGraphLibrary
{/*
 */
    public class Error : Item
    {
        internal Item Item1;
        internal Item Item2;
        internal List<string> Errors = new List<string>(1);

        #region Constructor  ==================================================
        internal Error(Store owner, Trait trait, string error = null)
        {
            Owner = owner;
            Trait = trait;

            if (error != null) Errors.Add(error);

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal void Add(string error)
        {
            Errors.Add(error);
        }
        internal int Count => Errors.Count;
        internal string GetError(int index = 0)
        {
            if (index < 0 || index > Count) return null;

            return Errors[index];
        }
        #endregion
    }
}
