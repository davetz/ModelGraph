using System;

namespace ModelGraphSTD
{
    internal class LiteralInt32 : ValueOfInt32
    {
        private string _text;
        private Int32 _value;

        internal LiteralInt32(ComputeStep step, Int32 value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override Int32 GetVal() => _step.IsNegated ? -_value : _value;
    }
}