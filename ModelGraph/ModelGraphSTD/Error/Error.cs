using System.Collections.Generic;

namespace ModelGraphSTD
{
    public class Error : Item
    {
        internal Item Item;
        internal Item Aux1;
        internal Item Aux2;
        internal Item Parent; // parent item associated with this error
        internal List<string> Errors = new List<string>(1);

        #region Constructor  ==================================================
        internal Error(StoreOf<Error> owner, Item item, Trait trait, string error = null)
        {
            Owner = owner;
            Item = item;
            Trait = trait;

            if (error != null) Errors.Add(error);

            owner.Add(this);
        }
        internal Error(StoreOf<Error> owner, Item item, Item aux1, Trait trait, string error = null)
        {
            Owner = owner;
            Item = item;
            Aux1 = aux1;
            Trait = trait;

            if (error != null) Errors.Add(error);

            owner.Add(this);
        }
        internal Error(StoreOf<Error> owner, Item item, Item aux1, Item aux2, Trait trait, string error = null)
        {
            Owner = owner;
            Item = item;
            Aux1 = aux1;
            Aux2 = aux2;
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
        internal void Clear() => Errors.Clear();

        internal int Count => Errors.Count;
        internal string GetError(int index = 0)
        {
            if (index < 0 || index > Count) return null;

            return Errors[index];
        }
        #endregion
    }
}
