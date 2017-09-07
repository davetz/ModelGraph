using System;
using System.Text;

namespace ModelGraphLibrary
{/*
 */
    internal class WhereQuery
    {
        protected Step _root;
        protected Parser _parser;
        protected Item _item;

        internal Parser SavedParser;

        internal NativeType NativeType => (_root != null) ? _root.NativeType : ((_parser != null) ? _parser.NativeType : NativeType.Invalid);

        internal WhereQuery(string text)
        {
            _parser = Parser.Create(text);
        }

        internal bool IsValid => (_root != null) ? true : ((_parser == null) ? false : _parser.IsValid); 
        internal string InputString => GetInputString();

        private string GetInputString()
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
        internal void Validate(Store sto, string text)
        {
            _root = null;
            _parser = Parser.Create(text);
            Validate(sto);
            
        }
        internal void Validate(Store sto)
        {
            _root = null;
            SavedParser = _parser;

            if (IsValid &&
                _parser.TryValidate(sto, () => { return _item; }) &&
                _parser.TrySimplify())
            {
                _root = _parser.Children[0].Step;
                _parser = null;
            }
        }

        internal bool Matches(Item item)
        {
            if (_root == null) return false;

            _item = item;
            _root.GetValue(out bool result);
            return result;
        }
    }
}
