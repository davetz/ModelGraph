using System;

namespace ModelGraphSTD
{
    internal class LiteralInt64 : ValueOfInt64
    {
        private string _text;
        private Int64 _value;

        internal LiteralInt64(ComputeStep step, Int64 value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override Int64 GetVal() => _step.IsNegated ? -_value : _value;
    }
}