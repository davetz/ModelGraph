using System;

namespace ModelGraphSTD
{
    internal class LiteralDateTime : ValueOfDateTime
    {
        private string _text;
        private DateTime _value;

        internal LiteralDateTime(ComputeStep step, DateTime value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override DateTime GetVal() => _value;
    }
}