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

        #region AddError  =====================================================
        private ErrorNone AddError(Item item, ErrorNone error)
        {
            _itemError[item] = error;
            return error;
        }
        private ErrorOne AddError(Item item, ErrorOne error)
        {
            _itemError[item] = error;
            return error;
        }
        private ErrorMany AddError(Item item, ErrorMany error)
        {
            _itemError[item] = error;
            return error;
        }

        private ErrorNoneAux AddError(Item item, Item aux1, ErrorNoneAux error)
        {
            _itemErrorAux1[(item, aux1)] = error;
            return error;
        }
        private ErrorOneAux AddError(Item item, Item aux1, ErrorOneAux error)
        {
            _itemErrorAux1[(item, aux1)] = error;
            return error;
        }
        private ErrorManyAux AddError(Item item, Item aux1, ErrorManyAux error)
        {
            _itemErrorAux1[(item, aux1)] = error;
            return error;
        }

        private ErrorNoneAux2 AddError(Item item, Item aux1, Item aux2, ErrorNoneAux2 error)
        {
            _itemErrorAux2[(item, aux1, aux2)] = error;
            return error;
        }
        private ErrorOneAux2 AddError(Item item, Item aux1, Item aux2, ErrorOneAux2 error)
        {
            _itemErrorAux2[(item, aux1, aux2)] = error;
            return error;
        }
        private ErrorManyAux2 AddError(Item item, Item aux1, Item aux2, ErrorManyAux2 error)
        {
            _itemErrorAux2[(item, aux1, aux2)] = error;
            return error;
        }
        #endregion

        #region ClearError  ===================================================
        internal void ClearError(Item item)
        {
            if (_itemError.TryGetValue(item, out Error error))
                ClearError(item, error);
        }
        internal void ClearError(Item item, Item aux1)
        {
            if (_itemErrorAux1.TryGetValue((item, aux1), out Error error))
                ClearError(item, aux1, error);
        }
        internal void ClearError(Item item, Item aux1, Item aux2)
        {
            if (_itemErrorAux2.TryGetValue((item, aux1, aux2), out Error error))
                ClearError(item, aux1, aux2, error);
        }

        internal void ClearError(Item item, Trait trait)
        {
            if (_itemError.TryGetValue(item, out Error error) && error.Trait == trait)
                ClearError(item, error);
        }
        internal void ClearError(Item item, Item aux1, Trait trait)
        {
            if (_itemErrorAux1.TryGetValue((item, aux1), out Error error) && error.Trait == trait)
                ClearError(item, aux1, error);
        }
        internal void ClearError(Item item, Item aux1, Item aux2, Trait trait)
        {
            if (_itemErrorAux2.TryGetValue((item, aux1, aux2), out Error error) && error.Trait == trait)
                ClearError(item, aux1, aux2, error);
        }

        internal void ClearError(Item item, Error error)
        {
            if (error.IsErrorAux) throw new System.Exception("Corrupt Error Hierarcy");
            ErrorStore.Remove(error);
            _itemError.Remove(item);
        }
        internal void ClearError(Item item, Item aux1, Error error)
        {
            if (!error.IsErrorAux1) throw new System.Exception("Corrupt Error Hierarcy");
            ErrorStore.Remove(error);
            _itemErrorAux1.Remove((item, aux1));
        }
        internal void ClearError(Item item, Item aux1, Item aux2, Error error)
        {
            if (!error.IsErrorAux1) throw new System.Exception("Corrupt Error Hierarcy");
            ErrorStore.Remove(error);
            _itemErrorAux2.Remove((item, aux1, aux2));
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

                ClearError(item, prevError);
            }
            return AddError(item, new ErrorNone(ErrorStore, item, trait));
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

                ClearError(item, aux1, prevError);
            }
            return AddError(item, aux1, new ErrorNoneAux(ErrorStore, item, aux1, trait));
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

                ClearError(item, aux1, aux2, prevError);
            }
            return AddError(item, aux1, aux2, new ErrorNoneAux2(ErrorStore, item, aux1, aux2, trait));
        }

        internal ErrorOne TryAddErrorOne(Item item, Trait trait, string text = null)
        {
            var prevError = TryGetError(item);
            if (prevError != null)
            {
                if (prevError is ErrorOne error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, prevError);
            }
            return AddError(item, new ErrorOne(ErrorStore, item, trait, text));
        }
        internal ErrorOneAux TryAddErrorOne(Item item, Item aux1, Trait trait, string text = null)
        {
            var prevError = TryGetError(item, aux1);
            if (prevError != null)
            {
                if (prevError is ErrorOneAux error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, aux1, prevError);
            }
            return AddError(item, aux1, new ErrorOneAux(ErrorStore, item, aux1, trait, text));
        }
        internal ErrorOneAux2 TryAddErrorOne(Item item, Item aux1, Item aux2, Trait trait, string text = null)
        {
            var prevError = TryGetError(item, aux1, aux2);
            if (prevError != null)
            {
                if (prevError is ErrorOneAux2 error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, aux1, aux2, prevError);
            }
            return AddError(item, aux1, aux2, new ErrorOneAux2(ErrorStore, item, aux1, aux2, trait, text));
        }

        internal ErrorMany TryAddErrorMany(Item item, Trait trait, string text = null)
        {
            var prevError = TryGetError(item);
            if (prevError != null)
            {
                if (prevError is ErrorMany error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, prevError);
            }
            return AddError(item, new ErrorMany(ErrorStore, item, trait, text));
        }
        internal ErrorManyAux TryAddErrorMany(Item item, Item aux1, Trait trait, string text = null)
        {
            var prevError = TryGetError(item, aux1);
            if (prevError != null)
            {
                if (prevError is ErrorManyAux error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, aux1, prevError);
            }
            return AddError(item, aux1, new ErrorManyAux(ErrorStore, item, aux1, trait, text));
        }
        internal ErrorManyAux2 TryAddErrorMany(Item item, Item aux1, Item aux2, Trait trait, string text = null)
        {
            var prevError = TryGetError(item, aux1, aux2);
            if (prevError != null)
            {
                if (prevError is ErrorManyAux2 error && error.Trait == trait)
                    return error; // this error already exists

                if (prevError.TraitIndex > TraitIndexOf(trait))
                    return null; // prevError has hight traitIndex and will not be replace

                ClearError(item, aux1, aux2, prevError);
            }
            return AddError(item, aux1, aux2, new ErrorManyAux2(ErrorStore, item, aux1, aux2, trait, text));
        }
        #endregion

        #region TryGetError  ==================================================
        internal Error TryGetError(Item item)
        {
            return (_itemError.TryGetValue(item, out Error error)) ? error : null;
        }
        internal Error TryGetError(Item item, Item aux1)
        {
            return (_itemErrorAux1.TryGetValue((item, aux1), out Error error)) ? error : null;
        }
        internal Error TryGetError(Item item, Item aux1, Item aux2)
        {
            return (_itemErrorAux2.TryGetValue((item, aux1, aux2), out Error error)) ? error :  null;
        }
        #endregion
    }
}
