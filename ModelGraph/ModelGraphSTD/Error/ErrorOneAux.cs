using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorOneAux : ErrorOne
    {
        internal Item Aux;

        #region Constructor  ==================================================
        internal ErrorOneAux(StoreOf<Error> owner, Item item, Item aux, Trait trait, string text = null) : base(owner,item,trait, text)
        {
            Aux = aux;
        }
        #endregion
    }
}
