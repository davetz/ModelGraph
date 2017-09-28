using System;
using System.Text;

namespace ModelGraphLibrary
{/*
 */
    internal class WhereSelect
    {
        private Item _item;
        private Step _root;
        private Parser _parser;

        internal WhereSelect(string text)
        {
            _parser = Parser.Create(text);
        }

        #region Property  =====================================================
        internal string InputString => GetText();
        internal bool IsValid => (_root != null) ? true : ((_parser == null) ? false : _parser.IsValid);
        internal ValueType ValueType => (_root != null) ? _root.ValueType : ((_parser != null) ? _parser.NativeType : ValueType.Invalid);

        private string GetText()
        {
            if (_parser != null)
            {
                return _parser.Text;
            }
            else if (_root != null)
            {
                var sb = new StringBuilder(100);
                _root.GetText(sb);
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Validate  =====================================================
        internal void Validate(Store sto, string text)
        {
            _root = null;
            _parser = Parser.Create(text);
            Validate(sto);
            
        }
        internal void Validate(Store sto)
        {
            _root = null;

            if (IsValid &&
                _parser.TryValidate(sto, () => { return _item; }) &&
                _parser.TrySimplify())
            {
                _root = _parser.Step;
                if (_root == null && _parser.Children.Count == 1)
                    _root = _parser.Children[0].Step;
                if (_root == null)
                    _parser.Error = ParseError.InvalidText;
                else
                    _parser = null;
            }
        }
        #endregion

        #region Matches / GetValue  ===========================================
        internal bool Matches(Item item)
        {
            _item = item;
            bool result = false;
            if (IsValid) _root.GetValue(out result);
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
