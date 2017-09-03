using System;

namespace ModelGraphLibrary
{/*
 */
    internal class WhereSelect
    {
        private Step _root;
        private Parser _parser;

        private Item _item;

        internal NativeType NativeType;

        internal WhereSelect(string text)
        {
            _parser = new Parser(text);
        }

        internal bool IsValid => (_root != null) ? true : ((_parser == null) ? false : _parser.IsValid); 
        internal bool IsUnresolved => (_root != null || _parser == null) ? false : _parser.IsUnresolved; 
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
            _root = null;
            _parser = new Parser(text);
            Validate(sto);
            
        }
        internal void Validate(Store sto)
        {
            _root = null;

            if (IsValid)
            {
                if (_parser.Validate(sto, () => { return _item; }))
                {

                }
            }
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
