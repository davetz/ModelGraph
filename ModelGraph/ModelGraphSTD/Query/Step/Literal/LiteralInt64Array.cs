using System;

namespace ModelGraphSTD
{
    internal class LiteralInt64Array : ValueOfInt64Array
    {
        private string _text;
        private Int64[] _value;

        internal LiteralInt64Array(ComputeStep step, Int64[] value, string text)
        {
            _step = step;
            _text = text;
            _value = value;
        }
        internal override string Text => _text;

        protected override Int64[] GetVal()
        {
            if (_step.IsNegated)
            {
                var N = _value.Length;
                var negate = new Int64[N];
                for (int i = 0; i < N; i++) { negate[i] = -_value[i]; }
                return negate;
            }
            else
                return _value;
        }
    }
}