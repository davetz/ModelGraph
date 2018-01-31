using System;
using System.Text;

namespace ModelGraphSTD
{/*
 */
    internal class WhereSelect
    {
        private Item _item;
        private ComputeStep _root; // root of the expression tree
        private bool _isValid;

        #region Constructor  ==================================================
        internal WhereSelect(string text)
        {
            _root = Parser.CreateExpressionTree(text);
            _isValid = _root.IsValid;
        }
        #endregion

        #region Property  =====================================================
        internal bool IsValid => _isValid;

        internal bool AnyChange => _root != null && _root.AnyChange;
        internal string InputString => GetText();
        internal ValType ValueType => (_isValid) ? _root.ValueType : ValType.IsInvalid;

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
            _isValid = _root.IsValid;
            return _isValid;
        }
        #endregion

        #region TryResolve  ===================================================
        /// <summary>
        /// Resolve the expression tree valueTypes, return true if any change
        /// </summary>
        const int maxResolveLoopCount = 100; // avoid infinite loops
        internal bool TryResolve()
        {
            if (!_isValid) return false;

            var failedToResolve = true;
            var anyChange = false;
            for (int i = 0; i < maxResolveLoopCount; i++)
            {
                var change = _root.TryResolve();
                anyChange |= change;
                if (change) continue;

                failedToResolve = false;
                break;
            }
            if (failedToResolve) throw new System.Exception($"Failed to resolve Where/Select clause: {InputString}");

            return anyChange;
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
            var isValid = (_root != null && _isValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsBool() : false;
            return isValid;
        }
        internal bool GetValue(Item item, out long value)
        {
            var isValid = (_root != null && _isValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsLong() : 0;
            return isValid;
        }
        internal bool GetValue(Item item, out double value)
        {
            var isValid = (_root != null && _isValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsDouble() : 0;
            return isValid;
        }
        internal bool GetValue(Item item, out string value)
        {
            var isValid = (_root != null && _isValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsString() : string.Empty;
            return isValid;
        }
        internal bool GetValue(Item item, out DateTime value)
        {
            var isValid = (_root != null && _isValid);

            _item = item;
            value = (isValid) ? _root.Evaluate.AsDateTime() : default(DateTime);
            return isValid;
        }
        #endregion
    }
}
