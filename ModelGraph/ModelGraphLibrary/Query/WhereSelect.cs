using System.Text;

namespace ModelGraphLibrary
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
        }
        #endregion

        #region Property  =====================================================
        internal bool IsValid => _root.IsValid;
        internal string InputString => GetText();
        internal ValueType ValueType => _root.ValueType;

        private string GetText()
        {
            var sb = new StringBuilder(100);
            _root.GetText(sb);
            return sb.ToString();
        }
        #endregion

        #region TryResolve / TryValidate  =====================================
        internal bool TryValidate(Store sto, string text)
        {
            _root = Parser.CreateExpressionTree(text);
            return TryValidate(sto);
        }

        internal bool TryResolve() => _root.TryResolve();
        internal bool TryValidate(Store sto) => _root.TryValidate(sto, () => _item);
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
