using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    internal class WhereSelect
    {
        private Item _item;
        private Store _store;

        private Step _root;
        private Parser _parser;

        internal NativeType NativeType;

        internal WhereSelect(string text)
        {
            _parser = new Parser(text);
        }

        internal bool IsValid => true;
        internal bool IsInvalid { get { return !IsValid; } }
        internal string InputString => GetInputString();
        private string GetInputString()
        {
            if (_parser != null)
            {
                return _parser.Text;
            }
            else if (_root != null)
            {
                return null;
            }
            else
            {
                return null;
            }
        }
        internal void Validate(Store sto, string text)
        {
            _store = sto;
            _root = null;
            _parser = new Parser(text);
        }
        internal void Validate(Store sto)
        {
            _store = sto;
        }

        internal bool Matches(Item item)
        {
            _item = item;
            bool result = false;
            if (IsValid) _root.GetValue(out result);
            return result;
        }
        internal IStepValue GetValue(Item item)
        {
            _item = item;
            return new InvalidStep();
        }

    }
}
