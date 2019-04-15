using System.Collections.Generic;

namespace ModelGraphSTD
{
    internal class ErrorMany : Error
    {
        private readonly List<string> _list = new List<string>(1);
        internal List<string> List => _list;

        #region Constructor  ==================================================
        internal ErrorMany(StoreOf<Error> owner, Item item, Trait trait, string text = null)
        {
            Owner = owner;
            Item = item;
            Trait = trait;

            if (text != null) _list.Add(text);

            owner.Add(this);
        }
        #endregion

        #region Overrides  ====================================================
        internal override void Add(string error)
        {
            _list.Add(error);
        }
        internal override void Clear() => _list.Clear();

        internal override string GetError(int index = 0)
        {
            if (index < 0 || index > Count) return null;

            return _list[index];
        }
        internal override int Count => _list.Count;
        #endregion
    }
}
