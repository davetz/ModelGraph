using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        private Dictionary<Item, List<Error>> _itemError = new Dictionary<Item, List<Error>>();

        #region RepositoryError  ==============================================
        public void AddRepositorReadError(string text)
        {
            var error = new Error(ErrorStore, this, Trait.ImportError);
            error.Item = this;
            error.Add(text);
            AddError(error);
        }
        public void AddRepositorWriteError(string text)
        {
            var error = new Error(ErrorStore, this, Trait.ExportError);
            error.Item = this;
            error.Add(text);
            AddError(error);
        }
        #endregion

        #region ClearItemErrors  ==============================================
        void ClearItemErrors()
        {
            _itemError.Clear();
        }
        #endregion

        #region AddError  =====================================================
        internal void AddError(Error error)
        {
            var item = error.Item;
            if (!_itemError.TryGetValue(item, out List<Error> errors))
            {
                errors = new List<Error>(2);
                _itemError.Add(item, errors);
            }
            errors.Add(error);
            item.HasError = true;
        }
        #endregion

        #region TryGetError  ==================================================
        internal Error TryGetError(Item item)
        {
            if (_itemError.TryGetValue(item, out List<Error> errors))
            {
                if (errors.Count > 0) return errors[0];
                _itemError.Remove(item);
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1)
        {
            if (aux1 is null) return TryGetError(item);

            if (_itemError.TryGetValue(item, out List<Error> errors))
            {
                foreach (var error in errors)
                {
                    if (error.Aux1 == aux1) return error;
                }
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1, Item aux2)
        {
            if (aux1 is null) return TryGetError(item);
            if (aux2 is null) return TryGetError(item, aux1);

            if (_itemError.TryGetValue(item, out List<Error> errors))
            {
                foreach (var error in errors)
                {
                    if (error.Aux1 == aux1 && error.Aux2 == aux2) return error;
                }
            }
            return null;
        }
        #endregion

        #region ClearError  ===================================================
        internal void ClearError(Item item, Error error)
        {
            if (_itemError.TryGetValue(item, out List<Error> errors))
            {
                if (error != null)
                {
                    errors.Remove(error);
                    ErrorStore.Remove(error);
                    item.HasError = errors.Count > 0;
                }
            }
            else
                item.HasError = false;
        }
        #endregion
    }
}
