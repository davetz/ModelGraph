using System;
using System.Text;

namespace ModelGraphSTD
{/*
 */
    internal class WhereSelect
    {
        private Item _item;
        private ComputeStep _root; // root of the expression tree

        #region Constructor  ==================================================
        internal WhereSelect(string text)
        {
            _root = Parser.CreateExpressionTree(text);
            IsValid = _root.IsValid;
        }
        #endregion

        #region Property  =====================================================
        internal bool IsValid { get; private set; }

        internal bool AnyError => _root != null && _root.AnyError;
        internal bool AnyChange => _root != null && _root.AnyChange;
        internal bool AnyOverflow => _root != null && _root.AnyOverflow;
        internal bool AnyUnresolved => _root != null && _root.AnyUnresolved;
        internal string InputString => GetText();
        internal ValType ValueType => (IsValid) ? _root.ValueType : ValType.IsInvalid;

        private string GetText()
        {
            var sb = new StringBuilder(100);
            _root.GetText(sb);
            return sb.ToString();
        }
        #endregion

        #region TryValidate  ==================================================
        /// <summary>
        /// Create the expression tree and validate all property name references
        /// </summary>
        internal bool TryValidate(Store sto, string text)
        {
            _root = Parser.CreateExpressionTree(text);
            return TryValidate(sto);
        }
        /// <summary>
        /// Validate all property name references
        /// </summary>
        internal bool TryValidate(Store sto)
        {
            _root.TryValidate(sto, () => _item);
            IsValid = _root.IsValid;
            return IsValid;
        }
        #endregion

        #region TryResolve  ===================================================
        /// <summary>
        /// Resolve the expression tree valueTypes, return true if any change
        /// </summary>
        const int maxResolveLoopCount = 100; // avoid infinite loops
        internal bool TryResolve()
        {
            if (!IsValid) return false;
            for (int i = 0; i < maxResolveLoopCount; i++)
            {
                var change = _root.TryResolve();
                if (change) continue;

                return false;
            }
            return true;
        }

        #endregion

        #region Matches / GetValue  ===========================================
        internal bool Matches(Item item)
        {
            if (_root == null || _root.ValueType != ValType.Bool) return false;

            _item = item;
            return _root.Evaluate.AsBool();
        }

        internal bool GetValue(Item item, out bool value)
        {
            var isValid = (_root != null && IsValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsBool() : false;
            return isValid;
        }
        internal bool GetValue(Item item, out Int64 value)
        {
            var isValid = (_root != null && IsValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsInt64() : 0;
            return isValid;
        }
        internal bool GetValue(Item item, out double value)
        {
            var isValid = (_root != null && IsValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsDouble() : 0;
            return isValid;
        }
        internal bool GetValue(Item item, out string value)
        {
            var isValid = (_root != null && IsValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsString() : string.Empty;
            return isValid;
        }
        internal bool GetValue(Item item, out DateTime value)
        {
            var isValid = (_root != null && IsValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsDateTime() : default(DateTime);
            return isValid;
        }
        #endregion
    }
}
