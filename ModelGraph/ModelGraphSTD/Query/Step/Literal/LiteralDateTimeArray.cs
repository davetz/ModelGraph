using System;

namespace ModelGraphSTD
{
    internal class LiteralDateTimeArray : ValueOfDateTimeArray
    {
        private string _text;
        private DateTime[] _value;

        internal LiteralDateTimeArray(ComputeStep step, DateTime[] value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override DateTime[] GetVal() => _value;
    }
}