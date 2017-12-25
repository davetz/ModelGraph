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

        #region TryResolve / TryValidate  =====================================
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
            TryResolve();
            return _isValid;
        }
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
        #endregion
    }
}
