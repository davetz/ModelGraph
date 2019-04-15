using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorManyAux : ErrorMany
    {
        internal Item Aux;

        #region Constructor  ==================================================
        internal ErrorManyAux(StoreOf<Error> owner, Item item, Item aux1, Trait trait, string text = null) : base(owner, item, trait, text)
        {
            Aux = aux1;
        }
        #endregion
    }
}
