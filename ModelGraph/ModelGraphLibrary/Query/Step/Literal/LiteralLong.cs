using System.Text;

namespace ModelGraphSTD
{
    internal class LiteralLong : ValueOfLong
    {
        private string _text;
        private long _value;

        internal LiteralLong(ComputeStep step, long value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override long GetVal() => _step.IsNegated ? -_value : _value;
    }
}