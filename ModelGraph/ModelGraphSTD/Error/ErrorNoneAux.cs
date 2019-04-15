using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorNoneAux : ErrorNone
    {
        internal Item Aux;

        #region Constructor  ==================================================
        internal ErrorNoneAux(StoreOf<Error> owner, Item item, Item aux, Trait trait) : base(owner, item, trait)
        {
            Aux = aux;
        }
        #endregion
    }
}
