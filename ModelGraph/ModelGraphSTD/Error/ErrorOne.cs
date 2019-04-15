using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorOne : Error
    {
        private string _text;

        #region Constructor  ==================================================
        internal ErrorOne(StoreOf<Error> owner, Item item, Trait trait, string text = null)
        {
            Owner = owner;
            Item = item;
            Trait = trait;
            _text = text;
            owner.Add(this);

        }
        #endregion

        #region Overrides  ====================================================
        internal override void Add(string text) => _text = text;
        internal override void Clear() => _text = null;

        internal override int Count => (_text is null) ? 0 : 1;
        internal override string GetError(int index = 0) => (_text is null) ? string.Empty : _text;
        #endregion
    }
}
