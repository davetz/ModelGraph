﻿using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        private Dictionary<Item, List<Error>> _itemError;

        #region ResetError  ===================================================
        //
        void ResetError()
        {
            _itemError = new Dictionary<Item,List<Error>>();
        }
        #endregion

        #region HasError  =====================================================
        internal bool HasItemError(Item item)
        {
            return _itemError.ContainsKey(item);
        }
        #endregion

        #region SetError  =====================================================
        //
        internal void SetError(Error error)
        {
            var item = error.Item1;
            if (!_itemError.TryGetValue(item, out List<Error> errors))
            {
                errors = new List<Error>(2);
                _itemError.Add(item, errors);
            }
            errors.Add(error);
        }
        #endregion

        #region GetError  =====================================================
        internal string GetError(Item item)
        {
            if (!_itemError.TryGetValue(item, out List<Error> errors)) return null;
            if (errors.Count == 0)
            {
                _itemError.Remove(item);
                return null;
            }
            return errors[0].GetError();        
        }
        #endregion

        #region GetErrors  ====================================================
        internal int GetErrorCount()
        {
            int count = 0;
            foreach (var e1 in _itemError)
            {
                count += e1.Value.Count;
            }
            return count;
        }
        internal bool TryGetErrors(out List<Error> items)
        {
            var errors = new List<Error>(GetErrorCount());

            foreach (var e1 in _itemError)
            {
                errors.AddRange(e1.Value);
            }
            items = errors;
            return (items.Count > 0);
        }
        #endregion

        #region ClearErrors  ===================================================
        internal void ClearErrors(Error error)
        {
            ClearErrors(error.Item1, error.Item2, error.Trait);
        }
        internal void ClearErrors(Item item)
        {
            _itemError.Remove(item);
        }
        internal void ClearErrors(Item item1, Item item2)
        {
            if (!_itemError.TryGetValue(item1, out List<Error> errors)) return;

            var last = errors.Count - 1;
            for (int i = last; i >= 0; i--)
            {
                if (errors[i].Item2 != item2) continue;
                errors.RemoveAt(i);
            }
            if (errors.Count == 0)
                _itemError.Remove(item1);
        }
        internal void ClearErrors(Item item, Item related, Trait trait)
        {
            if (!_itemError.TryGetValue(item, out List<Error> errors)) return;

            var last = errors.Count - 1;
            for (int i = last; i >= 0; i--)
            {
                if (errors[i].Trait != trait) continue;
                if (errors[i].Item2 != related) continue;
                errors.RemoveAt(i);
            }
            if (errors.Count == 0)
                _itemError.Remove(item);
        }
        #endregion

    }
}
