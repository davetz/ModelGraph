using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        private readonly Dictionary<Item, List<Error>> _itemErrors = new Dictionary<Item, List<Error>>();
        private readonly Dictionary<Item, List<Error>> _relatedErrors = new Dictionary<Item, List<Error>>();

        #region RepositoryError  ==============================================
        public void AddRepositorReadError(string text)
        {
            var error = new Error(ErrorStore, this, Trait.ImportError);
            error.Add(text);
            AddError(error);
        }
        public void AddRepositorWriteError(string text)
        {
            var error = new Error(ErrorStore, this, Trait.ExportError);
            error.Add(text);
            AddError(error);
        }
        #endregion



        #region ClearItemErrors  ==============================================
        void ClearItemErrors()
        {
            _itemErrors.Clear();
        }
        #endregion

        #region AddError  =====================================================
        internal void AddError(Error error)
        {
            var item = error.Item;
            if (!_itemErrors.TryGetValue(item, out List<Error> errors))
            {
                errors = new List<Error>(2);
                _itemErrors.Add(item, errors);
            }
            errors.Add(error);
            item.HasError = true;
        }
        #endregion

        #region GetError  =====================================================
        internal Error GetError(Item item, Trait trait, Item aux1, Item aux2 = null, Trait[] remove = null)
        {
            Error error = null;
            var errors = TryGetErrors(item);
            if (errors != null)
            {
                for (int i = (errors.Count - 1); i >= 0; i--)
                {
                    var err = errors[i];
                    if (err.Trait == trait) error = err; // error already exist
                    if (remove != null)
                    {
                        foreach (var tr in remove)
                        {
                            if (tr == trait) continue;
                            if (err.Trait != tr) continue;

                            errors.RemoveAt(i);
                            ErrorStore.Remove(err);
                        }
                    }
                }
                if (error is null)
                {
                    error = new Error(ErrorStore, item, aux1, aux2, trait);
                    errors.Add(error);
                }
            }
            else
            {
                error = new Error(ErrorStore, item, aux1, aux2, trait);
                errors = new List<Error>(2) { error };

                _itemErrors.Add(item, errors);
            }

            item.HasError = true;
            return error;
        }
        #endregion

        #region TryGetError  ==================================================
        internal List<Error> TryGetErrors(Item item)
        {
            if (item.HasError && _itemErrors.TryGetValue(item, out List<Error> errors))
            {               
                if (errors.Count > 0) return errors;

                item.HasError = false;
                _itemErrors.Remove(item);
            }
            return null;
        }
        internal Error TryGetError(Item item)
        {
            var errors = TryGetErrors(item);
            return errors?[0];
        }
        internal Error TryGetError(Item item, Trait trait)
        {
            var errors = TryGetErrors(item);
            if (errors is null) return null;

            foreach (var error in errors)
            {
                if (error.Trait == trait) return error;
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1)
        {
            if (aux1 is null) return TryGetError(item);

            var errors = TryGetErrors(item);
            if (errors is null) return null;

            foreach (var error in errors)
            {
                if (error.Aux1 == aux1) return error;
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1, Item aux2)
        {
            if (aux1 is null) return TryGetError(item);
            if (aux2 is null) return TryGetError(item, aux1);


            var errors = TryGetErrors(item);
            if (errors is null) return null;

            foreach (var error in errors)
            {
                if (error.Aux1 == aux1 && error.Aux2 == aux2) return error;
            }
            return null;
        }
        #endregion

        #region ClearError  ===================================================
        internal void ClearError(Item item, Error error)
        {
            var errors = TryGetErrors(item);
            if (errors is null)
            {
                if (error != null) throw new System.Exception("Corrupted itemError dictionary");
            }
            else
            {
                if (error != null)
                {
                    errors.Remove(error);
                    ErrorStore.Remove(error);
                    item.HasError = errors.Count > 0;
                }
            }
        }
        internal void ClearErrors(Item item, Trait[] traits)
        {
            var errors = TryGetErrors(item);
            if (errors != null)
            {
                for (int i = (errors.Count - 1); i >= 0; i--)
                {
                    var err = errors[i];
                    foreach (var tr in traits)
                    {
                        if (err.Trait != tr) continue;

                        errors.RemoveAt(i);
                        ErrorStore.Remove(err);
                    }
                }
                if (errors.Count == 0)
                {
                    _itemErrors.Remove(item);
                    item.HasError = false;
                }
            }
        }
        #endregion
    }
}
