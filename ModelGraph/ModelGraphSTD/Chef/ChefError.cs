using System.Collections.Generic;

namespace ModelGraphSTD
{
    public partial class Chef
    {
        private readonly Dictionary<Item, Error> _itemError = new Dictionary<Item, Error>(); //the most serious item error
        private readonly Dictionary<(Item, Item), Error> _itemErrorAux1 = new Dictionary<(Item, Item), Error>(); //the most serious item error
        private readonly Dictionary<(Item, Item, Item), Error> _itemErrorAux2 = new Dictionary<(Item, Item, Item), Error>(); //the most serious item error

        #region RepositoryError  ==============================================
        public void AddRepositorReadError(string text)
        {
            _itemError[ImportBinaryReader] = new ErrorOne(ErrorStore, this, Trait.ImportError);
        }
        public void AddRepositorWriteError(string text)
        {
            _itemError[ExportBinaryWriter] = new ErrorOne(ErrorStore, this, Trait.ExportError);
        }
        #endregion

        #region ClearItemErrors  ==============================================
        void ClearItemErrors()
        {
            _itemError.Clear();
        }
        #endregion

        #region TryAddError  ==================================================
        internal ErrorNone TryAddErrorNone(Item item, Trait trait)
        {
            var prevError = TryGetError(item);
            if (prevError != null)
            {
                if (prevError is ErrorNone error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemError.Remove(item);
            }

            var newError = new ErrorNone(ErrorStore, item, trait);
            _itemError[item] = newError;
            item.HasError = true; ;
            return newError;
        }
        internal ErrorNoneAux TryAddErrorNone(Item item, Item aux1, Trait trait)
        {
            var prevError = TryGetError(item, aux1);
            if (prevError != null)
            {
                if (prevError is ErrorNoneAux error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux1.Remove((item, aux1));
            }

            var newError = new ErrorNoneAux(ErrorStore, item, aux1,  trait);
            _itemErrorAux1[(item, aux1)] = newError;
            item.HasErrorAux1 = true;
            return newError;
        }
        internal ErrorNoneAux2 TryAddErrorNone(Item item, Item aux1, Item aux2, Trait trait)
        {
            var prevError = TryGetError(item, aux1, aux2);
            if (prevError != null)
            {
                if (prevError is ErrorNoneAux2 error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux2.Remove((item, aux1, aux2));
            }

            var newError = new ErrorNoneAux2(ErrorStore, item, aux1, aux2, trait);
            _itemErrorAux2[(item, aux1, aux2)] = newError;
            item.HasErrorAux2 = true;
            return newError;
        }
        internal ErrorOne TryAddErrorOne(Item item, Trait trait)
        {
            var prevError = TryGetError(item);
            if (prevError != null)
            {
                if (prevError is ErrorOne error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemError.Remove(item);
            }

            var newError = new ErrorOne(ErrorStore, item, trait);
            _itemError[item] = newError;
            item.HasError = true;
            return newError;
        }
        internal ErrorOneAux TryAddErrorOne(Item item, Item aux1, Trait trait)
        {
            var prevError = TryGetError(item, aux1);
            if (prevError != null)
            {
                if (prevError is ErrorOneAux error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux1.Remove((item, aux1));
            }

            var newError = new ErrorOneAux(ErrorStore, item, aux1, trait);
            _itemErrorAux1[(item, aux1)] = newError;
            item.HasErrorAux1 = true;
            return newError;
        }
        internal ErrorOneAux2 TryAddErrorOne(Item item, Item aux1, Item aux2, Trait trait)
        {
            var prevError = TryGetError(item, aux1, aux2);
            if (prevError != null)
            {
                if (prevError is ErrorOneAux2 error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux2.Remove((item, aux1, aux2));
            }

            var newError = new ErrorOneAux2(ErrorStore, item, aux1, aux2, trait);
            _itemErrorAux2[(item, aux1, aux2)] = newError;
            item.HasErrorAux2 = true;
            return newError;
        }
        internal ErrorMany TryAddErrorMany(Item item, Trait trait)
        {
            var prevError = TryGetError(item);
            if (prevError != null)
            {
                if (prevError is ErrorMany error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemError.Remove(item);
            }

            var newError = new ErrorMany(ErrorStore, item, trait);
            _itemError[item] = newError;
            item.HasError = true;
            return newError;
        }
        internal ErrorManyAux TryAddErrorMany(Item item, Item aux1, Trait trait)
        {
            var prevError = TryGetError(item, aux1);
            if (prevError != null)
            {
                if (prevError is ErrorManyAux error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux1.Remove((item, aux1));
            }

            var newError = new ErrorManyAux(ErrorStore, item, aux1, trait);
            _itemErrorAux1[(item, aux1)] = newError;
            item.HasErrorAux1 = true;
            return newError;
        }
        internal ErrorManyAux2 TryAddErrorMany(Item item, Item aux1, Item aux2, Trait trait)
        {
            var prevError = TryGetError(item, aux1, aux2);
            if (prevError != null)
            {
                if (prevError is ErrorManyAux2 error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ErrorStore.Remove(prevError);
                _itemErrorAux2.Remove((item, aux1, aux2));
            }

            var newError = new ErrorManyAux2(ErrorStore, item, aux1, aux2, trait);
            _itemErrorAux2[(item, aux1, aux2)] = newError;
            item.HasErrorAux2 = true;
            return newError;
        }

        #endregion

        #region ClearError  ===================================================
        internal void ClearError(Item item)
        {
            if (item.HasError && _itemError.TryGetValue(item, out Error error))
            {
                ErrorStore.Remove(error);
                _itemError.Remove(item);
            }
            item.HasError = false;
        }
        internal void ClearError(Item item, Item aux1)
        {
            if (item.HasErrorAux1 && _itemErrorAux1.TryGetValue((item, aux1), out Error error))
            {
                ErrorStore.Remove(error);
                _itemErrorAux1.Remove((item, aux1));
            }
            item.HasErrorAux1 = false;
        }
        internal void ClearError(Item item, Item aux1, Item aux2)
        {
            if (item.HasErrorAux2 && _itemErrorAux2.TryGetValue((item, aux1, aux2), out Error error))
            {
                ErrorStore.Remove(error);
                _itemErrorAux2.Remove((item, aux1, aux2));
            }
            item.HasErrorAux2 = false;
        }
        #endregion

        #region TryGetError  ==================================================
        internal Error TryGetError(Item item)
        {
            if (item.HasError)
            {
                if (_itemError.TryGetValue(item, out Error error)) return error;
                item.HasError = false;
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1)
        {
            if (item.HasErrorAux1)
            {
                if (_itemErrorAux1.TryGetValue((item, aux1), out Error error)) return error;
                item.HasErrorAux1 = false;
            }
            return null;
        }
        internal Error TryGetError(Item item, Item aux1, Item aux2)
        {
            if (item.HasErrorAux2)
            {
                if (_itemErrorAux2.TryGetValue((item, aux1, aux2), out Error error)) return error;
                item.HasErrorAux2 = false;
            }
            return null;
        }

        internal bool TestError(Item item) => (TryGetError(item) is null) ? false : true;
        internal bool TestError(Item item, Item aux1) => (TryGetError(item, aux1) is null) ? false : true;
        internal bool TestError(Item item, Item aux1, Item aux2) => (TryGetError(item, aux1, aux2) is null) ? false : true;
        #endregion
    }
}
