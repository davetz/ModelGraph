using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorManyAux2 : ErrorMany
    {
        internal Item Aux1;
        internal Item Aux2;

        #region Constructor  ==================================================
        internal ErrorManyAux2(StoreOf<Error> owner, Item item, Item aux1, Item aux2, Trait trait, string text = null) : base(owner, item, trait, text)
        {
            Aux1 = aux1;
            Aux2 = aux2;
        }
        #endregion
    }
}
