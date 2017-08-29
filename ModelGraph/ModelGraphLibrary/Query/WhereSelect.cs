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
        private char[] _input;

        private Step _root;
        internal NativeType NativeType;

        internal WhereSelect(string value) { }

        internal bool IsValid => true;
        internal bool IsInvalid { get { return !IsValid; } }
        internal string InputString => GetInputString();
        private string GetInputString()
        {
            if (_input == null) return null;

            var N = _input.Length;
            var sb = new StringBuilder(N);
            for (int i = 0; i < N; i++) { sb.Append(_input[i]); }
            return sb.ToString();
        }
        internal void Validate(Store sto, string input)
        {
            _store = sto;
            _input = input.ToCharArray();
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
