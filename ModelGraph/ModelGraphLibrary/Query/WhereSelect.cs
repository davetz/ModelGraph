using System.Text;

namespace ModelGraphLibrary
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
        internal string InputString => GetText();
        internal ValueType ValueType => (_isValid) ? _root.ValueType : ValueType.IsInvalid;

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
        const int maxResolveLoopCount = 100;
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
            if (_root == null || _root.ValueType != ValueType.Bool) return false;

            _item = item;
            _root.GetValue(out bool result);
            return result;
        }
        internal void GetValue(Item item, out bool value)
        {
            if (_root == null) { value = false; return; }

            _item = item;
             _root.GetValue(out value);
        }
        internal void GetValue(Item item, out byte value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out sbyte value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out short value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out ushort value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out int value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out uint value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out long value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out ulong value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out double value)
        {
            if (_root == null) { value = 0; return; }

            _item = item;
            _root.GetValue(out value);
        }
        internal void GetValue(Item item, out string value)
        {
            if (_root == null) { value = Chef.InvalidItem; return; }

            _item = item;
            _root.GetValue(out value);
        }
        #endregion
    }
}
